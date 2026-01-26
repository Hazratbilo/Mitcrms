using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.Entities;
using MITCRMS.Persistence.Context;

namespace MITCRMS.Implementation.Repository
{
    public class BursarRepository : BaseRepository, IBursarRepository
    {
        public BursarRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {
        }

        public async Task<List<Bursar>> GetBursarByDepartment(string DepartmentName)
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                return new List<Bursar>();
            }

            return await _mitcrmsContext.Set<Bursar>()
                .Include(b => b.User)
                .Include(b => b.Department)
                .AsNoTracking()
                .Where(b => b.Department != null && b.Department.DepartmentName == DepartmentName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Bursar>> GetAllBursarAndTheirDepartment()
        {
            return await _mitcrmsContext.Set<Bursar>()
                .Include(b => b.User)
                .Include(b => b.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Bursar> GetBusarByIdAsync(Guid BursarId)
        {
            return await _mitcrmsContext.Set<Bursar>()
                .Include(b => b.User)
                .Include(b => b.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == BursarId);
        }

        public Task<Bursar> GetBursarByIdAsync(Guid BursarId)
        {
            throw new NotImplementedException();
        }
    }
}