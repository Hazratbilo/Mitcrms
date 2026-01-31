using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Report;
using MITCRMS.Models.DTOs.Role;
using MITCRMS.Models.Entities;
using System.Threading;

namespace MITCRMS.Interface.Services
{
    public interface IReportServices
    {
        Task<BaseResponse<bool>> CreateReportAsync(CreateReportRequestModel request, Guid loggedInUserId, string role);

        Task<IEnumerable<Report>> GetReportsByUserAsync(Guid userId, string role);
        Task<BaseResponse<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByHodReportIdAsync(Guid userHodId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByTutorReportIdAsync(Guid userTutorId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByAdminReportIdAsync(Guid userAdminId, CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<ReportDto>>> GetMyReportsAsync(Guid userId);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByBursarReportIdAsync(Guid userBursarId, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> AcceptReport(Guid id);
        //Task<BaseResponse<IReadOnlyList<ReportDto>>> GetReportsAsync(CancellationToken cancellationToken);
        //Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByDepartmentReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetCancelledReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetCompletedReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid tutorId);
        //Task<BaseResponse<IEnumerable<ReportDto>>> GetAllReportsAsync();
        Task<BaseResponse<bool>> UpdateReport(Guid id, CreateReportRequestModel request);
    }
}
