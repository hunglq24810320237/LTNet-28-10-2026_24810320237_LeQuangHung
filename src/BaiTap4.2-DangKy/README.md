# Bài tập 4.2 — Form đăng ký học viên với MySQL

Ứng dụng Windows Forms .NET 10 lưu hồ sơ đăng ký vào database `ql_sinhvien`. Hồ sơ sinh viên lưu trong `SinhVien`; số điện thoại, khóa học và lựa chọn dịch vụ lưu trong `DangKyHocVien`. Khóa học được nạp từ `KhoaHoc`.

## Cơ sở dữ liệu

1. Bật MySQL trong XAMPP.
2. Chạy script [`../Database/ql_sinhvien.sql`](../Database/ql_sinhvien.sql) trong MySQL Workbench hoặc phpMyAdmin.
3. Chạy ứng dụng; mỗi lần đăng ký ghi hồ sơ và chi tiết trong cùng transaction.

Mặc định kết nối `127.0.0.1:3307`, user `root`, mật khẩu trống. Có thể tùy chỉnh `BAITAP_MYSQL_SERVER`, `BAITAP_MYSQL_PORT`, `BAITAP_MYSQL_USER`, `BAITAP_MYSQL_PASSWORD`.

## Tính năng

- `MaskedTextBox` mask `(000) 000-0000`; `DateTimePicker` hiển thị `dd/MM/yyyy`.
- ComboBox binding khóa học qua `DisplayMember` / `ValueMember`.
- RadioButton chọn giới tính; CheckBox chọn hình thức học và nhận tài liệu.
- Validation bằng `ErrorProvider`; tạo mã sinh viên `SV004`, `SV005`, ... và hiển thị tóm tắt đăng ký.

## Chạy

```powershell
dotnet run --project RegistrationApp.csproj
```
