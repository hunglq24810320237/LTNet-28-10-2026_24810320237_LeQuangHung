using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace LoginApp;

internal static class Database
{
    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BaiTap4.1Login");
    private static readonly string ConnectionString = new SqliteConnectionStringBuilder { DataSource = Path.Combine(Folder, "login.db") }.ToString();

    public static void Initialize()
    {
        Directory.CreateDirectory(Folder);
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY, Username TEXT NOT NULL UNIQUE COLLATE NOCASE, PasswordHash TEXT NOT NULL, Salt TEXT NOT NULL, CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);";
        command.ExecuteNonQuery();
        command.CommandText = "SELECT COUNT(*) FROM Users;";
        if (Convert.ToInt32(command.ExecuteScalar()) == 0)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            command.CommandText = "INSERT INTO Users (Username, PasswordHash, Salt) VALUES ($user, $hash, $salt);";
            command.Parameters.AddWithValue("$user", "student");
            command.Parameters.AddWithValue("$hash", Hash("123456", salt));
            command.Parameters.AddWithValue("$salt", Convert.ToBase64String(salt));
            command.ExecuteNonQuery();
        }
    }

    public static bool Validate(string username, string password)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT PasswordHash, Salt FROM Users WHERE Username = $user COLLATE NOCASE LIMIT 1;";
        command.Parameters.AddWithValue("$user", username);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return false;
        var stored = Convert.FromBase64String(reader.GetString(0));
        var salt = Convert.FromBase64String(reader.GetString(1));
        return CryptographicOperations.FixedTimeEquals(stored, Convert.FromBase64String(Hash(password, salt)));
    }

    private static string Hash(string password, byte[] salt) => Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, salt, 210_000, HashAlgorithmName.SHA256, 32));
}
