using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Role;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Models.DTOs.Users
{
    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseModel : BaseResponse
    {
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public IEnumerable<RoleDto> Roles { get; set; } = new List<RoleDto>();

        public TutorDto Tutor { get; set; }
        public HodDto Hod { get; set; }
        public BursarDto Bursar { get; set; }
        public AdminDto Admin { get; set; }
        public SuperAdminDto SuperAdmin { get; set; }
    }
}
