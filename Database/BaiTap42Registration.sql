IF DB_ID(N'BaiTap42Registration') IS NULL
BEGIN
    EXEC(N'CREATE DATABASE [BaiTap42Registration]');
END;
GO
USE BaiTap42Registration;
GO
IF OBJECT_ID(N'dbo.Courses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Courses
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Courses PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL CONSTRAINT UQ_Courses_Name UNIQUE,
        Fee INT NOT NULL CONSTRAINT CK_Courses_Fee CHECK (Fee >= 0)
    );
END;
GO
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
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Courses)
BEGIN
    INSERT INTO dbo.Courses (Name, Fee) VALUES
        (N'Lập trình C# căn bản', 2500000),
        (N'Phát triển ứng dụng .NET', 3500000),
        (N'Cơ sở dữ liệu SQL', 2800000),
        (N'Thiết kế giao diện WinForms', 2200000);
END;
GO
