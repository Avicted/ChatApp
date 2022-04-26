using System.Net.WebSockets;
using System.Text;

public class WebSocketService
{
    public List<ChatClient> websocketConnections = new List<ChatClient>();
    public List<ChatRoom> chatRooms = new List<ChatRoom>() {
        new ChatRoom() {
            Id = new Guid(),
            Clients = new List<WebSocket>(),
            Name = "default room ~",
            Messages = new List<ChatMessage>() {
                new ChatMessage() {
                    Id = new Guid(),
                    Message = "Welcome to the default room ~!",
                    SendDateTime = new DateTime(),
                }
            },
        }
    };


    public async Task Handle(Guid id, WebSocket webSocket)
    {
        var newClient = new ChatClient
        {
            Id = id,
            WebSocket = webSocket
        };

        lock (websocketConnections)
        {
            websocketConnections.Add(newClient);
        }

        await SendMessageToSockets($"User with id {id} has joined the server");

        while (webSocket.State == WebSocketState.Open)
        {
            var message = await ReceiveMessage(id, newClient);
            if (message != null)
                await SendMessageToSockets(message);
        }
    }

    public async Task<string> ReceiveMessage(Guid id, ChatClient chatClient)
    {
        var arraySegment = new ArraySegment<byte>(new byte[4096]);
        var receivedMessage = await chatClient.WebSocket.ReceiveAsync(arraySegment, CancellationToken.None);
        if (receivedMessage.MessageType == WebSocketMessageType.Text)
        {
            var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
            var username = chatClient.Username == null ? "anonymous" : chatClient.Username;

            if (!string.IsNullOrWhiteSpace(message))
                return $"{DateTime.Now}:[{username}]: {message}";
        }
        return null;
    }

    private async Task SendMessageToSockets(string message)
    {
        IEnumerable<ChatClient> toSentTo;

        lock (websocketConnections)
        {
            toSentTo = websocketConnections.ToList();
        }

        var tasks = toSentTo.Select(async websocketConnection =>
        {
            var bytes = Encoding.Default.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes);
            try
            {
                await websocketConnection.WebSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

            }
            catch (System.Exception)
            {

                throw;
            }
        });
        await Task.WhenAll(tasks);
    }
}