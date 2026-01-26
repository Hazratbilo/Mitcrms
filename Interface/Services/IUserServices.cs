using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Users;

namespace MITCRMS.Interface.Services
{
    public interface IUserServices
    {

        public Task<BaseResponse<LoginResponseModel>> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken);
        public Task<BaseResponse<UserDto>> GetUserProfileByUserId(Guid userId, CancellationToken cancellationToken);
    }
}
