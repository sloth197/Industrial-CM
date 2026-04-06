using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IndustrialIoTManager.Helpers;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class RemoteControlViewModel : ViewModelBase
{
    private const double FaultProbability = 0.10;
    private static readonly string[] FaultCatalog =
    [
        "E-341 과전류 보호 동작",
        "E-207 축 온도 상한 초과",
        "E-512 센서 피드백 불일치",
        "E-118 인터락 신호 미확인"
    ];

    private readonly RelayCommand _startCommand;
    private readonly RelayCommand _stopCommand;
    private readonly RelayCommand _resetCommand;
    private readonly Dictionary<string, DemoEquipmentState> _equipmentStates;
    private readonly Random _random = new();

    private string _selectedEquipment = string.Empty;
    private string _commandResult = "장비를 선택한 뒤 명령을 실행하세요.";
    private bool _isCommandInProgress;
    private double _commandProgress;
    private string _commandProgressText = "대기 중";
    private string _selectedLine = "-";
    private string _selectedStatus = "Idle";
    private string _selectedRpm = "0 rpm";
    private string _selectedTemperature = "0.0 °C";
    private string _selectedHeartbeat = "-";
    private int _selectedLatencyMs = 0;
    private string _activeFaultMessage = string.Empty;

    public RemoteControlViewModel()
    {
        _equipmentStates = new Dictionary<string, DemoEquipmentState>(StringComparer.OrdinalIgnoreCase)
        {
            ["Mixer-01"] = new DemoEquipmentState("A-라인", "Running", 1340, 62.1, 18),
            ["Conveyor-02"] = new DemoEquipmentState("B-라인", "Idle", 0, 34.5, 24),
            ["Press-03"] = new DemoEquipmentState("C-라인", "Stopped", 12, 41.2, 27),
            ["RobotArm-04"] = new DemoEquipmentState("D-라인", "Running", 920, 48.9, 16)
        };

        EquipmentNames = new ObservableCollection<string>(_equipmentStates.Keys);

        CommandLogs = new ObservableCollection<string>
        {
            $"{DateTime.Now:HH:mm:ss} | DEMO MODE 활성화: 가상 PLC 응답 시뮬레이션 시작"
        };

        _startCommand = new RelayCommand(_ => _ = ExecuteControlAsync("START"), CanStartOrStopControl);
        _stopCommand = new RelayCommand(_ => _ = ExecuteControlAsync("STOP"), CanStartOrStopControl);
        _resetCommand = new RelayCommand(_ => _ = ExecuteControlAsync("RESET"), CanResetControl);

        SelectedEquipment = EquipmentNames.FirstOrDefault() ?? string.Empty;
    }

    public string Title => "장비 원격 제어";
    public string Subtitle => "DEMO MODE: 명령 전송, 장비 응답, 로그 적재를 가상으로 재현";
    public string DemoScenario => "시나리오: START/STOP 명령 시 10% 확률로 Fault 발생, RESET으로 복구";

    public ObservableCollection<string> EquipmentNames { get; }
    public ObservableCollection<string> CommandLogs { get; }

    public string SelectedEquipment
    {
        get => _selectedEquipment;
        set
        {
            if (SetProperty(ref _selectedEquipment, value))
            {
                RefreshSelectedEquipmentSnapshot();
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

    public bool IsCommandInProgress
    {
        get => _isCommandInProgress;
        private set
        {
            if (SetProperty(ref _isCommandInProgress, value))
            {
                OnPropertyChanged(nameof(CanSelectEquipment));
                _startCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
                _resetCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool CanSelectEquipment => !IsCommandInProgress;

    public double CommandProgress
    {
        get => _commandProgress;
        private set => SetProperty(ref _commandProgress, value);
    }

    public string CommandProgressText
    {
        get => _commandProgressText;
        private set => SetProperty(ref _commandProgressText, value);
    }

    public string SelectedLine
    {
        get => _selectedLine;
        private set => SetProperty(ref _selectedLine, value);
    }

    public string SelectedStatus
    {
        get => _selectedStatus;
        private set
        {
            if (SetProperty(ref _selectedStatus, value))
            {
                OnPropertyChanged(nameof(SelectedStatusColor));
            }
        }
    }

    public string SelectedStatusColor => SelectedStatus switch
    {
        "Running" => "#047857",
        "Idle" => "#0369A1",
        "Stopped" => "#B45309",
        "Fault" => "#B91C1C",
        _ => "#475569"
    };

    public string SelectedRpm
    {
        get => _selectedRpm;
        private set => SetProperty(ref _selectedRpm, value);
    }

    public string SelectedTemperature
    {
        get => _selectedTemperature;
        private set => SetProperty(ref _selectedTemperature, value);
    }

    public string SelectedHeartbeat
    {
        get => _selectedHeartbeat;
        private set => SetProperty(ref _selectedHeartbeat, value);
    }

    public int SelectedLatencyMs
    {
        get => _selectedLatencyMs;
        private set => SetProperty(ref _selectedLatencyMs, value);
    }

    public string ActiveFaultMessage
    {
        get => _activeFaultMessage;
        private set
        {
            if (SetProperty(ref _activeFaultMessage, value))
            {
                OnPropertyChanged(nameof(HasActiveFault));
                _startCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasActiveFault => !string.IsNullOrWhiteSpace(ActiveFaultMessage);

    public ICommand StartCommand => _startCommand;
    public ICommand StopCommand => _stopCommand;
    public ICommand ResetCommand => _resetCommand;

    private bool CanBaseControl()
    {
        return !string.IsNullOrWhiteSpace(SelectedEquipment) && !IsCommandInProgress;
    }

    private bool CanStartOrStopControl(object? _)
    {
        return CanBaseControl() && !HasActiveFault;
    }

    private bool CanResetControl(object? _)
    {
        return CanBaseControl();
    }

    private async Task ExecuteControlAsync(string command)
    {
        if (IsCommandInProgress || !_equipmentStates.TryGetValue(SelectedEquipment, out var state))
        {
            return;
        }

        IsCommandInProgress = true;
        CommandProgress = 0;
        CommandProgressText = $"[{command}] 제어 세션 준비";
        AddLog($"{DateTime.Now:HH:mm:ss} | {SelectedEquipment} -> {command} 요청 접수");

        await AdvanceStepAsync(20, "PLC 세션 연결 확인");
        await AdvanceStepAsync(45, "제어 권한 검증");
        await AdvanceStepAsync(75, "장비 응답 대기");

        var outcome = ApplyCommandEffect(state, command);

        await AdvanceStepAsync(100, outcome.IsFault ? "FAULT 이벤트 처리" : "명령 완료 및 상태 동기화");

        if (outcome.IsFault)
        {
            CommandResult =
                $"{DateTime.Now:HH:mm:ss} | {SelectedEquipment} -> {command} 실패 " +
                $"({outcome.Detail}, 복구 필요: RESET)";
            AddLog($"{DateTime.Now:HH:mm:ss} | [ALARM] {SelectedEquipment} | {outcome.Detail}");
        }
        else
        {
            CommandResult =
                $"{DateTime.Now:HH:mm:ss} | {SelectedEquipment} -> {command} 성공 " +
                $"(상태: {state.Status}, 지연: {state.LatencyMs}ms)";

            if (outcome.IsRecoveredFromFault)
            {
                CommandResult += " | Fault 복구 완료";
                AddLog($"{DateTime.Now:HH:mm:ss} | {SelectedEquipment} Fault 상태 해제");
            }

            AddLog(CommandResult);
        }

        RefreshSelectedEquipmentSnapshot();
        CommandProgress = 0;
        CommandProgressText = "대기 중";
        IsCommandInProgress = false;
    }

    private async Task AdvanceStepAsync(double targetProgress, string text)
    {
        CommandProgressText = text;
        CommandProgress = targetProgress;
        await Task.Delay(_random.Next(240, 460));
    }

    private CommandOutcome ApplyCommandEffect(DemoEquipmentState state, string command)
    {
        state.LastHeartbeat = DateTime.Now;
        state.LatencyMs = _random.Next(14, 58);

        if (command.Equals("RESET", StringComparison.OrdinalIgnoreCase))
        {
            var recoveredFromFault = !string.IsNullOrWhiteSpace(state.FaultMessage);
            state.FaultMessage = string.Empty;
            state.Status = "Idle";
            state.Rpm = 0;
            state.Temperature = Math.Round(_random.NextDouble() * 6 + 33, 1);
            return new CommandOutcome(false, recoveredFromFault, "정상화");
        }

        if (_random.NextDouble() < FaultProbability)
        {
            var faultMessage = FaultCatalog[_random.Next(0, FaultCatalog.Length)];
            state.Status = "Fault";
            state.FaultMessage = faultMessage;
            state.Rpm = _random.Next(0, 40);
            state.Temperature = Math.Round(state.Temperature + (_random.NextDouble() * 8 + 4), 1);
            return new CommandOutcome(true, false, faultMessage);
        }

        state.FaultMessage = string.Empty;
        switch (command)
        {
            case "START":
                state.Status = "Running";
                state.Rpm = _random.Next(980, 1680);
                state.Temperature = Math.Round(_random.NextDouble() * 18 + 52, 1);
                break;
            case "STOP":
                state.Status = "Stopped";
                state.Rpm = _random.Next(0, 50);
                state.Temperature = Math.Round(Math.Max(31.5, state.Temperature - (_random.NextDouble() * 5 + 2)), 1);
                break;
            default:
                state.Status = "Fault";
                break;
        }

        return new CommandOutcome(false, false, string.Empty);
    }

    private void RefreshSelectedEquipmentSnapshot()
    {
        if (!_equipmentStates.TryGetValue(SelectedEquipment, out var state))
        {
            SelectedLine = "-";
            SelectedStatus = "-";
            SelectedRpm = "-";
            SelectedTemperature = "-";
            SelectedHeartbeat = "-";
            SelectedLatencyMs = 0;
            ActiveFaultMessage = string.Empty;
            return;
        }

        SelectedLine = state.Line;
        SelectedStatus = state.Status;
        SelectedRpm = $"{state.Rpm:N0} rpm";
        SelectedTemperature = $"{state.Temperature:0.0} °C";
        SelectedHeartbeat = state.LastHeartbeat.ToString("HH:mm:ss");
        SelectedLatencyMs = state.LatencyMs;
        ActiveFaultMessage = state.FaultMessage;

        _startCommand.RaiseCanExecuteChanged();
        _stopCommand.RaiseCanExecuteChanged();
        _resetCommand.RaiseCanExecuteChanged();
    }

    private void AddLog(string message)
    {
        CommandLogs.Insert(0, message);

        while (CommandLogs.Count > 30)
        {
            CommandLogs.RemoveAt(CommandLogs.Count - 1);
        }
    }

    private sealed class DemoEquipmentState(
        string line,
        string status,
        int rpm,
        double temperature,
        int latencyMs)
    {
        public string Line { get; } = line;
        public string Status { get; set; } = status;
        public int Rpm { get; set; } = rpm;
        public double Temperature { get; set; } = temperature;
        public int LatencyMs { get; set; } = latencyMs;
        public string FaultMessage { get; set; } = string.Empty;
        public DateTime LastHeartbeat { get; set; } = DateTime.Now;
    }

    private readonly record struct CommandOutcome(bool IsFault, bool IsRecoveredFromFault, string Detail);
}
