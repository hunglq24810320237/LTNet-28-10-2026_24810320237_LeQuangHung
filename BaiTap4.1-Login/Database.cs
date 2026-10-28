using Microsoft.Data.SqlClient;
using System.Security.Cryptography;

namespace LoginApp;

internal static class Database
{
    private const string DatabaseName = "ql_sinhvien";
    private static string Server => Environment.GetEnvironmentVariable("BAITAP_SQLSERVER_SERVER") ?? @".\SQLEXPRESS";

    private static SqlConnectionStringBuilder Builder(string database) => new()
    {
        DataSource = Server,
        InitialCatalog = database,
        IntegratedSecurity = true,
        Encrypt = true,
        TrustServerCertificate = true,
        ConnectTimeout = 10
    };

    public static void Initialize()
    {
        using (var master = new SqlConnection(Builder("master").ConnectionString))
        {
            master.Open();
            using var create = master.CreateCommand();
            create.CommandText = "IF DB_ID(N'ql_sinhvien') IS NULL CREATE DATABASE ql_sinhvien;";
            create.ExecuteNonQuery();
        }

        using var connection = new SqlConnection(Builder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            IF OBJECT_ID(N'dbo.SinhVien', N'U') IS NULL
                CREATE TABLE dbo.SinhVien (
                    MaSV VARCHAR(20) NOT NULL CONSTRAINT PK_SinhVien PRIMARY KEY,
                    HoTen NVARCHAR(100) NOT NULL,
                    NgaySinh DATE NULL,
                    GioiTinh NVARCHAR(10) NULL,
                    Lop NVARCHAR(50) NULL,
                    DiemTB DECIMAL(4,2) NULL
                );
            ALTER TABLE dbo.SinhVien ALTER COLUMN HoTen NVARCHAR(100) NOT NULL;
            ALTER TABLE dbo.SinhVien ALTER COLUMN GioiTinh NVARCHAR(10) NULL;
            ALTER TABLE dbo.SinhVien ALTER COLUMN Lop NVARCHAR(50) NULL;
            IF OBJECT_ID(N'dbo.KhoaHoc', N'U') IS NULL
                CREATE TABLE dbo.KhoaHoc (
                    MaKhoaHoc INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhoaHoc PRIMARY KEY,
                    TenKhoaHoc NVARCHAR(100) NOT NULL CONSTRAINT UQ_KhoaHoc_Ten UNIQUE,
                    HocPhi INT NOT NULL
                );
            IF OBJECT_ID(N'dbo.DangKyHocVien', N'U') IS NULL
                CREATE TABLE dbo.DangKyHocVien (
                    MaDangKy INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DangKyHocVien PRIMARY KEY,
                    MaSV VARCHAR(20) NOT NULL CONSTRAINT FK_DangKy_SinhVien REFERENCES dbo.SinhVien(MaSV),
                    SoDienThoai NVARCHAR(20) NOT NULL,
                    MaKhoaHoc INT NOT NULL CONSTRAINT FK_DangKy_KhoaHoc REFERENCES dbo.KhoaHoc(MaKhoaHoc),
                    HocTrucTuyen BIT NOT NULL,
                    NhanTaiLieu BIT NOT NULL,
                    NgayDangKy DATETIME2 NOT NULL CONSTRAINT DF_DangKy_Ngay DEFAULT SYSDATETIME()
                );
            IF OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NULL
                CREATE TABLE dbo.TaiKhoan (
                    TenDangNhap NVARCHAR(100) NOT NULL CONSTRAINT PK_TaiKhoan PRIMARY KEY,
                    MatKhauHash NVARCHAR(128) NOT NULL,
                    Salt NVARCHAR(64) NOT NULL,
                    MaSV VARCHAR(20) NULL CONSTRAINT FK_TaiKhoan_SinhVien REFERENCES dbo.SinhVien(MaSV),
                    NgayTao DATETIME2 NOT NULL CONSTRAINT DF_TaiKhoan_Ngay DEFAULT SYSUTCDATETIME()
                );
            IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV001')
                INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB)
                VALUES ('SV001', N'Nguyễn Văn An', '2003-05-15', N'Nam', N'CNTT K18', 8.50);
            IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV002')
                INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB)
                VALUES ('SV002', N'Trần Thị Bình', '2003-08-20', N'Nữ', N'CNTT K18', 9.20);
            IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV003')
                INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB)
                VALUES ('SV003', N'Lê Hoàng Cường', '2002-11-10', N'Nam', N'Kế Toán K18', 7.40);
            IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc)
                INSERT dbo.KhoaHoc (TenKhoaHoc, HocPhi) VALUES
                    (N'Lập trình C# căn bản', 2500000),
                    (N'Phát triển ứng dụng .NET', 3500000),
                    (N'Cơ sở dữ liệu SQL', 2800000),
                    (N'Thiết kế giao diện WinForms', 2200000);
            """;
        command.ExecuteNonQuery();

        command.CommandText = "IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = @username) INSERT dbo.TaiKhoan (TenDangNhap, MatKhauHash, Salt, MaSV) VALUES (@username, @hash, @salt, 'SV001');";
        var salt = RandomNumberGenerator.GetBytes(16);
        command.Parameters.AddWithValue("@username", "student");
        command.Parameters.AddWithValue("@hash", Hash("123456", salt));
        command.Parameters.AddWithValue("@salt", Convert.ToBase64String(salt));
        command.ExecuteNonQuery();
    }

    public static bool Validate(string username, string password)
    {
        using var connection = new SqlConnection(Builder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MatKhauHash, Salt FROM dbo.TaiKhoan WHERE TenDangNhap = @username;";
        command.Parameters.AddWithValue("@username", username);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return false;
        var stored = Convert.FromBase64String(reader.GetString(0));
        var salt = Convert.FromBase64String(reader.GetString(1));
        return CryptographicOperations.FixedTimeEquals(stored, Convert.FromBase64String(Hash(password, salt)));
    }

    private static string Hash(string password, byte[] salt) => Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA256, 32));
}
