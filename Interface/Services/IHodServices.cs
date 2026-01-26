using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Hod;

namespace MITCRMS.Interface.Services
{
    public interface IHodServices
    {
        Task<BaseResponse<bool>> CreateHodAsync(CreateHodRequestModel request);
        Task<BaseResponse<HodDto>> GetHodByIdAsync(Guid HodId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<HodDto>>> GetHodAsync(CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid HodId);
    }
}
