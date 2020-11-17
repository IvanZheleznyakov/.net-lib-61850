using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public class Response
    {
        public TypeOfResponseEnum TypeOfResponse { get; internal set; }
        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public MmsValue MmsValue { get; internal set; }
        public List<FileDirectory> FileDirectories { get; internal set; }
        public byte[] FileData { get; internal set; }
    }
}
