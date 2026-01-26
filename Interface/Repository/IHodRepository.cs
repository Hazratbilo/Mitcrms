using MITCRMS.Models.DTOs;
using MITCRMS.Models.Entities;
using System.Numerics;

namespace MITCRMS.Interface.Repository
{
    public interface IHodRepository : IBaseRepository
    {
            public Task<List<Hod>> GetHodByDepartment(string DepartmentName);
            public Task<IReadOnlyList<Hod>> GetAllHodAndTheirDepartment();
            public Task<Hod> GetHodByIdAsync(Guid HodId);
        //Task<bool> ExistsEmail(string email);

    }
    }

