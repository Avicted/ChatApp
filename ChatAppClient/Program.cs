using System.Net.WebSockets;
using System.Text;

Console.WriteLine("[ ChatApp client ]");
Console.WriteLine("     -> Connecting to WebSocket server");

var exitEvent = new ManualResetEvent(false);
var url = new Uri("ws://localhost:8000/api/ws");

using (var client = new ClientWebSocket())
{
    var cts = new CancellationTokenSource();

    cts.CancelAfter(TimeSpan.FromMinutes(2));

    try
    {
        await client.ConnectAsync(url, cts.Token);
        var n = 0;

        while (client.State == WebSocketState.Open)
        {
            string message = Console.ReadLine();

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
                    Console.WriteLine(responseMessage);

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
