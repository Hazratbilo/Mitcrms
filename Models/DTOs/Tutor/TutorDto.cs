using MITCRMS.Models.Entities;
using System.Reflection;

namespace MITCRMS.Models.DTOs.Tutor
{
    public class TutorDto
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int YearsOfExperience { get; set; }

        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
