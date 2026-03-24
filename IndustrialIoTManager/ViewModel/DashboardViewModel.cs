using System;
using System.Collections.ObjectModel;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel()
    {
        SummaryItems =
        [
            "온라인 장비 수: 12",
            "오프라인 장비 수: 1",
            "금일 경보 이벤트: 4",
            "평균 가동률: 94.6%",
            "최근 데이터 수집 시각: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        ];
    }

    public string Title => "메인 대시보드";

    public ObservableCollection<string> SummaryItems { get; }
}
