using Microsoft.EntityFrameworkCore;

namespace IndustrialIoTManager.Repository.Db;

public sealed class IndustrialIoTDbContext : DbContext
{
    public IndustrialIoTDbContext(DbContextOptions<IndustrialIoTDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
    public DbSet<RolePermissionEntity> RolePermissions => Set<RolePermissionEntity>();
    public DbSet<EquipmentGroupEntity> EquipmentGroups => Set<EquipmentGroupEntity>();
    public DbSet<EquipmentEntity> Equipments => Set<EquipmentEntity>();
    public DbSet<EquipmentConnectionEntity> EquipmentConnections => Set<EquipmentConnectionEntity>();
    public DbSet<SensorPointEntity> SensorPoints => Set<SensorPointEntity>();
    public DbSet<SensorReadingEntity> SensorReadings => Set<SensorReadingEntity>();
    public DbSet<AlarmRuleEntity> AlarmRules => Set<AlarmRuleEntity>();
    public DbSet<AlarmEventEntity> AlarmEvents => Set<AlarmEventEntity>();
    public DbSet<ControlCommandEntity> ControlCommands => Set<ControlCommandEntity>();
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
    public DbSet<SystemSettingEntity> SystemSettings => Set<SystemSettingEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.LoginId).HasColumnName("login_id").HasMaxLength(50).IsRequired();
            entity.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            entity.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.HasIndex(x => x.LoginId).IsUnique();
        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(x => x.RoleId);
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.RoleName).HasColumnName("role_name").HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(255);
            entity.HasIndex(x => x.RoleName).IsUnique();
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.ToTable("permissions");
            entity.HasKey(x => x.PermissionId);
            entity.Property(x => x.PermissionId).HasColumnName("permission_id");
            entity.Property(x => x.PermissionCode).HasColumnName("permission_code").HasMaxLength(120).IsRequired();
            entity.Property(x => x.PermissionName).HasColumnName("permission_name").HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.PermissionCode).IsUnique();
        });

        modelBuilder.Entity<UserRoleEntity>(entity =>
        {
            entity.ToTable("user_roles");
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.AssignedAt).HasColumnName("assigned_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");

            entity.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.RoleId);
        });

        modelBuilder.Entity<RolePermissionEntity>(entity =>
        {
            entity.ToTable("role_permissions");
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            entity.Property(x => x.RoleId).HasColumnName("role_id");
            entity.Property(x => x.PermissionId).HasColumnName("permission_id");

            entity.HasOne(x => x.Role)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Permission)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.PermissionId);
        });

        modelBuilder.Entity<EquipmentGroupEntity>(entity =>
        {
            entity.ToTable("equipment_groups");
            entity.HasKey(x => x.GroupId);
            entity.Property(x => x.GroupId).HasColumnName("group_id");
            entity.Property(x => x.GroupName).HasColumnName("group_name").HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasColumnName("description").HasMaxLength(255);
            entity.HasIndex(x => x.GroupName).IsUnique();
        });

        modelBuilder.Entity<EquipmentEntity>(entity =>
        {
            entity.ToTable("equipments");
            entity.HasKey(x => x.EquipmentId);
            entity.Property(x => x.EquipmentId).HasColumnName("equipment_id");
            entity.Property(x => x.EquipmentCode).HasColumnName("equipment_code").HasMaxLength(50).IsRequired();
            entity.Property(x => x.EquipmentName).HasColumnName("equipment_name").HasMaxLength(120).IsRequired();
            entity.Property(x => x.GroupId).HasColumnName("group_id");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(30).IsRequired();
            entity.Property(x => x.Location).HasColumnName("location").HasMaxLength(120);
            entity.Property(x => x.LastHeartbeatAt).HasColumnName("last_heartbeat_at").HasColumnType("datetime2");

            entity.HasIndex(x => x.EquipmentCode).IsUnique();
            entity.HasIndex(x => x.GroupId);

            entity.HasOne(x => x.Group)
                .WithMany(x => x.Equipments)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EquipmentConnectionEntity>(entity =>
        {
            entity.ToTable("equipment_connections");
            entity.HasKey(x => x.ConnectionId);
            entity.Property(x => x.ConnectionId).HasColumnName("connection_id");
            entity.Property(x => x.EquipmentId).HasColumnName("equipment_id");
            entity.Property(x => x.Protocol).HasColumnName("protocol").HasMaxLength(30).IsRequired();
            entity.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(45).IsRequired();
            entity.Property(x => x.Port).HasColumnName("port").IsRequired();
            entity.Property(x => x.PlcAddress).HasColumnName("plc_address").HasMaxLength(120);
            entity.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.HasIndex(x => x.EquipmentId);

            entity.HasOne(x => x.Equipment)
                .WithMany(x => x.Connections)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SensorPointEntity>(entity =>
        {
            entity.ToTable("sensor_points");
            entity.HasKey(x => x.SensorPointId);
            entity.Property(x => x.SensorPointId).HasColumnName("sensor_point_id");
            entity.Property(x => x.EquipmentId).HasColumnName("equipment_id");
            entity.Property(x => x.SensorCode).HasColumnName("sensor_code").HasMaxLength(60).IsRequired();
            entity.Property(x => x.SensorName).HasColumnName("sensor_name").HasMaxLength(120).IsRequired();
            entity.Property(x => x.Unit).HasColumnName("unit").HasMaxLength(30).IsRequired();
            entity.Property(x => x.DataType).HasColumnName("data_type").HasMaxLength(20).IsRequired();
            entity.Property(x => x.MinValue).HasColumnName("min_value").HasColumnType("decimal(18,4)");
            entity.Property(x => x.MaxValue).HasColumnName("max_value").HasColumnType("decimal(18,4)");
            entity.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true).IsRequired();
            entity.HasIndex(x => new { x.EquipmentId, x.SensorCode }).IsUnique();

            entity.HasOne(x => x.Equipment)
                .WithMany(x => x.SensorPoints)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SensorReadingEntity>(entity =>
        {
            entity.ToTable("sensor_readings");
            entity.HasKey(x => x.ReadingId);
            entity.Property(x => x.ReadingId).HasColumnName("reading_id");
            entity.Property(x => x.SensorPointId).HasColumnName("sensor_point_id");
            entity.Property(x => x.NumericValue).HasColumnName("numeric_value").HasColumnType("decimal(18,4)").IsRequired();
            entity.Property(x => x.Quality).HasColumnName("quality").HasMaxLength(20).IsRequired();
            entity.Property(x => x.SourceTimestamp).HasColumnName("source_timestamp").HasColumnType("datetime2").IsRequired();
            entity.Property(x => x.ReceivedAt).HasColumnName("received_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.HasIndex(x => new { x.SensorPointId, x.SourceTimestamp });

            entity.HasOne(x => x.SensorPoint)
                .WithMany(x => x.SensorReadings)
                .HasForeignKey(x => x.SensorPointId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AlarmRuleEntity>(entity =>
        {
            entity.ToTable("alarm_rules");
            entity.HasKey(x => x.RuleId);
            entity.Property(x => x.RuleId).HasColumnName("rule_id");
            entity.Property(x => x.SensorPointId).HasColumnName("sensor_point_id");
            entity.Property(x => x.RuleType).HasColumnName("rule_type").HasMaxLength(40).IsRequired();
            entity.Property(x => x.ThresholdValue).HasColumnName("threshold_value").HasColumnType("decimal(18,4)").IsRequired();
            entity.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20).IsRequired();
            entity.Property(x => x.IsEnabled).HasColumnName("is_enabled").HasDefaultValue(true).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.HasIndex(x => x.SensorPointId);

            entity.HasOne(x => x.SensorPoint)
                .WithMany(x => x.AlarmRules)
                .HasForeignKey(x => x.SensorPointId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AlarmEventEntity>(entity =>
        {
            entity.ToTable("alarm_events");
            entity.HasKey(x => x.EventId);
            entity.Property(x => x.EventId).HasColumnName("event_id");
            entity.Property(x => x.RuleId).HasColumnName("rule_id");
            entity.Property(x => x.EquipmentId).HasColumnName("equipment_id");
            entity.Property(x => x.SensorPointId).HasColumnName("sensor_point_id");
            entity.Property(x => x.Severity).HasColumnName("severity").HasMaxLength(20).IsRequired();
            entity.Property(x => x.Message).HasColumnName("message").HasMaxLength(1000).IsRequired();
            entity.Property(x => x.OccurredAt).HasColumnName("occurred_at").HasColumnType("datetime2").IsRequired();
            entity.Property(x => x.AckByUserId).HasColumnName("ack_by_user_id");
            entity.Property(x => x.AckAt).HasColumnName("ack_at").HasColumnType("datetime2");
            entity.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
            entity.HasIndex(x => new { x.Status, x.OccurredAt });
            entity.HasIndex(x => x.EquipmentId);
            entity.HasIndex(x => x.SensorPointId);

            entity.HasOne(x => x.Rule)
                .WithMany(x => x.AlarmEvents)
                .HasForeignKey(x => x.RuleId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.Equipment)
                .WithMany(x => x.AlarmEvents)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.SensorPoint)
                .WithMany(x => x.AlarmEvents)
                .HasForeignKey(x => x.SensorPointId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(x => x.AckByUser)
                .WithMany(x => x.AckedAlarmEvents)
                .HasForeignKey(x => x.AckByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ControlCommandEntity>(entity =>
        {
            entity.ToTable("control_commands");
            entity.HasKey(x => x.CommandId);
            entity.Property(x => x.CommandId).HasColumnName("command_id");
            entity.Property(x => x.EquipmentId).HasColumnName("equipment_id");
            entity.Property(x => x.CommandType).HasColumnName("command_type").HasMaxLength(40).IsRequired();
            entity.Property(x => x.PayloadJson).HasColumnName("payload_json").HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.RequestedByUserId).HasColumnName("requested_by_user_id");
            entity.Property(x => x.RequestedAt).HasColumnName("requested_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.Property(x => x.ResultStatus).HasColumnName("result_status").HasMaxLength(20).IsRequired();
            entity.Property(x => x.ResultMessage).HasColumnName("result_message").HasMaxLength(1000);
            entity.HasIndex(x => new { x.EquipmentId, x.RequestedAt });

            entity.HasOne(x => x.Equipment)
                .WithMany(x => x.ControlCommands)
                .HasForeignKey(x => x.EquipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.RequestedByUser)
                .WithMany(x => x.RequestedControlCommands)
                .HasForeignKey(x => x.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<AuditLogEntity>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(x => x.AuditId);
            entity.Property(x => x.AuditId).HasColumnName("audit_id");
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.ActionType).HasColumnName("action_type").HasMaxLength(60).IsRequired();
            entity.Property(x => x.TargetType).HasColumnName("target_type").HasMaxLength(60).IsRequired();
            entity.Property(x => x.TargetId).HasColumnName("target_id").HasMaxLength(80);
            entity.Property(x => x.DetailJson).HasColumnName("detail_json").HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.Property(x => x.ClientIp).HasColumnName("client_ip").HasMaxLength(45);
            entity.HasIndex(x => new { x.CreatedAt, x.UserId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<SystemSettingEntity>(entity =>
        {
            entity.ToTable("system_settings");
            entity.HasKey(x => x.SettingKey);
            entity.Property(x => x.SettingKey).HasColumnName("setting_key").HasMaxLength(100);
            entity.Property(x => x.SettingValue).HasColumnName("setting_value").HasColumnType("nvarchar(max)").IsRequired();
            entity.Property(x => x.Category).HasColumnName("category").HasMaxLength(60).IsRequired();
            entity.Property(x => x.UpdatedByUserId).HasColumnName("updated_by_user_id");
            entity.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2").HasDefaultValueSql("sysutcdatetime()");
            entity.HasIndex(x => x.Category);

            entity.HasOne(x => x.UpdatedByUser)
                .WithMany(x => x.UpdatedSettings)
                .HasForeignKey(x => x.UpdatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
