# BÁO CÁO BÀI TẬP THỰC HÀNH LẬP TRÌNH C# WINFORMS

## I. THÔNG TIN SINH VIÊN
* **Họ và tên:** Lê Quang Hưng
* **Mã số sinh viên:** 24810320237
* **Lớp học phần:** Lập trình .NET - Mã Lớp: d19qtanm1

* **Link Repository:** https://github.com/hunglq24810320237/LTNet-28-10-2026_24810320237_LeQuangHung

---

## II. NỘI DUNG BÀI TẬP
* **Bài tập 4.1:** Xây dựng Form đăng nhập chuẩn UX và xác thực tài khoản từ SQL Server.
* **Bài tập 4.2:** Xây dựng Form đăng ký học viên, binding khóa học và lưu hồ sơ vào SQL Server.
* Cả hai bài cùng sử dụng database `ql_sinhvien` theo cấu trúc của giảng viên; bảng `SinhVien` giữ ba dòng dữ liệu mẫu ban đầu.

---

## III. MINH HỌA GIAO DIỆN & KỸ THUẬT NỔI BẬT

### 1. Bài 4.1: Xây dựng Form Đăng nhập (Login Form)
* **Kỹ thuật áp dụng:** `ErrorProvider` kiểm tra tên đăng nhập và mật khẩu; `AcceptButton` / `CancelButton` cho phím Enter và Esc; `UseSystemPasswordChar` để hiện/ẩn mật khẩu.
* **Lưu trữ CSDL:** SQL Server database `ql_sinhvien`, bảng `dbo.TaiKhoan`; mật khẩu lưu dạng PBKDF2-SHA256 có salt. Tài khoản mẫu `student` / `123456` gắn với `SV001`.
* **Tài khoản mẫu:** `student` / `123456` (được khởi tạo khi chạy ứng dụng lần đầu).
* **Hướng dẫn chạy:** [BaiTap4.1-Login/README.md](BaiTap4.1-Login/README.md)

* **Hình ảnh minh họa giao diện:**

![Giao diện chính Bài 4.1](docs/bai4_1_login.png)
*Mô tả: Form đăng nhập với trường tên đăng nhập, mật khẩu, tùy chọn hiện mật khẩu và các nút thao tác.*

![Bắt lỗi Validation Bài 4.1](docs/bai4_1_error.png)
*Mô tả: ErrorProvider hiển thị lỗi khi để trống tên đăng nhập và mật khẩu.*

---

### 2. Bài 4.2: Form Đăng ký Học viên
* **Kỹ thuật áp dụng:** `MaskedTextBox` với mask `(000) 000-0000`, `DateTimePicker` định dạng `dd/MM/yyyy`, `ComboBox` binding qua `DisplayMember` / `ValueMember`, `RadioButton` chọn giới tính và `CheckBox` chọn hình thức học / tài liệu.
* **Lưu trữ CSDL:** Khóa học đọc từ `dbo.KhoaHoc`; hồ sơ sinh viên được ghi vào `dbo.SinhVien`, chi tiết form được ghi vào `dbo.DangKyHocVien` có khóa ngoại.
* **Kết quả:** Kiểm tra dữ liệu bắt buộc, sinh mã `SV004` trở lên, lưu hồ sơ và chi tiết đăng ký trong một transaction, hiển thị thông tin trên `MessageBox`.
* **Hướng dẫn chạy:** [BaiTap4.2-DangKy/README.md](BaiTap4.2-DangKy/README.md)

* **Hình ảnh minh họa giao diện:**

![Giao diện chính Bài 4.2](docs/bai4_2_main.png)
*Mô tả: Form đăng ký với thông tin cá nhân, danh sách khóa học đã nạp từ SQL Server và các lựa chọn đăng ký.*

![Bắt lỗi Validation Bài 4.2](docs/bai4_2_error.png)
*Mô tả: ErrorProvider yêu cầu nhập họ tên và số điện thoại theo đúng mask.*

---

### Cấu hình và chạy chương trình
* **Công nghệ:** C# Windows Forms, .NET 10, Microsoft SQL Server.
* **Yêu cầu:** Windows, .NET 10 SDK và SQL Server (mặc định `localhost\SQLEXPRESS`).
* Script T-SQL tạo database, bảng và dữ liệu mẫu: [`Database/ql_sinhvien.sql`](Database/ql_sinhvien.sql). Cú pháp đã chuyển sang T-SQL để chạy trong SSMS; dữ liệu và tên trường theo đề bài.

