using Microsoft.EntityFrameworkCore;
using MITCRMS.Interface.Repository;
using MITCRMS.Models.DTOs;
using MITCRMS.Models.Entities;
using MITCRMS.Persistence.Context;

namespace MITCRMS.Implementation.Repository
{
    public class TutorRepository : BaseRepository, ITutorRepository
    {
        public TutorRepository(MitcrmsContext mitcrmsContext) : base(mitcrmsContext)
        {

        }
   

        public async  Task<List<Tutor>> GetTutorByDepartment(string DepartmentName)
        {
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                return new List<Tutor>();
            }

            return await _mitcrmsContext.Set<Tutor>()
                .Include(t => t.User)
                .Include(t => t.Department)
                .AsNoTracking()
                .Where(t => t.Department != null && t.Department.DepartmentName == DepartmentName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Tutor>> GetAllTutorAndTheirDepartment()
        {
            return await _mitcrmsContext.Set<Tutor>()
                .Include(t => t.User)
                .Include(t => t.Department)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tutor> GetTutorByIdAsync(Guid TutorId)
        {
            return await _mitcrmsContext.Set<Tutor>()
                .Include(t => t.User)
                .Include(t => t.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == TutorId);
        }
    }
}
