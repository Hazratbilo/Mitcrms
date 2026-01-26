using MITCRMS.Models.Entities;

namespace MITCRMS.Interface.Repository
{
    public interface ITutorRepository : IBaseRepository
    {
        
        public Task<List<Tutor>> GetTutorByDepartment(string DepartmentName);
        public Task<IReadOnlyList<Tutor>> GetAllTutorAndTheirDepartment();
        public Task<Tutor> GetTutorByIdAsync(Guid TutorId);


}
}
