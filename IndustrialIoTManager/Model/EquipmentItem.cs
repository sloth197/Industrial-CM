using System;

namespace IndustrialIoTManager.Model;

public sealed class EquipmentItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime LastHeartbeat { get; init; }
}
