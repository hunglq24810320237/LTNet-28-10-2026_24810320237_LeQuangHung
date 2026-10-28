-- Chạy script này trên MySQL Workbench hoặc phpMyAdmin (XAMPP).
-- Dữ liệu mẫu và cấu trúc bảng SinhVien theo đề bài của giảng viên.
CREATE DATABASE IF NOT EXISTS ql_sinhvien
    CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE ql_sinhvien;

CREATE TABLE IF NOT EXISTS SinhVien (
    MaSV VARCHAR(20) PRIMARY KEY,
    HoTen VARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh VARCHAR(10),
    Lop VARCHAR(50),
    DiemTB DECIMAL(4,2)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB) VALUES
('SV001', 'Nguyễn Văn An', '2003-05-15', 'Nam', 'CNTT K18', 8.50),
('SV002', 'Trần Thị Bình', '2003-08-20', 'Nữ', 'CNTT K18', 9.20),
('SV003', 'Lê Hoàng Cường', '2002-11-10', 'Nam', 'Kế Toán K18', 7.40);

-- Bảng phục vụ ComboBox khóa học của Bài 4.2.
CREATE TABLE IF NOT EXISTS KhoaHoc (
    MaKhoaHoc INT AUTO_INCREMENT PRIMARY KEY,
    TenKhoaHoc VARCHAR(100) NOT NULL UNIQUE,
    HocPhi INT NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT IGNORE INTO KhoaHoc (TenKhoaHoc, HocPhi) VALUES
('Lập trình C# căn bản', 2500000),
('Phát triển ứng dụng .NET', 3500000),
('Cơ sở dữ liệu SQL', 2800000),
('Thiết kế giao diện WinForms', 2200000);

-- Chi tiết lựa chọn trong Form đăng ký; SinhVien giữ hồ sơ theo cấu trúc đề bài.
CREATE TABLE IF NOT EXISTS DangKyHocVien (
    MaDangKy BIGINT AUTO_INCREMENT PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    SoDienThoai VARCHAR(20) NOT NULL,
    MaKhoaHoc INT NOT NULL,
    HocTrucTuyen BOOLEAN NOT NULL DEFAULT FALSE,
    NhanTaiLieu BOOLEAN NOT NULL DEFAULT FALSE,
    NgayDangKy DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_DangKy_SinhVien FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    CONSTRAINT FK_DangKy_KhoaHoc FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Bài 4.1 lưu hash PBKDF2 và salt, không lưu mật khẩu dạng chữ thường.
CREATE TABLE IF NOT EXISTS TaiKhoan (
    TenDangNhap VARCHAR(100) PRIMARY KEY,
    MatKhauHash VARBINARY(32) NOT NULL,
    Salt VARBINARY(16) NOT NULL,
    MaSV VARCHAR(20),
    NgayTao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_TaiKhoan_SinhVien FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

SELECT MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB
FROM SinhVien
ORDER BY MaSV;
