using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace IndustrialIoTManager.Repository.Db;

public static class IndustrialIoTDbInitializer
{
    public static void EnsureCreatedAndSeed(DbContextOptions<IndustrialIoTDbContext> dbContextOptions)
    {
        using var db = new IndustrialIoTDbContext(dbContextOptions);
        db.Database.EnsureCreated();

        SeedRoles(db);
        SeedUsers(db);
    }

    private static void SeedRoles(IndustrialIoTDbContext db)
    {
        if (db.Roles.Any())
        {
            return;
        }

        db.Roles.AddRange(
            new RoleEntity { RoleName = "Administrator", Description = "System administrator" },
            new RoleEntity { RoleName = "Operator", Description = "Plant operator" },
            new RoleEntity { RoleName = "Viewer", Description = "Read-only observer" });

        db.SaveChanges();
    }

    private static void SeedUsers(IndustrialIoTDbContext db)
    {
        if (db.Users.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var users = new List<UserEntity>
        {
            new()
            {
                UserId = Guid.NewGuid(),
                LoginId = "admin",
                PasswordHash = "admin123",
                DisplayName = "Administrator",
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                UserId = Guid.NewGuid(),
                LoginId = "operator",
                PasswordHash = "operator123",
                DisplayName = "Operator",
                IsActive = true,
                CreatedAt = now
            },
            new()
            {
                UserId = Guid.NewGuid(),
                LoginId = "viewer",
                PasswordHash = "viewer123",
                DisplayName = "Viewer",
                IsActive = true,
                CreatedAt = now
            }
        };

        db.Users.AddRange(users);
        db.SaveChanges();

        var roleMap = db.Roles.ToDictionary(r => r.RoleName, r => r.RoleId, StringComparer.OrdinalIgnoreCase);
        if (roleMap.Count == 0)
        {
            return;
        }

        db.UserRoles.AddRange(
            new UserRoleEntity { UserId = users[0].UserId, RoleId = roleMap["Administrator"], AssignedAt = now },
            new UserRoleEntity { UserId = users[1].UserId, RoleId = roleMap["Operator"], AssignedAt = now },
            new UserRoleEntity { UserId = users[2].UserId, RoleId = roleMap["Viewer"], AssignedAt = now });

        db.SaveChanges();
    }
}
