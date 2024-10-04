using TeaWork.Data.Enums;

namespace TeaWork.Data.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime Deadline { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public TaskState State { get; set; }
        public TaskPriority Priority { get; set; }
        public int ToDoListId { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ToDoList? ToDoList { get; set; }
        public virtual List<TaskComment>? TaskComments { get; set; }
        public virtual List<TaskDistribution>? TasksDistributions { get; set; }
    }
}
