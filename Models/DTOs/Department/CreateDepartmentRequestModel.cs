using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Models.DTOs.Department
{
    public class CreateDepartmentRequestModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public List<TutorDto> Tutors { get; set; } = new();
        public List<BursarDto> Bursars { get; set; } = new();
        public List<AdminDto> Admins { get; set; } = new();
        public List<HodDto> Hods { get; set; } = new();
    }
}
