using System.ComponentModel.DataAnnotations;
using TeaWork.Data.Enums;

namespace TeaWork.Logic.Dto
{
    public class ProjectTaskAddDto
    {
        public DateTime? Start { get; set; }
        public DateTime? Deadline { get; set; }
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        public TaskState State { get; set; }
        public TaskPriority Priority { get; set; }
    }
}
