using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    [Flags]
    public enum ReportTriggerOptionsEnum
    {
        NONE = 0,
        /** send report when value of data changed */
        DATA_CHANGED = 1,
        /** send report when quality of data changed */
        QUALITY_CHANGED = 2,
        /** send report when data or quality is updated */
        DATA_UPDATE = 4,
        /** periodic transmission of all data set values */
        INTEGRITY = 8,
        /** general interrogation (on client request) */
        GI = 16
    }
}
