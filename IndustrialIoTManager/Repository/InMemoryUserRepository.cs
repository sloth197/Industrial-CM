using System.Collections.Generic;
using IndustrialIoTManager.Model;

namespace IndustrialIoTManager.Repository;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<UserAccount> _users =
    [
        new UserAccount { UserName = "admin", Password = "admin123", Role = "Administrator" },
        new UserAccount { UserName = "operator", Password = "operator123", Role = "Operator" },
        new UserAccount { UserName = "viewer", Password = "viewer123", Role = "Viewer" }
    ];

    public IReadOnlyList<UserAccount> GetUsers()
    {
        return _users;
    }
}
