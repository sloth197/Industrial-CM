using System;
using System.Windows.Input;
using IndustrialIoTManager.Helpers;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class SettingsViewModel : ViewModelBase
{
    private readonly RelayCommand _saveSettingsCommand;

    private string _plcIp = "192.168.0.10";
    private int _plcPort = 502;
    private string _deviceGatewayIp = "192.168.0.30";
    private int _deviceGatewayPort = 1883;
    private string _statusMessage = "기본 연결 설정이 로드되었습니다.";

    public SettingsViewModel()
    {
        _saveSettingsCommand = new RelayCommand(_ => SaveSettings(), _ => CanSaveSettings());
    }

    public string Title => "설정 화면";

    public string PlcIp
    {
        get => _plcIp;
        set
        {
            if (SetProperty(ref _plcIp, value))
            {
                _saveSettingsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public int PlcPort
    {
        get => _plcPort;
        set
        {
            if (SetProperty(ref _plcPort, value))
            {
                _saveSettingsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string DeviceGatewayIp
    {
        get => _deviceGatewayIp;
        set
        {
            if (SetProperty(ref _deviceGatewayIp, value))
            {
                _saveSettingsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public int DeviceGatewayPort
    {
        get => _deviceGatewayPort;
        set
        {
            if (SetProperty(ref _deviceGatewayPort, value))
            {
                _saveSettingsCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage, value);
    }

    public ICommand SaveSettingsCommand => _saveSettingsCommand;

    private bool CanSaveSettings()
    {
        return IsPortValid(PlcPort) &&
               IsPortValid(DeviceGatewayPort) &&
               !string.IsNullOrWhiteSpace(PlcIp) &&
               !string.IsNullOrWhiteSpace(DeviceGatewayIp);
    }

    private void SaveSettings()
    {
        StatusMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} 설정 저장 완료(더미)";
    }

    private static bool IsPortValid(int port)
    {
        return port is > 0 and < 65536;
    }
}
