using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public enum ControlModelEnum
    {
        State_Only = 0,
        Direct_Control_With_Normal_Security,
        Select_Before_Operate_With_Normal_Security,
        Direct_Control_With_Enhanced_Security,
        SBO_ENHANCED,
        Unknown,
    }
}
