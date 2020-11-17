using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public enum DataAccessErrorEnum
    {
        objectInvalidated = 0,
        hardwareFault = 1,
        temporarilyUnavailable = 2,
        objectAccessDenied = 3,
        objectUndefined = 4,
        invalidAddress = 5,
        typeUnsupported = 6,
        typeInconsistent = 7,
        objectAttributeInconsistent = 8,
        objectAccessUnsupported = 9,
        objectNonexistent = 10,
        objectValueInvalid = 11,

        none = 99
    }
}
