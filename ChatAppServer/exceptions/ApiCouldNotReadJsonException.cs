using System.Runtime.Serialization;

public class ApiCouldNotReadJsonException : Exception
{
    public ApiCouldNotReadJsonException()
    {
    }

    public ApiCouldNotReadJsonException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ApiCouldNotReadJsonException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}