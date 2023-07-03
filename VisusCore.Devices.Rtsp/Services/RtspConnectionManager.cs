using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.Lists.Indexes;
using OrchardCore.Lists.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using VisusCore.Consumer.Abstractions.Services;
using VisusCore.Devices.Core.Models;
using VisusCore.Devices.Models;
using VisusCore.Devices.Rtsp.Abstractions.Services;
using VisusCore.Devices.Rtsp.Core.Models;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.TenantHostedService.Core.Services;
using YesSql;
using YesSql.Services;

namespace VisusCore.Devices.Rtsp.Services;

public sealed class RtspConnectionManager : TenantBackgroundScopedService, IRtspConnectionManager, IAsyncDisposable
{
    private readonly IVideoStreamSegmentConsumerService _videoStreamSegmentConsumerService;
    private readonly ISession _session;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly ILogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<RtspStreamProducer, Task> _streamProducerTasks = new();
    private readonly Channel<ContentContextBase> _updateItemChannel = Channel.CreateUnbounded<ContentContextBase>();
    private CancellationToken _stoppingToken;
    private bool _disposed;
    private bool _disposedAsync;

    public RtspConnectionManager(
        IVideoStreamSegmentConsumerService videoStreamSegmentConsumerService,
        ISession session,
        IDataProtectionProvider dataProtectionProvider,
        ILogger<RtspConnectionManager> logger,
        ILoggerFactory loggerFactory)
    {
        _videoStreamSegmentConsumerService = videoStreamSegmentConsumerService;
        _session = session;
        _dataProtectionProvider = dataProtectionProvider;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        var deviceSettings = await GetDevicesAsync(stoppingToken);

        deviceSettings
            .SelectMany(deviceSetting =>
                deviceSetting.Streams.Select(streamSetting => CreateProducer(deviceSetting, streamSetting)))
            .ForEach(AddProducer);

        do
        {
            try
            {
                await UpdateItemAsync(await _updateItemChannel.Reader.ReadAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                // Nothing to do here.
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to read update channel.");
            }
        }
        while (!stoppingToken.IsCancellationRequested);
    }

    internal async Task PostChangeNotificationAsync(ContentContextBase context) =>
        await _updateItemChannel.Writer.WriteAsync(context, _stoppingToken);

    private async Task<IEnumerable<RtspDevice>> GetDevicesAsync(CancellationToken cancellationToken = default) =>
        await (await _session.QueryIndex<RtspDevicePartIndex>()
            .Where(index =>
                index.Latest
                && index.Published)
            .ListAsync())
            // We need this ToList() call to avoid parallel query execution on the same connection.
#pragma warning disable S2971 // "IEnumerable" LINQs should be simplified
            .ToList()
#pragma warning restore S2971 // "IEnumerable" LINQs should be simplified
            .ToAsyncEnumerable()
            .SelectManyAwait(async rtspDevicePart =>
            {
                try
                {
                    return new[]
                    {
                        await GetDeviceAsync(rtspDevicePart),
                    }
                    .ToAsyncEnumerable();
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Failed to get RTSP device with ID '{ContentItemId}'.",
                        rtspDevicePart.ContentItemId);

                    return AsyncEnumerable.Empty<RtspDevice>();
                }
            })
            .ToListAsync(cancellationToken);

    private async Task<RtspDevice> GetDeviceAsync(RtspDevicePartIndex rtspDevicePart)
    {
        var dataProtector = _dataProtectionProvider.CreateProtector(nameof(RtspDevicePart));
        var devicePart = await _session.QueryIndex<DevicePartIndex>()
            .Where(index =>
                index.Latest
                && index.Published
                && index.ContentItemId == rtspDevicePart.ContentItemId)
            .FirstOrDefaultAsync();

        return new RtspDevice
        {
            Id = rtspDevicePart.ContentItemId,
            Enabled = devicePart?.Enabled is true,
            HostName = rtspDevicePart.HostName,
            ConnectionTimeout = rtspDevicePart.ConnectionTimeout,
            Password = string.IsNullOrEmpty(rtspDevicePart.Password)
                        ? string.Empty
                        : dataProtector.Unprotect(rtspDevicePart.Password),
            RequireCredentials = rtspDevicePart.RequireCredentials,
            UserName = rtspDevicePart.UserName,
            Streams = await GetDeviceStreamsAsync(rtspDevicePart.ContentItemId),
        };
    }

