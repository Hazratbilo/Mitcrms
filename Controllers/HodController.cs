using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MITCRMS.Implementation.Services;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.Entities;
using System.Security.Claims;

namespace MITCRMS.Controllers
{
    public class HodController(IHodServices hodServices, IDepartmentServices departmentServices,
        IRoleServices roleServices, ILogger<HodController> logger, IUserServices userServices,
        IReportServices reportServices) : Controller
    {
        private readonly IHodServices _hodServices = hodServices ?? throw new ArgumentNullException(nameof(hodServices));
        private readonly IRoleServices roleService = roleServices ?? throw new ArgumentNullException(nameof(roleServices));
        private readonly ILogger<HodController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserServices _userService = userServices ?? throw new ArgumentNullException(nameof(userServices));
        private readonly IReportServices _reportServices = reportServices ?? throw new ArgumentNullException(nameof(reportServices));
        private readonly IDepartmentServices _departmentServices = departmentServices ?? throw new ArgumentNullException(nameof(departmentServices));

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Index()
        {
            var hods = await _hodServices.GetHodAsync(CancellationToken.None);

            return View(hods);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateHod()
        {
            var departments = await _departmentServices.GetAllDepartmentsAsync();
            var model = new CreateHodRequestModel
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
        public async Task<IActionResult> CreateHod(CreateHodRequestModel model)
        {

          
                //var departments = await _departmentServices.GetAllDepartmentsAsync();
                //model.Departments = departments.Data.Select(d => new SelectListItem
                //{
                //    Value = d.Id.ToString(),
                //    Text = d.DepartmentName
                //}).ToList();
                //return View(model);
   

                var hodStatus = await _hodServices.CreateHodAsync(model);
                if (hodStatus.Status)
                {
                    ViewBag.Alert = hodStatus.Status;
                    ViewBag.AlertType = "success";

                    return RedirectToAction("Index");
                }
               return View(model);
         
            }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Hod")]
        public async Task<IActionResult> HodProfile()
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

            var hod = await _userService.GetUserProfileByUserId(userId, CancellationToken.None);

            if (hod == null || !hod.Status) return NotFound(hod.Message);

            return View(hod);




        }

        [HttpGet]
        public async Task<IActionResult> GetMyReport()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(User.FindFirstValue(ClaimTypes.GivenName));

            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("HodProefile", "Hod");
            }
            if (!Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid user ID format.");
            }
            var reports = await _reportServices.GetAllByHodReportIdAsync(userId, CancellationToken.None);

            return View(reports);
        }
    }
}
