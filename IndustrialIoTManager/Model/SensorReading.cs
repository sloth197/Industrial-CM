using System;

namespace IndustrialIoTManager.Model;

public sealed class SensorReading
{
    public string EquipmentName { get; init; } = string.Empty;
    public string SensorType { get; init; } = string.Empty;
    public double Value { get; init; }
    public string Unit { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
}
