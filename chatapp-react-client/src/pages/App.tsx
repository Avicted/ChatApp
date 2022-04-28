import Chat from "../components/Chat";
import { OnlineUsersList } from "../components/OnlineUsersList";

const App = (): JSX.Element => {
    return (
        <div className="App">
            <Chat />
            <OnlineUsersList />
        </div>
    );
};

export default App;
