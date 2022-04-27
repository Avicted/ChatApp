export enum MessageType {
    Message = "Message",
    InfoToUser = "InfoToUser",
    ServerInfo = "ServerInfo",
}

interface IChatMessage {
    AuthorUsername: string;
    Id: string;
    MessageType: MessageType;
    Message: string;
    SendDateTime: string;
}

export default IChatMessage;