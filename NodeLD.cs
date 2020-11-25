using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    class NodeLD: NodeBase
    {
        public NodeLD(string Name)
            : base(Name)
        {
        }

        internal override void SaveModel(List<String> lines, bool fromSCL)
        {
            // Syntax: LD(<logical device name>){…}
            // Logical device name is the end of the LD Name string, it begins with model name which has to be subtracted
            string ldname = Name.Substring((Parent as NodeIed).IedModelName.Length);
            lines.Add("LD(" + ldname + "){");
            foreach (NodeBase b in _childNodes)
            {
                b.SaveModel(lines, fromSCL);
            }
            lines.Add("}");
        }
    }
}
