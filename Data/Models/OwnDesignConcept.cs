namespace TeaWork.Data.Models
{
    public class OwnDesignConcept
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int ProjectId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Project? Project { get; set; }
        public virtual List<DesignConceptComment>? DesignConceptComments { get; set; }
    }
}
