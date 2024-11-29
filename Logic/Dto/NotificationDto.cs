using System.ComponentModel.DataAnnotations;
using TeaWork.Data.Enums;

namespace TeaWork.Logic.Dto
{
    public class NotificationDto
    {
        public string? UserId { get; set; }
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public NotificationType NotifiType { get; set; }
    }
}
