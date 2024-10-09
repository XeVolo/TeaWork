using TeaWork.Data.Models;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IInvitationService
    {
        Task SendInvitation(string userId, int projectId);
        Task<List<Invitation>> GetNewInvitations();
        Task AcceptInvitation(int invitationId);
        Task DeclineInvitation(int invitationId);
        Task<bool> IsInvitationExist(string userId, int projectId);
    }
}
