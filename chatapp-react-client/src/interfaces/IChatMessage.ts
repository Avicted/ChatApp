export enum MessageType {
    Message,
    InfoToUser,
    ServerInfo,
    Command,
}

export interface IChatMessage {
    Id: string;
    Topic: string;
    AuthorId: string;
    AuthorUsername: string;
    MessageType: MessageType;
    Message: string;
    SendDateTime: string;
    WelcomeData?: IWelcomeData;
}

export interface IWelcomeData {
    UserId: string;
    Username: string;
}
