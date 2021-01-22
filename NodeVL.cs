using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class NodeVL : NodeBase
    {
        public NodeVL(string Name)
            : base(Name)
        {
            Defined = false;
            Activated = false;
            Deletable = false;
        }

        public bool Deletable { get; set; }

        public bool Activated { get; set; }

        public bool Defined { get; set; }

        //public EventHandler OnDefinedSuccess;
        //public EventHandler OnDeleteSuccess;

        public NodeData urcb { get; set; }
    }
}
