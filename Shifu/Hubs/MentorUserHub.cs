using Microsoft.AspNetCore.SignalR;

namespace Shifu.Hubs;

public class MentorUserHub : Hub
{
    public async Task JoinUserGroup(int userId) => await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
    
    public async Task LeaveUserGroup(int userId) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");

    public async Task SendMessageToUserGroup(int userId, string message, bool sentByMentor, int? mentorId)
    {
        await Clients.Group($"user-{userId}").SendAsync("ReceiveMessage", userId, message, sentByMentor, mentorId);
    }
}