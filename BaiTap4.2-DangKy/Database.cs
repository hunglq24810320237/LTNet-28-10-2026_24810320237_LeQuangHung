using MySql.Data.MySqlClient;
using System.Data;

namespace RegistrationApp;

internal static class Database
{
    private const string DatabaseName = "ql_sinhvien";

    private static string ConnectionString => new MySqlConnectionStringBuilder
    {
        Server = Environment.GetEnvironmentVariable("BAITAP_MYSQL_SERVER") ?? "127.0.0.1",
        Port = uint.TryParse(Environment.GetEnvironmentVariable("BAITAP_MYSQL_PORT"), out var port) ? port : 3307,
        UserID = Environment.GetEnvironmentVariable("BAITAP_MYSQL_USER") ?? "root",
        Password = Environment.GetEnvironmentVariable("BAITAP_MYSQL_PASSWORD") ?? string.Empty,
        Database = DatabaseName,
        CharacterSet = "utf8mb4"
    }.ConnectionString;

    public static void Initialize()
    {
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS SinhVien (
                MaSV VARCHAR(20) PRIMARY KEY,
                HoTen VARCHAR(100) NOT NULL,
                NgaySinh DATE NULL,
                GioiTinh VARCHAR(10) NULL,
                Lop VARCHAR(50) NULL,
                DiemTB DECIMAL(4,2) NULL
            ) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
            CREATE TABLE IF NOT EXISTS KhoaHoc (
                MaKhoaHoc INT AUTO_INCREMENT PRIMARY KEY,
                TenKhoaHoc VARCHAR(100) NOT NULL UNIQUE,
                HocPhi INT NOT NULL
            ) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
            CREATE TABLE IF NOT EXISTS DangKyHocVien (
                MaDangKy BIGINT AUTO_INCREMENT PRIMARY KEY,
                MaSV VARCHAR(20) NOT NULL,
                SoDienThoai VARCHAR(20) NOT NULL,
                MaKhoaHoc INT NOT NULL,
                HocTrucTuyen BOOLEAN NOT NULL,
                NhanTaiLieu BOOLEAN NOT NULL,
                NgayDangKy DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT FK_DangKy_SinhVien FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
                CONSTRAINT FK_DangKy_KhoaHoc FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc)
            ) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
            CREATE TABLE IF NOT EXISTS TaiKhoan (
                TenDangNhap VARCHAR(100) PRIMARY KEY,
                MatKhauHash VARBINARY(32) NOT NULL,
                Salt VARBINARY(16) NOT NULL,
                MaSV VARCHAR(20) NULL,
                NgayTao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT FK_TaiKhoan_SinhVien FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV)
            ) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
            INSERT IGNORE INTO SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB) VALUES
                ('SV001', 'Nguyễn Văn An', '2003-05-15', 'Nam', 'CNTT K18', 8.50),
                ('SV002', 'Trần Thị Bình', '2003-08-20', 'Nữ', 'CNTT K18', 9.20),
                ('SV003', 'Lê Hoàng Cường', '2002-11-10', 'Nam', 'Kế Toán K18', 7.40);
            INSERT IGNORE INTO KhoaHoc (MaKhoaHoc, TenKhoaHoc, HocPhi) VALUES
                (1, 'Lập trình C# căn bản', 2500000),
                (2, 'Phát triển ứng dụng .NET', 3500000),
                (3, 'Cơ sở dữ liệu SQL', 2800000),
                (4, 'Thiết kế giao diện WinForms', 2200000);
            """;
        command.ExecuteNonQuery();
    }

    public static List<Course> GetCourses()
    {
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MaKhoaHoc, TenKhoaHoc, HocPhi FROM KhoaHoc ORDER BY MaKhoaHoc;";
        using var reader = command.ExecuteReader();
        var courses = new List<Course>();
        while (reader.Read()) courses.Add(new Course { Id = reader.GetInt32(0), Name = reader.GetString(1), Fee = reader.GetInt32(2) });
        return courses;
    }

    public static Registration Save(Registration registration)
    {
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
        var lockAcquired = false;
        try
        {
            using (var lockCommand = connection.CreateCommand())
            {
                lockCommand.Transaction = transaction;
                lockCommand.CommandText = "SELECT GET_LOCK('ql_sinhvien_student_id', 10);";
                lockAcquired = Convert.ToInt32(lockCommand.ExecuteScalar()) == 1;
            }
            if (!lockAcquired) throw new TimeoutException("Không thể tạo mã sinh viên, vui lòng thử lại.");

            using (var studentCommand = connection.CreateCommand())
            {
                studentCommand.Transaction = transaction;
                studentCommand.CommandText = "SELECT COALESCE(MAX(CAST(SUBSTRING(MaSV, 3) AS UNSIGNED)), 0) + 1 FROM SinhVien WHERE MaSV REGEXP '^SV[0-9]+$';";
                var nextNumber = Convert.ToInt32(studentCommand.ExecuteScalar());
                registration.StudentId = $"SV{nextNumber:D3}";
                studentCommand.CommandText = "INSERT INTO SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Lop, DiemTB) VALUES (@id, @name, @birth, @gender, @class, NULL);";
                studentCommand.Parameters.AddWithValue("@id", registration.StudentId);
                studentCommand.Parameters.AddWithValue("@name", registration.FullName);
                studentCommand.Parameters.AddWithValue("@birth", registration.BirthDate.Date);
                studentCommand.Parameters.AddWithValue("@gender", registration.Gender);
                studentCommand.Parameters.AddWithValue("@class", registration.CourseName);
                studentCommand.ExecuteNonQuery();
            }

            using (var registrationCommand = connection.CreateCommand())
            {
                registrationCommand.Transaction = transaction;
                registrationCommand.CommandText = "INSERT INTO DangKyHocVien (MaSV, SoDienThoai, MaKhoaHoc, HocTrucTuyen, NhanTaiLieu) VALUES (@student, @phone, @course, @online, @materials); SELECT LAST_INSERT_ID();";
                registrationCommand.Parameters.AddWithValue("@student", registration.StudentId);
                registrationCommand.Parameters.AddWithValue("@phone", registration.Phone);
                registrationCommand.Parameters.AddWithValue("@course", registration.CourseId);
                registrationCommand.Parameters.AddWithValue("@online", registration.IsOnline);
                registrationCommand.Parameters.AddWithValue("@materials", registration.NeedsMaterials);
                registration.Id = Convert.ToInt64(registrationCommand.ExecuteScalar());
            }

            transaction.Commit();
            return registration;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            if (lockAcquired)
            {
                using var releaseCommand = connection.CreateCommand();
                releaseCommand.CommandText = "SELECT RELEASE_LOCK('ql_sinhvien_student_id');";
                releaseCommand.ExecuteScalar();
            }
        }
    }

    public static int Count()
    {
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM DangKyHocVien;";
        return Convert.ToInt32(command.ExecuteScalar());
    }
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
