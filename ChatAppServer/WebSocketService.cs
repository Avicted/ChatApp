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
                    Id = Guid.NewGuid(),
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
            WebSocket = webSocket,
            Username = "anonymous",
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

        // Send all existing messages to the new user
        Console.WriteLine($"        -> Sending {chatRooms[0].Messages.Count} messages to the new client.");

        /* for (var i = 0; i < chatRooms[0].Messages.Count; i++)
        {
            ChatMessage existingChatMessage = chatRooms[0].Messages[i];
            await SendMessageToSockets(existingChatMessage, new List<ChatClient>() { newClient });
        } */


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
        ChatMessage welcomeMessage = new WelcomeNewUserMessage()
        {
            Id = Guid.NewGuid(),
            Message = $"You can set your username with: set username <bob>",
            SendDateTime = DateTime.Now,
            AuthorUsername = "Server",
            MessageType = MessageType.InfoToUser,
            WelcomeData = new WelcomeData()
            {
                UserId = id,
                Username = "anonymous"
            }
        };
        await SendMessageToSockets(welcomeMessage, new List<ChatClient>() { newClient });

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
                Console.WriteLine(e.Message);

                lock (websocketConnections)
                {
                    websocketConnections.Remove(newClient);
                }

                lock (chatRooms)
                {
                    chatRooms[0].Clients.Remove(newClient);
                }

                throw new ApiRemotePartyClosedConnectionException(e.Message, e.InnerException);
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

            if (!receivedMessage.CloseStatus.HasValue && receivedMessage.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                Console.WriteLine("message:");
                Console.WriteLine(message);
                ChatMessage? chatMessage = null;

                try
                {
                    chatMessage = JsonConvert.DeserializeObject<ChatMessage>(message);
                }
                catch
                {
                    // throw new ApiCouldNotReadJsonException(e.Message, e.InnerException);
                }

                string? username = chatClient.Username == null ? "anonymous" : chatClient.Username;

                if (message == null)
                    return null;

                if (chatMessage != null)
                {
                    switch (chatMessage.MessageType)
                    {
                        case MessageType.Message:
                            Console.WriteLine("MessageType.Message");
                            return chatMessage;
                        case MessageType.InfoToUser:
                            Console.WriteLine("MessageType.InfoToUser");
                            break;
                        case MessageType.ServerInfo:
                            Console.WriteLine("MessageType.ServerInfo");
                            break;
                        case MessageType.Command:
                            Console.WriteLine("MessageType.Command");

                            // Does the user want to issue a command?
                            // set username <username_here>
                            var tokens = chatMessage.Message.Split(" ");
                            if (tokens.Length == 3)
                            {
                                Console.WriteLine($"tokens[0]: {tokens[0]}\ntokens[1]: {tokens[1]}\ntokens[2]: {tokens[2]}\n");

                                if (tokens[0] == "set")
                                {
                                    if (tokens[1] == "username")
                                    {
                                        Console.WriteLine("if (tokens[1] == username)");
                                        var user = websocketConnections.FirstOrDefault(u => u.Id == chatClient.Id);
                                        Console.WriteLine($"user: {user}");

                                        if (user != null)
                                        {
                                            Console.WriteLine($"Setting the username of user Id: {chatClient.Id} to:\n{tokens[2].Trim()}");
                                            user.Username = tokens[2].Trim();

                                            ChatMessage welcomeMessage = new WelcomeNewUserMessage()
                                            {
                                                Id = chatClient.Id,
                                                Message = $"Username successfully set!",
                                                SendDateTime = DateTime.Now,
                                                AuthorUsername = "Server",
                                                MessageType = MessageType.InfoToUser,
                                                WelcomeData = new WelcomeData()
                                                {
                                                    UserId = id,
                                                    Username = user.Username
                                                }
                                            };
                                            await SendMessageToSockets(welcomeMessage, new List<ChatClient>() { chatClient });
                                        }

                                    }
                                }
                            }
                            break;
                        default:
                            ChatMessage unknownCommandMessage = new ChatMessage()
                            {
                                Id = chatClient.Id,
                                Message = $"Unknown command!",
                                SendDateTime = DateTime.Now,
                                AuthorUsername = "Server",
                                MessageType = MessageType.InfoToUser,
                            };
                            await SendMessageToSockets(unknownCommandMessage, new List<ChatClient>() { chatClient });

                            return null;
                    }
                }


                if (!string.IsNullOrWhiteSpace(message))
                {
                    chatMessage = new ChatMessage()
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

            if (receivedMessage.CloseStatus != null)
            {
                await chatClient.WebSocket.CloseAsync(
                    receivedMessage.CloseStatus.Value,
                    receivedMessage.CloseStatusDescription,
                    CancellationToken.None
                );
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
            string? chatMessageString = await Task.Run(() => JsonConvert.SerializeObject(chatMessage));
            var chatMessageBytes = Encoding.Default.GetBytes(chatMessageString);
            var arraySegment = new ArraySegment<byte>(chatMessageBytes);

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
            catch (System.Exception e)
            {
                throw new ApiCouldNotSendWebSocketMessageException(e.Message, e.InnerException);
            }
        });
        await Task.WhenAll(tasks);
    }
}