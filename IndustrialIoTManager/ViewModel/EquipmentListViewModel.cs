using System;
using System.Collections.ObjectModel;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class EquipmentListViewModel : ViewModelBase
{
    public EquipmentListViewModel()
    {
        Equipments =
        [
            new EquipmentItem { Id = 1, Name = "Mixer-01", Status = "Running", LastHeartbeat = DateTime.Now.AddSeconds(-12) },
            new EquipmentItem { Id = 2, Name = "Conveyor-02", Status = "Running", LastHeartbeat = DateTime.Now.AddSeconds(-8) },
            new EquipmentItem { Id = 3, Name = "Press-03", Status = "Idle", LastHeartbeat = DateTime.Now.AddMinutes(-1) },
            new EquipmentItem { Id = 4, Name = "RobotArm-04", Status = "Maintenance", LastHeartbeat = DateTime.Now.AddMinutes(-7) }
        ];
    }

    public string Title => "설비/장비 목록 관리";

    public ObservableCollection<EquipmentItem> Equipments { get; }
}
