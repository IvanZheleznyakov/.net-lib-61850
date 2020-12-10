using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public class ReadDataSetResponse : IResponse
    {
        public List<DataAccessErrorEnum> TypeOfErrors { get; internal set; }
        public List<MmsValue> MmsValues { get; internal set; }
    }
}
