using MITCRMS.Models.DTOs;
using MITCRMS.Models.DTOs.Role;

namespace MITCRMS.Interface.Services
{
    public interface IRoleServices
    {
        Task<BaseResponse<RoleDto>> GetRoleByIdAsync(Guid roleId, CancellationToken cancellationToken);
        Task<BaseResponse<IReadOnlyList<RoleDto>>> GetRoleAsync(string param, CancellationToken cancellationToken);
        Task<BaseResponse<IEnumerable<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid roleId);
    }
}
