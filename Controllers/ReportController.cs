using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MITCRMS.Contract.Services;
using MITCRMS.Implementation.Services;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs.Report;
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

        public ReportController(
            IReportServices reportServices,
            IDepartmentServices departmentServices,
            IIdentityService identityService,
            ILogger<ReportController> logger)
        {
            _reportServices = reportServices;
            _departmentServices = departmentServices;
            _identityService = identityService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var currentUser = await _identityService.GetLoggedInUser();
            var roles = await _identityService.GetRolesAsync(currentUser);

            if (roles.Contains("SuperAdmin"))
            {
                var resp = await _reportServices.GetReportsAsync(cancellationToken);
                return View(resp.Data ?? Enumerable.Empty<ReportDto>());
            }

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
        [HttpPost]
        [Authorize(Roles = "Tutor,Hod,Bursar,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(CreateReportRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                var depts = await _departmentServices.GetDepartmentsSelectList();
                ViewData["Departments"] = new SelectList(depts, "Value", "Text");
                return View(model);
            }

            var currentUser = await _identityService.GetLoggedInUser();
            var roles = await _identityService.GetRolesAsync(currentUser);

            if (roles.Contains("Tutor"))
                model.TutorId = currentUser.TutorId;
            if (roles.Contains("Hod"))
                model.HodId = currentUser.HodId;
            if (roles.Contains("Bursar"))
                model.BursarId = currentUser.BursarId;
            if (roles.Contains("Admin"))
                model.AdminId = currentUser.AdminId;

            var resp = await _reportServices.CreateReportAsync(model);
            if (!resp.Status)
            {
                ModelState.AddModelError(string.Empty, resp.Message ?? "Could not create report");
                var depts = await _departmentServices.GetDepartmentsSelectList();
                ViewData["Departments"] = new SelectList(depts, "Value", "Text");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var resp = await _reportServices.GetReportByIdAsync(id, CancellationToken.None);
            if (!resp.Status || resp.Data == null)
                return NotFound(resp.Message);

            return View(resp.Data);
        }

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
            var resp = await _reportServices.DeleteAsync(id);
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

        //[HttpGet]
        //    [Authorize(Roles = "SuperAdmin")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> GetAllReports(CancellationToken cancellationToken)
        //    {
        //        var response = await _reportServices.GetAllReportsAsync();
        //        if (!response.Status)
        //        {
        //            ViewBag.Error = response.Message;
        //            return View(new List<ReportDto>());
        //        }
        //        return View(response.Data);
        //    }
        [HttpGet]
        public async Task<IActionResult> GetMyReports(Guid id, CancellationToken cancellationToken)
        {
            var response = await _reportServices.GetReportByIdAsync(id, cancellationToken);

            if (!response.Status)
            {
                ViewBag.ErrorMessage = response.Message;
                return View("GetMyReports"); // NO redirect
            }

            return View(response.Data);
        }


    }
}
