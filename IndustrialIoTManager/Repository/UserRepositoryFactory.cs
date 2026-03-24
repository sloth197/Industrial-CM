using IndustrialIoTManager.Helpers;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.Repository.Db;
using Microsoft.EntityFrameworkCore;

namespace IndustrialIoTManager.Repository;

public static class UserRepositoryFactory
{
    public static IUserRepository Create()
    {
        var options = AppConfiguration.LoadDataAccessOptions();
        return options.Mode switch
        {
            DataAccessMode.SqlServer => CreateSqlServerRepository(options),
            _ => new InMemoryUserRepository()
        };
    }

    private static IUserRepository CreateSqlServerRepository(DataAccessOptions options)
    {
        try
        {
            var dbContextOptions = new DbContextOptionsBuilder<IndustrialIoTDbContext>()
                .UseSqlServer(options.SqlServerConnectionString)
                .Options;

            IndustrialIoTDbInitializer.EnsureCreatedAndSeed(dbContextOptions);
            return new EfUserRepository(dbContextOptions);
        }
        catch
        {
            return new InMemoryUserRepository();
        }
    }
}
