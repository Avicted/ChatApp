using System.Net.WebSockets;
using ChatAppServer.Models;

namespace ChatAppServer.Interfaces;

public interface IWebSocketService
{
    List<ChatClient> GetWebSocketConnections();

    Task Handle(Guid id, WebSocket webSocket);

    Task<ChatMessage?> ReceiveMessage(Guid id, ChatClient chatClient);
}