﻿using System.Runtime.Serialization;

namespace Application.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string? message) : base(message)
    {
    }

    public ResourceNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ResourceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
