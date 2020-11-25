using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    class NodeFC: NodeBase
    {
        public NodeFC(string Name)
            : base(Name)
        {
        }

        internal override void SaveModel(List<String> lines, bool fromSCL)
        {
            // Pass saving to next level
            foreach (NodeBase b in _childNodes)
            {
                b.SaveModel(lines, fromSCL);
            }
        }

    }
}
