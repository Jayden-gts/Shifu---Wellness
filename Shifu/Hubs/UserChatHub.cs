using Microsoft.AspNetCore.SignalR;

namespace Shifu.Hubs;

public class UserChatHub : Hub
{
    public async Task SendMessage(int mentorId, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", mentorId, message);
    }
    
    
    // add other stuff as optional 
    
}