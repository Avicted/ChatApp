import { ITopic } from "./ITopic";

export interface IChatClient {
    id: string;
    webSocket: string;
    username: string;
    topicsSubscribedTo: ITopic[];
}
