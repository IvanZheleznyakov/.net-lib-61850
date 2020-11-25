using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class Response
    {
        internal Response()
        {

        }

        public TypeOfResponseEnum TypeOfResponse { get; internal set; }
        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public MmsValue MmsValue { get; internal set; }
        public List<FileDirectory> FileDirectories { get; internal set; }
        public byte[] FileData { get; internal set; }
        public WriteResponse WriteResponse { get; internal set; }
    }

    public class WriteResponse
    {
        internal WriteResponse()
        {

        }

        public List<DataAccessErrorEnum> TypeOfErrors { get; internal set; }
        public List<string> Names { get; internal set; }
    }
}
