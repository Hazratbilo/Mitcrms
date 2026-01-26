using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Report;

namespace MITCRMS.Interface.Services
{
    public interface IReportServices
    {
        Task<BaseResponse<bool>> CreateReportAsync(CreateReportRequestModel request);
        Task<BaseResponse<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByHodReportIdAsync(Guid userHodId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByTutorReportIdAsync(Guid userTutorId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByAdminReportIdAsync(Guid userAdminId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByBursarReportIdAsync(Guid userBursarId, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> AcceptReport(Guid id);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetAllByDepartmentReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetCancelledReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<ReportDto>>> GetCompletedReportsAsync(CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid tutorId);
        Task<BaseResponse<bool>> UpdateReport(Guid id, CreateReportRequestModel request);
    }
}
