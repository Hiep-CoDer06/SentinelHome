# Backend — Task 2

`SentinelHome.Api` là điểm vào duy nhất của backend. Nó tham chiếu `SentinelHome.Data` để lưu PostgreSQL và `SentinelHome.Contracts` để dùng DTO chung với WPF.

Nền tảng hiện đã có: EF Core model 7 bảng, ràng buộc/index cơ bản, `/api/health`, `POST /api/auth/login`, Problem Details, JWT pipeline và SignalR hub `/hubs/alerts`. Các endpoint nghiệp vụ còn lại sẽ được thêm theo tài liệu thiết kế API.

Trong Development, API gọi `EnsureCreated` và tạo một tài khoản `admin` nếu bảng `Users` chưa có dữ liệu. PostgreSQL phải chạy theo `ConnectionStrings:SentinelHome`; nếu không, health check vẫn hoạt động còn login trả HTTP 503 với hướng dẫn cấu hình.

Thiết lập biến bí mật trước khi chạy: dùng user-secrets cho `ConnectionStrings:SentinelHome` và `Jwt:Key`; không đưa password PostgreSQL thật vào `appsettings.json`.
