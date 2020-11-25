using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    class NodeVLM: NodeBase
    {
        public NodeVLM(string Name)
            : base(Name)
        {
        }

        public NodeBase LinkedNode { get; set; }
        public string SCL_FCDesc { get; set; }
        public string SCL_ServerLink { get; set; }
    }
}
