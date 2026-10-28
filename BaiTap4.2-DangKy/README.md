# Bài tập 4.2 — Form đăng ký học viên + SQL Server

Ứng dụng Windows Forms .NET 10 dùng SQL Server với database `BaiTap42Registration`. Bảng `dbo.Courses` lưu khóa học; bảng `dbo.Registrations` lưu hồ sơ và liên kết khóa học bằng khóa ngoại.

## Chạy ứng dụng

```powershell
dotnet run --project RegistrationApp.csproj
```

Mặc định kết nối `localhost\SQLEXPRESS` bằng Windows Authentication. Ứng dụng tự tạo database, bảng và dữ liệu khóa học mẫu khi chạy lần đầu. Có thể đặt biến môi trường `BAITAP_SQLSERVER_SERVER` nếu SQL Server dùng instance khác.

Cũng có thể chạy script trong SSMS: [`../Database/BaiTap42Registration.sql`](../Database/BaiTap42Registration.sql).

## Tính năng

- `MaskedTextBox` theo mẫu `(000) 000-0000`; ngày sinh hiển thị `dd/MM/yyyy`.
- `ComboBox` binding với `DisplayMember=Name`, `ValueMember=Id`.
- Danh sách khóa học được đọc từ SQL Server và bind vào ComboBox bằng DisplayMember / ValueMember.
- Chọn giới tính bằng RadioButton, hình thức học và tài liệu bằng CheckBox.
- Kiểm tra dữ liệu, ghi hồ sơ vào SQL Server, hiển thị bản tóm tắt qua MessageBox và mã đăng ký.
