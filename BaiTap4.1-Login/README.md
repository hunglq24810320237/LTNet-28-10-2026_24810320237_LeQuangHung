# Bài tập 4.1 — Login Form chuẩn UX + SQL Server

Ứng dụng Windows Forms .NET 10 lưu tài khoản trên SQL Server trong database `BaiTap41Login`, bảng `dbo.Users`.

## Chạy ứng dụng

```powershell
dotnet run --project LoginApp.csproj
```

Mặc định kết nối `localhost\SQLEXPRESS` bằng Windows Authentication. Ứng dụng tự tạo database và bảng khi chạy lần đầu. Có thể đặt biến môi trường `BAITAP_SQLSERVER_SERVER` nếu SQL Server dùng instance khác. Tài khoản mẫu `student` / `123456` được khởi tạo khi bảng chưa có tài khoản. Mật khẩu được băm PBKDF2-SHA256 và lưu kèm salt, không lưu dạng văn bản thuần.

Có thể chạy script tạo database / bảng trong SSMS từ [`../Database/BaiTap41Login.sql`](../Database/BaiTap41Login.sql).

## Tính năng

- `txtUsername`, `txtPassword`, `chkShowPassword`, `btnLogin`, `btnExit`, `ErrorProvider`.
- Enter đăng nhập, Esc thoát; hiện/ẩn mật khẩu bằng `UseSystemPasswordChar`.
- Kiểm tra trường bắt buộc bằng ErrorProvider và xác thực thông tin với SQL Server.
