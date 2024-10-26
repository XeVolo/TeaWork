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
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;

        public NotificationService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity =userIdentity;
        }
        public async Task NewNotification(NotificationDto notificationData)
        {

            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
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
        public async Task<Notification> NewInvitation(string userId, int projectId)
        {

            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var project = await _context.Projects.FirstOrDefaultAsync(m => m.Id == projectId);

                Notification notification = new Notification
                {
                    UserId = userId,
                    Title = "New Invitation",
                    Description = project.Title,
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
                throw new NotImplementedException();
            }
        }

        public async Task<Notification> GetNotificationById(int id)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(m => m.Id == id);
                
                return notification!;
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
                using var _context = _dbContextFactory.CreateDbContext();
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

                using var _context = _dbContextFactory.CreateDbContext();
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
