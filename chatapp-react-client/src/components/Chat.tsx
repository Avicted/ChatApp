import { useCallback, useEffect, useState } from "react";
import useWebSocket, { ReadyState } from "react-use-websocket";
import { MessageType, IChatMessage } from "../interfaces/IChatMessage";
import { formatDistance } from "date-fns";
import { ITopic } from "../interfaces/ITopic";

import StatusBox from "../components/StatusBox";

const Chat = (): JSX.Element => {
    const socketUrl = `${process.env.REACT_APP_WEBSOCKET_URL}`;

    const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);

    const [messageHistory, setMessageHistory] = useState<IChatMessage[]>([]);
    const [userInput, setUserInput] = useState<string>("");
    const [username, setUsername] = useState<string>("anonymous");
    const [ourUserId, setOurUserId] = useState<string>("");
    const [topic, setTopic] = useState<ITopic>({ name: "general" });

    const handleClickSendMessage = useCallback(
        () =>
            sendMessage(
                JSON.stringify({
                    AuthorId: ourUserId,
                    AuthorUsername: username,
                    MessageType: userInput.startsWith("set ")
                        ? MessageType.Command
                        : MessageType.Message,
                    Message: userInput,
                    Topic: topic,
                })
            ),
        [userInput, username, ourUserId]
    );

    const handleUserInput = (e: any) => {
        console.log({
            info: handleUserInput,
            "e.target.value": e.target.value,
            userInput,
        });

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
            setMessageHistory((prev) => prev.concat(chatMessage));

            const chatMessage: IChatMessage = {
                ...JSON.parse(lastMessage.data),
            };

            console.log({
                info: "chatMessage.MessageType",
                "chatMessage.MessageType": chatMessage.MessageType,
                chatMessage,
                "MessageType.InfoToUser": MessageType.InfoToUser,
            });

            // Commands
            if (chatMessage.MessageType === MessageType.Message) {
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
        }
    }, [lastMessage, setMessageHistory]);

    // Straight from the toppa my tou
    const keyPress = (e: any) => {
        if (e.keyCode == 13) {
            handleClickSendMessage();
            setUserInput("");
        }
    };

    return (
        <div className="Chat">
            <div>
                <StatusBox connection={connectionStatus} username={username} />
            </div>
            <div id="message-box">
                <ul>
                    {messageHistory.map((m: IChatMessage, index: number) => (
                        <li className="Message" key={index}>
                            <b className="DateTimeSend">
                                {formatDistance(
                                    new Date(),
                                    new Date(m.SendDateTime ?? null),
                                    {
                                        addSuffix: false,
                                    }
                                )}{" "}
                                ::{" "}
                            </b>
                            <b className="Author">{m.AuthorUsername} ::</b>
                            <span className="MessageText">
                                &nbsp;{m.Message}
                            </span>
                        </li>
                    ))}
                </ul>
            </div>
            <div id="input-box">
                <input
                    type="text"
                    onChange={(e: any) => handleUserInput(e)}
                    onKeyDown={(e: any) => keyPress(e)}
                    value={userInput}
                />
                <button
                    onClick={(e: any) => handleClickSendMessage()}
                    disabled={readyState !== ReadyState.OPEN}
                >
                    <b>Send</b>
                </button>
            </div>
        </div>
    );
};

export default Chat;
