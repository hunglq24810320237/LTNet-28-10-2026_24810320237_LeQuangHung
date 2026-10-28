using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace LoginApp;

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

        var salt = RandomNumberGenerator.GetBytes(16);
        command.CommandText = "INSERT IGNORE INTO TaiKhoan (TenDangNhap, MatKhauHash, Salt, MaSV) VALUES (@username, @hash, @salt, 'SV001');";
        command.Parameters.AddWithValue("@username", "student");
        command.Parameters.AddWithValue("@hash", Hash("123456", salt));
        command.Parameters.AddWithValue("@salt", salt);
        command.ExecuteNonQuery();
    }

    public static bool Validate(string username, string password)
    {
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MatKhauHash, Salt FROM TaiKhoan WHERE TenDangNhap = @username;";
        command.Parameters.AddWithValue("@username", username);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return false;
        var stored = (byte[])reader[0];
        var salt = (byte[])reader[1];
        return CryptographicOperations.FixedTimeEquals(stored, Hash(password, salt));
    }

    private static byte[] Hash(string password, byte[] salt) =>
        Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA256, 32);
}
