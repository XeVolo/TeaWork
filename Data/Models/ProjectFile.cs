namespace TeaWork.Data.Models
{
    public class ProjectFile
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileStorageUrl { get; set; }
        public string? ContentType { get; set; }
        public int FileSize { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsDeleted { get; set; }
        public string? UserId { get; set; }
        public int ProjectId { get; set; }

        public virtual Project? Project { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
