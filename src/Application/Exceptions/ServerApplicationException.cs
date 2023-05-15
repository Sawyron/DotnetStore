using Microsoft.AspNetCore.Http;
using System.Runtime.Serialization;

namespace Application.Core;

public abstract class ServerApplicationException : Exception
{
    protected ServerApplicationException(int statusCode = StatusCodes.Status500InternalServerError)
    {
        StatusCode = statusCode;
    }

    protected ServerApplicationException(string? message, int statusCode = StatusCodes.Status500InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }

    protected ServerApplicationException(SerializationInfo info, StreamingContext context, int statusCode = StatusCodes.Status500InternalServerError)
        : base(info, context)
    {
        StatusCode = statusCode;
    }

    protected ServerApplicationException(string? message, Exception? innerException, int statusCode = StatusCodes.Status500InternalServerError) : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; init; }
}
