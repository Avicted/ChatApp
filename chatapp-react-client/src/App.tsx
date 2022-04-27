import { useEffect, useState } from "react";

interface AppProps {}

enum MessageType {
    Message = "Message",
    InfoToUser = "InfoToUser",
    ServerInfo = "ServerInfo",
}

interface ChatMessage {
    AuthorUsername: string;
    Id: string;
    MessageType: MessageType;
    Message: string;
    SendDateTime: string;
}

// App wraps all routes e.g. all pages
export const App: React.FunctionComponent<AppProps> = ({}) => {
    var W3CWebSocket = require("websocket").w3cwebsocket;

    const [messages, setMessages] = useState<ChatMessage[]>([]);

    useEffect(() => {
        const client = new W3CWebSocket("ws://127.0.0.1:8000/api/ws");

        client.onopen = () => {
            console.log("WebSocket Client Connected");
        };

        client.onmessage = (message: MessageEvent) => {
            const json: ChatMessage = JSON.parse(message.data);
            console.log(json.Message);

            setMessages([...messages, json]);
        };

        client.onerror = function () {
            console.log("Connection Error");
        };
    }, []);

    return (
        <>
            <h1>Messages</h1>
            {messages.map((message, index) => (
                <p key={index}>
                    [{message.SendDateTime}][{message.AuthorUsername}][
                    {message.MessageType.toString()}]: {message.Message}
                </p>
            ))}
        </>
    );
};

export default App;
