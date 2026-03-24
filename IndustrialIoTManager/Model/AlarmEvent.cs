using System;

namespace IndustrialIoTManager.Model;

public sealed class AlarmEvent
{
    public DateTime OccurredAt { get; init; }
    public string Severity { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
