using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ToMainApi.Models.Enums;

namespace ToMainApi.Services.Notifications
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var role = Context.User.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"UserId: {Context.UserIdentifier}, ConnectionId: {Context.ConnectionId}");
            switch (role)
            {
                case "Admin":
                    await Groups.AddToGroupAsync(
                        Context.ConnectionId,
                        NotificationGroupTypes.Admins);
                    break;

                case "Moderator":
                    await Groups.AddToGroupAsync(
                        Context.ConnectionId,
                        NotificationGroupTypes.Moderators);
                    break;

                case "Agent":
                    await Groups.AddToGroupAsync(
                        Context.ConnectionId,
                        NotificationGroupTypes.Agents);
                    break;
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Отключился: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
