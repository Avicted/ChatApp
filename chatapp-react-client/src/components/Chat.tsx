import { ChangeEvent, useCallback, useEffect, useState } from "react";
import useWebSocket, { ReadyState } from "react-use-websocket";
import IChatMessage, {MessageType} from "../interfaces/IChatMessage";

const Chat = (): JSX.Element => {
  const socketUrl = "ws://127.0.0.1:8000/api/ws";

  const { sendMessage, lastMessage, readyState } = useWebSocket(socketUrl);

  const [messageHistory, setMessageHistory] = useState<any[]>([]);
  const [userInput, setUserInput] = useState<string>("");
  
  const handleClickSendMessage = useCallback(() => sendMessage(
    JSON.stringify({
      "AuthorUsername": "test",
      "Id": "test",
      "MessageType": MessageType,
      "Message": userInput,
      "SendDateTime": "now"
    })
  ), []);
  
  const handleUserInput = (e: any) => {
    setUserInput(e.target.value);
  }

  
  const connectionStatus = {
        [ReadyState.CONNECTING]: "Connecting",
        [ReadyState.OPEN]: "Open",
        [ReadyState.CLOSING]: "Closing",
        [ReadyState.CLOSED]: "Closed",
        [ReadyState.UNINSTANTIATED]: "Uninstantiated",
    }[readyState];

  useEffect(() => {
      if (lastMessage !== null) {
          setMessageHistory((prev) => prev.concat(lastMessage as any));
      }
  }, [lastMessage, setMessageHistory]);

  return(
    <div className="Chat">
      <h3>{connectionStatus === "Open" ? <b className="t-green">Online</b> : <b className="t-red">Offline</b>}</h3>
      <div id="message-box">
      <ul>
        {
          messageHistory.map(m => 
            <li className="Message">
              <b className="Author">{JSON.parse(m.data)["AuthorUsername"]} :: </b>
              <span className="MessageText">{JSON.parse(m.data)["Message"]}</span>
            </li>
          )
        }    
      </ul>  
      </div>
      <div id="input-box">
        <input type="text" value={userInput} onChange={handleUserInput}/>
        <button
          onClick={handleClickSendMessage}
          disabled={readyState !== ReadyState.OPEN}
        >
          <b>Send</b>
        </button>
      </div>
    </div>
  );
}

export default Chat;