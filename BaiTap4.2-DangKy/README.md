# Bài tập 4.2 — Form đăng ký học viên + SQLite

Ứng dụng Windows Forms .NET 10 dùng SQLite. CSDL đặt tại `%LocalAppData%/BaiTap4.2Registration/registrations.db`; bảng `Courses` lưu khóa học, bảng `Registrations` lưu hồ sơ và liên kết khóa học bằng khóa ngoại.

## Chạy ứng dụng

```powershell
dotnet run --project RegistrationApp.csproj
```

## Tính năng

- `MaskedTextBox` theo mẫu `(000) 000-0000`; ngày sinh hiển thị `dd/MM/yyyy`.
- `ComboBox` binding với `DisplayMember=Name`, `ValueMember=Id`.
- Chọn giới tính bằng RadioButton, hình thức học và tài liệu bằng CheckBox.
- Kiểm tra dữ liệu, ghi hồ sơ vào SQLite, hiển thị bản tóm tắt qua MessageBox và mã đăng ký.
