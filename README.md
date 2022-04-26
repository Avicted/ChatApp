# WebSocket chat server.

# Development
Has been setup with a VSCode development container with dotnet.
```bash
# Run the app
./develop
```

```bash
# Connect to the WebSocket
websocat ws://127.0.0.1:8000/api/ws

<any text here> -> enter to send a message through the websocket.

# Commands:
set username <username>
```

Swagger at:
http://localhost:8000/swagger/index.html