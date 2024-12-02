using Microsoft.AspNetCore.Components.Authorization;
using TeaWork.Data.Enums;
using TeaWork.Data.Models;
using TeaWork.Data;
using TeaWork.Logic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using TeaWork.Logic.DbContextFactory;

namespace TeaWork.Logic.Services
{
    public class ConversationService : IConversationService
    {
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IUserIdentity _userIdentity;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            ILogger<ConversationService> logger, 
            IDbContextFactory dbContextFactory, 
            IUserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _userIdentity = userIdentity;
            _logger = logger;
        }
        public async Task<Conversation> AddConversation(ConversationType conversationType,string name)
        {

            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var conversation = new Conversation 
                { 
                    ConversationType = conversationType, 
                    Name             = name 
                }; 
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
                return conversation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add conversation.");
                throw;
            }
        }
        public async Task AddMember(Conversation conversation, string userId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();

                var newconversationmember = new ConversationMember 
                { 
                    UserId         = userId, 
                    ConversationId = conversation.Id 
                };
                _context.ConversationMembers.Add(newconversationmember);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add member.");
                throw;
            }
        }
        public async Task<List<Conversation>> GetMyConversations()
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();

                var currentUser = await _userIdentity.GetLoggedUser();

                var conversations = await _context.Conversations
                    .Where(c => _context.ConversationMembers
                        .Any(cm => cm.ConversationId == c.Id && cm.UserId == currentUser.Id))
                    .ToListAsync();

                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get conversations.");
                throw;
            }
        }
        public async Task<List<Conversation>> GetConversationsByUserId(string userId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();

                var conversations = await _context.Conversations
                    .Where(c => _context.ConversationMembers
                        .Any(cm => cm.ConversationId == c.Id && cm.UserId == userId))
                    .ToListAsync();

                return conversations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get conversation.");
                throw;
            }
        }
        public async Task<Conversation> GetConversationById(int id)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var conversation = await _context.Conversations
                    .FirstOrDefaultAsync(x => x.Id == id);
                return conversation!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get conversation.");
                throw;
            }
        }
        public async Task<List<Message>> GetMessegesByConversation(int id)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                var messages = await _context.Messages
                    .Where(x => x.ConversationId == id)
                    .OrderBy(x => x.SendTime)
                    .Include(x => x.Sender)
                    .ToListAsync();
                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get messages.");
                throw;
            }
        }
        public async Task<Message> NewMessage(int conversationId, string content)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                Message message = new Message
                {
                    ConversationId = conversationId,
                    SenderId = currentUser.Id,
                    Content = content,
                    SendTime = DateTime.Now,
                };
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();                
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create message.");
                throw;
            }
        }
        public async Task<string> GetConversationName(int conversationId)
        {
            try
            {               
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                var conversation = await _context.Conversations
                    .Include(x=>x.ConversationMembers)
                    .FirstOrDefaultAsync(x => x.Id == conversationId);

                if (conversation != null)
                {
                    if (conversation.ConversationType.Equals(ConversationType.GroupChat))
                    {
                        return conversation.Name ?? conversation.Id.ToString();
                    }

                    if (conversation.ConversationMembers != null)
                    {
                        var otherUser = conversation.ConversationMembers
                                .FirstOrDefault(cm => cm.UserId != currentUser.Id);
                        if (otherUser != null)
                        {
                            var name = await _context.Users
                                .Where(x => x.Id == otherUser.UserId)
                                .Select(x => x.Email)
                                .FirstOrDefaultAsync();

                            return name ?? "Unknown User";
                        }
                    }
                
                }
                return conversationId.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get conversation name.");
                throw;
            }
        }
        public async Task<bool> CheckUserAccess(int conversationId)
        {
            try
            {
                await using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();

                var conversation = await _context.ConversationMembers
                    .Where(x=>x.UserId.Equals(currentUser.Id))
                    .Where(x=>x.ConversationId==conversationId)
                    .FirstOrDefaultAsync();

                return conversation != null;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user access.");
                throw;
            }
        }

    }
}
