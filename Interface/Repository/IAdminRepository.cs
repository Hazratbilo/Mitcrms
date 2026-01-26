using MITCRMS.Models.Entities;

namespace MITCRMS.Interface.Repository
{
    public interface IAdminRepository : IBaseRepository
    {
        public Task<IReadOnlyList<Admin>> GetAllAdminAndTheirDepartment();

        public Task<Admin> GetAdminByIdAsync(Guid AdminId);

    }
}
