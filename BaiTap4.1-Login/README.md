# Bài tập 4.1 — Form đăng nhập với MySQL

Ứng dụng Windows Forms .NET 10 xác thực tài khoản từ database `ql_sinhvien`, bảng `TaiKhoan`, liên kết tới bảng `SinhVien`.

## Cơ sở dữ liệu

1. Bật MySQL trong XAMPP.
2. Chạy script [`../Database/ql_sinhvien.sql`](../Database/ql_sinhvien.sql) trong MySQL Workbench hoặc phpMyAdmin.
3. Chạy ứng dụng. Lần đầu chạy, chương trình tạo tài khoản mẫu `student` / `123456` nếu chưa có. Mật khẩu được băm PBKDF2-SHA256 kèm salt.

Mặc định kết nối `127.0.0.1:3307`, user `root`, mật khẩu trống. Có thể tùy chỉnh `BAITAP_MYSQL_SERVER`, `BAITAP_MYSQL_PORT`, `BAITAP_MYSQL_USER`, `BAITAP_MYSQL_PASSWORD`.

## Tính năng

- `ErrorProvider` kiểm tra tên đăng nhập và mật khẩu bắt buộc.
- Enter kích hoạt đăng nhập; Esc đóng form.
- `CheckBox` hiện/ẩn mật khẩu bằng `UseSystemPasswordChar`.
- Xác thực thông tin trực tiếp với MySQL qua thư viện `MySql.Data`.

## Chạy

```powershell
dotnet run --project LoginApp.csproj
```
