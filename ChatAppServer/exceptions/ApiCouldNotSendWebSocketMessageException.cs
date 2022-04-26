using System.Runtime.Serialization;

public class ApiCouldNotSendWebSocketMessageException : Exception
{
    public ApiCouldNotSendWebSocketMessageException()
    {
    }

    public ApiCouldNotSendWebSocketMessageException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ApiCouldNotSendWebSocketMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}