namespace TeaWork.Data.Models
{
    public class TaskDistribution
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string? UserId { get; set; }
        public virtual ProjectTask? Task { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
