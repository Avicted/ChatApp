## WebSocket chat server.

Create a new database migration:

```bash
dotnet ef migrations add "<name_of_migration" \ 
--output-dir Database/Migrations \
--context ChatDbContext
```

Update the database with the migration(s):

```bash
dotnet-ef database update
```

```bash
# Connect to the WebSocket
curl -v --include \
     -x POST \
     --no-buffer \
     --header "Connection: Upgrade" \
     --header "Upgrade: websocket" \
     -d '{ "username": "bob" }' \
     http://localhost:8000/api/ws
```