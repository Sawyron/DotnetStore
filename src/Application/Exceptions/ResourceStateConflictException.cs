using System.Runtime.Serialization;

namespace Application.Exceptions;

public class ResourceStateConflictException : Exception
{
    public ResourceStateConflictException(string? message) : base(message)
    {
    }

    public ResourceStateConflictException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ResourceStateConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
