# Bài tập 4.1 — Login Form chuẩn UX + SQLite

Ứng dụng Windows Forms .NET 10 lưu tài khoản trong SQLite tại `%LocalAppData%/BaiTap4.1Login/login.db`.

## Chạy ứng dụng

```powershell
dotnet run --project LoginApp.csproj
```

Lần chạy đầu tạo CSDL và tài khoản mẫu `student` / `123456`. Mật khẩu được băm PBKDF2-SHA256 và lưu kèm salt, không lưu dạng văn bản thuần.

## Tính năng

- `txtUsername`, `txtPassword`, `chkShowPassword`, `btnLogin`, `btnExit`, `ErrorProvider`.
- Enter đăng nhập, Esc thoát; hiện/ẩn mật khẩu bằng `UseSystemPasswordChar`.
- Kiểm tra trường bắt buộc bằng ErrorProvider và xác thực thông tin với SQLite.
