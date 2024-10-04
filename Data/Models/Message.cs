namespace TeaWork.Data.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string? SenderId { get; set; }
        public DateTime SendTime { get; set; }
        public string? Content { get; set; }
        public virtual Conversation? Conversation { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
    }
}
