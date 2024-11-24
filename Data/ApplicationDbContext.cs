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
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<ProjectFile> ProjectFiles { get; set; }
        public DbSet<PrivateTask> PrivateTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>()
                .HasOne(c => c.Project)
                .WithOne(p => p.Conversation)
                .HasForeignKey<Project>(p => p.ProjectConversationId);

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.ConversationMembers)
                .WithOne(cm => cm.Conversation)
                .HasForeignKey(cm => cm.ConversationId);

            modelBuilder.Entity<Conversation>()
                .HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId);

            modelBuilder.Entity<ConversationMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ConversationMembers)
                .HasForeignKey(cm => cm.UserId);

            modelBuilder.Entity<DesignConceptComment>()
                .HasOne(d => d.User)
                .WithMany(u => u.DesignConceptComments)
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<DesignConceptComment>()
                .HasOne(d => d.OwnDesignConcept)
                .WithMany(o => o.DesignConceptComments)
                .HasForeignKey(d => d.OwnDesignConceptId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectMembers)
                .WithOne(pm => pm.Project)
                .HasForeignKey(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMembers)
                .HasForeignKey(pm => pm.UserId);

            modelBuilder.Entity<OwnDesignConcept>()
                .HasOne(odc => odc.User)
                .WithMany(u => u.OwnDesignConcepts)
                .HasForeignKey(odc => odc.UserId);

            modelBuilder.Entity<OwnDesignConcept>()
                .HasOne(odc => odc.Project)
                .WithMany(p => p.OwnDesignConcepts)
                .HasForeignKey(odc => odc.ProjectId);

            modelBuilder.Entity<ToDoList>()
                .HasOne(tdl => tdl.Project)
                .WithOne(p => p.ToDoList)
                .HasForeignKey<Project>(p => p.ToDoListId);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<ProjectTask>()
                .HasOne(t => t.ToDoList)
                .WithMany(tdl => tdl.Tasks)
                .HasForeignKey(t => t.ToDoListId);

            modelBuilder.Entity<TaskDistribution>()
                .HasOne(td => td.User)
                .WithMany(u => u.TasksDistributions)
                .HasForeignKey(td => td.UserId);

            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(tc => tc.UserId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);
            
            modelBuilder.Entity<Invitation>()
                .HasOne (n => n.User)
                .WithMany(u=>u.Invitations)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<Invitation>()
                .HasOne(n=>n.Project)
                .WithMany(p => p.Invitations)
                .HasForeignKey(n => n.ProjectId);

        }
    }

}
