using System.Collections.Generic;
using System.Linq;
using IndustrialIoTManager.Model;
using Microsoft.EntityFrameworkCore;

namespace IndustrialIoTManager.Repository.Db;

public sealed class EfUserRepository : IUserRepository
{
    private readonly DbContextOptions<IndustrialIoTDbContext> _dbContextOptions;

    public EfUserRepository(DbContextOptions<IndustrialIoTDbContext> dbContextOptions)
    {
        _dbContextOptions = dbContextOptions;
    }

    public IReadOnlyList<UserAccount> GetUsers()
    {
        using var db = new IndustrialIoTDbContext(_dbContextOptions);

        var users = db.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => u.IsActive)
            .Select(u => new UserAccount
            {
                UserName = u.LoginId,
                Password = u.PasswordHash,
                Role = u.UserRoles
                    .Select(ur => ur.Role.RoleName)
                    .FirstOrDefault() ?? "Viewer"
            })
            .ToList();

        return users;
    }
}
