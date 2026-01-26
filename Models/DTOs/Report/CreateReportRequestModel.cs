using MITCRMS.Models.DTOs.Admin;
using MITCRMS.Models.DTOs.Bursar;
using MITCRMS.Models.DTOs.Hod;
using MITCRMS.Models.DTOs.Tutor;

namespace MITCRMS.Models.DTOs.Report
{
    public class CreateReportRequestModel
    {
        public string Tittle { get; set; }
        public string Content { get; set; }
        public Guid TutorId { get; set; }
        public TutorDto Tutor { get; set; }

        public Guid BursarId { get; set; }
        public BursarDto Bursar { get; set; }
        public Guid HodId { get; set; }
        public HodDto Hod { get; set; }
        public Guid AdminId { get; set; }
        public AdminDto Admin { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
