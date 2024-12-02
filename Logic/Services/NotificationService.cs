using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Dto;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TeaWork.Logic.DbContextFactory;

namespace TeaWork.Logic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly UserIdentity _userIdentity;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IDbContextFactory dbContextFactory, 
            UserIdentity userIdentity,
            ILogger<NotificationService> logger)
        {
            _dbContextFactory = dbContextFactory;
            _userIdentity =userIdentity;
            _logger = logger;
        }
        public async Task NewNotification(NotificationDto notificationData)
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
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
                _logger.LogError(ex, "Failed to add notification.");
                throw;
            }
        }
        public async Task<Notification> NewInvitation(string userId, int projectId)
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == projectId);

                Notification notification = new Notification
                {
                    UserId = userId,
                    Title = "New Invitation",
                    Description = project!.Title,
                    CreationDate = DateTime.Now,
                    NotificationType = NotificationType.Invitation,
                    Status = NotificationonStatus.New,

                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return notification;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add invitation notification.");
                throw;
            }
        }
        public async Task NotificationDisplayed(Notification notification)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                notification.Status = NotificationonStatus.Seen;
                _context.Attach(notification!).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to display notification.");
                throw;
            }
        }
        public async Task<List<Notification>> GetMyNewNotifications()
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var notifications = await _context.Notifications
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .Where(x => x.Status.Equals(NotificationonStatus.New))
                    .ToListAsync();

                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get notifications.");
                throw;
            }
        }
    }
}
