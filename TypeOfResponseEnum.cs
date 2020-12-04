using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public enum TypeOfResponseEnum
    {
        ERROR,
        WRITE_RESPONSE,
        REPORT_CONTROL_BLOCK,
        MMS_VALUE,
        FILE_DIRECTORY,
        FILE
    }
}
