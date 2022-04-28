using System.Net.WebSockets;

namespace ChatAppServer.Models;

public class ChatRoom
{
    public Guid Id { get; set; }
    public String Name { get; set; } = null!;
    public List<ChatClient> Clients { get; set; } = new List<ChatClient>();
    public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatClient
{
    public Guid Id { get; set; }
    public WebSocket WebSocket { get; set; } = null!;
    public String Username { get; set; } = null!;
    public List<Topic> TopicsSubscribedTo { get; set; } = new List<Topic>();
}

public enum MessageType
{
    Message,
    InfoToUser,
    ServerInfo,
    Command,
    Voice,
}

public class Topic
{
    public String Name { get; set; } = null!;
}


public class ChatMessage
{
    public Guid Id { get; set; }
    public MessageType MessageType { get; set; }
    public Topic Topic { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public String AuthorUsername { get; set; } = null!;
    public String Message { get; set; } = null!;
    public DateTime SendDateTime { get; set; }
}

public class WelcomeData
{
    public Guid UserId { get; set; }
    public String Username { get; set; } = null!;
}

public class WelcomeNewUserMessage : ChatMessage
{
    public WelcomeData WelcomeData { get; set; } = null!;
}