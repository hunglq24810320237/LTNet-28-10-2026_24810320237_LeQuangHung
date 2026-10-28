# QUẢN LÝ SINH VIÊN - MYSQL DATABASE

## I. THÔNG TIN SINH VIÊN

* **Họ và tên:** Lê Quang Hưng
* **Mã số sinh viên:** 24810320237
* **Lớp:** D19QTANM1
* **Môn học:** Lập trình .NET / Windows Forms
* **Chủ đề:** LTNet-28-10-2026_24810320237_LeQuangHung

---

## II. GIỚI THIỆU

Ứng dụng Windows Forms quản lý hồ sơ sinh viên bằng MySQL. Chương trình tải danh sách sinh viên khi mở form và hỗ trợ thêm, cập nhật, xóa, chọn dòng để xem chi tiết, kiểm tra dữ liệu nhập bằng `ErrorProvider`.

### Công nghệ

* C# Windows Forms, .NET 10
* MySQL và thư viện `MySql.Data`
* Bảng `SinhVien` theo cấu trúc CSDL giảng viên cung cấp

---

## III. CẤU TRÚC CƠ SỞ DỮ LIỆU

Database: `ql_sinhvien` (charset `utf8mb4`, collation `utf8mb4_unicode_ci`).

| Cột | Kiểu dữ liệu | Mô tả |
|---|---|---|
| `MaSV` | `VARCHAR(20)` | Mã sinh viên, khóa chính |
| `HoTen` | `VARCHAR(100)` | Họ tên, bắt buộc |
| `NgaySinh` | `DATE` | Ngày sinh |
| `GioiTinh` | `VARCHAR(10)` | Giới tính |
| `Lop` | `VARCHAR(50)` | Lớp học |
| `DiemTB` | `DECIMAL(4,2)` | Điểm trung bình |

Script tạo database, bảng và ba dòng dữ liệu mẫu ở [src/Database/ql_sinhvien.sql](src/Database/ql_sinhvien.sql).

---

## IV. CHỨC NĂNG

1. Hiển thị dữ liệu `SinhVien` trong `DataGridView`.
2. Thêm sinh viên mới; báo lỗi nếu thiếu mã, họ tên hoặc lớp, và báo nếu trùng mã.
3. Cập nhật thông tin sinh viên được chọn.
4. Xóa sinh viên sau khi xác nhận.
5. Nút làm mới xóa dữ liệu trên form để nhập bản ghi mới.
6. Hiển thị ngày sinh theo `dd/MM/yyyy`; điểm trung bình trong khoảng 0–10.

---

## V. HÌNH ẢNH THAM KHẢO

Các hình dưới đây là ảnh tham khảo giao diện mẫu đã được cung cấp trong yêu cầu. Chúng không phải ảnh chụp phiên chạy của mã nguồn trong repository này.

### 1. Giao diện và danh sách sinh viên

![Ảnh tham khảo giao diện quản lý sinh viên](docs/bai_db_ui.png)

### 2. Kiểm tra dữ liệu nhập

![Ảnh tham khảo ErrorProvider](docs/bai_db_error.png)

### 3. Thông báo thêm sinh viên thành công

![Ảnh tham khảo thêm mới thành công](docs/bai_db_result.png)

---

