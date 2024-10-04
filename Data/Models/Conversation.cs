using Microsoft.CodeAnalysis;
using TeaWork.Data.Enums;

namespace TeaWork.Data.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ConversationType ConversationType { get; set; }
        public bool IsActive { get; set; }
        public virtual List<ConversationMember>? ConversationMembers { get; set; }
        public virtual List<Message>? Messages { get; set; }
        public virtual Project? Project { get; set; }
    }
}
