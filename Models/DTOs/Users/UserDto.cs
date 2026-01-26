using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Role;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Models.DTOs.Users
{
    public class UserDto
    {

        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }

        public string Email { get; set; }
        public ICollection<RoleDto> Roles { get; set; } = [];
        public TutorDto Tutor { get; set; }
        public HodDto Hod { get; set; }
        public AdminDto Admin { get; set; }
        public BursarDto Bursar { get; set; }
        public SuperAdminDto SuperAdmin { get; set; }

    }
}

