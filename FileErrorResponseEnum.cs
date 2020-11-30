using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public enum FileErrorResponseEnum
    {
        other = 0,
        filenameambiguous = 1,
        filebusy = 2,
        filenamesyntaxError = 3,
        contenttypeinvalid = 4,
        positioninvalid = 5,
        fileaccesdenied = 6,
        filenonexistent = 7,
        duplicatefilename = 8,
        insufficientspaceinfilestore = 9,

        none = 99
    }
}
