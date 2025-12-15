using Microsoft.AspNetCore.SignalR;

namespace TechDemoDashboard.Hubs;

/// <summary>
/// SignalR Hub for real-time chat demonstration.
/// Showcases broadcast messaging, direct messaging, and group messaging.
/// </summary>
public class ChatHub : Hub
{
    /// <summary>
    /// Broadcasts a message to all connected clients.
    /// </summary>
    /// <param name="user">The username of the sender</param>
    /// <param name="message">The message content</param>
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message, DateTime.Now);
    }

    /// <summary>
    /// Sends a notification when a user connects.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Sends a notification when a user disconnects.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
