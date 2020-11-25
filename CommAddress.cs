using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal struct CommAddress
    {
        internal string Domain;
        internal string Variable;
        internal string LogicalNode;
        internal string VariablePath;
        internal NodeBase owner;
    }
}
