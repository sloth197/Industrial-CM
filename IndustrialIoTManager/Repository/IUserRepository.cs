using System.Collections.Generic;
using IndustrialIoTManager.Model;

namespace IndustrialIoTManager.Repository;

public interface IUserRepository
{
    IReadOnlyList<UserAccount> GetUsers();
}
