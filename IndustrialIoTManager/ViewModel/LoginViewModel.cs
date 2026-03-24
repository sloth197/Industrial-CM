using System;
using System.Windows.Input;
using IndustrialIoTManager.Helpers;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.Service;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;
    private readonly Action<UserAccount> _onLoginSuccess;
    private readonly RelayCommand _loginCommand;

    private string _userName = string.Empty;
    private string _password = string.Empty;
    private string _statusMessage = "더미 계정: admin / admin123";

    public LoginViewModel(IAuthService authService, Action<UserAccount> onLoginSuccess)
    {
        _authService = authService;
        _onLoginSuccess = onLoginSuccess;
        _loginCommand = new RelayCommand(ExecuteLogin, CanLogin);
    }

    public string UserName
    {
        get => _userName;
        set
        {
            if (SetProperty(ref _userName, value))
            {
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
            {
                _loginCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public ICommand LoginCommand => _loginCommand;

    private bool CanLogin(object? _)
    {
        return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);
    }

    private void ExecuteLogin(object? _)
    {
        if (_authService.ValidateCredentials(UserName, Password, out var user) && user is not null)
        {
            StatusMessage = $"{user.UserName} 계정으로 로그인 성공";
            _onLoginSuccess(user);
            return;
        }

        StatusMessage = "로그인 실패: 아이디 또는 비밀번호를 확인하세요.";
    }
}
