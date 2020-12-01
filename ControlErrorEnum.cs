using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public enum ControlErrorEnum
    {
        NoError = 0,
        Unknown = 1,
        TimeoutTestNotOk = 2,
        OperatortestNotOk = 3
    }
}
