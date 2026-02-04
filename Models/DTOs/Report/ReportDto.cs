using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Department;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;
using MITCRMS.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace MITCRMS.Models.DTOs.Report
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }

        public Guid? TutorId { get; set; }
        public TutorDto Tutor { get; set; }

        public Guid? HodId { get; set; }
        public HodDto Hod { get; set; }

        public Guid? BursarId { get; set; }
        public BursarDto Bursar { get; set; }

        public Guid? AdminId { get; set; }
        public AdminDto Admin { get; set; }
        public Guid? DepartmentId { get; set; }
        public DepartmentDto DepartmentName { get; set; }

        public string Tittle { get; set; }
        public string Content { get; set; }
        public string? FileUrl { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime? ReportDate { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedByAdminId { get; set; }
    }
}
