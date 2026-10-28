using Microsoft.Data.Sqlite;

namespace RegistrationApp;

internal static class Database
{
    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BaiTap4.2Registration");
    private static readonly string ConnectionString = new SqliteConnectionStringBuilder { DataSource = Path.Combine(Folder, "registrations.db") }.ToString();

    public static void Initialize()
    {
        Directory.CreateDirectory(Folder);
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            PRAGMA foreign_keys = ON;
            CREATE TABLE IF NOT EXISTS Courses (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL UNIQUE, Fee INTEGER NOT NULL);
            CREATE TABLE IF NOT EXISTS Registrations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                Phone TEXT NOT NULL,
                BirthDate TEXT NOT NULL,
                Gender TEXT NOT NULL,
                CourseId INTEGER NOT NULL REFERENCES Courses(Id),
                IsOnline INTEGER NOT NULL,
                NeedsMaterials INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
            INSERT OR IGNORE INTO Courses (Id, Name, Fee) VALUES (1, 'Lập trình C# căn bản', 2500000), (2, 'Phát triển ứng dụng .NET', 3500000), (3, 'Cơ sở dữ liệu SQL', 2800000), (4, 'Thiết kế giao diện WinForms', 2200000);
            """;
        command.ExecuteNonQuery();
    }

    public static void Save(Registration registration)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Registrations (FullName, Phone, BirthDate, Gender, CourseId, IsOnline, NeedsMaterials) VALUES ($name, $phone, $birth, $gender, $course, $online, $materials); SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$name", registration.FullName);
        command.Parameters.AddWithValue("$phone", registration.Phone);
        command.Parameters.AddWithValue("$birth", registration.BirthDate.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$gender", registration.Gender);
        command.Parameters.AddWithValue("$course", registration.CourseId);
        command.Parameters.AddWithValue("$online", registration.IsOnline ? 1 : 0);
        command.Parameters.AddWithValue("$materials", registration.NeedsMaterials ? 1 : 0);
        registration.Id = Convert.ToInt64(command.ExecuteScalar());
    }

    public static int Count() { using var connection = new SqliteConnection(ConnectionString); connection.Open(); using var command = connection.CreateCommand(); command.CommandText = "SELECT COUNT(*) FROM Registrations;"; return Convert.ToInt32(command.ExecuteScalar()); }
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
    public string FullName { get; init; } = "";
    public string Phone { get; init; } = "";
    public DateTime BirthDate { get; init; }
    public string Gender { get; init; } = "";
    public int CourseId { get; init; }
    public string CourseName { get; init; } = "";
    public bool IsOnline { get; init; }
    public bool NeedsMaterials { get; init; }
}
