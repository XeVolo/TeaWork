using System.ComponentModel.DataAnnotations;

namespace TeaWork.Logic.Dto
{
    public class ReportForm
    {
        [StringLength(500, ErrorMessage = "Summary cannot exceed 500 characters.")]
        public string? Summary { get; set; }
        public List<UserForm> UserForms { get; set; }
        [StringLength(500, ErrorMessage = "Additional Comment cannot exceed 500 characters.")]
        public string? AdditionalComment { get; set; }
    }
    public class UserForm
    {
        public string? User {  get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
