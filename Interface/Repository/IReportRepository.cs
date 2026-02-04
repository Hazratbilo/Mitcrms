using MITCRMS.Contract.Entity;
using MITCRMS.Models.Entities;
using System.Linq.Expressions;

namespace MITCRMS.Interface.Repository
{
    public interface IReportRepository:IBaseRepository
    {
        public Task<Report> AddReport(Report report);
        Task<IEnumerable<Report>> GetAll(Expression<Func<Report, bool>> predicate);
        //Task<IReadOnlyList<Report>> GetAllReport();
        Task<IReadOnlyList<Report>> GetReportByDepartment();
        Task<IReadOnlyList<Report>> GetAllCancelledReport();
        Task<IReadOnlyList<Report>> GetAllCompletedReport();
        Task<IEnumerable<Report>> GetAlReports();

        bool AcceptReport(Report report);
        Task<Report> GetRepordById(Guid id);
        Task<IReadOnlyList<Report>> GetReportsByHodId(Guid userHodId);
        Task<IReadOnlyList<Report>>GetReportsByTutorId(Guid usertutorId);
        Task<IReadOnlyList<Report>> GetReportsByBursarId(Guid userbursarId);
        public Task<bool> DeleteReport(Guid id);
        Task<IReadOnlyList<Report>> GetReportsByAdminId(Guid useradminId);
        public Task<Report> GetReportById(Guid id);
        Task<IReadOnlyList<Report>> GetMyReports(Expression<Func<Report, bool>> expression);


    }

}
