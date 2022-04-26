using System.Net.WebSockets;
using System.Text;
using Spectre.Console;
using System.Text.Json;

Console.WriteLine("[ ChatApp client ]");

var exitEvent = new ManualResetEvent(false);
var url = new Uri("ws://localhost:8000/api/ws");

using (var client = new ClientWebSocket())
{
    var cts = new CancellationTokenSource();

    cts.CancelAfter(TimeSpan.FromMinutes(2));

    try
    {
        await client.ConnectAsync(url, cts.Token);

        while (client.State == WebSocketState.Open)
        {
            var message = AnsiConsole.Ask<string>("[green]message[/]: ");

            if (!string.IsNullOrEmpty(message))
            {
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);

                var responseBuffer = new byte[1024 * 8];
                var offset = 0;
                var packet = 1024;

                while (true)
                {
                    ArraySegment<byte> byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                    WebSocketReceiveResult response = await client.ReceiveAsync(byteReceived, cts.Token);
                    var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                    // Console.WriteLine(responseMessage);

                    ChatMessage chatMessage = JsonSerializer.Deserialize<ChatMessage>(responseMessage);

                    if (chatMessage != null)
                    {
                        AnsiConsole.Write($"[{chatMessage.SendDateTime}] [{chatMessage.AuthorUsername}] - {chatMessage.Message}\n");
                    }

                    if (response.EndOfMessage)
                        break;
                }

            }
        }
    }
    catch (WebSocketException e)
    {
        Console.WriteLine(e.Message);
        Environment.Exit(0);
    }
}
