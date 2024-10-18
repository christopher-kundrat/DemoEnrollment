using DemoEnrollmentSystem.DataServices;
using DemoEnrollmentSystem.Interfaces;
using DemoEnrollmentSystem.Services;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// One utitlizes an interface the other does not. I'm just showing off the different ways
builder.Services.AddScoped<IDemoEnrollmentService, DemoEnrollmentService>();
builder.Services.AddScoped<DemoEnrollmentDataService>();

// Read the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton<SqliteConnection>(_ =>
    new SqliteConnection(connectionString));


var app = builder.Build();

Batteries.Init();

//Creating a SQLite DB if one doesn't already exist
CreateDB();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void CreateDB()
{
    var dbPath = Path.Combine(Environment.CurrentDirectory, "Data", "demoEnrollment.db");

    //Check to make sure it doesn't already exist
    if (!File.Exists(dbPath))
    {
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            // Creating the tables as described in the schema
            var createTables = @"
                CREATE TABLE IF NOT EXISTS students (
                    student_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    email TEXT UNIQUE NOT NULL
                );

                CREATE TABLE IF NOT EXISTS professors (
                    professor_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    email TEXT UNIQUE NOT NULL
                );

                CREATE TABLE IF NOT EXISTS courses (
                    course_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    course_name TEXT NOT NULL,
                    professor_id INTEGER,
                    FOREIGN KEY (professor_id) REFERENCES professors(professor_id)
                );

                CREATE TABLE IF NOT EXISTS enrollments (
                    enrollment_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    student_id INTEGER,
                    course_id INTEGER,
                    UNIQUE(student_id, course_id),
                    FOREIGN KEY (student_id) REFERENCES students(student_id),
                    FOREIGN KEY (course_id) REFERENCES courses(course_id)
                );

                CREATE TABLE IF NOT EXISTS grades (
                    grade_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    enrollment_id INTEGER,
                    grade DECIMAL(5,2) CHECK(grade >= 0 AND grade <= 100),
                    FOREIGN KEY (enrollment_id) REFERENCES enrollments(enrollment_id)
                );
            ";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = createTables;
                command.ExecuteNonQuery();
            }

            // Insert Dummy Data for testing purposes
            var insertData = @"
                INSERT INTO students (name, email) VALUES 
                    ('Alice Johnson', 'alice@example.com'),
                    ('Bob Smith', 'bob@example.com'),
                    ('John Smith', 'johnsmith@example.com'),
                    ('Emily Johnson', 'emilyjohnson@example.com'),
                    ('Michael Brown', 'michaelbrown@example.com'),
                    ('Sarah Davis', 'sarahdavis@example.com'),
                    ('David Wilson', 'davidwilson@example.com'),
                    ('Laura Miller', 'lauramiller@example.com'),
                    ('James Taylor', 'jamestaylor@example.com'),
                    ('Jessica Anderson', 'jessicaanderson@example.com'),
                    ('Daniel Martinez', 'danielmartinez@example.com'),
                    ('Linda Thomas', 'lindathomas@example.com');

                INSERT INTO professors (name, email) VALUES 
                    ('Dr. Emily Brown', 'emily@example.com'),
                    ('Dr. John Green', 'john@example.com'),
                    ('Professor Alice Thompson', 'alicethompson@example.com'),
                    ('Professor Mark Reynolds', 'markreynolds@example.com'),
                    ('Dr. Susan Carter', 'susancarter@example.com'),
                    ('Professor Robert Lewis', 'robertlewis@example.com');

                INSERT INTO courses (course_name, professor_id) VALUES 
                    ('Computer Science 101', 1),
                    ('Mathematics 101', 2),
                    ('English 103', 3),
                    ('Biology 101', 4),
                    ('History 102', 5),
                    ('Photography 101', 6);
            ";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = insertData;
                command.ExecuteNonQuery();
            }
        }
    }
}
