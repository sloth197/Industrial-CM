using System;
using System.Collections.Generic;

namespace IndustrialIoTManager.Repository.Db;

public sealed class UserEntity
{
    public Guid UserId { get; set; }
    public string LoginId { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    public ICollection<ControlCommandEntity> RequestedControlCommands { get; set; } = new List<ControlCommandEntity>();
    public ICollection<AuditLogEntity> AuditLogs { get; set; } = new List<AuditLogEntity>();
    public ICollection<AlarmEventEntity> AckedAlarmEvents { get; set; } = new List<AlarmEventEntity>();
    public ICollection<SystemSettingEntity> UpdatedSettings { get; set; } = new List<SystemSettingEntity>();
}

public sealed class RoleEntity
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
}

public sealed class PermissionEntity
{
    public int PermissionId { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public string PermissionName { get; set; } = string.Empty;

    public ICollection<RolePermissionEntity> RolePermissions { get; set; } = new List<RolePermissionEntity>();
}

public sealed class UserRoleEntity
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}

public sealed class RolePermissionEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    public RoleEntity Role { get; set; } = null!;
    public PermissionEntity Permission { get; set; } = null!;
}

public sealed class EquipmentGroupEntity
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<EquipmentEntity> Equipments { get; set; } = new List<EquipmentEntity>();
}

public sealed class EquipmentEntity
{
    public Guid EquipmentId { get; set; }
    public string EquipmentCode { get; set; } = string.Empty;
    public string EquipmentName { get; set; } = string.Empty;
    public int? GroupId { get; set; }
    public string Status { get; set; } = "Unknown";
    public string? Location { get; set; }
    public DateTime? LastHeartbeatAt { get; set; }

    public EquipmentGroupEntity? Group { get; set; }
    public ICollection<EquipmentConnectionEntity> Connections { get; set; } = new List<EquipmentConnectionEntity>();
    public ICollection<SensorPointEntity> SensorPoints { get; set; } = new List<SensorPointEntity>();
    public ICollection<AlarmEventEntity> AlarmEvents { get; set; } = new List<AlarmEventEntity>();
    public ICollection<ControlCommandEntity> ControlCommands { get; set; } = new List<ControlCommandEntity>();
}

public sealed class EquipmentConnectionEntity
{
    public Guid ConnectionId { get; set; }
    public Guid EquipmentId { get; set; }
    public string Protocol { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? PlcAddress { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EquipmentEntity Equipment { get; set; } = null!;
}

public sealed class SensorPointEntity
{
    public Guid SensorPointId { get; set; }
    public Guid EquipmentId { get; set; }
    public string SensorCode { get; set; } = string.Empty;
    public string SensorName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string DataType { get; set; } = "number";
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public bool IsEnabled { get; set; } = true;

    public EquipmentEntity Equipment { get; set; } = null!;
    public ICollection<SensorReadingEntity> SensorReadings { get; set; } = new List<SensorReadingEntity>();
    public ICollection<AlarmRuleEntity> AlarmRules { get; set; } = new List<AlarmRuleEntity>();
    public ICollection<AlarmEventEntity> AlarmEvents { get; set; } = new List<AlarmEventEntity>();
}

public sealed class SensorReadingEntity
{
    public long ReadingId { get; set; }
    public Guid SensorPointId { get; set; }
    public decimal NumericValue { get; set; }
    public string Quality { get; set; } = "Good";
    public DateTime SourceTimestamp { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public SensorPointEntity SensorPoint { get; set; } = null!;
}

public sealed class AlarmRuleEntity
{
    public Guid RuleId { get; set; }
    public Guid SensorPointId { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public decimal ThresholdValue { get; set; }
    public string Severity { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public SensorPointEntity SensorPoint { get; set; } = null!;
    public ICollection<AlarmEventEntity> AlarmEvents { get; set; } = new List<AlarmEventEntity>();
}

public sealed class AlarmEventEntity
{
    public long EventId { get; set; }
    public Guid? RuleId { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid? SensorPointId { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid? AckByUserId { get; set; }
    public DateTime? AckAt { get; set; }
    public string Status { get; set; } = "Open";

    public AlarmRuleEntity? Rule { get; set; }
    public EquipmentEntity Equipment { get; set; } = null!;
    public SensorPointEntity? SensorPoint { get; set; }
    public UserEntity? AckByUser { get; set; }
}

public sealed class ControlCommandEntity
{
    public long CommandId { get; set; }
    public Guid EquipmentId { get; set; }
    public string CommandType { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
    public Guid RequestedByUserId { get; set; }
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string ResultStatus { get; set; } = "Pending";
    public string? ResultMessage { get; set; }

    public EquipmentEntity Equipment { get; set; } = null!;
    public UserEntity RequestedByUser { get; set; } = null!;
}

public sealed class AuditLogEntity
{
    public long AuditId { get; set; }
    public Guid? UserId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
    public string? TargetId { get; set; }
    public string DetailJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ClientIp { get; set; }

    public UserEntity? User { get; set; }
}

public sealed class SystemSettingEntity
{
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid? UpdatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserEntity? UpdatedByUser { get; set; }
}
