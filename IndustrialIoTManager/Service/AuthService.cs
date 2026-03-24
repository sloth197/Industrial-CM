using System;
using System.Linq;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.Repository;

namespace IndustrialIoTManager.Service;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool ValidateCredentials(string userName, string password, out UserAccount? user)
    {
        user = _userRepository.GetUsers().FirstOrDefault(u =>
            string.Equals(u.UserName, userName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(u.Password, password, StringComparison.Ordinal));

        return user is not null;
    }
}
