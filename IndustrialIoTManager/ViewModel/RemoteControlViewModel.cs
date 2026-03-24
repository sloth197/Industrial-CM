using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using IndustrialIoTManager.Helpers;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class RemoteControlViewModel : ViewModelBase
{
    private readonly RelayCommand _startCommand;
    private readonly RelayCommand _stopCommand;
    private readonly RelayCommand _resetCommand;

    private string _selectedEquipment = string.Empty;
    private string _commandResult = "장비를 선택한 뒤 명령을 실행하세요.";

    public RemoteControlViewModel()
    {
        EquipmentNames =
        [
            "Mixer-01",
            "Conveyor-02",
            "Press-03",
            "RobotArm-04"
        ];

        _startCommand = new RelayCommand(_ => ExecuteControl("START"), CanControl);
        _stopCommand = new RelayCommand(_ => ExecuteControl("STOP"), CanControl);
        _resetCommand = new RelayCommand(_ => ExecuteControl("RESET"), CanControl);
    }

    public string Title => "장비 원격 제어";

    public ObservableCollection<string> EquipmentNames { get; }

    public string SelectedEquipment
    {
        get => _selectedEquipment;
        set
        {
            if (SetProperty(ref _selectedEquipment, value))
            {
                _startCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
                _resetCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string CommandResult
    {
        get => _commandResult;
        private set => SetProperty(ref _commandResult, value);
    }

    public ICommand StartCommand => _startCommand;
    public ICommand StopCommand => _stopCommand;
    public ICommand ResetCommand => _resetCommand;

    private bool CanControl(object? _)
    {
        return !string.IsNullOrWhiteSpace(SelectedEquipment);
    }

    private void ExecuteControl(string command)
    {
        CommandResult = $"{DateTime.Now:HH:mm:ss} | {SelectedEquipment} -> {command} 명령 전송(더미)";
    }
}
