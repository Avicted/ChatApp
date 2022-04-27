using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

public class WebSocketService
{
    public List<ChatClient> websocketConnections = new List<ChatClient>();
    public List<ChatRoom> chatRooms = new List<ChatRoom>() {
        new ChatRoom() {
            Id = new Guid(),
            Clients = new List<ChatClient>(),
            Name = "~ default room ~",
            Messages = new List<ChatMessage>() {
                new ChatMessage() {
                    Id = new Guid(),
                    Message = "Welcome to the ~ default room ~ !",
                    SendDateTime = DateTime.Now,
                    AuthorUsername = "Server",
                    MessageType = MessageType.InfoToUser
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

        lock (chatRooms)
        {
            chatRooms[0].Clients.Add(newClient);
        }

        // Console.WriteLine($"{chatRooms}");


        ChatMessage userJoinedMessage = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            Message = $"User with id {id} has joined the server",
            SendDateTime = DateTime.Now,
            AuthorUsername = "Server",
            MessageType = MessageType.ServerInfo

        };

        await SendMessageToSockets(userJoinedMessage, null);

        // Send only to the new user joining
        ChatMessage setUsernameInfoMessage = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            Message = $"You can set your username with: set username <bob>",
            SendDateTime = DateTime.Now,
            AuthorUsername = "Server",
            MessageType = MessageType.InfoToUser
        };
        await SendMessageToSockets(setUsernameInfoMessage, new List<ChatClient>() { newClient });

        while (webSocket.State == WebSocketState.Open)
        {
            try
            {
                var message = await ReceiveMessage(id, newClient);
                if (message != null)
                    await SendMessageToSockets(message, null);
            }
            catch (WebSocketException e)
            {
                lock (websocketConnections)
                {
                    websocketConnections.Remove(newClient);
                }

                lock (chatRooms)
                {
                    chatRooms[0].Clients.Remove(newClient);
                }

                throw new ApiRemotePartyClosedConnectionException();
            }

        }

        webSocket.Dispose();
    }

    public async Task<ChatMessage?> ReceiveMessage(Guid id, ChatClient chatClient)
    {
        var arraySegment = new ArraySegment<byte>(new byte[4096]);

        try
        {
            var receivedMessage = await chatClient.WebSocket.ReceiveAsync(arraySegment, CancellationToken.None);


            if (receivedMessage == null)
            {
                return null;
            }

            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                var username = chatClient.Username == null ? "anonymous" : chatClient.Username;

                // Does the user want to issue a command?
                // set username avic
                var tokens = message.Split(" ");
                if (tokens.Length == 3)
                {
                    // Console.WriteLine($"tokens[0]: {tokens[0]}\ntokens[1]: {tokens[1]}\ntokens[2]: {tokens[2]}\n");

                    if (tokens[0] == "set")
                    {
                        if (tokens[1] == "username")
                        {
                            lock (websocketConnections)
                            {
                                var user = websocketConnections.FirstOrDefault(u => u.Id == chatClient.Id);

                                if (user != null)
                                {
                                    Console.WriteLine($"Setting the username of user Id: {chatClient.Id} to:\n{tokens[2].Trim()}");
                                    user.Username = tokens[2].Trim();
                                }
                            }
                        }
                    }
                }


                if (!string.IsNullOrWhiteSpace(message))
                {
                    var chatMessage = new ChatMessage()
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        SendDateTime = DateTime.Now,
                        AuthorUsername = username,
                        MessageType = MessageType.Message
                    };

                    return chatMessage;
                }
            }

            return null;
        }
        catch (WebSocketException webSocketException)
        {
            if (webSocketException.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
                // Custom logic
            }

            return null;
        }
    }

    private async Task SendMessageToSockets(ChatMessage chatMessage, List<ChatClient>? receivers)
    {
        IEnumerable<ChatClient> toSentTo;

        if (receivers != null)
        {
            toSentTo = (IEnumerable<ChatClient>)receivers;
        }
        else
        {
            lock (websocketConnections)
            {
                toSentTo = websocketConnections.ToList();
            }
        }

        lock (chatRooms)
        {
            chatRooms[0].Messages.Add(chatMessage);
        }

        var tasks = toSentTo.Select(async websocketConnection =>
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(chatMessage));
            var bytes = Encoding.Default.GetBytes(json);
            var arraySegment = new ArraySegment<byte>(bytes);

            try
            {
                if (websocketConnection.WebSocket.State == WebSocketState.Open)
                {
                    await websocketConnection.WebSocket.SendAsync(
                        arraySegment,
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
            catch (System.Exception)
            {
                throw new ApiCouldNotSendWebSocketMessageException();
            }
        });
        await Task.WhenAll(tasks);
    }
}