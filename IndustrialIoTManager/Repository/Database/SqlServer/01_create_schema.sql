-- IndustrialIoTManager MVP schema (SQL Server)
-- Encoding: UTF-8

CREATE TABLE dbo.users
(
    user_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_users PRIMARY KEY,
    login_id NVARCHAR(50) NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    display_name NVARCHAR(100) NOT NULL,
    is_active BIT NOT NULL CONSTRAINT DF_users_is_active DEFAULT (1),
    created_at DATETIME2 NOT NULL CONSTRAINT DF_users_created_at DEFAULT (SYSUTCDATETIME())
);
GO

CREATE UNIQUE INDEX UX_users_login_id ON dbo.users(login_id);
GO

CREATE TABLE dbo.roles
(
    role_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_roles PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255) NULL
);
GO

CREATE UNIQUE INDEX UX_roles_role_name ON dbo.roles(role_name);
GO

CREATE TABLE dbo.permissions
(
    permission_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_permissions PRIMARY KEY,
    permission_code NVARCHAR(120) NOT NULL,
    permission_name NVARCHAR(120) NOT NULL
);
GO

CREATE UNIQUE INDEX UX_permissions_permission_code ON dbo.permissions(permission_code);
GO

CREATE TABLE dbo.user_roles
(
    user_id UNIQUEIDENTIFIER NOT NULL,
    role_id INT NOT NULL,
    assigned_at DATETIME2 NOT NULL CONSTRAINT DF_user_roles_assigned_at DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_user_roles PRIMARY KEY (user_id, role_id),
    CONSTRAINT FK_user_roles_users_user_id FOREIGN KEY (user_id) REFERENCES dbo.users(user_id) ON DELETE CASCADE,
    CONSTRAINT FK_user_roles_roles_role_id FOREIGN KEY (role_id) REFERENCES dbo.roles(role_id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_user_roles_role_id ON dbo.user_roles(role_id);
GO

CREATE TABLE dbo.role_permissions
(
    role_id INT NOT NULL,
    permission_id INT NOT NULL,
    CONSTRAINT PK_role_permissions PRIMARY KEY (role_id, permission_id),
    CONSTRAINT FK_role_permissions_roles_role_id FOREIGN KEY (role_id) REFERENCES dbo.roles(role_id) ON DELETE CASCADE,
    CONSTRAINT FK_role_permissions_permissions_permission_id FOREIGN KEY (permission_id) REFERENCES dbo.permissions(permission_id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_role_permissions_permission_id ON dbo.role_permissions(permission_id);
GO

CREATE TABLE dbo.equipment_groups
(
    group_id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_equipment_groups PRIMARY KEY,
    group_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255) NULL
);
GO

CREATE UNIQUE INDEX UX_equipment_groups_group_name ON dbo.equipment_groups(group_name);
GO

CREATE TABLE dbo.equipments
(
    equipment_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_equipments PRIMARY KEY,
    equipment_code NVARCHAR(50) NOT NULL,
    equipment_name NVARCHAR(120) NOT NULL,
    group_id INT NULL,
    status NVARCHAR(30) NOT NULL,
    location NVARCHAR(120) NULL,
    last_heartbeat_at DATETIME2 NULL,
    CONSTRAINT FK_equipments_equipment_groups_group_id FOREIGN KEY (group_id) REFERENCES dbo.equipment_groups(group_id) ON DELETE SET NULL
);
GO

CREATE UNIQUE INDEX UX_equipments_equipment_code ON dbo.equipments(equipment_code);
CREATE INDEX IX_equipments_group_id ON dbo.equipments(group_id);
GO

CREATE TABLE dbo.equipment_connections
(
    connection_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_equipment_connections PRIMARY KEY,
    equipment_id UNIQUEIDENTIFIER NOT NULL,
    protocol NVARCHAR(30) NOT NULL,
    ip_address NVARCHAR(45) NOT NULL,
    port INT NOT NULL,
    plc_address NVARCHAR(120) NULL,
    is_enabled BIT NOT NULL CONSTRAINT DF_equipment_connections_is_enabled DEFAULT (1),
    created_at DATETIME2 NOT NULL CONSTRAINT DF_equipment_connections_created_at DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_equipment_connections_equipments_equipment_id FOREIGN KEY (equipment_id) REFERENCES dbo.equipments(equipment_id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_equipment_connections_equipment_id ON dbo.equipment_connections(equipment_id);
GO

CREATE TABLE dbo.sensor_points
(
    sensor_point_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_sensor_points PRIMARY KEY,
    equipment_id UNIQUEIDENTIFIER NOT NULL,
    sensor_code NVARCHAR(60) NOT NULL,
    sensor_name NVARCHAR(120) NOT NULL,
    unit NVARCHAR(30) NOT NULL,
    data_type NVARCHAR(20) NOT NULL,
    min_value DECIMAL(18,4) NULL,
    max_value DECIMAL(18,4) NULL,
    is_enabled BIT NOT NULL CONSTRAINT DF_sensor_points_is_enabled DEFAULT (1),
    CONSTRAINT FK_sensor_points_equipments_equipment_id FOREIGN KEY (equipment_id) REFERENCES dbo.equipments(equipment_id) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX UX_sensor_points_equipment_id_sensor_code ON dbo.sensor_points(equipment_id, sensor_code);
GO

CREATE TABLE dbo.sensor_readings
(
    reading_id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_sensor_readings PRIMARY KEY,
    sensor_point_id UNIQUEIDENTIFIER NOT NULL,
    numeric_value DECIMAL(18,4) NOT NULL,
    quality NVARCHAR(20) NOT NULL,
    source_timestamp DATETIME2 NOT NULL,
    received_at DATETIME2 NOT NULL CONSTRAINT DF_sensor_readings_received_at DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_sensor_readings_sensor_points_sensor_point_id FOREIGN KEY (sensor_point_id) REFERENCES dbo.sensor_points(sensor_point_id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_sensor_readings_sensor_point_id_source_timestamp ON dbo.sensor_readings(sensor_point_id, source_timestamp DESC);
GO

CREATE TABLE dbo.alarm_rules
(
    rule_id UNIQUEIDENTIFIER NOT NULL CONSTRAINT PK_alarm_rules PRIMARY KEY,
    sensor_point_id UNIQUEIDENTIFIER NOT NULL,
    rule_type NVARCHAR(40) NOT NULL,
    threshold_value DECIMAL(18,4) NOT NULL,
    severity NVARCHAR(20) NOT NULL,
    is_enabled BIT NOT NULL CONSTRAINT DF_alarm_rules_is_enabled DEFAULT (1),
    created_at DATETIME2 NOT NULL CONSTRAINT DF_alarm_rules_created_at DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_alarm_rules_sensor_points_sensor_point_id FOREIGN KEY (sensor_point_id) REFERENCES dbo.sensor_points(sensor_point_id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_alarm_rules_sensor_point_id ON dbo.alarm_rules(sensor_point_id);
GO

CREATE TABLE dbo.alarm_events
(
    event_id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_alarm_events PRIMARY KEY,
    rule_id UNIQUEIDENTIFIER NULL,
    equipment_id UNIQUEIDENTIFIER NOT NULL,
    sensor_point_id UNIQUEIDENTIFIER NULL,
    severity NVARCHAR(20) NOT NULL,
    message NVARCHAR(1000) NOT NULL,
    occurred_at DATETIME2 NOT NULL,
    ack_by_user_id UNIQUEIDENTIFIER NULL,
    ack_at DATETIME2 NULL,
    status NVARCHAR(20) NOT NULL,
    CONSTRAINT FK_alarm_events_alarm_rules_rule_id FOREIGN KEY (rule_id) REFERENCES dbo.alarm_rules(rule_id) ON DELETE SET NULL,
    CONSTRAINT FK_alarm_events_equipments_equipment_id FOREIGN KEY (equipment_id) REFERENCES dbo.equipments(equipment_id),
    CONSTRAINT FK_alarm_events_sensor_points_sensor_point_id FOREIGN KEY (sensor_point_id) REFERENCES dbo.sensor_points(sensor_point_id) ON DELETE SET NULL,
    CONSTRAINT FK_alarm_events_users_ack_by_user_id FOREIGN KEY (ack_by_user_id) REFERENCES dbo.users(user_id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_alarm_events_status_occurred_at ON dbo.alarm_events(status, occurred_at DESC);
CREATE INDEX IX_alarm_events_equipment_id ON dbo.alarm_events(equipment_id);
CREATE INDEX IX_alarm_events_sensor_point_id ON dbo.alarm_events(sensor_point_id);
GO

CREATE TABLE dbo.control_commands
(
    command_id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_control_commands PRIMARY KEY,
    equipment_id UNIQUEIDENTIFIER NOT NULL,
    command_type NVARCHAR(40) NOT NULL,
    payload_json NVARCHAR(MAX) NOT NULL,
    requested_by_user_id UNIQUEIDENTIFIER NOT NULL,
    requested_at DATETIME2 NOT NULL CONSTRAINT DF_control_commands_requested_at DEFAULT (SYSUTCDATETIME()),
    result_status NVARCHAR(20) NOT NULL,
    result_message NVARCHAR(1000) NULL,
    CONSTRAINT FK_control_commands_equipments_equipment_id FOREIGN KEY (equipment_id) REFERENCES dbo.equipments(equipment_id),
    CONSTRAINT FK_control_commands_users_requested_by_user_id FOREIGN KEY (requested_by_user_id) REFERENCES dbo.users(user_id)
);
GO

CREATE INDEX IX_control_commands_equipment_id_requested_at ON dbo.control_commands(equipment_id, requested_at DESC);
GO

CREATE TABLE dbo.audit_logs
(
    audit_id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_audit_logs PRIMARY KEY,
    user_id UNIQUEIDENTIFIER NULL,
    action_type NVARCHAR(60) NOT NULL,
    target_type NVARCHAR(60) NOT NULL,
    target_id NVARCHAR(80) NULL,
    detail_json NVARCHAR(MAX) NOT NULL,
    created_at DATETIME2 NOT NULL CONSTRAINT DF_audit_logs_created_at DEFAULT (SYSUTCDATETIME()),
    client_ip NVARCHAR(45) NULL,
    CONSTRAINT FK_audit_logs_users_user_id FOREIGN KEY (user_id) REFERENCES dbo.users(user_id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_audit_logs_created_at_user_id ON dbo.audit_logs(created_at DESC, user_id);
GO

CREATE TABLE dbo.system_settings
(
    setting_key NVARCHAR(100) NOT NULL CONSTRAINT PK_system_settings PRIMARY KEY,
    setting_value NVARCHAR(MAX) NOT NULL,
    category NVARCHAR(60) NOT NULL,
    updated_by_user_id UNIQUEIDENTIFIER NULL,
    updated_at DATETIME2 NOT NULL CONSTRAINT DF_system_settings_updated_at DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT FK_system_settings_users_updated_by_user_id FOREIGN KEY (updated_by_user_id) REFERENCES dbo.users(user_id) ON DELETE SET NULL
);
GO

CREATE INDEX IX_system_settings_category ON dbo.system_settings(category);
GO
