using System;

namespace GilGoblin.Functions
{
    [Serializable]
    public class DBStatusException : Exception
    {
        public DBStatusException() { }

        public DBStatusException(string message)
            : base(message) { }

        public DBStatusException(string message, Exception inner)
            : base(message, inner) { }
    }
    [Serializable]
    public class ParameterException : Exception
    {
        public ParameterException() { }

        public ParameterException(string message)
            : base(message) { }

        public ParameterException(string message, Exception inner)
            : base(message, inner) { }
    }
    [Serializable]
    public class OperationException : Exception
    {
        public OperationException() { }

        public OperationException(string message)
            : base(message) { }

        public OperationException(string message, Exception inner)
            : base(message, inner) { }
    }
}
