using TeaWork.Data.Enums;

namespace TeaWork.Data.Models
{
    public class ProjectMember
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? UserId { get; set; }
        public ProjectMemberRole Role { get; set; }
        public virtual Project? Project { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
