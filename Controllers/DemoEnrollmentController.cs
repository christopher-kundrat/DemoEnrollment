using DemoEnrollmentSystem.Entities.RequestEntities;
using DemoEnrollmentSystem.Entities.ResponseEntities;
using DemoEnrollmentSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;



namespace DemoEnrollmentSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoEnrollmentController : ControllerBase
    {
        private readonly IDemoEnrollmentService _demoEnrollmentService;

        private readonly ILogger<DemoEnrollmentController> _logger;

        public DemoEnrollmentController(IDemoEnrollmentService demoEnrollmentService, ILogger<DemoEnrollmentController> logger)
        {
            _logger = logger;
            _demoEnrollmentService = demoEnrollmentService;
        }

        [HttpGet("GetAvailableCourses")]
        public GetAvailableCoursesResponse GetAvailableCourses()
        {
           GetAvailableCoursesResponse availableCourses = _demoEnrollmentService.GetAvailableCourses();

            return availableCourses;
        }
        [HttpPost("EnrollStudent")]
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollstudentRequest)
        {
            return _demoEnrollmentService.EnrollStudent(enrollstudentRequest);
        }

        /* 
         * Using Put as grades would technically be correct as grades might be updated later, 
         * a lot of systems struggle with that so I'm using post and will just handle update logic server side. 
        */
        [HttpPost("AssignGrade")]
        public AssignGradeResponse AssignGrade(AssignGradeRequest assignGradeRequest)
        {
            return _demoEnrollmentService.AssignGrade(assignGradeRequest);
        }

        [HttpGet("GetAverageGrade")]
        public GetAverageGradeResponse GetAverageGrade()
        {
            return _demoEnrollmentService.GetAverageGrade();
        }

    }
}