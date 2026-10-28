using Microsoft.Data.SqlClient;
using System.Security.Cryptography;

namespace LoginApp;

internal static class Database
{
    private const string DatabaseName = "BaiTap41Login";

    private static SqlConnectionStringBuilder CreateBuilder(string database) => new()
    {
        DataSource = Environment.GetEnvironmentVariable("BAITAP_SQLSERVER_SERVER") ?? @".\SQLEXPRESS",
        InitialCatalog = database,
        IntegratedSecurity = true,
        Encrypt = true,
        TrustServerCertificate = true,
        ConnectTimeout = 10
    };

    public static void Initialize()
    {
        using (var master = new SqlConnection(CreateBuilder("master").ConnectionString))
        {
            master.Open();
            using var createDatabase = master.CreateCommand();
            createDatabase.CommandText = "DECLARE @sql NVARCHAR(MAX); IF DB_ID(@databaseName) IS NULL BEGIN SET @sql = N'CREATE DATABASE ' + QUOTENAME(@databaseName); EXEC(@sql); END;";
            createDatabase.Parameters.AddWithValue("@databaseName", DatabaseName);
            createDatabase.ExecuteNonQuery();
        }

        using var connection = new SqlConnection(CreateBuilder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Users
                (
                    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
                    Username NVARCHAR(100) NOT NULL,
                    PasswordHash NVARCHAR(128) NOT NULL,
                    Salt NVARCHAR(64) NOT NULL,
                    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
                    CONSTRAINT UQ_Users_Username UNIQUE (Username)
                );
            END;
            """;
        command.ExecuteNonQuery();

        command.CommandText = "IF NOT EXISTS (SELECT 1 FROM dbo.Users) INSERT INTO dbo.Users (Username, PasswordHash, Salt) VALUES (@username, @hash, @salt);";
        var salt = RandomNumberGenerator.GetBytes(16);
        command.Parameters.AddWithValue("@username", "student");
        command.Parameters.AddWithValue("@hash", Hash("123456", salt));
        command.Parameters.AddWithValue("@salt", Convert.ToBase64String(salt));
        command.ExecuteNonQuery();
    }

    public static bool Validate(string username, string password)
    {
        using var connection = new SqlConnection(CreateBuilder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT PasswordHash, Salt FROM dbo.Users WHERE Username = @username;";
        command.Parameters.AddWithValue("@username", username);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return false;
        var stored = Convert.FromBase64String(reader.GetString(0));
        var salt = Convert.FromBase64String(reader.GetString(1));
        return CryptographicOperations.FixedTimeEquals(stored, Convert.FromBase64String(Hash(password, salt)));
    }

    private static string Hash(string password, byte[] salt) => Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA256, 32));
}
