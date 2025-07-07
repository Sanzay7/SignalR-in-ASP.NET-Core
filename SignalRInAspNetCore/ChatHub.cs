using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRInAspNetCore
{
    public class ChatHub : Hub
    {
        // Track group memberships manually
        private static readonly ConcurrentDictionary<string, HashSet<string>> GroupMembers = new();

        public async Task SendMessage(string user, string message, string groupName)
        {
            var connectionId = Context.ConnectionId;

            // Check if the user is in the group
            if (GroupMembers.TryGetValue(groupName, out var members) && members.Contains(connectionId))
            {
                await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "System", $"You are not in the group '{groupName}'. Message not sent.");
            }
        }

        public async Task BroadCastMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;

            await Groups.AddToGroupAsync(connectionId, groupName);

            GroupMembers.AddOrUpdate(groupName,
                _ => new HashSet<string> { connectionId },
                (_, existing) => { existing.Add(connectionId); return existing; });

            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"A user joined the group {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;

            await Groups.RemoveFromGroupAsync(connectionId, groupName);

            if (GroupMembers.TryGetValue(groupName, out var members))
            {
                members.Remove(connectionId);
            }

            await Clients.Group(groupName).SendAsync("ReceiveMessage", "System", $"A user left the group {groupName}");
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Clean up user from all groups
            foreach (var kvp in GroupMembers)
            {
                kvp.Value.Remove(Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }
}
