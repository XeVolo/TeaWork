using Microsoft.AspNetCore.Identity;
using TeaWork.Data.Models;

namespace TeaWork.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public virtual List<ConversationMember>? ConversationMembers { get; set; }
        public virtual List<Message>? Messages { get; set; }
        public virtual List<ProjectMember>? ProjectMembers { get; set; }
        public virtual List<OwnDesignConcept>? OwnDesignConcepts { get; set; }
        public virtual List<ProjectTask>? Tasks { get; set; }
        public virtual List<TaskDistribution>? TasksDistributions { get; set; }
        public virtual List<TaskComment>? Comments { get; set; }
        public virtual List<Notification>? Notifications { get; set; }
        public virtual List<DesignConceptComment>? DesignConceptComments { get; set; }

        public virtual List<Invitation>? Invitations { get; set; }

    }

}
