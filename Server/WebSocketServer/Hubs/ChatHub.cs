using Microsoft.AspNetCore.SignalR;
using WebSocketServer.Models;

namespace WebSocketServer.Hubs;

public class ChatHub(ILogger<ChatHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("Client connected: " + Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("Client disconnected: " + Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(ChatMessage message)
    {
        Console.WriteLine($"Username : {message.Username} / Message : {message.Message}");
        Console.WriteLine($"Time : {message.Timestamp}");
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}
