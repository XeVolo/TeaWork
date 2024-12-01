using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface INotificationService
    {
        Task NewNotification(NotificationDto notificationdata);
        Task NotificationDisplayed(Notification notification);
        Task<List<Notification>> GetMyNewNotifications();
        Task <Notification>NewInvitation(string userId, int projectId);
    }
}
