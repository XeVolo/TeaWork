﻿using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TeaWork.Data;
using TeaWork.Data.Models;
using TeaWork.Logic.Services.Interfaces;

namespace TeaWork.Logic.Hubs
{
    public class CommunicationHub : Hub
    {
        private readonly IConversationService _conversationService;

        public CommunicationHub(IConversationService conversationService)
        {
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

        public async Task SendGroupMessage(int messageId, string groupName)
           => await Clients.Group(groupName).SendAsync("ReceiveMessage", messageId);
        public async Task SendGroupMessageNotification(string senderId, string message, string groupName)
           => await Clients.Group(groupName).SendAsync("ReceiveMessageNotification", senderId, message);

        public async Task SendGroupDesignConcept(string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveDesignConcept");

        public async Task SendGroupDesignConceptNotification(string title, string message, string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveDesignConceptNotification", title, message);

        public async Task SendGroupDesignConceptCommentNotification(string title, string message, string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveDesignConceptCommentNotification", title, message);

        public async Task SendGroupTask(string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveTask");

        public async Task SendGroupTaskNotification(string title, string message, string groupName)
            => await Clients.Group(groupName).SendAsync("ReceiveTaskNotification", title, message);

        public async Task SendInvitationNotification(string title, string description, string groupName)
           => await Clients.Group(groupName).SendAsync("ReceiveInvationNotification", title, description);



    }
}
