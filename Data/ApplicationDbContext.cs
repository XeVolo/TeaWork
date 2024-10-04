using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeaWork.Data.Models;

namespace TeaWork.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ConversationMember> ConversationMembers { get; set; }
        public DbSet<DesignConceptComment> DesignConceptComments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<OwnDesignConcept> OwnDesignConcepts { get; set; }
        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }
        public DbSet<TaskDistribution> TaskDistributions { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
