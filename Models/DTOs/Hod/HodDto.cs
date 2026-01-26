using MITCRMS.Models.Entities;
using System.Reflection;

namespace MITCRMS.Models.DTOs.Hod
{
    public class HodDto
    {

        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
