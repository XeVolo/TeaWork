using TeaWork.Data.Enums;
using TeaWork.Data.Models;

namespace TeaWork.Logic.Services.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation> AddConversation(ConversationType conversationType);
        Task Delete(int id);
        Task AddMember(Conversation conversation, string userId);
        Task<List<Conversation>> GetMyConversations();
        Task<Conversation> GetConversationById(int id);
        Task<List<Message>> GetMessegesByConversation(int id);
        Task<Message> NewMessage(int conversationId, string content);
        Task<Message> GetMessegesById(int id);
    }
}
