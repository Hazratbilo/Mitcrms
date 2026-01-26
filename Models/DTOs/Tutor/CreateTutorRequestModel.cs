using Microsoft.AspNetCore.Mvc.Rendering;
using MITCRMS.Models.Entities;
using System.Reflection;

namespace MITCRMS.Models.DTOs.Tutor
{
    public class CreateTutorRequestModel
    {

        public DateTime DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string ConfirmPassword { get; set; }
        public string PhoneNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public Guid DepartmentId { get; set; } 
        public List<SelectListItem> Departments { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public List<Guid> RoleIds { get; set; } = [];
    }
}
