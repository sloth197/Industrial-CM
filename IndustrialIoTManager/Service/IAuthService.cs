using IndustrialIoTManager.Model;

namespace IndustrialIoTManager.Service;

public interface IAuthService
{
    bool ValidateCredentials(string userName, string password, out UserAccount? user);
}
