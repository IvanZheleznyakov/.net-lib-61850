using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class SelectResponse
    {
        public bool IsSelected { get; internal set; }
        public DataAccessErrorEnum TypeOfError { get; internal set; }
    }
}
