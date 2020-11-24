using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public enum ReasonForInclusionEnum
    {
        NOT_INCLUDED = 0,
        DATA_CHANGE = 1,
        QUALITY_CHANGE = 2,
        DATA_UPDATE = 3,
        INTEGRITY = 4,
        GI = 5,
        UNKNOWN = 6,
    }
}
