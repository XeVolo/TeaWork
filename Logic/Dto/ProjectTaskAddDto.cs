using TeaWork.Data.Enums;

namespace TeaWork.Logic.Dto
{
    public class ProjectTaskAddDto
    {
        public DateTime Deadline { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TaskState State { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
