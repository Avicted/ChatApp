using System.Net.WebSockets;
using System.Text;
using Spectre.Console;
using System.Text.Json;

var rule = new Rule("[green]ChatApp client[/]\n");
rule.Alignment = Justify.Left;
rule.RuleStyle("green dim");
AnsiConsole.Write(rule);

var exitEvent = new ManualResetEvent(false);
var url = new Uri("ws://localhost:8000/api/ws");


var menuOptionsSelected = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Main menu")
        .PageSize(10)
        .AddChoices(new[] {
            MenuOptions.Connect.ToString(),
            MenuOptions.Exit.ToString(),
        }));


if (menuOptionsSelected == MenuOptions.Exit.ToString())
    Environment.Exit(0);

using (var client = new ClientWebSocket())
{
    var cts = new CancellationTokenSource();

    cts.CancelAfter(TimeSpan.FromMinutes(2));

    try
    {
        // Synchronous
        await AnsiConsole.Status()
            .Start("Connecting to the WebSocket ChatApp server...", async ctx =>
            {
                // Simulate some work
                await client.ConnectAsync(url, cts.Token);

                // Update the status and spinner
                ctx.Status("Successfully connected!");
                ctx.Spinner(Spinner.Known.Star);
                ctx.SpinnerStyle(Style.Parse("green"));
            });

        AnsiConsole.Markup("1");
        while (client.State == WebSocketState.Open)
        {
            AnsiConsole.Markup("2");
            var message = AnsiConsole.Ask<string>("[green]message[/]: ");

            if (!string.IsNullOrEmpty(message))
            {
                AnsiConsole.Markup("3");
                ArraySegment<byte> bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                await client.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cts.Token);
                const int kilobyte = 1024;
                var responseBuffer = new byte[kilobyte * 8];
                var offset = 0;
                var packet = kilobyte;

                while (true)
                {
                    ArraySegment<byte> byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                    WebSocketReceiveResult response = await client.ReceiveAsync(byteReceived, cts.Token);
                    var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                    // Console.WriteLine(responseMessage);

                    ChatMessage chatMessage = JsonSerializer.Deserialize<ChatMessage>(responseMessage);

                    if (chatMessage != null)
                    {
                        AnsiConsole.Markup($"[springgreen3_1]({chatMessage.SendDateTime})[/] [lightsalmon1]{chatMessage.AuthorUsername}[/] - [mistyrose1]{chatMessage.Message}[/]\n");
                    }

                    if (response.EndOfMessage)
                        break;
                }

            }
        } // end of while
    }
    catch (WebSocketException e)
    {
        AnsiConsole.Markup($"[red]{e.Message}[/]");
        Environment.Exit(0);
    }
}


enum MenuOptions
{
    Connect,
    Exit
}
