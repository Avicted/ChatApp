import ReactDOM from "react-dom/client";
import App from "./pages/App";
import "./styles/index.scss";

const root = ReactDOM.createRoot(
    document.getElementById("root") as HTMLElement
);
root.render(<App />);