using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Report;
using MITCRMS.Models.DTOs.Role;
using MITCRMS.Models.DTOs.Tutor;
using MITCRMS.Models.Entities;
using MITCRMS.Models.Enum;



namespace MITCRMS.Implementation.Services
{
    public class ReportServices : IReportServices
    {
        private readonly IReportRepository _reportRepository;
        private readonly IHodRepository _hodRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IBursarRepository _bursarRepository;
        private readonly ITutorRepository _tutorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReportServices> _logger;

        public ReportServices(
            IReportRepository reportRepository,
            IAdminRepository adminRepository,
            IBursarRepository bursarRepository,
            IHodRepository hodRepository,
            ITutorRepository tutorRepository,
            IUnitOfWork unitOfWork,
            ILogger<ReportServices> logger)
        {
            _reportRepository = reportRepository;
            _hodRepository = hodRepository;
            _adminRepository = adminRepository;
            _bursarRepository = bursarRepository;
            _tutorRepository = tutorRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<bool>> AcceptReport(Guid id)
        {
            var report = await _reportRepository.Get<Report>(r => r.Id == id);
            if (report == null)
            {
                _logger.LogError($"Report with Id: '{id}' cannot be found");
                return new BaseResponse<bool>
                {
                    Message = $"Report with Id: '{id}' cannot be found",
                    Status = false
                };
            }


            var success = _reportRepository.AcceptReport(report);
            if (!success)
            {
                _logger.LogError("Report couldn't be approved");
                return new BaseResponse<bool>
                {
                    Message = "Report couldn't be approved",
                    Status = false
                };
            }

            return new BaseResponse<bool>
            {
                Message = "Report approved",
                Status = true
            };
        }

        public async Task<BaseResponse<bool>> CreateReportAsync(string fileUrl, 
       CreateReportRequestModel request,
       Guid loggedInUserId,
       string role)
        {
            if (fileUrl is null && request.Content is null)
                return new BaseResponse<bool> { Status = false, Message = "Content and file cannot be empty" };


            Tutor tutor = null;
            Admin admin = null;
            Hod hod = null;
            Bursar bursar = null;

            var normalizedRole = role.Trim().ToLowerInvariant();

            switch (normalizedRole)
            {
                case "tutor":
                    tutor = await _tutorRepository.Get<Tutor>(t => t.UserId == loggedInUserId);
                    if (tutor == null)
                        return new BaseResponse<bool> { Status = false, Message = "Tutor not found" };
                    break;

                case "admin":
                    admin = await _adminRepository.Get<Admin>(a => a.Id == loggedInUserId);
                    if (admin == null)
                        return new BaseResponse<bool> { Status = false, Message = "Admin not found" };
                    break;

                case "hod":
                    hod = await _hodRepository.Get<Hod>(h => h.Id == loggedInUserId);
                    if (hod == null)
                        return new BaseResponse<bool> { Status = false, Message = "HOD not found" };
                    break;

                case "bursar":
                    bursar = await _bursarRepository.Get<Bursar>(b => b.Id == loggedInUserId);
                    if (bursar == null)
                        return new BaseResponse<bool> { Status = false, Message = "Bursar not found" };
                    break;

                default:
                    return new BaseResponse<bool> { Status = false, Message = "Invalid user role" };
            }

            var report = new Report
            {
                TutorId = tutor?.Id,
                AdminId = admin?.Id,
                HodId = hod?.Id,
                BursarId = bursar?.Id,
                DepartmentId = request.DepartmentId,
                Tittle = request.Tittle,
                Content = request.Content,
                FileUrl = fileUrl,
                Status = ReportStatus.Pending,
                DateCreated = DateTime.UtcNow
            };

            await _reportRepository.Add(report);
            var result = await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            return result > 0 ? new BaseResponse<bool> { Status = true, Message = "Report created successfully"
            } 
            : new BaseResponse<bool> { Status = false, Message = "Report submission failed"};
        }


        public async Task<BaseResponse<bool>> DeleteAsync(Guid tutorId)
        {
            var report = await _reportRepository.Get<Report>(r => r.Id == tutorId);
            if (report == null)
            {
                _logger.LogError("Report not found: {ReportId}", tutorId);
                return new BaseResponse<bool> { Message = "Report not found", Status = false };
            }

            report.Status = ReportStatus.Rejected;
            report.ApprovedAt = null;
            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<bool> { Message = "Report rejected", Status = true };
        }

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByHodReportIdAsync(Guid userHodId, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetReportsByHodId(userHodId);
            if (reports == null || !reports.Any())
            {
                _logger.LogInformation("No reports found for Hod {HodId}", userHodId);
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No report found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(ap => MapToDto(ap)).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetCancelledReportsAsync(CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetAllCancelledReport();
            if (reports == null || !reports.Any())
            {
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No reports found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(MapToDto).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetCompletedReportsAsync(CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetAllCompletedReport();
            if (reports == null || !reports.Any())
            {
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No reports found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(MapToDto).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

        public async Task<BaseResponse<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetRepordById(id);
            if (report == null)
            {
                return new BaseResponse<ReportDto> { Message = "Report not found", Status = false };
            }

            return new BaseResponse<ReportDto> { Message = "Report fetched", Status = true, Data = MapToDto(report) };
        }

        //public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetReportsAsync(CancellationToken cancellationToken)
        //{
        //    var reports = await _reportRepository.GetAllReports();
        //    if (reports == null || !reports.Any())
        //    {
        //        return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No reports found", Status = false, Data = Array.Empty<ReportDto>() };
        //    }

        //    var data = reports.Select(MapToDto).ToList();
        //    return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        //}

        public async Task<BaseResponse<bool>> UpdateReport(Guid id, CreateReportRequestModel request)
        {
            if (request == null)
                return new BaseResponse<bool> { Message = "Invalid request", Status = false };

            var report = await _reportRepository.Get<Report>(r => r.Id == id);
            if (report == null)
                return new BaseResponse<bool> { Message = "Report not found", Status = false };

            if (report.Status == ReportStatus.Approved)
                return new BaseResponse<bool> { Message = "Approved reports cannot be edited", Status = false };

            report.Tittle = string.IsNullOrWhiteSpace(request.Tittle) ? report.Tittle : request.Tittle;
            report.Content = string.IsNullOrWhiteSpace(request.Content) ? report.Content : request.Content;
            if (request.DepartmentId != Guid.Empty)
                report.DepartmentId = request.DepartmentId;

            _reportRepository.Update(report);
            await _unitOfWork.SaveChangesAsync();

            return new BaseResponse<bool> { Message = "Report updated", Status = true };
        }
        private static ReportDto MapToDto(Report r)
        {
            return new ReportDto
            {
                Id = r.Id,
                DateCreated = r.DateCreated,
                Tittle = r.Tittle,
                Content = r.Content,
                Status = r.Status,
                ApprovedAt = r.ApprovedAt,
                //ApprovedByAdminId = r.ApprovedByAdminId,
                HodId = r.HodId,
                TutorId = r.TutorId,
                AdminId = r.AdminId,
                BursarId = r.BursarId,

                Hod = r.Hod != null ? new Models.DTOs.Hod.HodDto
                {
                    Id = r.Hod.Id,
                    FullName = r.Hod.FullName(),
                    Email = r.Hod.User?.Email,
                    PhoneNumber = r.Hod.PhoneNumber,
                    UserId = r.Hod.UserId
                } : null,


                Bursar = r.Bursar != null ? new Models.DTOs.Bursar.BursarDto
                {
                    Id = r.Bursar.Id,
                    FullName = r.Bursar.FullName(),
                    Email = r.Bursar.User?.Email,
                    PhoneNumber = r.Bursar.PhoneNumber,
                    UserId = r.Bursar.UserId
                } : null,
                Admin = r.Admin != null ? new Models.DTOs.Admin.AdminDto
                {
                    Id = r.Admin.Id,
                    FullName = r.Admin.FullName(),
                    Email = r.Admin.User?.Email,
                    PhoneNumber = r.Admin.PhoneNumber,
                    UserId = r.Admin.UserId
                } : null,
                Tutor = r.Tutor != null ? new TutorDto
                {
                    Id = r.Tutor.Id,
                    FullName = r.Tutor.FullName(),
                    Email = r.Tutor.User?.Email,
                    PhoneNumber = r.Tutor.PhoneNumber,
                    UserId = r.Tutor.UserId
                } : null
            };
        }

    

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByTutorReportIdAsync(Guid userTutorId, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetReportsByTutorId(userTutorId);
            if (reports == null || !reports.Any())
            {
                _logger.LogInformation("No reports found for Tutor {TutorId}", userTutorId);
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No report found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(ap => MapToDto(ap)).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByAdminReportIdAsync(Guid userAdminId, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetReportsByAdminId(userAdminId);
            if (reports == null || !reports.Any())
            {
                _logger.LogInformation("No reports found for Admin {AdminId}", userAdminId);
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No report found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(ap => MapToDto(ap)).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByBursarReportIdAsync(Guid userBursarId, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetReportsByBursarId(userBursarId);
            if (reports == null || !reports.Any())
            {
                _logger.LogInformation("No reports found for Bursar {BursarId}", userBursarId);
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No report found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(ap => MapToDto(ap)).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

        public Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByDepartmentReportsAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Report>> GetReportsByUserAsync(Guid userId, string role)
        {
            IEnumerable<Report> reports = new List<Report>();

            switch (role.ToLower())
            {
                case "tutor":
                    reports = await _reportRepository.GetAll(r => r.TutorId == userId);
                    break;

                case "hod":
                    reports = await _reportRepository.GetAll(r => r.HodId == userId);
                    break;

                case "admin":
                    reports = await _reportRepository.GetAll(r => r.AdminId == userId);
                    break;

                case "bursar":
                    reports = await _reportRepository.GetAll(r => r.BursarId == userId);
                    break;

                default:
                    _logger.LogWarning("Invalid role provided to GetReportsByUserAsync");
                    break;
            }

            return reports;
        }
        

        public async Task<BaseResponse<IEnumerable<ReportDto>>> GetMyReportsAsync(Guid userId)
        {
            var getAllReports = await _reportRepository.GetMyReports(r => r.Tutor.UserId == userId || r.Admin.UserId == userId || r.Bursar.UserId == userId || r.Hod.UserId == userId);
            if (!getAllReports.Any())
            {
                _logger.LogError($"No Note found");
                return new BaseResponse<IEnumerable<ReportDto>>
                {
                    Message = $"No Note found",
                    Status = false,
                };
            }
            _logger.LogInformation("All Report fetched successfully");
            return new BaseResponse<IEnumerable<ReportDto>>
            {
                Message = "All Report fetched successfully",
                Status = true,
                Data = getAllReports.Select(dpt => new ReportDto
                {
                    Id = dpt.Id,
                    Tittle = dpt.Tittle,
                    Content = dpt.Content,
                    DateCreated = dpt.DateCreated,
                }).ToList()
            };
        }
        public async Task<BaseResponse<ReportDto>> GetReportById(Guid id)
        {
            var getNote = await _reportRepository.GetReportById(id);
            if (getNote == null)
            {
                _logger.LogError($"Report with id {id} not found");
                return new BaseResponse<ReportDto>
                {
                    Message = $"Report with id {id} not found",
                    Status = false,
                };
            }
            _logger.LogInformation("Report fetched successfully");
            return new BaseResponse<ReportDto>
            {
                Message = "Report fetched successfully",
                Status = true,
                Data = new ReportDto
                {
                    Id = getNote.Id,
                    Tittle = getNote.Tittle,
                    Content = getNote.Content,
                    DateCreated = getNote.DateCreated,

                }
            };
        }
        public async Task<BaseResponse<bool>> DeleteReport(Guid id)
        {
            var getNote = await _reportRepository.GetReportById(id);
            if (getNote == null)
            {
                _logger.LogError($"Report with id {id} not found");
                return new BaseResponse<bool>
                {
                    Message = $"Report with id {id} not found",
                    Status = false
                };
            }

            var deleteNote = await _reportRepository.DeleteReport(id);
            if (deleteNote)
            {
                _logger.LogInformation("Delete Report Success");
                return new BaseResponse<bool>
                {
                    Message = "Delete Report Success",
                    Status = true
                };
            }
            _logger.LogError("Delete Report Failed");
            return new BaseResponse<bool>
            {
                Message = "Delete Report Failed",
                Status = false
            };
        }

        public Task<BaseResponse<IEnumerable<ReportDto>>> GetAllReportsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
