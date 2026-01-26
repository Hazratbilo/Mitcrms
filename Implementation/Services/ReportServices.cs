using MITCRMS.Interface.Repository;
using MITCRMS.Interface.Services;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Report;
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

        public async Task<BaseResponse<bool>> CreateReportAsync(CreateReportRequestModel request)
        {
            if (request == null)
            {
                return new BaseResponse<bool> { Message = "Invalid request", Status = false };
            }

            if (request.DepartmentId == Guid.Empty)
            {
                return new BaseResponse<bool> { Message = "DepartmentId is required", Status = false };
            }
            Bursar bursar = null;
            Admin admin = null;
            Tutor tutor = null;
            Hod hod = null;

            if (request.TutorId != Guid.Empty)
            {
                tutor = await _tutorRepository.Get<Tutor>(t => t.Id == request.TutorId);
                if (tutor == null)
                {
                    return new BaseResponse<bool> { Message = "Tutor not found", Status = false };
                }
            }

            if (request.HodId!=Guid.Empty)
            {
                hod = await _hodRepository.Get<Hod>(h => h.Id == request.HodId);
                if (hod == null)
                {
                    return new BaseResponse<bool> { Message = "Hod not found", Status = false };
                }
            }
            if (request.AdminId!=Guid.Empty)
            {
                admin = await _adminRepository.Get<Admin>(a => a.Id == request.AdminId);
                if (admin == null)
                {
                    return new BaseResponse<bool> { Message = "Admin not found", Status = false };
                }
            }
            if (request.BursarId!=Guid.Empty)
            {
                bursar = await _bursarRepository.Get<Bursar>(b => b.Id == request.BursarId);
                if (tutor == null)
                {
                    return new BaseResponse<bool> { Message = "Bursar not found", Status = false };
                }
            }


            if (tutor == null && hod == null && bursar==null && admin==null)
            {
                return new BaseResponse<bool> { Message = "Either TutorId,HodId,BursarId or AdminId must be provided", Status = false };
            }

            var report = new Report
            {
                TutorId = tutor.Id,
                AdminId = admin.Id,
                HodId = hod.Id,
                BursarId =bursar.Id,
                DepartmentId = request.DepartmentId,
                Tittle = request.Tittle,
                Content = request.Content,
                Status = ReportStatus.Pending,
                DateCreated = DateTime.UtcNow
            };

            var created = await _reportRepository.Add(report);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            if (created == null)
            {
                _logger.LogError("Report couldn't be created");
                return new BaseResponse<bool> { Message = "Report couldn't be created", Status = false };
            }

            return new BaseResponse<bool> { Message = "Report created", Status = true };
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

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetReportsAsync(CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetAllReport();
            if (reports == null || !reports.Any())
            {
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No reports found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(MapToDto).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
        }

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

        public async Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllReportsAsync(CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetAllReport();
            if (reports == null || !reports.Any())
            {
                return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "No reports found", Status = false, Data = Array.Empty<ReportDto>() };
            }

            var data = reports.Select(MapToDto).ToList();
            return new BaseResponse<IReadOnlyList<ReportDto>> { Message = "Data fetched successfully", Status = true, Data = data };
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
    }
}