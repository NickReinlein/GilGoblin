using System;
using System.Runtime.Serialization;

public class DatabaseException : ApplicationException
{
    public DatabaseException()
    {
    }

    public DatabaseException(string? message) : base(message)
    {
    }

    public DatabaseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        throw new NotImplementedException();
    }
}