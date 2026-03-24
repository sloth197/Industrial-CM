using System;
using System.Collections.ObjectModel;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class AlarmEventLogViewModel : ViewModelBase
{
    public AlarmEventLogViewModel()
    {
        Events =
        [
            new AlarmEvent { OccurredAt = DateTime.Now.AddMinutes(-2), Severity = "High", Source = "Press-03", Message = "압력 임계치 초과" },
            new AlarmEvent { OccurredAt = DateTime.Now.AddMinutes(-11), Severity = "Medium", Source = "Mixer-01", Message = "온도 상승 경향 감지" },
            new AlarmEvent { OccurredAt = DateTime.Now.AddMinutes(-20), Severity = "Low", Source = "RobotArm-04", Message = "예방 점검 주기 도래" }
        ];
    }

    public string Title => "알람/이벤트 로그 조회";

    public ObservableCollection<AlarmEvent> Events { get; }
}
