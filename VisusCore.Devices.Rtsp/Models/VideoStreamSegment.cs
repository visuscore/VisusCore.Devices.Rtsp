using System;
using VisusCore.Consumer.Abstractions.Models;

namespace VisusCore.Devices.Rtsp.Models;

public class VideoStreamSegment : IVideoStreamSegment
{
    private readonly byte[] _init;
    private readonly byte[] _data;

    public string StreamId { get; init; }
    public ReadOnlySpan<byte> Init { get => _init; init => _init = value.ToArray(); }
    public ReadOnlySpan<byte> Data { get => _data; init => _data = value.ToArray(); }
    public long? Duration { get; init; }
    public long? TimestampUtc { get; init; }
    public long? TimestampProvided { get; init; }
    public long? FrameCount { get; init; }
}
