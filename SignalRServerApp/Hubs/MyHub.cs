using Microsoft.AspNetCore.SignalR;

namespace SignalRServerApp.Hubs;

public class MyHub : Hub
{
    public Task SendMessage(string userAction, string message)
    {
        return Clients.All.SendAsync("ReceiveMessage", userAction, message);
    }
}
