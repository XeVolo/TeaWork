using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TeaWork.Logic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;

        public NotificationService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = new UserIdentity(context, authenticationStateProvider);
        }
        public async Task NewNotification(NotificationDto notificationData)
        {

            try
            {
                Notification notification = new Notification
                {
                    UserId = notificationData.UserId,
                    Title = notificationData.Title,
                    Description = notificationData.Description,
                    CreationDate = DateTime.Now,
                    NotificationType = notificationData.NotifiType,
                    Status = NotificationonStatus.New,

                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task NotificationDisplayed()
        {

        }
        public async Task<List<Notification>> GetMyNewNotifications()
        {

            try
            {

                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var notifications = await _context.Notifications
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .Where(x => x.Status.Equals(NotificationonStatus.New))
                    .ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<Notification>> GetMyAllNotifications()
        {
            try
            {

                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var notifications = await _context.Notifications
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
    }
}
