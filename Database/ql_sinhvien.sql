/* T-SQL script for SQL Server Management Studio.
   It keeps the teacher's database/table/sample records and adds the two assignment tables. */

IF DB_ID(N'ql_sinhvien') IS NULL
    CREATE DATABASE ql_sinhvien;
GO

USE ql_sinhvien;
GO

IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SinhVien
    (
        MaSV VARCHAR(20) NOT NULL CONSTRAINT PK_SinhVien PRIMARY KEY,
        HoTen NVARCHAR(100) NOT NULL,
        NgaySinh DATE NULL,
        GioiTinh NVARCHAR(10) NULL,
        Lop NVARCHAR(50) NULL,
        DiemTB DECIMAL(4,2) NULL
    );
END;
GO

/* Use Unicode columns/literals so Vietnamese names are preserved in SQL Server. */
ALTER TABLE dbo.SinhVien ALTER COLUMN HoTen NVARCHAR(100) NOT NULL;
ALTER TABLE dbo.SinhVien ALTER COLUMN GioiTinh NVARCHAR(10) NULL;
ALTER TABLE dbo.SinhVien ALTER COLUMN Lop NVARCHAR(50) NULL;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV001')
    INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB)
    VALUES ('SV001', N'Nguyễn Văn An', '2003-05-15', N'Nam', N'CNTT K18', 8.50);
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV002')
    INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB)
    VALUES ('SV002', N'Trần Thị Bình', '2003-08-20', N'Nữ', N'CNTT K18', 9.20);
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV003')
    INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB)
    VALUES ('SV003', N'Lê Hoàng Cường', '2002-11-10', N'Nam', N'Kế Toán K18', 7.40);
UPDATE dbo.SinhVien SET HoTen=N'Nguyễn Văn An', NgaySinh='2003-05-15', GioiTinh=N'Nam', Lop=N'CNTT K18', DiemTB=8.50 WHERE MaSV='SV001';
UPDATE dbo.SinhVien SET HoTen=N'Trần Thị Bình', NgaySinh='2003-08-20', GioiTinh=N'Nữ', Lop=N'CNTT K18', DiemTB=9.20 WHERE MaSV='SV002';
UPDATE dbo.SinhVien SET HoTen=N'Lê Hoàng Cường', NgaySinh='2002-11-10', GioiTinh=N'Nam', Lop=N'Kế Toán K18', DiemTB=7.40 WHERE MaSV='SV003';
GO

IF OBJECT_ID(N'dbo.KhoaHoc', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.KhoaHoc
    (
        MaKhoaHoc INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhoaHoc PRIMARY KEY,
        TenKhoaHoc NVARCHAR(100) NOT NULL CONSTRAINT UQ_KhoaHoc_Ten UNIQUE,
        HocPhi INT NOT NULL
    );
END;
IF OBJECT_ID(N'dbo.DangKyHocVien', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DangKyHocVien
    (
        MaDangKy INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DangKyHocVien PRIMARY KEY,
        MaSV VARCHAR(20) NOT NULL CONSTRAINT FK_DangKy_SinhVien REFERENCES dbo.SinhVien(MaSV),
        SoDienThoai NVARCHAR(20) NOT NULL,
        MaKhoaHoc INT NOT NULL CONSTRAINT FK_DangKy_KhoaHoc REFERENCES dbo.KhoaHoc(MaKhoaHoc),
        HocTrucTuyen BIT NOT NULL,
        NhanTaiLieu BIT NOT NULL,
        NgayDangKy DATETIME2 NOT NULL CONSTRAINT DF_DangKy_Ngay DEFAULT SYSDATETIME()
    );
END;
IF OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TaiKhoan
    (
        TenDangNhap NVARCHAR(100) NOT NULL CONSTRAINT PK_TaiKhoan PRIMARY KEY,
        MatKhauHash NVARCHAR(128) NOT NULL,
        Salt NVARCHAR(64) NOT NULL,
        MaSV VARCHAR(20) NULL CONSTRAINT FK_TaiKhoan_SinhVien REFERENCES dbo.SinhVien(MaSV),
        NgayTao DATETIME2 NOT NULL CONSTRAINT DF_TaiKhoan_Ngay DEFAULT SYSUTCDATETIME()
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc)
    INSERT dbo.KhoaHoc (TenKhoaHoc, HocPhi) VALUES
        (N'Lập trình C# căn bản', 2500000),
        (N'Phát triển ứng dụng .NET', 3500000),
        (N'Cơ sở dữ liệu SQL', 2800000),
        (N'Thiết kế giao diện WinForms', 2200000);
UPDATE dbo.KhoaHoc SET TenKhoaHoc=N'Lập trình C# căn bản', HocPhi=2500000 WHERE MaKhoaHoc=1;
UPDATE dbo.KhoaHoc SET TenKhoaHoc=N'Phát triển ứng dụng .NET', HocPhi=3500000 WHERE MaKhoaHoc=2;
UPDATE dbo.KhoaHoc SET TenKhoaHoc=N'Cơ sở dữ liệu SQL', HocPhi=2800000 WHERE MaKhoaHoc=3;
UPDATE dbo.KhoaHoc SET TenKhoaHoc=N'Thiết kế giao diện WinForms', HocPhi=2200000 WHERE MaKhoaHoc=4;
GO

SELECT MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB
FROM dbo.SinhVien
ORDER BY MaSV;
GO
