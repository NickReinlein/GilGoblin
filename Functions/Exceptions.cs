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
}
