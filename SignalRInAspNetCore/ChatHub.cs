using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRInAspNetCore
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string groupName)
        {
            // Broadcast to only the specified group
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }

        public async Task BroadCastMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Optional: notify others in the group someone joined
            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"A user joined the group {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"A user left the group {groupName}");
        }
    }
}
