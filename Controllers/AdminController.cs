using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MITCRMS.Implementation.Services;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;
using System.Security.Claims;

namespace MITCRMS.Controllers
{
    public class AdminController(IAdminServices adminServices, IDepartmentServices departmentServices,
        IRoleServices roleServices, ILogger<AdminController> logger, IUserServices userServices,
        IReportServices reportServices) : Controller
    {
        private readonly IAdminServices _adminServices = adminServices ?? throw new ArgumentNullException(nameof(adminServices));
        private readonly IRoleServices roleService = roleServices ?? throw new ArgumentNullException(nameof(roleServices));
        private readonly ILogger<AdminController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserServices _userService = userServices ?? throw new ArgumentNullException(nameof(userServices));
        private readonly IReportServices _reportServices = reportServices ?? throw new ArgumentNullException(nameof(reportServices));
        private readonly IDepartmentServices _departmentServices = departmentServices ?? throw new ArgumentNullException(nameof(departmentServices));

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var admin = await _adminServices.GetAdminAsync(CancellationToken.None);
            return View(admin);
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAdmin()
        {
            var departments = await _departmentServices.GetAllDepartmentsAsync();
            var model = new CreateAdminRequestModel
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
        public async Task<IActionResult> CreateAdmin(CreateAdminRequestModel model)
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

            var adminStatus = await _adminServices.CreateAdminAsync(model);
            if (adminStatus.Status)
            {
                ViewBag.Alert = adminStatus.Status;
                ViewBag.AlertType = "success";

                return RedirectToAction("Index");
            }
            else
            {

                ViewBag.Alert = adminStatus.Status;
                ViewBag.AlertType = "danger";
                return View(model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> AdminProfile()
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

            var admin = await _userService.GetUserProfileByUserId(userId, CancellationToken.None);

            if (admin == null || !admin.Status) return NotFound(admin.Message);

            return View(admin);




        }

        [HttpGet]
        public async Task<IActionResult> GetMyReport()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(User.FindFirstValue(ClaimTypes.GivenName));

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("AdminProfile", "Admin");
            }
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID format.");
            }
            var reports = await _reportServices.GetAllByAdminReportIdAsync(userId, CancellationToken.None);

            return View(reports);
        }
    }
}
