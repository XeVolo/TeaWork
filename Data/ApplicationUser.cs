using Microsoft.AspNetCore.Identity;
using TeaWork.Data.Models;

namespace TeaWork.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public List<ConversationMember>? ConversationMembers { get; set; }
        public List<Message>? Messages { get; set; }
        public List<ProjectMember>? ProjectMembers { get; set; }
        public List<OwnDesignConcept>? OwnDesignConcepts { get; set; }
        public List<ProjectTask>? Tasks { get; set; }
        public List<TaskDistribution>? TasksDistributions { get; set; }
        public List<TaskComment>? Comments { get; set; }
        public List<Notification>? Notifications { get; set; }
        public List<DesignConceptComment>? DesignConceptComments { get; set; }
    }

}
