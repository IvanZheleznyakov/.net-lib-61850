using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public class FileResponse: IResponse
    {
        public FileErrorResponseEnum TypeOfError { get; internal set; }
        public FileBuffer FileBuffer { get; internal set; }
    }
}
