using System.Runtime.Serialization;

namespace ChatAppServer.Exceptions;
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