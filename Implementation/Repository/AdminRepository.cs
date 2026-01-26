using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Persistence.Context;

namespace MITCRMS.Implementation.Repository
{
    public class AdminRepository : BaseRepository, IAdminRepository
    {
        public AdminRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {

        }


        public async Task<List<Admin>> GetAdminByDepartment(string DepartmentName)
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                return new List<Admin>();
            }

            return await _mitcrmsContext.Set<Admin>()
                .Include(a => a.User)
                .Include(a => a.Department)
                .AsNoTracking()
                .Where(a => a.Department != null && a.Department.DepartmentName == DepartmentName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Admin>> GetAllAdminAndTheirDepartment()
        {
            return await _mitcrmsContext.Set<Admin>()
                .Include(a => a.User)
                .Include(a => a.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Admin> GetAdminByIdAsync(Guid AdminId)
        {
            return await _mitcrmsContext.Set<Admin>()
                .Include(a => a.User)
                .Include(a => a.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == AdminId);
        }

        public async Task<int> GetTutorCounts()
        {
            return await _mitcrmsContext.Set<Tutor>().CountAsync();
        }
    }
}