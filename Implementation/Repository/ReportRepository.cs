using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Models.Enum;
using MITCRMS.Persistence.Context;

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
    }
}