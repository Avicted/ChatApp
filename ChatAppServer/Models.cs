using System.Net.WebSockets;

public class ChatRoom
{
    public Guid Id { get; set; }
    public String Name { get; set; } = null!;
    public List<ChatClient> Clients { get; set; } = null!;
    public List<ChatMessage> Messages { get; set; } = null!;
}

public class ChatClient
{
    public Guid Id { get; set; }
    public WebSocket WebSocket { get; set; } = null!;
    public String Username { get; set; } = null!;
}


public class ChatMessage
{
    public Guid Id { get; set; }
    public String AuthorUsername { get; set; } = null!;
    public String Message { get; set; } = null!;
    public DateTime SendDateTime { get; set; }
}
