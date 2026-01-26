using MITCRMS.Contract.Entity;

namespace MITCRMS.Models.Entities
{
    public class Tutor : BaseUser
    {
        public int YearsOfExperience { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Department Department { get; set; }
        public Guid DepartmentId { get; set; }

        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
