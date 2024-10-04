using TeaWork.Data.Enums;

namespace TeaWork.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationonStatus Status { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
