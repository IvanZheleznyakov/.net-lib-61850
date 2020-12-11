using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace lib61850net
{
    internal enum NodeState
    {
        Initial,
        Read,
        Reported,
        GI,
        Periodic,
        Written
    }

    internal class NodeBase : IComparable<NodeBase>
    {
        private List<String> _fc = new List<string>();
        protected List<NodeBase> _childNodes;
        private int _actualChildNode;
        protected string _address;
        protected bool _addressLock = false;
        internal event EventHandler StateChanged;
        private NodeState _nodeState;
        // Persistence for SCL server library objects
        internal object SCLServerModelObject { get; set; }

        internal NodeBase(string Name)
        {
            this.Name = Name;
            _childNodes = new List<NodeBase>();
            _nodeState = NodeState.Initial;
            IsIecModel = false;
        }

        internal NodeState NodeState
        {
            get
            {
                lock (this)
                    return _nodeState;
            }
            set
            {
                bool fire = false;

                lock (this)
                    if (!_nodeState.Equals(value))
                    {
                        _nodeState = value;
                        fire = true;
                    }
                if (fire && StateChanged != null)
                {
                    StateChanged(this, new EventArgs());
                }
            }
        }

        internal string Name { get; private set; }
        internal void NameSet4Test(string name)
        {
            this.Name = name;
        }
        internal string TypeId { get; set; }

        internal bool IsIecModel { get; set; }

        internal object Tag { get; set; }
        internal object TagR { get; set; }    // reserve for secondary Iec TreeView

        internal NodeBase Parent { get; set; }

        /*internal List<String> FC
        {
            get
            {
                return _fc;
            }
        }*/

        internal NodeBase[] GetChildNodes()
        {
            return (NodeBase[])_childNodes.ToArray();
        }

        internal NodeBase GetChildNode(int idx)
        {
            try
            {
                return _childNodes[idx];
            }
            catch
            {
                return null;
            }
        }

        internal List<string> GetChildNodeNames()
        {
            List<string> names = new List<string>();
            foreach (NodeBase nb in _childNodes)
            {
                names.Add(nb.IecAddress);
            }

            return names;
        }

        internal List<string> GetDataSetChildsWithFC()
        {
            List<string> names = new List<string>();
            foreach (NodeBase nb in _childNodes)
            {
                if (nb is NodeDO)
                {
                    names.Add(nb.IecAddress + "[" + (nb as NodeDO).FC.ToString() + "]");
                }
                else if (nb is NodeData)
                {
                    names.Add(nb.IecAddress + "[" + (nb as NodeData).FC.ToString() + "]");
                }
            }

            return names;
        }

        internal List<string> GetChildNodeNames(bool isReportsNeeded, bool isDatasetsNeeded)
        {
            List<string> names = new List<string>();
            foreach (NodeBase nb in _childNodes)
            {
                if (!isReportsNeeded && nb is NodeRCB)
                {
                    continue;
                }
                if (!isDatasetsNeeded && nb is NodeVL)
                {
                    continue;
                }
                names.Add((nb is NodeRCB) ? (nb as NodeRCB).ReportName : nb.Name);
            }
            return names;
        }

        internal List<string> GetChildNodeNames(FunctionalConstraintEnum FC)
        {
            List<string> names = new List<string>();
            foreach (NodeBase nb in _childNodes)
            {
                if (nb is NodeDO && (nb as NodeDO).FC == FC)
                {
                    names.Add(nb.Name);
                }
            }
            return names;
        }

        internal bool isArray()
        {
            if (isLeaf()) return false;
            foreach (NodeBase nb in _childNodes)
            {
                if (!nb.isArrayElement())
                    return false;
            }
            return true;
        }

        internal bool isArrayElement()
        {
            if (!Name.StartsWith("["))
                return false;
            if (!Name.EndsWith("]"))
                return false;
            return true;
        }

        internal NodeBase findArray()
        {
            NodeBase arr = this;
            while (arr != null)
            {
                if (arr.isArray())
                {
                    return arr;
                }
                arr = arr.Parent;
            }
            return null;
        }

        internal bool isLeaf()
        {
            return _childNodes.Count == 0;
        }

        internal int getArraySize()
        {
            if (isArray()) return _childNodes.Count;
            if (isArrayElement()) return Parent._childNodes.Count;
            return 0;
        }

        internal int GetChildCount()
        {
            return _childNodes.Count;
        }

        internal NodeBase AddChildNode(NodeBase Node, bool AddDeep = false)
        {
            if (Node == null) return null;       // defensive
            if (Node == this) return null;
            foreach (NodeBase n in _childNodes)
            {
                if (Node.Name == n.Name)
                {
                    if (AddDeep)
                        foreach (NodeBase nc in Node._childNodes)
                            n.AddChildNode(nc, true);
                    return n;
                }
            }
            _childNodes.Add(Node);
            Node.Parent = this;
            return Node;
        }

        internal NodeBase ForceAddChildNode(NodeBase Node)
        {
            _childNodes.Add(Node);
            Node.Parent = this;
            return Node;
        }

        internal NodeBase LinkChildNodeByAddress(NodeBase Node)
        {
            foreach (var n in _childNodes.Where(n => Node.CommAddress.Variable == n.CommAddress.Variable))
            {
                return n;
            }
            _childNodes.Add(Node);
            return Node;
        }

        internal NodeBase LinkChildNodeByName(NodeBase Node)
        {
            foreach (var n in _childNodes.Where(n => Node.Name == n.Name))
            {
                return n;
            }
            _childNodes.Add(Node);
            return Node;
        }

        /// <summary>
        /// Links the node without checking to see if a node with the same name exists
        /// </summary>
        /// <param name="Node"></param>
        /// <returns> the linked node </returns>
        internal NodeBase ForceLinkChildNode(NodeBase Node)
        {
            _childNodes.Add(Node);
            return Node;
        }

        internal NodeBase LinkChildNode(NodeBase Node)
        {
            foreach (NodeBase n in _childNodes)
            {
                /*
                if (Node._name == n._name)
                    return n;
                 */
                if (Node.IecAddress == n.IecAddress)
                    return n;
            }
            _childNodes.Add(Node);
            return Node;
        }

        internal void RemoveChildNode(NodeBase Node)
        {
            _childNodes.Remove(Node);
        }

        internal void ReplaceChildNode(NodeBase Node, NodeBase newNode)
        {
            int i = _childNodes.IndexOf(Node);
            if (i >= 0)
            {
                _childNodes.RemoveAt(i);
                _childNodes.Insert(i, newNode);
            }
        }

        internal void Remove()
        {
            _childNodes.Clear();
            if (Parent != null) Parent.RemoveChildNode(this);
            //Tag = null;
        }

        internal NodeBase GetActualChildNode()
        {
            if (_childNodes.Count <= _actualChildNode) return null;
            return (NodeBase)_childNodes[_actualChildNode];
        }

        internal NodeBase NextActualChildNode()
        {
            _actualChildNode++;
            if (_actualChildNode >= _childNodes.Count)
            {
                _actualChildNode = 0;
                return null;
            }
            return (NodeBase)_childNodes[_actualChildNode];
        }

        internal NodeBase FindChildNode(string Name)
        {
            return _childNodes.FirstOrDefault(n => n.Name == Name);
        }

        internal NodeBase FindSubNode(string subName)
        {
            string[] parts = subName.Split(new char[] { '/', '.', '$' });
            NodeBase n = this;
            int i = 0;
            do
            {
                n = n.FindChildNode(parts[i]);
                i++;
                if (i == parts.Length) return n;
            } while (i < parts.Length && n != null);
            return null;
        }

        internal void ResetActualChildNode()
        {
            _actualChildNode = 0;
        }

        internal void ResetAllChildNodes()
        {
            _actualChildNode = 0;
            foreach (var n in _childNodes)
            {
                n.ResetAllChildNodes();
            }
        }

        internal string IecAddress
        {
            get
            {
                if (_addressLock)
                    return _address;

                string address = "";
                NodeBase tmpn = this;
                List<string> parts = new List<string>();
                bool iecModel = false;

                do
                {
                    if (!(tmpn is NodeFC))
                        parts.Add(tmpn.Name);
                    tmpn = tmpn.Parent;
                    if (tmpn != null) iecModel = tmpn.IsIecModel;
                } while (tmpn != null && (!(tmpn is NodeIed) || iecModel));

                for (int i = parts.Count - 1; i >= 0; i--)
                {
                    //if (i == parts.Count - 4)
                    //    continue;
                    address += parts[i];
                    if (iecModel)
                    {
                        if (i == parts.Count - 2)
                        {
                            if (i != 0)
                                address += "/";
                        }
                        else if (i != 0 && i != parts.Count - 1)
                            if (parts[i - 1].Contains('/'))
                                address += "->";
                            else
                                address += ".";
                    }
                    else
                    {
                        if (i == parts.Count - 1)
                        {
                            if (i != 0)
                                address += "/";
                        }
                        else if (i != 0)
                            address += ".";
                    }
                }
                return address;
            }
            set
            {
                _address = value;
                _addressLock = true;
            }
        }

        internal CommAddress CommAddress
        {
            get
            {
                CommAddress commAddress = new CommAddress(); ;
                NodeBase tmpn = this;
                commAddress.owner = this;

                List<string> parts = new List<string>();

                do
                {
                    parts.Add(tmpn.Name);
                    tmpn = tmpn.Parent;
                } while (tmpn != null);

                commAddress.Variable = "";
                commAddress.VariablePath = "";
                for (int i = parts.Count - 2; i >= 0; i--)
                {
                    if (i == parts.Count - 2)
                    {
                        commAddress.Domain = parts[i];
                    }
                    else
                    {
                        commAddress.Variable += parts[i];
                        if (i == parts.Count - 3)
                            commAddress.LogicalNode = parts[i];
                        if (i != 0)
                            commAddress.Variable += "$";
                    }
                    if (i < parts.Count - 3)
                    {
                        commAddress.VariablePath = String.Concat(commAddress.VariablePath, "$", parts[i]);
                    }
                }
                return commAddress;
            }
        }

        internal CommAddress CommAddressDots
        {
            get
            {
                CommAddress commAddress = CommAddress;
                commAddress.Variable = commAddress.Variable.Replace('$', '.');
                return commAddress;
            }
        }

        internal Iec61850State GetIecs()
        {
            NodeBase b = GetIedNode();
            if (b != null)
                return (b as NodeIed).iecs;
            return null;
        }

        internal NodeIed GetIedNode()
        {
            NodeBase b = this;
            do
            {
                if (b is NodeIed)
                    return (b as NodeIed);
                b = b.Parent;
            } while (b != null);
            return null;
        }

        internal virtual NodeBase FindNodeByValue(MmsTypeEnum dataType, object dataValue, ref NodeBase ContinueAfter)
        {
            if (dataValue == null)
                return null;
            NodeBase res = null;
            if (_childNodes.Count > 0)
            {
                foreach (NodeBase b in _childNodes)
                {
                    res = b.FindNodeByValue(dataType, dataValue, ref ContinueAfter);
                    if (res != null && ContinueAfter == null)
                        return res;
                }
            }
            return null;
        }

        internal void SortImmediateChildren()
        {
            _childNodes = _childNodes.OrderBy(n => n.Name).ToList();
        }

        public int CompareTo(NodeBase other)
        {
            return string.Compare(Name, other.Name, StringComparison.CurrentCultureIgnoreCase);
        }

        internal virtual void SaveModel(List<String> lines, bool fromSCL)
        {
            return;
        }

        internal void GetAllLeaves(List<NodeBase> leaves)
        {
            foreach (NodeBase b in _childNodes)
            {
                if (b._childNodes.Count == 0)
                    leaves.Add(b);  // Leaf
                else
                    b.GetAllLeaves(leaves);
            }
        }
    }
}
