import { useCallback, useEffect, useState } from "react";
import useWebSocket, { ReadyState } from "react-use-websocket";
import {
    MessageType,
    IChatMessage,
    IWelcomeNewUserMessage,
} from "../interfaces/IChatMessage";

const Chat = (): JSX.Element => {
    const socketUrl = "ws://127.0.0.1:8000/api/ws";

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);

    const [messageHistory, setMessageHistory] = useState<IChatMessage[]>([]);
    const [userInput, setUserInput] = useState<string>("");
    const [username, setUsername] = useState<string>("anonymous");
    const [ourUserId, setOurUserId] = useState<string>("");

    const handleClickSendMessage = useCallback(
        () =>
            sendMessage(
                JSON.stringify({
                    AuthorUsername: username,
                    MessageType: userInput.startsWith("set ")
                        ? MessageType.Command
                        : MessageType.Message,
                    Message: userInput,
                })
            ),
        []
    );

    const handleUserInput = (e: any) => {
        setUserInput(e.target.value);
    };

    const connectionStatus = {
        [ReadyState.CONNECTING]: "Connecting",
        [ReadyState.OPEN]: "Open",
        [ReadyState.CLOSING]: "Closing",
        [ReadyState.CLOSED]: "Closed",
        [ReadyState.UNINSTANTIATED]: "Uninstantiated",
    }[readyState];

    useEffect(() => {
        // Handle incomming messages
        if (lastMessage !== null) {
            const chatMessage: IWelcomeNewUserMessage = {
                ...JSON.parse(lastMessage.data),
            };

            console.log({
                info: "chatMessage.MessageType",
                "chatMessage.MessageType": chatMessage.MessageType,
                chatMessage,
                "MessageType.InfoToUser": MessageType.InfoToUser,
            });

            // Commands
            if (chatMessage.MessageType === MessageType.Command) {
                if (chatMessage.AuthorId === ourUserId) {
                    const newUsername = chatMessage.Message.split(" ")[2];
                    console.log(`settings our username to: ${newUsername}`);
                    setUsername(newUsername);
                }
            }

            // Set our user Id once we connect
            if (chatMessage.MessageType === MessageType.InfoToUser) {
                if (chatMessage.WelcomeData) {
                    console.log(`received welcome new user data`);
                    console.log(chatMessage.WelcomeData);

                    setUsername(chatMessage.WelcomeData.Username);
                    setOurUserId(chatMessage.WelcomeData.UserId);
                }
            }

            setMessageHistory((prev) => prev.concat(chatMessage));
        }
    }, [lastMessage, setMessageHistory]);

    return (
        <div className="Chat">
            <div>
                <h3>
                    {connectionStatus === "Open" ? (
                        <b className="t-green">Online</b>
                    ) : (
                        <b className="t-red">Offline</b>
                    )}
                </h3>
                <p>
                    Username: <b>{username}</b>
                </p>
                <p>
                    Our user Id: <b>{ourUserId}</b>
                </p>
            </div>
            <div id="message-box">
                <ul>
                    {messageHistory.map((m: IChatMessage, index: number) => (
                        <li className="Message" key={index}>
                            <b className="DateTimeSend">{m.SendDateTime} :: </b>
                            <b className="Author">{m.AuthorUsername} :: </b>
                            <span className="MessageText">{m.Message}</span>
                        </li>
                    ))}
                </ul>
            </div>
            <div id="input-box">
                <input
                    type="text"
                    value={userInput}
                    onChange={(e) => handleUserInput(e)}
                />
                <button
                    onClick={handleClickSendMessage}
                    disabled={readyState !== ReadyState.OPEN}
                >
                    <b>Send</b>
                </button>
            </div>
        </div>
    );
};

export default Chat;
