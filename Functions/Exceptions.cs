using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Functions
{
    [Serializable]
    public class DBStatusException: Exception
    {
        public DBStatusException() { }

        public DBStatusException(string message)
            : base(message) { }

        public DBStatusException(string message, Exception inner)
            : base(message, inner) { }
    }
}
