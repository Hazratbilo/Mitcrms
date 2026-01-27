using MITCRMS.Contract.Entity;
using MITCRMS.Models.Enum;

namespace MITCRMS.Models.Entities
{
    public class Report : BaseEntity
    {
        public string Tittle { get; set; }
        public string Content { get; set; }



        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }


        public Guid TutorId { get; set; }
        public Tutor Tutor { get; set; }

        public Guid HodId { get; set; }
        public Hod Hod { get; set; }

        public Guid AdminId {  get; set; }
        public Admin Admin { get; set; }

        public Guid BursarId {  get; set; }
        public Bursar Bursar { get; set; }




        //public Guid? ApprovedByAdminId { get; set; }

        //public SuperAdmin? ApprovedByAdmin { get; set; }
        public DateTime? ReportDate { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public ReportStatus Status { get; set; } = ReportStatus.Pending;
    }
}
