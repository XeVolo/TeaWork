using Microsoft.AspNetCore.SignalR;

namespace TeaWork.Logic.Hubs
{
    public class CommunicationHub : Hub
    {
        public async Task SendMessage(int messageId)
        {
            await Clients.All.SendAsync("ReceiveMessage", messageId);
        }
        public async Task SendMessageNotification(string senderId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessageNotification", senderId, message);
        }
    }
}
