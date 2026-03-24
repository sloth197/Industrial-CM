using System;
using System.Collections.ObjectModel;
using IndustrialIoTManager.Model;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class SensorMonitoringViewModel : ViewModelBase
{
    public SensorMonitoringViewModel()
    {
        SensorReadings =
        [
            new SensorReading { EquipmentName = "Mixer-01", SensorType = "Temperature", Value = 62.3, Unit = "C", Timestamp = DateTime.Now.AddSeconds(-4) },
            new SensorReading { EquipmentName = "Conveyor-02", SensorType = "Vibration", Value = 1.2, Unit = "mm/s", Timestamp = DateTime.Now.AddSeconds(-6) },
            new SensorReading { EquipmentName = "Press-03", SensorType = "Pressure", Value = 7.8, Unit = "bar", Timestamp = DateTime.Now.AddSeconds(-9) },
            new SensorReading { EquipmentName = "RobotArm-04", SensorType = "Current", Value = 13.5, Unit = "A", Timestamp = DateTime.Now.AddSeconds(-3) }
        ];
    }

    public string Title => "센서 데이터 모니터링";

    public ObservableCollection<SensorReading> SensorReadings { get; }
}
