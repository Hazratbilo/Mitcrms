using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Persistence.Context;

namespace MITCRMS.Implementation.Repository
{
    public class HodRepository : BaseRepository, IHodRepository
    {
        public HodRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {
        }

        public async Task<List<Hod>> GetHodByDepartment(string DepartmentName)
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                return new List<Hod>();
            }

            return await _mitcrmsContext.Set<Hod>()
                .Include(h => h.User)
                .Include(h => h.Department)
                .AsNoTracking()
                .Where(h => h.Department != null && h.Department.DepartmentName == DepartmentName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Hod>> GetAllHodAndTheirDepartment()
        {
            return await _mitcrmsContext.Set<Hod>()
                .Include(h => h.User)
                .Include(h => h.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Hod> GetHodByIdAsync(Guid HodId)
        {
            return await _mitcrmsContext.Set<Hod>()
                .Include(h => h.User)
                .Include(h => h.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == HodId);
        }

        //public async Task<bool> ExistsEmail(string email)
        //{
        //    return await _mitcrmsContext.hods.AnyAsync(std => std.Email == email);
        //}

    }
}