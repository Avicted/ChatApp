var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add our services
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
    // context.Response.WriteAsync("");
});

app.MapGet("/api/chatrooms", () =>
{

});


app.UseRouting();

app.UseWebSockets();

app.Run();
