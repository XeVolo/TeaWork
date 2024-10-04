namespace TeaWork.Data.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Deadline { get; set; }
        public string? Description { get; set; }
        public int ToDoListId { get; set; }
        public int ProjectConversationId { get; set; }
        public bool IsActive { get; set; }
        public virtual List<ProjectMember>? ProjectMembers { get; set; }
        public virtual ToDoList? ToDoList { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }
}