    private async Task<RtspDevice> GetDeviceAsync(string id)
    {
        var rtspDevicePart = (await _session.QueryIndex<RtspDevicePartIndex>()
            .Where(index =>
                index.Latest
                && index.Published
                && index.ContentItemId == id)
            .ListAsync())
            .FirstOrDefault();
        if (rtspDevicePart is null)
        {
            return null;
        }

        return await GetDeviceAsync(rtspDevicePart);
    }

    private async Task<IEnumerable<RtspDeviceStream>> GetDeviceStreamsAsync(string deviceId)
    {
        var streamItems = (await _session.QueryIndex<ContainedPartIndex>()
            .Where(index =>
                index.Latest
                && index.Published
                && index.ListContentItemId == deviceId)
            .ListAsync())
            .ToList();

        return (await _session.QueryIndex<RtspDeviceStreamPartIndex>()
            .Where(index =>
                index.Latest
                && index.Published
                && index.ContentItemId.IsIn(streamItems.Select(streamItem => streamItem.ContentItemId)))
            .ListAsync())
            // We need this ToList() call to avoid parallel query execution on the same connection.
#pragma warning disable S2971 // "IEnumerable" LINQs should be simplified
            .ToList()
#pragma warning restore S2971 // "IEnumerable" LINQs should be simplified
            .Select(streamPart => new RtspDeviceStream
            {
                Id = streamPart.ContentItemId,
                Enabled = streamPart.Enabled,
                AllowAudio = streamPart.AllowAudio,
                Path = streamPart.Path,
                Port = streamPart.Port,
                PreferTcp = streamPart.PreferTcp,
                Type = streamPart.Type,
            })
            .ToList();
    }

    private async Task RestartProducerAsync(RtspStreamProducer producer)
    {
        if (producer.IsRunning)
        {
            try
            {
                await producer.StopAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to stop RTSP stream producer for device with ID '{DeviceId}' and stream with ID '{StreamId}'.",
                    producer.Device.Id,
                    producer.Stream.Id);
            }
        }

