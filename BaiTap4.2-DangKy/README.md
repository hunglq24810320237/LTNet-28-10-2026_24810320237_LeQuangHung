# Bài tập 4.2 — Form đăng ký học viên + SQL Server

Ứng dụng Windows Forms .NET 10 lưu học viên vào database `ql_sinhvien`. Dữ liệu chính được ghi vào `dbo.SinhVien`; bảng `dbo.DangKyHocVien` lưu điện thoại, khóa học và các lựa chọn, liên kết với `SinhVien`. Danh sách khóa học lấy từ `dbo.KhoaHoc`.

## Chạy ứng dụng

```powershell
dotnet run --project RegistrationApp.csproj
```

Mặc định kết nối `localhost\SQLEXPRESS` bằng Windows Authentication. Ứng dụng tự tạo database, bảng và dữ liệu mẫu khi chạy lần đầu. Mã sinh viên mới được sinh tự động theo dạng `SV004`, `SV005`, ... Có thể đặt biến môi trường `BAITAP_SQLSERVER_SERVER` nếu SQL Server dùng instance khác.

Cũng có thể chạy script T-SQL chung trong SSMS: [`../Database/ql_sinhvien.sql`](../Database/ql_sinhvien.sql).

## Tính năng

- `MaskedTextBox` theo mẫu `(000) 000-0000`; ngày sinh hiển thị `dd/MM/yyyy`.
- `ComboBox` binding với `DisplayMember=Name`, `ValueMember=Id`; khóa học được nạp từ SQL Server.
- Chọn giới tính bằng RadioButton, hình thức học và tài liệu bằng CheckBox.
- Kiểm tra dữ liệu, ghi hồ sơ sinh viên và chi tiết đăng ký trong cùng một transaction SQL Server, hiển thị mã sinh viên và bản tóm tắt qua MessageBox.
