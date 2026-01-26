using MITCRMS.Contract.Entity;
using MITCRMS.Models.Enum;
using System.Numerics;

namespace MITCRMS.Models.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public Guid TutorId { get; set; }

        public Tutor Tutor { get; set; }
        public Guid HodId { get; set; }
        public Hod Hod { get; set; }
        public Guid AdminId { get; set; }
        public Admin Admin { get; set; }
        public Guid SuperAdminId { get; set; }
        public SuperAdmin SuperAdmin { get; set; }
        public Guid BursarId { get; set; }
        public Bursar Bursar { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = [];


        public string ChangePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new ArgumentException("Password cannot be empty", nameof(newPassword));
            }
            PasswordHash = newPassword;
            return PasswordHash;
        }
    }
}
