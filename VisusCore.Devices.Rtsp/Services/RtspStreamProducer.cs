using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VisusCore.AidStack.Extensions;
using VisusCore.Consumer.Abstractions.Services;
using VisusCore.Devices.Rtsp.Models;
using VisusCore.Native.Ffmpeg.Core.StreamSegmenter;

namespace VisusCore.Devices.Rtsp.Services;

public class RtspStreamProducer : IDisposable, IAsyncDisposable
{
    private readonly IVideoStreamSegmentConsumerService _videoStreamSegmentConsumerService;
    private readonly ILogger _logger;
    private CancellationTokenSource _stoppingToken;
    private Task _segmenterTask;
    private bool _disposed;
    private bool _disposedAsync;

    private string RtspUrl
    {
        get
        {
            var uriBuilder = new UriBuilder("rtsp", Device.HostName, Stream.Port);
            if (!string.IsNullOrWhiteSpace(Stream.Path) && Stream.Path.Contains('?'))
            {
                var pathParts = Stream.Path.Split('?');
                uriBuilder.Path = pathParts[0];
                uriBuilder.Query = pathParts[1];
            }
            else
            {
                uriBuilder.Path = Stream.Path;
            }

            if (Device.RequireCredentials)
            {
                uriBuilder.UserName = Device.UserName;
                uriBuilder.Password = Device.Password;
            }

            return uriBuilder.Uri.ToString();
        }
    }

    public RtspDevice Device { get; }

    public RtspDeviceStream Stream { get; }

    public bool IsRunning { get; private set; }

    public RtspStreamProducer(
        RtspDevice rtspDevice,
        RtspDeviceStream rtspDeviceStream,
        IVideoStreamSegmentConsumerService videoStreamSegmentConsumerService,
        ILogger logger)
    {
        Device = rtspDevice;
        Stream = rtspDeviceStream;
        _videoStreamSegmentConsumerService = videoStreamSegmentConsumerService;
        _logger = logger;
    }

    public Task ProduceAsync(CancellationToken cancellationToken = default)
    {
        if (_stoppingToken is not null || _segmenterTask is not null)
        {
            throw new InvalidOperationException("Producer is already running.");
        }

        _stoppingToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _segmenterTask = SegmenterAsync(_stoppingToken.Token);

        IsRunning = true;

        return _segmenterTask;
    }

    public async Task StopAsync()
    {
        if (_stoppingToken is null || _segmenterTask is null)
        {
            throw new InvalidOperationException("Producer is not running.");
        }

        _stoppingToken.Cancel();

        try
        {
            await _segmenterTask.WaitAsync(default(CancellationToken));
        }
        catch
        {
            // Nothing to do here.
        }
        finally
        {
            _stoppingToken = null;
            _segmenterTask = null;
        }

        IsRunning = false;
    }

    public async Task SegmenterAsync(CancellationToken cancellationToken)
    {
        if (!Device.Enabled || !Stream.Enabled)
        {
            await cancellationToken.WaitAsync(Timeout.InfiniteTimeSpan);

            return;
        }

        do
        {
            try
            {
                await CaptureSegmentsAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Nothing to do here.
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while capturing segments.");
            }
        }
        while (!cancellationToken.IsCancellationRequested);
    }

    private async Task CaptureSegmentsAsync(CancellationToken cancellationToken)
    {
        using var segmenter = new Segmenter(
            new RtspStreamSource(
                RtspUrl,
                preferTcp: Stream.PreferTcp,
                timeout: Device.ConnectionTimeout > 0
                    ? TimeSpan.FromSeconds(Device.ConnectionTimeout)
                    : Timeout.InfiniteTimeSpan),
            Stream.AllowAudio);

        await segmenter.StartAsync(cancellationToken);

        var segment = default(Segment);
        while ((segment = await segmenter.GetNextSegmentAsync(cancellationToken)) is not null)
        {
            await _videoStreamSegmentConsumerService.ConsumeAsync(
                new VideoStreamSegment
                {
                    StreamId = Stream.Id,
                    Data = segment.Data,
                    Duration = segment.Duration,
                    FrameCount = segment.FrameCount,
                    Init = segment.Init,
                    TimestampUtc = segment.TimestampUtc,
                    TimestampProvided = segment.TimestampProvided,
                },
                cancellationToken);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                DisposeAsync()
                    .AsTask()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }

            _stoppingToken?.Dispose();
            _stoppingToken = null;

            _disposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposedAsync || _disposed)
        {
            return;
        }

        if (_stoppingToken is not null && _segmenterTask is not null)
        {
            _stoppingToken.Cancel();

            try
            {
                await StopAsync();
            }
            catch
            {
                // Nothing to do here.
            }
        }

        Dispose(disposing: false);
        _disposedAsync = true;
        GC.SuppressFinalize(this);
    }
}
