using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SentinelHome.Contracts;
using SentinelHome.Desktop.Models;
using SentinelHome.Desktop.Services;

namespace SentinelHome.Desktop.ViewModels;

public partial class MainViewModel(
    IHealthApiClient healthApiClient,
    IAuthService authService,
    IWindowNavigator windowNavigator,
    IFrontendMode frontendMode) : ObservableObject
{
    [ObservableProperty] private string _currentPage = "Dashboard";
    [ObservableProperty] private bool _isCheckingConnection = true;
    [ObservableProperty] private string _apiStatusText = "Đang kiểm tra nguồn dữ liệu…";
    [ObservableProperty] private Brush _apiStatusBrush = Brushes.Goldenrod;
    [ObservableProperty] private int _unreadAlertCount;
    [ObservableProperty] private string _signedInUsername = "";
    [ObservableProperty] private string _signedInRole = "";
    [ObservableProperty] private bool _isAdmin;
    [ObservableProperty] private string _lastSyncedText = "Vừa cập nhật";
    [ObservableProperty] private DashboardAlert? _selectedAlert;
    [ObservableProperty] private string _historySearchText = "";
    [ObservableProperty] private string _historyFilter = "Tất cả";
    [ObservableProperty] private bool _isSoundEnabled = true;
    [ObservableProperty] private double _minimumConfidence = 0.75;
    [ObservableProperty] private bool _isDemoLoopEnabled = true;
    [ObservableProperty] private string _settingsSavedText = "Chưa có thay đổi cần lưu.";

    public ObservableCollection<DashboardMetric> Metrics { get; } =
    [
        new("", "Sự kiện hôm nay", "47", "+12% so với hôm qua", StatusTone.Info),
        new("", "Cảnh báo đang mở", "3", "01 mức nghiêm trọng", StatusTone.Danger),
        new("", "Camera trực tuyến", "01 / 01", "Nguồn demo ổn định", StatusTone.Success),
        new("", "Thời gian phản hồi", "1.2s", "Từ phát hiện đến cảnh báo", StatusTone.Warning)
    ];

    public ObservableCollection<DashboardAlert> ActiveAlerts { get; } = [];

    private readonly List<DashboardAlert> _allAlertsSeed =
    [
        new("evt-001", "High", "Người xuất hiện", "96.1%", "19:41:52", "Camera sân sau",
            "Phát hiện người trong khu vực theo dõi ngoài giờ.", "",
            [new("Người", "96.1%", true), new("Chó", "88.1%", false)],
            [new("NGƯỜI", "96.1%", "High", 0.354, 0.243, 0.125, 0.505),
             new("CHÓ", "88.1%", "Medium", 0.6, 0.495, 0.154, 0.281)]),
        new("evt-002", "Medium", "Chó xuất hiện", "88.1%", "19:41:48", "Camera sân sau",
            "Đối tượng động vật đang đi qua vùng giám sát.", "",
            [new("Chó", "88.1%", false)],
            [new("CHÓ", "88.1%", "Medium", 0.6, 0.495, 0.154, 0.281)]),
        new("evt-003", "Low", "Mèo xuất hiện", "78.4%", "19:36:12", "Camera sân sau",
            "Sự kiện thông tin, không yêu cầu phản hồi khẩn.", "",
            [new("Mèo", "78.4%", false)],
            [new("MÈO", "78.4%", "Low", 0.09, 0.55, 0.14, 0.3)])
    ];

    private readonly List<RecentDetection> _allDetections =
    [
        new("19:41:52", "Người", "Camera sân sau", "96.1%", "High", "Chưa xử lý", "21/09/2026", ""),
        new("19:41:48", "Chó", "Camera sân sau", "88.1%", "Medium", "Chưa xử lý", "21/09/2026", ""),
        new("19:36:12", "Mèo", "Camera sân sau", "78.4%", "Low", "Đã xử lý", "21/09/2026", ""),
        new("19:22:31", "Người", "Camera cổng", "91.3%", "High", "Đã xử lý", "21/09/2026", ""),
        new("18:17:05", "Chim", "Camera sân sau", "67.2%", "Low", "Đã xử lý", "21/09/2026", ""),
        new("16:02:18", "Chó hoang", "Camera sân sau", "83.6%", "Medium", "Đã xử lý", "20/09/2026", "")
    ];

    public ObservableCollection<RecentDetection> RecentDetections { get; } = [];
    public ObservableCollection<RecentDetection> HistoryDetections { get; } = [];

    /// <summary>
    /// Overlay của sự kiện đang hiển thị trên khung video trực tiếp — khi có API/video thật,
    /// service ingest sẽ đẩy overlay mới nhất vào collection này qua SignalR thay vì mock tĩnh.
    /// </summary>
    public ObservableCollection<DetectionOverlay> LiveOverlays { get; } = [];

    public bool IsMockMode => frontendMode.UseMockData;
    public string DataSourceText => IsMockMode
        ? "Chế độ demo · video và sự kiện được mô phỏng cục bộ"
        : "Theo dõi phát hiện mới từ nguồn video";
    public bool HasActiveAlerts => ActiveAlerts.Count > 0;
    public bool HasHistoryResults => HistoryDetections.Count > 0;
    public string SelectedAlertTitle => SelectedAlert?.Species ?? "Chọn một cảnh báo";
    public string SelectedAlertDescription => SelectedAlert?.Description ?? "Chọn một cảnh báo ở danh sách để xem bằng chứng và thao tác xử lý.";
    public string SelectedAlertMetadata => SelectedAlert is null
        ? "Chưa có cảnh báo được chọn"
        : $"{SelectedAlert.Camera} · {SelectedAlert.CapturedAt} · {SelectedAlert.Confidence}";
    public string MinimumConfidenceText => $"{MinimumConfidence:P0}";

    public void LoadSession()
    {
        var session = authService.CurrentSession;
        SignedInUsername = session?.Username ?? "Chưa đăng nhập";
        SignedInRole = session?.Role.ToString() ?? string.Empty;
        IsAdmin = session?.Role == UserRole.Admin;

        ActiveAlerts.Clear();
        foreach (var alert in _allAlertsSeed)
            ActiveAlerts.Add(alert);
        UnreadAlertCount = ActiveAlerts.Count;

        LiveOverlays.Clear();
        foreach (var overlay in _allAlertsSeed[0].Overlays)
            LiveOverlays.Add(overlay);

        RecentDetections.Clear();
        foreach (var detection in _allDetections.Take(5))
            RecentDetections.Add(detection);
        ApplyHistoryFilter();
        SelectedAlert = ActiveAlerts.FirstOrDefault();
    }

    partial void OnSelectedAlertChanged(DashboardAlert? value)
    {
        OnPropertyChanged(nameof(SelectedAlertTitle));
        OnPropertyChanged(nameof(SelectedAlertDescription));
        OnPropertyChanged(nameof(SelectedAlertMetadata));
    }

    partial void OnHistorySearchTextChanged(string value) => ApplyHistoryFilter();
    partial void OnHistoryFilterChanged(string value) => ApplyHistoryFilter();
    partial void OnMinimumConfidenceChanged(double value) => OnPropertyChanged(nameof(MinimumConfidenceText));

    [RelayCommand]
    private void Navigate(string page) => CurrentPage = page;

    [RelayCommand]
    private void SelectAlert(DashboardAlert? alert) => SelectedAlert = alert;

    [RelayCommand]
    private void ViewEventDetail(DashboardAlert? alert)
    {
        var item = alert ?? SelectedAlert;
        if (item is not null)
            windowNavigator.ShowEventDetail(item, AcknowledgeAlert);
    }

    [RelayCommand]
    private void AcknowledgeAlert(DashboardAlert? alert)
    {
        var item = alert ?? SelectedAlert;
        if (item is null || !ActiveAlerts.Remove(item))
            return;

        UnreadAlertCount = ActiveAlerts.Count;
        SelectedAlert = ActiveAlerts.FirstOrDefault();
        OnPropertyChanged(nameof(HasActiveAlerts));
        SettingsSavedText = $"Đã đánh dấu “{item.Species}” lúc {item.CapturedAt} là đã xử lý.";
    }

    /// <summary>
    /// Mô phỏng việc backend gửi SignalR "NewAlert" — dùng để demo AlertPopupWindow khi
    /// chưa có Task 2 thật. Khi nối API thật, SignalRService sẽ gọi thẳng logic tương đương.
    /// </summary>
    [RelayCommand]
    private void SimulateNewAlert()
    {
        var template = _allAlertsSeed[Random.Shared.Next(_allAlertsSeed.Count)];
        var freshAlert = template with
        {
            Id = $"evt-{Guid.NewGuid():N}"[..7],
            CapturedAt = DateTime.Now.ToString("HH:mm:ss")
        };

        ActiveAlerts.Insert(0, freshAlert);
        UnreadAlertCount = ActiveAlerts.Count;
        SelectedAlert = freshAlert;
        OnPropertyChanged(nameof(HasActiveAlerts));
        windowNavigator.ShowAlertPopup(freshAlert, AcknowledgeAlert, ViewEventDetail);
    }

    [RelayCommand]
    private void FilterHistory(string filter) => HistoryFilter = filter;

    [RelayCommand]
    private void SaveSettings()
    {
        SettingsSavedText = $"Đã lưu cấu hình lúc {DateTime.Now:HH:mm:ss}. Ngưỡng cảnh báo: {MinimumConfidence:P0}.";
    }

    [RelayCommand]
    private void Logout()
    {
        authService.SignOut();
        windowNavigator.ShowLoginWindow();
    }

    [RelayCommand]
    private async Task RefreshApiStatusAsync(CancellationToken cancellationToken)
    {
        IsCheckingConnection = true;
        try
        {
            var health = await healthApiClient.GetHealthAsync(cancellationToken);
            ApiStatusText = health?.Status == "mock-ready"
                ? "Dữ liệu mô phỏng đang sẵn sàng"
                : health is null ? "Nguồn dữ liệu không phản hồi" : "API sẵn sàng";
            ApiStatusBrush = health is null ? Brushes.OrangeRed : Brushes.LimeGreen;
            LastSyncedText = $"Cập nhật {DateTime.Now:HH:mm:ss}";
        }
        catch (HttpRequestException)
        {
            ApiStatusText = "Không kết nối được API";
            ApiStatusBrush = Brushes.OrangeRed;
        }
        finally
        {
            IsCheckingConnection = false;
        }
    }

    private void ApplyHistoryFilter()
    {
        var search = HistorySearchText.Trim();
        var items = _allDetections.Where(item =>
            (HistoryFilter == "Tất cả" ||
             HistoryFilter == "Chưa xử lý" && item.Status == "Chưa xử lý" ||
             HistoryFilter == "Đã xử lý" && item.Status == "Đã xử lý") &&
            (string.IsNullOrWhiteSpace(search) ||
             item.Species.Contains(search, StringComparison.OrdinalIgnoreCase) ||
             item.Camera.Contains(search, StringComparison.OrdinalIgnoreCase)));

        HistoryDetections.Clear();
        foreach (var item in items)
            HistoryDetections.Add(item);
        OnPropertyChanged(nameof(HasHistoryResults));
    }
}
