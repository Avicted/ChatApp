export enum MessageType {
    Message,
    InfoToUser,
    ServerInfo,
    Command,
}

export interface IChatMessage {
    Id: string;
    AuthorId: string;
    AuthorUsername: string;
    MessageType: MessageType;
    Message: string;
    SendDateTime: string;
}

export interface ISendChatMessage {
    AuthorId: string;
    AuthorUsername: string;
    MessageType: MessageType;
    Message: string;
}

export interface IWelcomeData {
    UserId: string;
    Username: string;
}

export interface IWelcomeNewUserMessage {
    Id: string;
    AuthorId: string;
    AuthorUsername: string;
    MessageType: MessageType;
    Message: string;
    SendDateTime: string;
    WelcomeData: IWelcomeData;
}
