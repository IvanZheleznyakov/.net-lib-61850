using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class NodeIed : NodeBase
    {
        internal NodeIed(string Name, Iec61850Model _model)
            : base(Name)
        {
            model = _model;
        }
        
        Iec61850Model model;

        internal string VendorName { get; set; }

        internal string ModelName { get; set; }

        internal string Revision { get; set; }

        internal bool DefineNVL { get; set; }

        internal bool Identify { get; set; }

        internal NodeBase FindNodeByAddress(string Domain, string IecAddress, bool FindList = false)
        {
            if (Domain == null || IecAddress == null)
                return null;
            NodeBase b = this.FindChildNode(Domain);
            if (b != null)
            {
                if (FindList)
                {
                    if ((b = b.FindChildNode(IecAddress)) == null)
                    {
                        return null;
                    }
                }
                else
                {
                    if (Name == "urcbs" || Name == "brcbs")
                    {
                        if ((b = b.FindChildNode(IecAddress)) == null)
                        {
                            return null;
                        }
                    }
                    else if (b is NodeFile && IecAddress == "")
                    {
                        return b;
                    }
                    else
                    {
                        string[] parts = IecAddress.Split(new char[] { '$', '.' });
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (parts[i] != "" && (b = b.FindChildNode(parts[i])) == null)
                            {
                                return null;
                            }
                        }
                    }
                }
                return b;
            }
            return null;
        }

        internal NodeBase FindNodeByAddress(string CompleteIecAddress, bool FindList = false)
        {
            if (CompleteIecAddress == null)
                return null;
            string[] parts = CompleteIecAddress.Split(new char[] { '/' }, 2);
            if (parts.Length == 2)
                return FindNodeByAddress(parts[0], parts[1], FindList);
            return null;
        }

        internal NodeBase FindNodeByAddressWithDots(string domain, string iecAddress, bool findList = false)
        {
            if (domain == null || iecAddress == null)
                return null;
            NodeBase b = this.FindChildNode(domain);
            if (b != null)
            {
                if (findList)
                {
                    if ((b = b.FindChildNode(iecAddress)) == null)
                    {
                        return null;
                    }
                }
                else
                {
                    if (Name == "urcbs" || Name == "brcbs")
                    {
                        if ((b = b.FindChildNode(iecAddress)) == null)
                        {
                            return null;
                        }
                    }
                    else if (b is NodeFile && iecAddress == "")
                    {
                        return b;
                    }
                    else
                    {
                        string[] parts = iecAddress.Split(new char[] { '.' });
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if ((b = b.FindChildNode(parts[i])) == null)
                            {
                                return null;
                            }
                        }
                    }
                }
                return b;
            }
            return null;
        }

        internal NodeBase FindNodeByAddressWithDots(string completeIecAddress, bool findList = false)
        {
            if (completeIecAddress == null)
            {
                return null;
            }
            string[] parts = completeIecAddress.Split(new char[] { '/' }, 2);
            if (parts.Length == 2)
            {
                return FindNodeByAddressWithDots(parts[0], parts[1], findList);
            }
            return null;
        }

        internal NodeBase FindFileByName(string fullName)
        {
            if (fullName == null)
            {
                return null;
            }

            string[] parts = fullName.Split(new char[] { '/' });
            if (parts.Length == 1)
            {
                return null;
            }

            NodeBase b = this;

            for (int i = 1; i != parts.Length; ++i)
            {
                if ((b = b.FindChildNode(parts[i])) == null)
                {
                    return null;
                }
            }

            return b;
        }

        internal Iec61850State iecs { get; set; }

        internal override void SaveModel(List<String> lines, bool fromSCL)
        {
            // Syntax: MODEL(<model name>){…}
            lines.Add("MODEL(" + IedModelName + "){");
            foreach (NodeBase b in _childNodes)
            {
                b.SaveModel(lines, fromSCL);
            }
            lines.Add("}");
        }

        internal string IedModelName { get; set; }

        internal Iec61850Model Model { get { return model; } }

        internal bool isIecTree() { return model.iec == this; }

    }
}
