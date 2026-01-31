using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Department;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Models.DTOs.Report
{
    public class CreateReportRequestModel
    {
     
            public Guid DepartmentId { get; set; }    // Department for which the report is created
            public string Tittle { get; set; }         // Title of the report
            public string Content { get; set; }       // Content/details of the report
        

    }
}
