using TeaWork.Data.Enums;

namespace TeaWork.Data.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int ProjectId { get; set; }
        public InvitationStatus Status { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Project? Project { get; set; }

    }
}
