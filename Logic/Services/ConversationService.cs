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
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly UserIdentity _userIdentity;

        public ConversationService(IDbContextFactory dbContextFactory, AuthenticationStateProvider authenticationStateProvider, UserIdentity userIdentity)
        {
            _dbContextFactory = dbContextFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _userIdentity = userIdentity;
        }
        public async Task<Conversation> AddConversation(ConversationType conversationType)
        {

            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                Conversation conversation = new Conversation { ConversationType = conversationType };
                _context.Conversations.Add(conversation);
                await _context.SaveChangesAsync();
                return conversation;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var conversation = await _context.Conversations.FirstOrDefaultAsync(m => m.Id == id);
                _context.Conversations.Remove(conversation!);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task AddMember(Conversation conversation, string userId)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ConversationMember newconversationmember = new ConversationMember { UserId = userId, ConversationId = conversation.Id };
                _context.ConversationMembers.Add(newconversationmember);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<Conversation>> GetMyConversations()
        {
            List<Conversation> conversations = new List<Conversation>();
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                var conversationMembers = await _context.ConversationMembers.Where(x => x.UserId.Equals(currentUser.Id)).ToListAsync();
                foreach (var conversationMember in conversationMembers)
                {
                    var conversation = await _context.Conversations.Where(x => x.Id == conversationMember.ConversationId).FirstOrDefaultAsync();
                    if (conversation != null)
                        conversations.Add(conversation);
                }
                return conversations;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<Conversation>> GetConversationsByUserId(string userId)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                List<Conversation> conversations = new List<Conversation>();
                var conversationMembers = await _context.ConversationMembers.Where(x => x.UserId.Equals(userId)).ToListAsync();
                foreach (var conversationMember in conversationMembers)
                {
                    var conversation = await _context.Conversations.Where(x => x.Id == conversationMember.ConversationId).FirstOrDefaultAsync();
                    if (conversation != null)
                        conversations.Add(conversation);
                }
                return conversations;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<Conversation> GetConversationById(int id)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var conversation = await _context.Conversations
                    .Include(x => x.Messages)
                    .FirstOrDefaultAsync(x => x.Id == id);
                return conversation!;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<List<Message>> GetMessegesByConversation(int id)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var messages = await _context.Messages
                    .Where(x => x.ConversationId == id)
                    .OrderBy(x => x.SendTime)
                    .Include(x => x.Sender)
                    .ToListAsync();
                return messages;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<Message> NewMessage(int conversationId, string content)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                ApplicationUser currentUser = await _userIdentity.GetLoggedUser();
                Message message = new Message
                {
                    ConversationId = conversationId,
                    SenderId = currentUser.Id,
                    Content = content,
                    SendTime = DateTime.Now,
                };
                _context.Messages.Add(message);
                _context.SaveChanges();
                return message;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }
        public async Task<Message> GetMessegesById(int id)
        {
            try
            {
                using var _context = _dbContextFactory.CreateDbContext();
                var message = await _context.Messages
                    .FirstOrDefaultAsync(x => x.Id == id); ;
                return message!;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }
        }

    }
}
