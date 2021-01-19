using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public class IedException : Exception
    {
        public IedException(string message) : base(message)
        {
        }
    }
}
