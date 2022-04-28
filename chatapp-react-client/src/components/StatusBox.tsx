import FontAwesome, { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCloud } from "@fortawesome/free-solid-svg-icons";

type StatusBoxProps = {
    connection: string;
    username: string;
};

const StatusBox = ({ connection, username }: StatusBoxProps): JSX.Element => {
    return (
        <div className="StatusBox">
            {connection ? (
                <FontAwesomeIcon color={"#8ec07c"} icon={faCloud} />
            ) : (
                <FontAwesomeIcon color={"#fb4934"} icon={faCloud} />
            )}
            <b>::</b>
            <span>{username}</span>
        </div>
    );
};

export default StatusBox;
