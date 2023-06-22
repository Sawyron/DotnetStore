using System.Runtime.Serialization;

namespace Application.Exceptions;

public class ResourceDublicationException : Exception
{
    public ResourceDublicationException(string? message) : base(message)
    {
    }

    public ResourceDublicationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ResourceDublicationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
