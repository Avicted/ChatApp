import { ChangeEvent, useCallback, useEffect, useState } from "react";
import useWebSocket, { ReadyState } from "react-use-websocket";

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
    //Public API that will echo messages sent to it back to the client
    const [socketUrl, setSocketUrl] = useState("ws://127.0.0.1:8000/api/ws");
    const [messageHistory, setMessageHistory] = useState<any[]>([]);

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);

    useEffect(() => {
        if (lastMessage !== null) {
            setMessageHistory((prev) => prev.concat(lastMessage as any));
        }
    }, [lastMessage, setMessageHistory]);

    const handleClickChangeSocketUrl = useCallback(
        () => setSocketUrl("ws://127.0.0.1:8000/api/ws"),
        []
    );

    const handleClickSendMessage = useCallback(() => sendMessage("Hello"), []);

    const connectionStatus = {
        [ReadyState.CONNECTING]: "Connecting",
        [ReadyState.OPEN]: "Open",
        [ReadyState.CLOSING]: "Closing",
        [ReadyState.CLOSED]: "Closed",
        [ReadyState.UNINSTANTIATED]: "Uninstantiated",
    }[readyState];

    return (
        <div>
            <button onClick={handleClickChangeSocketUrl}>
                Click Me to change Socket Url
            </button>
            <button
                onClick={handleClickSendMessage}
                disabled={readyState !== ReadyState.OPEN}
            >
                Click Me to send 'Hello'
            </button>
            <span>The WebSocket is currently {connectionStatus}</span>
            {lastMessage ? <span>Last message: {lastMessage.data}</span> : null}
            <ul>
                {messageHistory.map((message, idx) => (
                    <span key={idx}>
                        {message ? <pre>{message.data}</pre> : null}
                    </span>
                ))}
            </ul>
        </div>
    );
};

export default App;
