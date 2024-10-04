using TeaWork.Data.Enums;

namespace TeaWork.Logic.Dto
{
    public class NotificationDto
    {
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public NotificationType NotifiType { get; set; }
    }
}
