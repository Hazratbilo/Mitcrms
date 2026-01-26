using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Tutor;
using System.Security.Claims;

namespace MITCRMS.Controllers
{

    public class BursarController(IBursarServices bursarServices, IDepartmentServices departmentServices,
        IRoleServices roleServices, ILogger<BursarController> logger, IUserServices userServices,
        IReportServices reportServices) : Controller
    {
        private readonly IBursarServices _bursarServices = bursarServices ?? throw new ArgumentNullException(nameof(bursarServices));
        private readonly IRoleServices roleService = roleServices ?? throw new ArgumentNullException(nameof(roleServices));
        private readonly ILogger<BursarController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserServices _userService = userServices ?? throw new ArgumentNullException(nameof(userServices));
        private readonly IReportServices _reportServices = reportServices ?? throw new ArgumentNullException(nameof(reportServices));
        private readonly IDepartmentServices _departmentServices = departmentServices ?? throw new ArgumentNullException(nameof(departmentServices));

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var bursar = await _bursarServices.GetBursarAsync(CancellationToken.None);
            return View(bursar);
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateBursar()
        {
            var departments = await _departmentServices.GetAllDepartmentsAsync();
            var model = new CreateBursarRequestModel
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
        public async Task<IActionResult> CreateBursar(CreateBursarRequestModel model)
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

            var bursarStatus = await _bursarServices.CreateBursarAsync(model);
            if (bursarStatus.Status)
            {
                ViewBag.Alert = bursarStatus.Status;
                ViewBag.AlertType = "success";

                return RedirectToAction("Index");
            }
            else
            {

                ViewBag.Alert = bursarStatus.Status;
                ViewBag.AlertType = "danger";
                return View(model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "SuperAdmin, B#ursar")]
        public async Task<IActionResult> BursarProfile()
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

            var bursar = await _userService.GetUserProfileByUserId(userId, CancellationToken.None);

            if (bursar == null || !bursar.Status) return NotFound(bursar.Message);

            return View(bursar);




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
