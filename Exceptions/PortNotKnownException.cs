using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_V_App.Exceptions
{
    [Serializable]
    public class PortNotKnownException : Exception
    {
        public PortNotKnownException()
        {
        }

        public PortNotKnownException(string message)
            : base(message)
        {
        }

        public PortNotKnownException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
