using MITCRMS.Models.Entities;

namespace MITCRMS.Interface.Repository
{
    public interface IReportRepository:IBaseRepository
    {
        //public Task<Report> AddReport(Report report);
        Task<IReadOnlyList<Report>> GetAllReport();
        Task<IReadOnlyList<Report>> GetReportByDepartment();
        Task<IReadOnlyList<Report>> GetAllCancelledReport();
        Task<IReadOnlyList<Report>> GetAllCompletedReport();


        bool AcceptReport(Report report);
        Task<Report> GetRepordById(Guid id);
        Task<IReadOnlyList<Report>> GetReportsByHodId(Guid userHodId);
        Task<IReadOnlyList<Report>>GetReportsByTutorId(Guid usertutorId);
        Task<IReadOnlyList<Report>> GetReportsByBursarId(Guid userbursarId);
        Task<IReadOnlyList<Report>> GetReportsByAdminId(Guid useradminId);


    }

}
