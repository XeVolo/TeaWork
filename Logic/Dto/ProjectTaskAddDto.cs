using TeaWork.Data.Enums;

namespace TeaWork.Logic.Dto
{
    public class ProjectTaskAddDto
    {
        public DateTime? Start { get; set; }
        public DateTime? Deadline { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskState State { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
