using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MITCRMS.Implementation.Services;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Tutor;
using System.Security.Claims;

namespace MITCRMS.Controllers
{
    public class TutorController(ITutorServices tutorServices, IDepartmentServices departmentServices,
        IRoleServices roleServices, ILogger<TutorController> logger, IUserServices userServices,
        IReportServices reportServices) : Controller
    {
        private readonly ITutorServices _tutorServices = tutorServices ?? throw new ArgumentNullException(nameof(tutorServices));
        private readonly IRoleServices roleService = roleServices ?? throw new ArgumentNullException(nameof(roleServices));
        private readonly ILogger<TutorController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserServices _userService = userServices ?? throw new ArgumentNullException(nameof(userServices));
        private readonly IReportServices _reportServices = reportServices ?? throw new ArgumentNullException(nameof(reportServices));
        private readonly IDepartmentServices _departmentServices = departmentServices ?? throw new ArgumentNullException(nameof(departmentServices));

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var tutor = await _tutorServices.GetTutorAsync(CancellationToken.None);
            return View(tutor);
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateTutor()
        {
            var departments = await _departmentServices.GetAllDepartmentsAsync();
            var model = new CreateTutorRequestModel
            {
                Departments = departments.Data.Select(d => new SelectListItem

                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateTutor(CreateTutorRequestModel model)
        {

            //if (!ModelState.IsValid)
            //{
            //    var departments = await _departmentServices.GetAllDepartmentsAsync();
            //    model.Departments = departments.Data.Select(d => new SelectListItem
            //    {
            //        Value = d.Id.ToString(),
            //        Text = d.DepartmentName
            //    }).ToList();
            //    return View(model);
            //}

            var tutorStatus = await _tutorServices.CreateTutorAsync(model);
            if (tutorStatus.Status)
            {
                ViewBag.Alert = tutorStatus.Status;
                ViewBag.AlertType = "success";

                return RedirectToAction("Index");
            }
            else
            {

                ViewBag.Alert = tutorStatus.Status;
                ViewBag.AlertType = "danger";
                return View(model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin,Tutor")]
        public async Task<IActionResult> TutorProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(User.FindFirstValue(ClaimTypes.GivenName));

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login", "User");
            }
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID format.");
            }

            var tutor = await _userService.GetUserProfileByUserId(userId, CancellationToken.None);

            if (tutor == null || !tutor.Status) return NotFound(tutor.Message);

            return View(tutor);




        }

        [HttpGet]
        public async Task<IActionResult> GetMyReport()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(User.FindFirstValue(ClaimTypes.GivenName));

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Bursarprofile", "Bursar");
            }
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID format.");
            }
            var reports = await _reportServices.GetAllByBursarReportIdAsync(userId, CancellationToken.None);

            return View(reports);
        }
    }
}
