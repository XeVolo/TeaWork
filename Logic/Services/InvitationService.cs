using Microsoft.EntityFrameworkCore;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.CodeAnalysis;
using TeaWork.Logic.DbContextFactory;

namespace TeaWork.Logic.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IUserIdentity _userIdentity;
        private readonly IProjectService _projectService;
        private readonly IConversationService _conversationService;
        private readonly ILogger<InvitationService> _logger;


        public InvitationService(
            IDbContextFactory dbContextFactory, 
            IUserIdentity userIdentity, 
            IConversationService conversationService,
            IProjectService projectService,
            ILogger<InvitationService>logger)
        {
            _dbContextFactory = dbContextFactory;
            _userIdentity = userIdentity;
            _conversationService =conversationService;
            _projectService = projectService;
            _logger = logger;
        }
        public async Task SendInvitation(string userId, int projectId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                Invitation invitation = new Invitation
                {
                    UserId = userId,
                    ProjectId = projectId,
                    Status = InvitationStatus.Processed,
                };
                _context.Invitations.Add(invitation);
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send invitation.");
                throw;
            }
        }
        public async Task<List<Invitation>> GetNewInvitations()
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                var invitations = await _context.Invitations
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .Where(x => x.Status.Equals(InvitationStatus.Processed))
                    .Include(x => x.Project)
                    .ToListAsync();
                return invitations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get new invitations.");
                throw;
            }
        }
        public async Task AcceptInvitation(int invitationId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var invitation = await _context.Invitations.FirstOrDefaultAsync(x => x.Id == invitationId);
                if (invitation != null)
                {
                    var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == invitation.ProjectId);
                    if (project != null)
                    {
                        var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == project.ProjectConversationId);
                        if (conversation != null)
                        {
                            await _projectService.AddProjectMember(project, currentUser, ProjectMemberRole.User);
                            await _conversationService.AddMember(conversation, currentUser.Id);

                            invitation.Status = InvitationStatus.Accepted;
                            _context.Attach(invitation).State = EntityState.Modified;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to accept invitation.");
                throw;
            }
        }
        public async Task DeclineInvitation(int invitationId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var invitation = _context.Invitations.FirstOrDefault(x => x.Id == invitationId);
                if (invitation != null)
                {
                    invitation.Status = InvitationStatus.Rejected;
                    _context.Attach(invitation).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decline invitation.");
                throw;
            }

        }
        public async Task<bool> IsInvitationExist(string userId, int projectId)
        {
            await using var _context = _dbContextFactory.CreateDbContext();

            bool isInvitationExists = await _context.Invitations
                .AnyAsync(pm => pm.UserId.Equals(userId) && pm.ProjectId == projectId);

            bool isMemberExists = await _context.ProjectMembers
                .AnyAsync(pm => pm.UserId == userId && pm.ProjectId == projectId);

            return isInvitationExists || isMemberExists;
        }
    }
}
