using Microsoft.Data.Sqlite;
using DemoEnrollmentSystem.Entities;
using Microsoft.Extensions.Logging;
using DemoEnrollmentSystem.Controllers;

namespace DemoEnrollmentSystem.DataServices
{
    public class DemoEnrollmentDataService
    {
        private readonly SqliteConnection _connection;
        private readonly ILogger<DemoEnrollmentController> _logger;

        public DemoEnrollmentDataService(SqliteConnection connection, ILogger<DemoEnrollmentController> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public List<AvailableCourse> getAvailableCoursesData()
        {
            var courses = new List<AvailableCourse>();

            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT c.course_id, c.course_name, p.name FROM courses c INNER JOIN professors p ON c.professor_id = p.professor_id ";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var availableCourse = new AvailableCourse
                        {
                            course_id = reader.GetInt32(0),
                            course_name = reader.GetString(1),
                            professor_name = reader.GetString(2)
                        };
                        courses.Add(availableCourse);
                    }
                }
            }

            _connection.Close();

            return courses;
        }

        public List<AverageGrade> getAverageGradeData()
        {
            var averageGrades = new List<AverageGrade>();

            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT c.course_name, AVG(g.grade)
                    FROM grades g INNER JOIN enrollments e ON g.enrollment_id = e.enrollment_id 
                    INNER JOIN  courses c ON e.course_id = c.course_id 
                    GROUP BY c.course_name";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var averageGrade = new AverageGrade
                        {
                            course_name = reader.GetString(0),
                            average_grade = reader.GetInt32(1),

                        };
                        averageGrades.Add(averageGrade);
                    }
                }
            }

            _connection.Close();

            return averageGrades;
        }

        public bool isValidStudent (int student_id)
        {
            // We'll check if this student shows up with this id.
            int count = 0;

            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM students WHERE student_id = $student_id";
                cmd.Parameters.AddWithValue("student_id", student_id);
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }

            _connection.Close();

            return count > 0; 

        }

        public bool isValidCourse(int course_id)
        {
            // We'll check if this student shows up with this id.
            int count = 0;

            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM courses WHERE course_id = $course_id";
                cmd.Parameters.AddWithValue("course_id", course_id);
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }

            _connection.Close();

            return count > 0;

        }

        public bool isStudentEnrolled(int student_id, int course_id)
        {
            // We'll check if this student shows up with this id.
            int count = 0;

            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM enrollments WHERE student_id = $student_id AND course_id = $course_id";
                cmd.Parameters.AddWithValue("student_id", student_id);
                cmd.Parameters.AddWithValue("course_id", course_id);
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }

            _connection.Close();

            return count > 0;

        }
        public bool isValidEnrollment(int enrollment_id)
        {
            // We'll check if this enrollment shows up with this id.
            int count = 0;

            _connection.Open();
            using (var cmd = _connection.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM enrollments WHERE enrollment_id = $enrollment_id";
                cmd.Parameters.AddWithValue("enrollment_id", enrollment_id);
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }

            _connection.Close();

            return count > 0;

        }

        public bool addStudentEnrollment(int student_id, int course_id)
        {
            try
            {

                // We'll check if this student shows up with this id.
                int rowsUpdated = 0;

                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO enrollments (student_id, course_id) VALUES ($student_id, $course_id)";
                    cmd.Parameters.AddWithValue("student_id", student_id);
                    cmd.Parameters.AddWithValue("course_id", course_id);

                    rowsUpdated = cmd.ExecuteNonQuery();
                }

                _connection.Close();

                return rowsUpdated > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred: {ex.Message}");
                return false; 
            }
        }

        public bool assignGrade(int enrollment_id, decimal grade)
        {
            try
            {
                int rowsUpdated = 0;
                int count = 0;
                
                // First Check to see if their is already a grade
                _connection.Open();
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT count(*) FROM grades WHERE enrollment_id = $enrollment_id";
                    cmd.Parameters.AddWithValue("enrollment_id", enrollment_id);
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                _connection.Close();

                // if a grade alreaady exists update it, otherwise add a new record
                if (count > 0)
                {
                    _connection.Open();
                    using (var cmd = _connection.CreateCommand())
                    {
                        cmd.CommandText = "UPDATE grades SET grade = $grade WHERE enrollment_id = $enrollment_id";
                        cmd.Parameters.AddWithValue("enrollment_id", enrollment_id);
                        cmd.Parameters.AddWithValue("grade", grade);

                        rowsUpdated = cmd.ExecuteNonQuery();
                    }

                    _connection.Close();

                } else
                {
                    _connection.Open();
                    using (var cmd = _connection.CreateCommand())
                    {
                        cmd.CommandText = "INSERT INTO grades (enrollment_id, grade) VALUES ($enrollment_id, $grade)";
                        cmd.Parameters.AddWithValue("enrollment_id", enrollment_id);
                        cmd.Parameters.AddWithValue("grade", grade);

                        rowsUpdated = cmd.ExecuteNonQuery();
                    }

                    _connection.Close();
                }

                return rowsUpdated > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
