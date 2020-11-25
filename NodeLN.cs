using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    class NodeLN: NodeBase
    {
        public NodeLN(string Name)
            : base(Name)
        {
        }

        internal override void SaveModel(List<String> lines, bool fromSCL)
        {
            // Syntax: LN(<logical node name>){…}
            lines.Add("LN(" + Name + "){");

            foreach (NodeBase b in _childNodes)
            {
                b.SaveModel(lines, fromSCL);
            }
            lines.Add("}");
        }
    }
}