        _streamProducerTasks[producer] = producer.ProduceAsync(_stoppingToken);
    }

    private RtspStreamProducer CreateProducer(RtspDevice device, RtspDeviceStream stream)
    {
        var logger = _loggerFactory.CreateLogger($"{typeof(RtspStreamProducer).FullName}[{device.Id}][{stream.Id}]");

        return new RtspStreamProducer(device, stream, _videoStreamSegmentConsumerService, logger);
    }

    private async Task TryAddProducerByIdsAsync(string deviceId, string streamId)
    {
        var newRtspDevice = await GetDeviceAsync(deviceId);
        var newRtspDeviceStream = newRtspDevice?.Streams.FirstOrDefault(stream => stream.Id == streamId);
        if (newRtspDeviceStream is null)
        {
            return;
        }

        var existingRtspDevice = _streamProducerTasks.Keys.FirstOrDefault(producer => producer.Device.Id == deviceId)
            ?.Device;
        if (existingRtspDevice?.Streams.Any(stream => stream.Id == streamId) is true)
        {
            return;
        }

        if (existingRtspDevice is not null)
        {
            existingRtspDevice.Streams = existingRtspDevice.Streams
                .Concat(new[] { newRtspDeviceStream });
        }

        existingRtspDevice ??= newRtspDevice;

        AddProducer(CreateProducer(existingRtspDevice, newRtspDeviceStream));
    }

    private void AddProducer(RtspStreamProducer producer)
    {
        if (!_streamProducerTasks.TryAdd(producer, producer.ProduceAsync(_stoppingToken)))
        {
            _logger.LogWarning(
                "Failed to add RTSP stream producer for device '{DeviceId}' and stream '{StreamId}'.",
                producer.Device.Id,
                producer.Stream.Id);
        }
    }

    private async Task RemoveProducerAsync(RtspStreamProducer producer)
    {
        if (!_streamProducerTasks.TryRemove(producer, out _))
        {
            _logger.LogWarning(
                "Failed to remove RTSP stream producer for device '{DeviceId}' and stream '{StreamId}'.",
                producer.Device.Id,
                producer.Stream.Id);

            return;
        }

        try
        {
            await producer.StopAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to stop RTSP stream producer for device '{DeviceId}' and stream '{StreamId}'.",
                producer.Device.Id,
                producer.Stream.Id);
        }
        finally
        {
            await producer.DisposeAsync();
        }
    }

    private async Task UpdateProducerAsync(RtspStreamProducer producer)
    {
        var newRtspDevice = await GetDeviceAsync(producer.Device.Id);
        var newRtspDeviceStream = newRtspDevice?.Streams.FirstOrDefault(stream => stream.Id == producer.Stream.Id);
        if (newRtspDevice is null || newRtspDeviceStream is null)
        {
            return;
        }

        var existingRtspDeviceStream = producer.Device.Streams.FirstOrDefault(stream => stream.Id == newRtspDeviceStream.Id);
        var streamChanged = existingRtspDeviceStream is not null && !existingRtspDeviceStream.Equals(newRtspDeviceStream);
        if (streamChanged)
        {
            existingRtspDeviceStream.AllowAudio = newRtspDeviceStream.AllowAudio;
            existingRtspDeviceStream.Enabled = newRtspDeviceStream.Enabled;
            existingRtspDeviceStream.Path = newRtspDeviceStream.Path;
            existingRtspDeviceStream.Port = newRtspDeviceStream.Port;
            existingRtspDeviceStream.PreferTcp = newRtspDeviceStream.PreferTcp;
            existingRtspDeviceStream.Type = newRtspDeviceStream.Type;
        }

        var deviceChanged = !producer.Device.Equals(newRtspDevice);
        if (deviceChanged)
        {
            producer.Device.ConnectionTimeout = newRtspDevice.ConnectionTimeout;
            producer.Device.Enabled = newRtspDevice.Enabled;
            producer.Device.HostName = newRtspDevice.HostName;
            producer.Device.Password = newRtspDevice.Password;
            producer.Device.RequireCredentials = newRtspDevice.RequireCredentials;
            producer.Device.UserName = newRtspDevice.UserName;
        }

        if (!deviceChanged && !streamChanged)
        {
            return;
        }

        var producersToRestart = _streamProducerTasks.Keys
            .Where(key =>
            {
                if (key.Device.Id == producer.Device.Id && deviceChanged)
                {
                    return true;
                }

                return key.Stream.Id == producer.Stream.Id;
            })
            .ToList();

        foreach (var producerToRestart in producersToRestart)
        {
            await RestartProducerAsync(producerToRestart);
        }
    }

    private async Task UpdateItemAsync(ContentContextBase context)
    {
        if (!context.ContentItem.Has<DevicePart>()
            && !context.ContentItem.Has<RtspDevicePart>()
            && !context.ContentItem.Has<RtspDeviceStreamPart>())
        {
            return;
        }

        var removed = context is RemoveContentContext;
        var streamProducers = _streamProducerTasks.Keys.Where(producer =>
            producer.Device.Id == context.ContentItem.ContentItemId
            || producer.Stream.Id == context.ContentItem.ContentItemId)
            .ToList();

        if (removed && streamProducers.Any())
        {
            foreach (var producer in streamProducers)
            {
                await RemoveProducerAsync(producer);
            }

            return;
        }

        if (streamProducers.Any())
        {
            foreach (var producer in streamProducers)
            {
                await UpdateProducerAsync(producer);
            }

            return;
        }

        var streamProducer = streamProducers.FirstOrDefault();
        if (streamProducer is null
            && context.ContentItem.As<RtspDeviceStreamPart>() is { } rtspDeviceStreamPart
            && context.ContentItem.As<ContainedPart>() is { } containedPart)
        {
            await TryAddProducerByIdsAsync(containedPart.ListContentItemId, rtspDeviceStreamPart.ContentItem.ContentItemId);
        }
    }

    public override void Dispose()
    {
        if (!_disposed)
        {
            DisposeAsync()
                .AsTask()
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            _disposed = true;
        }

        base.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposedAsync || _disposed)
        {
            return;
        }

        var producers = _streamProducerTasks.Keys.ToList();
        foreach (var producer in producers)
        {
            await RemoveProducerAsync(producer);
        }

        _disposedAsync = true;
    }
}
