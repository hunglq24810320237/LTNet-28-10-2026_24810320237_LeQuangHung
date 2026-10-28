using Microsoft.Data.SqlClient;

namespace RegistrationApp;

internal static class Database
{
    private const string DatabaseName = "BaiTap42Registration";

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
            IF OBJECT_ID(N'dbo.Courses', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Courses
                (
                    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Courses PRIMARY KEY,
                    Name NVARCHAR(150) NOT NULL CONSTRAINT UQ_Courses_Name UNIQUE,
                    Fee INT NOT NULL CONSTRAINT CK_Courses_Fee CHECK (Fee >= 0)
                );
            END;

            IF OBJECT_ID(N'dbo.Registrations', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Registrations
                (
                    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Registrations PRIMARY KEY,
                    FullName NVARCHAR(150) NOT NULL,
                    Phone NVARCHAR(20) NOT NULL,
                    BirthDate DATE NOT NULL,
                    Gender NVARCHAR(10) NOT NULL,
                    CourseId INT NOT NULL,
                    IsOnline BIT NOT NULL,
                    NeedsMaterials BIT NOT NULL,
                    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Registrations_CreatedAt DEFAULT SYSDATETIME(),
                    CONSTRAINT FK_Registrations_Courses FOREIGN KEY (CourseId) REFERENCES dbo.Courses(Id)
                );
            END;

            IF NOT EXISTS (SELECT 1 FROM dbo.Courses)
            BEGIN
                INSERT INTO dbo.Courses (Name, Fee) VALUES
                    (N'Lập trình C# căn bản', 2500000),
                    (N'Phát triển ứng dụng .NET', 3500000),
                    (N'Cơ sở dữ liệu SQL', 2800000),
                    (N'Thiết kế giao diện WinForms', 2200000);
            END;
            """;
        command.ExecuteNonQuery();
    }

    public static List<Course> GetCourses()
    {
        using var connection = new SqlConnection(CreateBuilder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Fee FROM dbo.Courses ORDER BY Id;";
        using var reader = command.ExecuteReader();
        var courses = new List<Course>();
        while (reader.Read()) courses.Add(new Course { Id = reader.GetInt32(0), Name = reader.GetString(1), Fee = reader.GetInt32(2) });
        return courses;
    }

    public static int Save(Registration registration)
    {
        using var connection = new SqlConnection(CreateBuilder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO dbo.Registrations (FullName, Phone, BirthDate, Gender, CourseId, IsOnline, NeedsMaterials)
            OUTPUT INSERTED.Id
            VALUES (@name, @phone, @birth, @gender, @course, @online, @materials);
            """;
        command.Parameters.AddWithValue("@name", registration.FullName);
        command.Parameters.AddWithValue("@phone", registration.Phone);
        command.Parameters.AddWithValue("@birth", registration.BirthDate.Date);
        command.Parameters.AddWithValue("@gender", registration.Gender);
        command.Parameters.AddWithValue("@course", registration.CourseId);
        command.Parameters.AddWithValue("@online", registration.IsOnline);
        command.Parameters.AddWithValue("@materials", registration.NeedsMaterials);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public static int Count()
    {
        using var connection = new SqlConnection(CreateBuilder(DatabaseName).ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM dbo.Registrations;";
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
    public string FullName { get; init; } = "";
    public string Phone { get; init; } = "";
    public DateTime BirthDate { get; init; }
    public string Gender { get; init; } = "";
    public int CourseId { get; init; }
    public string CourseName { get; init; } = "";
    public bool IsOnline { get; init; }
    public bool NeedsMaterials { get; init; }
}
