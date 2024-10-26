using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TeaWork.Data;
using TeaWork.Data.Models;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Hubs
{
    public class CommunicationHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IConversationService _conversationService;

        public CommunicationHub(ApplicationDbContext context, IConversationService conversationService)
        {
            _context = context;
            _conversationService = conversationService;
        }
        public override async Task OnConnectedAsync()
        {
            string userId = Context.GetHttpContext().Request.Query["userId"];
            if (userId != null)
            {
                List<Conversation> conversations = await _conversationService.GetConversationsByUserId(userId);

                foreach (var conversation in conversations)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, conversation.Id.ToString());
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }


        public async Task SendMessage(int messageId)
        {
            await Clients.All.SendAsync("ReceiveMessage", messageId);
        }
        public async Task SendGroupMessage(int messageId,string groupName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", messageId);
        }

        public async Task SendMessageNotification(string senderId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessageNotification", senderId, message);
        }
        public async Task SendGroupMessageNotification(string senderId, string message, string groupName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessageNotification", senderId, message);
        }

        public async Task SendGroupDesignConcept(string groupName) 
            => await Clients.Group(groupName).SendAsync("ReceiveDesignConcept");

        public async Task SendGroupDesignConceptNotification(string title, string message, string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveDesignConceptNotification", title, message);

        public async Task SendGroupDesignConceptCommentNotification(string title, string message, string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveDesignConceptCommentNotification", title, message);

        public async Task SendInvitationNotification(string title, string description)
        {
            await Clients.All.SendAsync("ReceiveInvationNotification", title, description);
        }
    }
}
