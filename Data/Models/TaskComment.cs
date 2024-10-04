namespace TeaWork.Data.Models
{
    public class TaskComment
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime CreationDate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ProjectTask? Task { get; set; }
    }
}
