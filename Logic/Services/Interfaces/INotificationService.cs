using TeaWork.Data.Models;
using TeaWork.Logic.Dto;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface INotificationService
    {
        Task NewNotification(NotificationDto notificationdata);
        Task<Notification> GetNotificationById(int id);
        Task NotificationDisplayed(Notification notification);
        Task<List<Notification>> GetMyNewNotifications();
        Task<List<Notification>> GetMyAllNotifications();
        Task <Notification>NewInvitation(string userId, int projectId);
    }
}
