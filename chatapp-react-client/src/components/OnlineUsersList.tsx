import { useEffect, useState } from "react";
import { IChatClient } from "../interfaces/IChatClient";

interface OnlineUsersListProps {}

export const OnlineUsersList: React.FunctionComponent<
    OnlineUsersListProps
> = ({}) => {
    const [users, setUsers] = useState<IChatClient[] | undefined>(undefined);
    const [error, setError] = useState<string>("");

    const fetchUsers = (): void => {
        // Fetch the online users at component mount
        fetch("http://127.0.0.1:8000/api/websocketConnections")
            .then(async (res: Response) => {
                const users: IChatClient[] = await res.json();
                setUsers(users);
            })
            .catch((error: any) => {
                setError(error);
            });
    };

    useEffect(() => {
        fetchUsers();
    }, []);

    if (error.length > 0) {
        return (
            <div>
                <p>error: {error}</p>
            </div>
        );
    }

    if (users === undefined) return null;

    return (
        <div id="message-box">
            {users && (
                <>
                    <p>Online Users:</p>
                    <ul>
                        {users.map((user: IChatClient, index: number) => {
                            return (
                                <>
                                    <p key={index}>{user.username}</p>
                                </>
                            );
                        })}
                    </ul>
                </>
            )}
        </div>
    );
};
