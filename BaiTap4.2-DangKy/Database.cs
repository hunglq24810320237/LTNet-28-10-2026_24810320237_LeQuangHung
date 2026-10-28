using Microsoft.Data.SqlClient;
using System.Data;

namespace RegistrationApp;

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
    }

    public static List<Course> GetCourses()
    {
        using var connection = new SqlConnection(Builder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MaKhoaHoc, TenKhoaHoc, HocPhi FROM dbo.KhoaHoc ORDER BY MaKhoaHoc;";
        using var reader = command.ExecuteReader();
        var courses = new List<Course>();
        while (reader.Read()) courses.Add(new Course { Id = reader.GetInt32(0), Name = reader.GetString(1), Fee = reader.GetInt32(2) });
        return courses;
    }

    public static Registration Save(Registration registration)
    {
        using var connection = new SqlConnection(Builder(DatabaseName).ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
        try
        {
            using var studentCommand = connection.CreateCommand();
            studentCommand.Transaction = transaction;
            studentCommand.CommandText = "SELECT ISNULL(MAX(TRY_CONVERT(INT, SUBSTRING(MaSV, 3, 18))), 0) + 1 FROM dbo.SinhVien WITH (UPDLOCK, HOLDLOCK) WHERE MaSV LIKE 'SV[0-9]%';";
            var nextNumber = Convert.ToInt32(studentCommand.ExecuteScalar());
            registration.StudentId = $"SV{nextNumber:D3}";
            studentCommand.CommandText = "INSERT dbo.SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB) VALUES (@id, @name, @birth, @gender, @class, NULL);";
            studentCommand.Parameters.AddWithValue("@id", registration.StudentId);
            studentCommand.Parameters.AddWithValue("@name", registration.FullName);
            studentCommand.Parameters.AddWithValue("@birth", registration.BirthDate.Date);
            studentCommand.Parameters.AddWithValue("@gender", registration.Gender);
            studentCommand.Parameters.AddWithValue("@class", registration.CourseName);
            studentCommand.ExecuteNonQuery();

            using var registrationCommand = connection.CreateCommand();
            registrationCommand.Transaction = transaction;
            registrationCommand.CommandText = "INSERT dbo.DangKyHocVien (MaSV, SoDienThoai, MaKhoaHoc, HocTrucTuyen, NhanTaiLieu) OUTPUT INSERTED.MaDangKy VALUES (@student, @phone, @course, @online, @materials);";
            registrationCommand.Parameters.AddWithValue("@student", registration.StudentId);
            registrationCommand.Parameters.AddWithValue("@phone", registration.Phone);
            registrationCommand.Parameters.AddWithValue("@course", registration.CourseId);
            registrationCommand.Parameters.AddWithValue("@online", registration.IsOnline);
            registrationCommand.Parameters.AddWithValue("@materials", registration.NeedsMaterials);
            registration.Id = Convert.ToInt64(registrationCommand.ExecuteScalar());
            transaction.Commit();
            return registration;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public static int Count() { using var connection = new SqlConnection(Builder(DatabaseName).ConnectionString); connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "SELECT COUNT(*) FROM dbo.DangKyHocVien;"; return Convert.ToInt32(command.ExecuteScalar()); }
}

internal sealed class Course
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public int Fee { get; init; }
    public override string ToString() => Name;
}

internal sealed class Registration
{
    public long Id { get; set; }
    public string StudentId { get; set; } = "";
    public string FullName { get; init; } = "";
    public string Phone { get; init; } = "";
    public DateTime BirthDate { get; init; }
    public string Gender { get; init; } = "";
    public int CourseId { get; init; }
    public string CourseName { get; init; } = "";
    public bool IsOnline { get; init; }
    public bool NeedsMaterials { get; init; }
}
