# Frontend — Task 3

`SentinelHome.Desktop` là WPF client độc lập. Nó không tham chiếu DbContext hoặc PostgreSQL; mọi dữ liệu đi qua REST/SignalR của API.

## Giao diện

Desktop app sử dụng phong cách **security operations console**: live feed với bounding-box mô phỏng ở trung tâm, danh sách cảnh báo đang mở, metric cards, biểu đồ nhịp phát hiện 24 giờ và activity feed. Các màn hình **Tổng quan**, **Lịch sử sự kiện** và **Cấu hình hệ thống** đều chạy được với dữ liệu mock.

Toàn bộ màu là semantic token trong `App.xaml`: nền navy sâu, mint cho trạng thái an toàn/live, amber cho cần chú ý, coral cho nguy cơ. Giao diện dùng icon Windows Fluent nhất quán và các control quan trọng có nhãn trợ năng.

## Đăng nhập

Ứng dụng luôn mở màn hình đăng nhập trước. Sau khi thành công, JWT chỉ được giữ trong bộ nhớ và được tự gắn vào các HTTP request bảo vệ; đóng app hoặc bấm **Đăng xuất** sẽ xóa token.

Mặc định `Api:UseMockData` là `true`: frontend tự dùng `MockAuthService` và `MockHealthApiClient`, không gọi API/PostgreSQL. Đăng nhập UI demo bằng `admin` / `demo123`. Khi Task 2 sẵn sàng, đổi thành `false` để dùng `AuthService` và API thật.

Môi trường Development có tài khoản seed lần đầu:

- Username: `admin`
- Password: `SentinelHome123!`

Đây chỉ là thông tin demo. Đổi `DevelopmentSeed` và `Jwt:Key` trước khi đưa bản build cho người khác dùng.
