using Microsoft.EntityFrameworkCore;
using MITCRMS.Contract.Entity;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Models.Enum;
using MITCRMS.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MITCRMS.Implementation.Repository
{
    public class ReportRepository : BaseRepository, IReportRepository
    {
        public ReportRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {
        }

        public bool AcceptReport(Report report)
        {
            if (report == null) return false;

            try
            {
                report.Status = ReportStatus.Approved;
                report.ApprovedAt = DateTime.UtcNow;

                _mitcrmsContext.Set<Report>().Update(report);
                _mitcrmsContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public async Task<Report> AddReport(Report report)
        //{
        //    await _mitcrmsContext.Set<Report>().AddAsync(report);
        //    await _mitcrmsContext.SaveChangesAsync();
        //    return Report;
        //}

        public async Task<IReadOnlyList<Report>> GetAllCancelledReport()
        {
            return await _mitcrmsContext.Set<Report>()
                .Where(r => r.Status == ReportStatus.Rejected)
                .OrderByDescending(r => r.DateCreated)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Report>> GetAllCompletedReport()
        {
            return await _mitcrmsContext.Set<Report>()
                .Where(r => r.Status == ReportStatus.Approved)
                .OrderByDescending(r => r.DateCreated)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Report>> GetAllReport()
        {
            return await _mitcrmsContext.Set<Report>()
                .OrderByDescending(r => r.DateCreated)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Report> GetRepordById(Guid id)
        {
            return await _mitcrmsContext.Set<Report>()
                .Where(r => r.Id == id)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public Task<IReadOnlyList<Report>> GetReportByDepartment()
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Report>> GetReportsByAdminId(Guid useradminId)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Report>> GetReportsByBursarId(Guid userbursarId)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Report>> GetReportsByHodId(Guid userHodId)
        {
            var deptIds = await _mitcrmsContext.Set<Report>()
                .Where(d => d.HodId == userHodId)
                .Select(d => d.Id)
                .ToListAsync();

            if (deptIds == null || deptIds.Count == 0)
                return new List<Report>();

            return await _mitcrmsContext.Set<Report>()
                .Where(r => deptIds.Contains(r.DepartmentId))
                .OrderByDescending(r => r.DateCreated)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Report>> GetReportsByTutorId(Guid tutorId)
        {
            var reportsByAuthor = await _mitcrmsContext.Set<Report>()
                .Where(r => r.TutorId == tutorId)
                .OrderByDescending(r => r.DateCreated)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .ToListAsync();

            if (reportsByAuthor != null && reportsByAuthor.Count > 0)
                return reportsByAuthor;

            var tutor = await _mitcrmsContext.Set<Tutor>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tutorId);

            if (tutor == null)
                return new List<Report>();

            return await _mitcrmsContext.Set<Report>()
                .Where(r => r.DepartmentId == tutor.DepartmentId)
                .OrderByDescending(r => r.DateCreated)
                .Include(r => r.Department)
                .Include(r => r.Tutor)
                    .ThenInclude(t => t.User)
                .Include(r => r.Hod)
                    .ThenInclude(h => h.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetAll(Expression<Func<Report, bool>> predicate)
        {
            return await _mitcrmsContext.Set<Report>()
                                 .Where(predicate)
                                 .Include(r => r.Department)
                                 .Include(r => r.Tutor)
                                 .Include(r => r.Hod)
                                 .Include(r => r.Admin)
                                 .Include(r => r.Bursar)
                                 .ToListAsync();
        }

        public async Task<Report> AddReport(Report report)
        {
            await _mitcrmsContext.Set<Report>().AddAsync(report);
            return report;
        }
        //public async Task<List<Report>> GetAllReports()
        //{

        //    var report = await _mitcrmsContext.Set<Report>().ToListAsync();
        //    return report;
        //}
        public async Task<IEnumerable<Report>> GetAllReports()
        {
            var report = await _mitcrmsContext.Set<Report>().ToListAsync();
              return report;
        }


        public Task<IEnumerable<Report>> GetAlReports()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<Report>> GetMyReports(Expression<Func<Report, bool>> expression)
        {
            return await _mitcrmsContext.Set<Report>()
                .Where(expression)
                .Include(r => r.Bursar)
                .Include(r => r.Admin)
                .Include(r => r.Hod)
                .Include(r => r.Tutor)
                .OrderByDescending(r => r.DateCreated)
                .AsSplitQuery()
                .ToListAsync();
        }
        public async Task<Report> GetReportById(Guid id)
        {
            return await _mitcrmsContext.Set<Report>().FirstOrDefaultAsync(x => x.Id == id);

        }
        public async Task<bool> DeleteReport(Guid id)
        {
            var note = await _mitcrmsContext.Set<Report>().FindAsync(id);
            _mitcrmsContext.Set<Report>().Remove(note);
            await _mitcrmsContext.SaveChangesAsync();
            return true;
        }

    }
}
