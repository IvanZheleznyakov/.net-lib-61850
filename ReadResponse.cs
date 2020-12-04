using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public class ReadResponse: IResponse
    {
        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public MmsValue MmsValue { get; internal set; }
    }
}
