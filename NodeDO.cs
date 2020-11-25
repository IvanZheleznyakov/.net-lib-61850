using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    //class NodeDO : NodeBase
    class NodeDO : NodeData
    {
        private string _type = "";

        public string SCL_UpperDOName { get; set; }

        public NodeDO(string Name)
            : base(Name)
        {
        }

        internal override void SaveModel(List<String> lines, bool fromSCL)
        {
            // Syntax: DO(<data object name> <nb of array elements>){…}
            int nrElem = 0;
            NodeBase nextnb = this;

            if (isArray())
            {
                nrElem = getArraySize();
                // Array has got an artificial level with array members, this is not part of model definition
                if (_childNodes.Count > 0)
                    nextnb = _childNodes[0];
            }

            lines.Add("DO(" + Name + " " + nrElem.ToString() + "){");
            foreach (NodeBase b in nextnb.GetChildNodes())
            {
                b.SaveModel(lines, fromSCL);
            }
            lines.Add("}");
        }
    }
}
