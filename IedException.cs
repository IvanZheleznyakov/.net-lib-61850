using System;

namespace lib61850net
{
    public class IedException : Exception
    {
        public IedException(string message) : base(message)
        {
        }
    }
}
