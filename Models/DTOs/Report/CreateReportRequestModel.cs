using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Department;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Models.DTOs.Report
{
    public class CreateReportRequestModel
    {
     
            public Guid DepartmentId { get; set; } 
            public string Tittle { get; set; }     
            public string Content { get; set; }
            public string? FileUrl { get; set; }

    }
}
