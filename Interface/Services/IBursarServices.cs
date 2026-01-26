using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Hod;

namespace MITCRMS.Interface.Services
{
    public interface IBursarServices
    {
        Task<BaseResponse<bool>> CreateBursarAsync(CreateBursarRequestModel request);
        Task<BaseResponse<BursarDto>> GetBursarByIdAsync(Guid BursarId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<BursarDto>>> GetBursarAsync(CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid BursarId);
    }
}

