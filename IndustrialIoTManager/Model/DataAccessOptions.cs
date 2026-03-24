namespace IndustrialIoTManager.Model;

public enum DataAccessMode
{
    InMemory,
    SqlServer
}

public sealed class DataAccessOptions
{
    public const string DefaultSqlServerConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=IndustrialIoTManager;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

    public DataAccessMode Mode { get; init; } = DataAccessMode.InMemory;
    public string SqlServerConnectionString { get; init; } = DefaultSqlServerConnectionString;
}
