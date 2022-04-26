using System.Runtime.Serialization;

public class ApiRemotePartyClosedConnectionException : Exception
{
    public ApiRemotePartyClosedConnectionException()
    {
    }

    public ApiRemotePartyClosedConnectionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ApiRemotePartyClosedConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}