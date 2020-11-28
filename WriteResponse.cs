using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public class WriteResponse
    {
        public WriteResponse()
        {

        }

        public List<DataAccessErrorEnum> TypeOfErrors { get; internal set; }
        public List<string> Names { get; internal set; }
    }
}
