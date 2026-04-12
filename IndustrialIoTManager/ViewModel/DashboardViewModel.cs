using System;
using System.Collections.ObjectModel;
using IndustrialIoTManager.ViewModel.Base;

namespace IndustrialIoTManager.ViewModel;

public sealed class DashboardViewModel : ViewModelBase
{
    public DashboardViewModel()
    {
        LastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        KpiItems =
        [
            new DashboardKpiItem("금일 생산 실적", "12,480 EA", "+4.2% vs 전일", "#0F766E"),
            new DashboardKpiItem("양품률", "98.7%", "+0.4pp", "#0369A1"),
            new DashboardKpiItem("설비 가동률", "94.6%", "-1.1pp", "#B45309"),
            new DashboardKpiItem("출하 대기 오더", "17 건", "긴급 3건", "#B91C1C")
        ];

        ProductionOrders =
        [
            new ProductionOrderStatusItem("PO-260406-01", "A-라인", "Servo Motor", "진행중", "73%", "18:30"),
            new ProductionOrderStatusItem("PO-260406-02", "B-라인", "Inverter Module", "지연", "56%", "17:40"),
            new ProductionOrderStatusItem("PO-260406-03", "C-라인", "Heat Exchanger", "완료", "100%", "15:10"),
            new ProductionOrderStatusItem("PO-260406-04", "D-라인", "Pump Housing", "진행중", "64%", "20:00")
        ];

        InventoryItems =
        [
            new InventoryStatusItem("AL6061-BLK", 3200, 2500, 78),
            new InventoryStatusItem("STEEL-S45C", 4200, 2800, 92),
            new InventoryStatusItem("BEARING-6206", 980, 1200, 48),
            new InventoryStatusItem("CABLE-4P-20M", 530, 500, 60)
        ];

        AlertItems =
        [
            new AlertItem("High", "열처리로 2호기 온도 편차 경보", "10:42", "#DC2626"),
            new AlertItem("Medium", "A-라인 계획 대비 생산량 8% 미달", "09:55", "#D97706"),
            new AlertItem("Low", "창고 3구역 재고 실사 예정", "09:10", "#0284C7")
        ];

        QuickActions =
        [
            new QuickActionItem("긴급 오더 PO-260406-02 원자재 배정", "구매팀 / 김OO", "11:30"),
            new QuickActionItem("B-라인 야간조 작업 지시 확정", "생산관리 / 박OO", "13:00"),
            new QuickActionItem("월간 품질 리포트 승인", "품질팀 / 이OO", "15:00")
        ];
    }

    public string Title => "산업 통합 운영 대시보드";
    public string Subtitle => "생산, 품질, 재고, 설비 현황을 한 화면에서 관리";
    public string LastUpdated { get; }

    public ObservableCollection<DashboardKpiItem> KpiItems { get; }
    public ObservableCollection<ProductionOrderStatusItem> ProductionOrders { get; }
    public ObservableCollection<InventoryStatusItem> InventoryItems { get; }
    public ObservableCollection<AlertItem> AlertItems { get; }
    public ObservableCollection<QuickActionItem> QuickActions { get; }
}

public sealed class DashboardKpiItem(string name, string value, string trend, string trendColor)
{
    public string Name { get; } = name;
    public string Value { get; } = value;
    public string Trend { get; } = trend;
    public string TrendColor { get; } = trendColor;
}

public sealed class ProductionOrderStatusItem(
    string orderNo,
    string line,
    string product,
    string status,
    string progress,
    string dueTime)
{
    public string OrderNo { get; } = orderNo;
    public string Line { get; } = line;
    public string Product { get; } = product;
    public string Status { get; } = status;
    public string Progress { get; } = progress;
    public string DueTime { get; } = dueTime;
}

public sealed class InventoryStatusItem(string materialCode, int onHand, int safetyStock, double fillRate)
{
    public string MaterialCode { get; } = materialCode;
    public int OnHand { get; } = onHand;
    public int SafetyStock { get; } = safetyStock;
    public double FillRate { get; } = fillRate;
    public string OnHandSummary => $"현재고 {OnHand:N0} / 안전재고 {SafetyStock:N0}";
    public string FillRateText => $"{FillRate:0}%";
}

public sealed class AlertItem(string severity, string message, string time, string severityColor)
{
    public string Severity { get; } = severity;
    public string Message { get; } = message;
    public string Time { get; } = time;
    public string SeverityColor { get; } = severityColor;
}

public sealed class QuickActionItem(string title, string owner, string dueTime)
{
    public string Title { get; } = title;
    public string Owner { get; } = owner;
    public string DueTime { get; } = dueTime;
}
