using MITCRMS.Contract.Entity;

namespace MITCRMS.Models.Entities
{
    public class SuperAdmin : BaseUser
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
