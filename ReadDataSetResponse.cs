using System.Collections.Generic;

namespace lib61850net
{
    public class ReadDataSetResponse : IResponse
    {
        public List<DataAccessErrorEnum> TypeOfErrors { get; internal set; }
        public List<MmsValue> MmsValues { get; internal set; }
    }
}
