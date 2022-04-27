using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add our services
// remove default logging providers
builder.Logging.ClearProviders();
// Serilog configuration		
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

// Register Serilog
builder.Logging.AddSerilog(logger);

builder.Services.AddSingleton<WebSocketService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/ws", async (HttpContext context) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        using (var scope = app.Services.CreateScope())
        {
            var websocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
            await websocketService.Handle(Guid.NewGuid(), webSocket);
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.MapGet("/api/websocketConnections", () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var websocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
        return websocketService.websocketConnections.ToList();
    }
});

app.MapGet("/api/chatrooms", () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var websocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
        return websocketService.chatRooms.ToList();
    }
});


app.UseRouting();

app.UseWebSockets();

app.Run();
