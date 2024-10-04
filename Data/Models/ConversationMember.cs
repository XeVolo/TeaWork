namespace TeaWork.Data.Models
{
    public class ConversationMember
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int ConversationId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }
}
