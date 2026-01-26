using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Hod;

namespace MITCRMS.Interface.Services
{
    public interface IAdminServices
    {
        Task<BaseResponse<bool>> CreateAdminAsync(CreateAdminRequestModel request);
        Task<BaseResponse<AdminDto>> GetAdminByIdAsync(Guid AdminId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<AdminDto>>> GetAdminAsync(CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid AdminId);
    }
}

