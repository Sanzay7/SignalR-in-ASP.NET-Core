using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRInAspNetCore
{
      public class ChatHub : Hub
        {
            public async Task SendMessage(string user, string message, string groupName)
            {
                await Clients.Group(groupName).SendAsync("ReceivedMessage", user, message);
            }
            public async Task BroadCastMessage(string user, string message)
            {
                await Clients.All.SendAsync(user, message);
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", groupName,"");
        }
        public async Task LeaveGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} Left {groupName}","system");
        }
    }



}
