using Microsoft.EntityFrameworkCore;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.CodeAnalysis;

namespace TeaWork.Logic.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;
        private readonly IProjectService _projectService;
        private readonly IConversationService _conversationService;


        public InvitationService(ApplicationDbContext context, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = new UserIdentity(context, authenticationStateProvider);
            _conversationService = new ConversationService(context, authenticationStateProvider);
            _projectService = new ProjectService(context, authenticationStateProvider);
        }
        public async Task SendInvitation(string userId, int projectId)
        {
            try
            {                              

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
                throw new NotImplementedException();
            }
        }
        public async Task<List<Invitation>> GetNewInvitations()
        {

            try
            {
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
                throw new NotImplementedException();
            }
        }
        public async Task AcceptInvitation(int invitationId)
        {
            try
            {
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var invitation = await _context.Invitations.FirstOrDefaultAsync(x => x.Id == invitationId);
                var project = await _context.Projects.FirstOrDefaultAsync(x => x.Id == invitation.ProjectId);
                var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == project.ProjectConversationId);
                invitation.Status= InvitationStatus.Accepted;

                await _projectService.AddProjectMember(project, currentUser, ProjectMemberRole.User);
                await _conversationService.AddMember(conversation, currentUser.Id);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task DeclineInvitation(int invitationId)
        {
            try
            {
                var invitation = _context.Invitations.FirstOrDefault(x => x.Id == invitationId);
                invitation.Status = InvitationStatus.Rejected;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

        }
        public async Task<bool> IsInvitationExist(string userId, int projectId)
        {
            bool isInvitationExists = await _context.Invitations
                .AnyAsync(pm => pm.UserId.Equals(userId) && pm.ProjectId == projectId);

            bool isMemberExists = await _context.ProjectMembers
                .AnyAsync(pm => pm.UserId == userId && pm.ProjectId == projectId);

            return isInvitationExists || isMemberExists;
        }
    }
}
