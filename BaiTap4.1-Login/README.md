# Bài tập 4.1 — Login Form chuẩn UX + SQL Server

Ứng dụng Windows Forms .NET 10 xác thực tài khoản từ database `ql_sinhvien`, bảng `dbo.TaiKhoan`. Bảng này liên kết với `dbo.SinhVien` qua mã sinh viên.

## Chạy ứng dụng

```powershell
dotnet run --project LoginApp.csproj
```

Mặc định kết nối `localhost\SQLEXPRESS` bằng Windows Authentication. Hai ứng dụng cùng dùng database `ql_sinhvien`; ứng dụng tự tạo các bảng còn thiếu và dữ liệu mẫu khi chạy lần đầu. Có thể đặt biến môi trường `BAITAP_SQLSERVER_SERVER` nếu SQL Server dùng instance khác. Tài khoản mẫu `student` / `123456` được khởi tạo khi chưa có tài khoản. Mật khẩu được băm PBKDF2-SHA256 và lưu kèm salt.

Cũng có thể chạy script T-SQL chung trong SSMS: [`../Database/ql_sinhvien.sql`](../Database/ql_sinhvien.sql). Script giữ database `ql_sinhvien`, bảng `SinhVien` và ba dòng dữ liệu mẫu của đề bài.

## Tính năng

- `txtUsername`, `txtPassword`, `chkShowPassword`, `btnLogin`, `btnExit`, `ErrorProvider`.
- Enter đăng nhập, Esc thoát; hiện/ẩn mật khẩu bằng `UseSystemPasswordChar`.
- Kiểm tra trường bắt buộc bằng ErrorProvider và xác thực thông tin với SQL Server.
