using MITCRMS.Models.Entities;

namespace MITCRMS.Models.DTOs.Role
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
