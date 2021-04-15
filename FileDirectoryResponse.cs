using System.Collections.Generic;

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
