using MITCRMS.Contract.Entity;
using System.Collections;

namespace MITCRMS.Models.Entities
{
    public class Department : BaseEntity
    {
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public ICollection<Tutor> Tutors { get; set; }
        public Guid? HodId {  get; set; }
        public Hod? Hod { get; set; }
        public ICollection<Hod> Hods { get; set; } = new List<Hod>();
        public ICollection<Admin> Admins { get; set; }
        public ICollection<Bursar> Bursars { get; set; }
        public ICollection<Report> Reports { get; set; }

    }
}
