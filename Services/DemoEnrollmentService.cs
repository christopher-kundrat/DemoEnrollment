using DemoEnrollmentSystem.DataServices;
using DemoEnrollmentSystem.Entities;
using DemoEnrollmentSystem.Entities.RequestEntities;
using DemoEnrollmentSystem.Entities.ResponseEntities;
using DemoEnrollmentSystem.Interfaces;

namespace DemoEnrollmentSystem.Services
{
    public class DemoEnrollmentService : IDemoEnrollmentService
    {
        private readonly DemoEnrollmentDataService _demoEnrollmentDataService;

        public DemoEnrollmentService(DemoEnrollmentDataService demoEnrollmentDataService)
        {
            _demoEnrollmentDataService = demoEnrollmentDataService;
        }

        public GetAvailableCoursesResponse GetAvailableCourses()
        {
            GetAvailableCoursesResponse response = new GetAvailableCoursesResponse();

            response.availableCourses = _demoEnrollmentDataService.getAvailableCoursesData();

            return response;
        }

        public EnrollStudentResponse EnrollStudent (EnrollStudentRequest enrollstudentRequest)
        {
            // First validate that the student exists
            bool isValidStudent = _demoEnrollmentDataService.isValidStudent(enrollstudentRequest.student_id);

            if (!isValidStudent)
            {
                return new EnrollStudentResponse { isSuccess = false, message = "Student not found." };
            }

            // Validate that the class is valid
            bool isValidCourse = _demoEnrollmentDataService.isValidCourse(enrollstudentRequest.course_id);

            if (!isValidCourse)
            {
                return new EnrollStudentResponse { isSuccess = false, message = "Course not found." };
            }

            /* Check to see if the student is already enrolled
             * There is a check in SQL, but that logic should also be handled server side
            */
            bool isStudentEnrolled = _demoEnrollmentDataService.isStudentEnrolled(enrollstudentRequest.student_id, enrollstudentRequest.course_id);
            if (isStudentEnrolled)
            {
                return new EnrollStudentResponse { isSuccess = false, message = "Student is already enrolled in this course." };
            }

            bool studentEnrolledSucces = _demoEnrollmentDataService.addStudentEnrollment(enrollstudentRequest.student_id, enrollstudentRequest.course_id);

            if (studentEnrolledSucces)
            {
                return new EnrollStudentResponse { isSuccess = true, message = "Student enrolled succesfully." };

            }

            return new EnrollStudentResponse  { isSuccess = false, message = "An error occured when trying to enroll student." };

        }

        public AssignGradeResponse AssignGrade(AssignGradeRequest assignGradeRequest)
        {
            //Validate if the enrollment is real
            bool isValidEnrollment = _demoEnrollmentDataService.isValidEnrollment(assignGradeRequest.enrollment_id);

            if (!isValidEnrollment)
            {
                return new AssignGradeResponse { isSuccess = false, message = "Enrollment not found." };
            }

            // This will handle both a new grade as well as update an existing grade
            bool assignGradeSuccess = _demoEnrollmentDataService.assignGrade(assignGradeRequest.enrollment_id, assignGradeRequest.grade);

            if (assignGradeSuccess)
            {
                return new AssignGradeResponse { isSuccess = true, message = "Grade assigned succesfully." };

            }

            return new AssignGradeResponse { isSuccess = false, message = "An error occured when trying to assign a grade." };
        }

        public GetAverageGradeResponse GetAverageGrade()
        { 
            GetAverageGradeResponse response = new GetAverageGradeResponse();

            response.averageGrades = _demoEnrollmentDataService.getAverageGradeData();

            return response;

        }
    }
}
