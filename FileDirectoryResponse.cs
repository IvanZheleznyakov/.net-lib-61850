using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class FileDirectoryResponse: IResponse
    {
        internal FileDirectoryResponse()
        {

        }

        public FileErrorResponseEnum TypeOfError { get; internal set; }
        public List<FileDirectory> FileDirectories { get; internal set; }
    }
}
