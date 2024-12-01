using System.ComponentModel.DataAnnotations;

namespace TeaWork.Logic.Dto
{
    public class DesignConceptDto
    {

        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
