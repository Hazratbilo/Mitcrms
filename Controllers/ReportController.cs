using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MITCRMS.Contract.Services;
using MITCRMS.Implementation.Services;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Report;
using MITCRMS.Models.Entities;
using System.Security.Claims;

namespace MITCRMS.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportServices _reportServices;
        private readonly IDepartmentServices _departmentServices;
        private readonly IIdentityService _identityService;
        private readonly IReportServices reportServices;
        private readonly ILogger<ReportController> _logger;
        private readonly IWebHostEnvironment _env;

        public ReportController(
            IReportServices reportServices,
            IDepartmentServices departmentServices,
            IIdentityService identityService, IWebHostEnvironment env,
            ILogger<ReportController> logger)
        {
            _reportServices = reportServices;
            _departmentServices = departmentServices;
            _env = env;
            _identityService = identityService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var currentUser = await _identityService.GetLoggedInUser();
            var roles = await _identityService.GetRolesAsync(currentUser);

            //if (roles.Contains("SuperAdmin"))
            //{
            //    var resp = await _reportServices.GetAllBy(cancellationToken);
            //    return View(resp.Data ?? Enumerable.Empty<ReportDto>());
            //}

            if (roles.Contains("Hod"))
            {
                var hodId = currentUser.HodId;
                var hodall = await _reportServices.GetAllByHodReportIdAsync(hodId, cancellationToken);

                var hodReports = (hodall.Data ?? Enumerable.Empty<ReportDto>()).Where(r => r.HodId == hodId);
                return View(hodReports);

            }
            else if (roles.Contains("Tutor"))
            {
                var tutorId = currentUser.TutorId;
                var tutorall = await _reportServices.GetAllByTutorReportIdAsync(tutorId, cancellationToken);

                var tutorReports = (tutorall.Data ?? Enumerable.Empty<ReportDto>()).Where(r => r.TutorId == tutorId);
                return View(tutorReports);

            }
            else if (roles.Contains("Bursar"))
            {
                var bursarId = currentUser.BursarId;
                var bursarall = await _reportServices.GetAllByBursarReportIdAsync(bursarId, cancellationToken);
                var BursarReports = (bursarall.Data ?? Enumerable.Empty<ReportDto>()).Where(r => r.BursarId == bursarId);
                return View(BursarReports);

            }
            else if (roles.Contains("Admin"))
            {
                var adminId = currentUser.AdminId;
                var adminall = await _reportServices.GetAllByAdminReportIdAsync(adminId, cancellationToken);

                var adminReports = (adminall.Data ?? Enumerable.Empty<ReportDto>()).Where(r => r.AdminId == adminId);
                return View(adminReports);

            }
            else
            {
                return View(null);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Tutor,Hod,Bursar,Admin")]
        public async Task<IActionResult> CreateReport()
        {
            var depts = await _departmentServices.GetDepartmentsSelectList();
            ViewData["Departments"] = new SelectList(depts, "Value", "Text");
            return View(new CreateReportRequestModel());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(IFormFile file, CreateReportRequestModel model)
        {


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || roleClaim == null)
            {
                TempData["Error"] = "User information not found. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var role = roleClaim.Value;


            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadsPath = Path.Combine(_env.WebRootPath, "reports");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            var fileUrl = $"{Request.Scheme}://{Request.Host}/reports/{fileName}";

            var result = await _reportServices.CreateReportAsync(fileUrl, model, userId, role);

            if (!result.Status)
            {
                TempData["Error"] = result.Message;
                return View(model);
            }

            TempData["Success"] = "Report created successfully!";
            return RedirectToAction("GetMyReports", "Report");
        }


        //[HttpGet]
        //public async Task<IActionResult> Details(Guid id)
        //{
        //    var resp = await _reportServices.GetReportByIdAsync(id, CancellationToken.None);
        //    if (!resp.Status || resp.Data == null)
        //        return NotFound(resp.Message);

        //    return View(resp.Data);
        //}

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var resp = await _reportServices.GetReportByIdAsync(id, CancellationToken.None);
            if (!resp.Status || resp.Data == null)
                return NotFound(resp.Message);

            var currentUser = await _identityService.GetLoggedInUser();
            var model = new CreateReportRequestModel
            {
                Tittle = resp.Data.Tittle,
                Content = resp.Data.Content,
            };

            var depts = await _departmentServices.GetDepartmentsSelectList();
            ViewData["Departments"] = new SelectList(depts, "Value", "Text");
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CreateReportRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                var depts = await _departmentServices.GetDepartmentsSelectList();
                ViewData["Departments"] = new SelectList(depts, "Value", "Text");
                return View(model);
            }

            var respExisting = await _reportServices.GetReportByIdAsync(id, CancellationToken.None);
            if (!respExisting.Status || respExisting.Data == null)
                return NotFound(respExisting.Message);

            var currentUser = await _identityService.GetLoggedInUser();
            var updateResp = await _reportServices.UpdateReport(id, model);
            if (!updateResp.Status)
            {
                ModelState.AddModelError(string.Empty, updateResp.Message ?? "Update failed");
                var depts = await _departmentServices.GetDepartmentsSelectList();
                ViewData["Departments"] = new SelectList(depts, "Value", "Text");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
 

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var resp = await _reportServices.GetReportByIdAsync(id, CancellationToken.None);
            if (!resp.Status || resp.Data == null)
                return NotFound(resp.Message);

            var currentUser = await _identityService.GetLoggedInUser();

            return View(resp.Data);
        }


        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var resp = await _reportServices.DeleteReport(id);
            if (!resp.Status)
            {
                TempData["Error"] = resp.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid id)
        {
            var resp = await _reportServices.AcceptReport(id);
            if (!resp.Status)
            {
                TempData["Error"] = resp.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        [HttpGet]
        public async Task<IActionResult> GetMyReports()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if(!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest("Invalid user ID");
            }

            Console.WriteLine("UserId: " + userId);

            var resp = await _reportServices.GetMyReportsAsync(userId);

            if (!resp.Status || resp.Data == null)
                return NotFound(resp.Message);
            return View(resp.Data);
        }
        public async Task<IActionResult> DeleteReport(Guid id)
        {
            var dept = await _reportServices.GetReportById(id);
            if (!dept.Status) return NotFound();

            return View(dept);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReportConfirmed(Guid id)
        {
            await _reportServices.DeleteReport(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var department = await _reportServices.GetReportById(id);
            if (department == null)
            {
                throw new Exception("Request not found!");
            }

            return View(department);
        }

    }
}

