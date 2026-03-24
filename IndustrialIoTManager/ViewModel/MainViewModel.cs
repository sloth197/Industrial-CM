using System;
using System.Collections.Generic;
using System.Windows.Input;
using IndustrialIoTManager.Helpers;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.Repository;
using IndustrialIoTManager.Service;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class MainViewModel : ViewModelBase
{
    private readonly Dictionary<string, ViewModelBase> _pages;
    private readonly RelayCommand _navigateCommand;
    private readonly RelayCommand _logoutCommand;

    private ViewModelBase? _currentPage;
    private bool _isAuthenticated;
    private string _currentUserName = "미로그인";
    private string _currentUserRole = "-";

    public MainViewModel()
    {
        IUserRepository userRepository = new InMemoryUserRepository();
        IAuthService authService = new AuthService(userRepository);

        var loginViewModel = new LoginViewModel(authService, HandleLoginSuccess);

        _pages = new Dictionary<string, ViewModelBase>(StringComparer.OrdinalIgnoreCase)
        {
            ["Login"] = loginViewModel,
            ["Dashboard"] = new DashboardViewModel(),
            ["EquipmentList"] = new EquipmentListViewModel(),
            ["SensorMonitoring"] = new SensorMonitoringViewModel(),
            ["RemoteControl"] = new RemoteControlViewModel(),
            ["AlarmEventLog"] = new AlarmEventLogViewModel(),
            ["UserRoleManagement"] = new UserRoleManagementViewModel(),
            ["Settings"] = new SettingsViewModel()
        };

        _navigateCommand = new RelayCommand(ExecuteNavigate, CanNavigate);
        _logoutCommand = new RelayCommand(_ => ExecuteLogout(), _ => IsAuthenticated);

        CurrentPage = loginViewModel;
    }

    public ViewModelBase? CurrentPage
    {
        get => _currentPage;
        private set => SetProperty(ref _currentPage, value);
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        private set
        {
            if (SetProperty(ref _isAuthenticated, value))
            {
                _navigateCommand.RaiseCanExecuteChanged();
                _logoutCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string CurrentUserName
    {
        get => _currentUserName;
        private set => SetProperty(ref _currentUserName, value);
    }

    public string CurrentUserRole
    {
        get => _currentUserRole;
        private set => SetProperty(ref _currentUserRole, value);
    }

    public ICommand NavigateCommand => _navigateCommand;
    public ICommand LogoutCommand => _logoutCommand;

    private bool CanNavigate(object? parameter)
    {
        if (parameter is not string targetPage || !_pages.ContainsKey(targetPage))
        {
            return false;
        }

        return IsAuthenticated || targetPage.Equals("Login", StringComparison.OrdinalIgnoreCase);
    }

    private void ExecuteNavigate(object? parameter)
    {
        if (parameter is not string targetPage || !_pages.TryGetValue(targetPage, out var page))
        {
            return;
        }

        if (!IsAuthenticated && !targetPage.Equals("Login", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CurrentPage = page;
    }

    private void HandleLoginSuccess(UserAccount user)
    {
        IsAuthenticated = true;
        CurrentUserName = user.UserName;
        CurrentUserRole = user.Role;
        CurrentPage = _pages["Dashboard"];
    }

    private void ExecuteLogout()
    {
        IsAuthenticated = false;
        CurrentUserName = "미로그인";
        CurrentUserRole = "-";
        CurrentPage = _pages["Login"];
    }
}
