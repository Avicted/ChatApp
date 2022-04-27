using ChatAppServer.Interfaces;
using ChatAppServer.Models;

namespace ChatAppServer.Services;

public class VoiceService : IVoiceService
{
    private WebSocketService _webSocketService;
    private List<ChatClient> websocketConnections = new List<ChatClient>();
    public VoiceService(WebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }
}