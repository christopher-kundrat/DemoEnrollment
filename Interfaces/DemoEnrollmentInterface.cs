using DemoEnrollmentSystem.Entities.RequestEntities;
using DemoEnrollmentSystem.Entities.ResponseEntities;

namespace DemoEnrollmentSystem.Interfaces
{
    public interface IDemoEnrollmentService
    {
        GetAvailableCoursesResponse GetAvailableCourses();
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollstudentRequest);
        AssignGradeResponse AssignGrade(AssignGradeRequest assignGradeRequest);
        GetAverageGradeResponse GetAverageGrade();
    }
}
