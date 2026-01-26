using MITCRMS.Models.Entities;

namespace MITCRMS.Interface.Repository
{
    public interface IBursarRepository : IBaseRepository
    {
        public Task<List<Bursar>> GetBursarByDepartment(string DepartmentName);
        public Task<IReadOnlyList<Bursar>> GetAllBursarAndTheirDepartment();
        public Task<Bursar> GetBursarByIdAsync(Guid BursarId);

    }
}
