using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal enum ActionRequested
    {
        Write,
        WriteAsStructure,
        Read,
        DefineNVL,
        DeleteNVL,
        GetDirectory,
        OpenFile,
        ReadFile,
        CloseFile,
        FileDelete
    }
}
