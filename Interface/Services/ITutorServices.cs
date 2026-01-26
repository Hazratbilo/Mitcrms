using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Interface.Services
{
    public interface ITutorServices
    {
        Task<BaseResponse<bool>> CreateTutorAsync(CreateTutorRequestModel request);
        Task<BaseResponse<TutorDto>> GetTutorByIdAsync(Guid tutorId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<TutorDto>>> GetTutorAsync(CancellationToken cancellationToken);

        Task<BaseResponse<bool>> DeleteAsync(Guid TutorId);
    }
}
