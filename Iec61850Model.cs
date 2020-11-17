using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEDExplorer
{
    public class Iec61850Model
    {
        /// <summary>
        /// Server data
        /// </summary>
        public NodeIed ied;
        /// <summary>
        /// Server data ordered by IEC61850 data model
        /// </summary>
        public NodeIed iec;
        /// <summary>
        /// Server named variable lists
        /// </summary>
        public NodeIed lists;
        /// <summary>
        /// Server RP blocks (reports)
        /// </summary>
        public NodeIed urcbs;
        /// <summary>
        /// Server BR blocks (reports)
        /// </summary>
        public NodeIed brcbs;
        /// <summary>
        /// Server files
        /// </summary>
        public NodeIed files;
        /// Enum types
        /// </summary>
        public NodeIed enums;

        public Dictionary<string, NodeBase> addressNodesPairs;

        public Iec61850Model(Iec61850State iecs)
        {
            ied = new NodeIed("ied", this);
            iec = new NodeIed("iec", this);
            lists = new NodeIed("lists", this);
            urcbs = new NodeIed("urcbs", this);
            brcbs = new NodeIed("brcbs", this);
            files = new NodeIed("files", this);
            enums = new NodeIed("enums", this);
            ied.iecs = iecs;
            iec.iecs = iecs;
            iec.IsIecModel = true;
            lists.iecs = iecs;
            files.iecs = iecs;
            urcbs.iecs = iecs;
            brcbs.iecs = iecs;
            enums.iecs = iecs;

            addressNodesPairs = new Dictionary<string, NodeBase>();
        }

        public void BuildIECModelFromMMSModel()
        {
            iec.DefineNVL = ied.DefineNVL;
            iec.Revision = ied.Revision;
            iec.VendorName = ied.VendorName;
            iec.ModelName = ied.ModelName;

            string iecAddress = iec.IecAddress;

            //  addressNodesPairs.Add(iecAddress, iec);

            foreach (NodeLD ld in ied.GetChildNodes())      // LD level
            {
                NodeLD ild = new NodeLD(ld.Name);
                ild.IsIecModel = true;
                ild = (NodeLD)iec.AddChildNode(ild);
                //  addressNodesPairs.Add(ild.IecAddress, ild);

                foreach (NodeLN ln in ld.GetChildNodes())   // LN level
                {
                    NodeLN iln = new NodeLN(ln.Name);
                    iln.IsIecModel = true;

                    iln = (NodeLN)ild.AddChildNode(iln);
                    //iecAddress = iln.IecAddress;
                    //if (addressNodesPairs.ContainsKey(iecAddress))
                    //{
                    //    addressNodesPairs.Add(iecAddress, iln);
                    //}
                    foreach (NodeFC fc in ln.GetChildNodes())   // FC level - skipping
                    {
                        if (fc.Name == "RP" || fc.Name == "BR")
                            continue;
                        // keep knowing FC for DA
                        foreach (NodeDO dO in fc.GetChildNodes())   // DO level
                        {
                            NodeDO ido = new NodeDO(dO.Name);
                            ido.IsIecModel = true;
                            iecAddress = ido.IecAddress;

                            // AddChildNode returns original object if the same name found (new object is forgotten)
                            ido = (NodeDO)iln.AddChildNode(ido);
                            //if (!addressNodesPairs.ContainsKey(iecAddress))
                            //{
                            //    addressNodesPairs.Add(iecAddress, ido);
                            //}
                            // At this point, it can happen that we get DO more than once (same DO in several FC)
                            // For DOs, this is ok, FC is not relevant for DOs
                            // Next level is peculiar: can be DO (subDataObject) or a DA
                            // At this point, we will distinguish between DO and DA as follows:
                            // At the first guess, we suppose DA
                            // We will LINK the corresponding DA from MMS tree, and record the FC
                            // If another object with the same name comes in (from another FC branch in MMS tree)
                            // That means that we are not DA but DO (multiple FCs)
                            // And this all has to be done recursively
                            foreach (NodeBase da in dO.GetChildNodes())
                            {
                                recursiveLinkDA(da, ido, fc);
                            }
                        }
                    }
                }
            }
            // Add rcbs to LNs
            foreach (NodeLD ld in urcbs.GetChildNodes())      // LD level
            {
                foreach (NodeRCB urcb in ld.GetChildNodes())
                {
                    NodeBase ln = iec.FindNodeByAddress(ld.Name, urcb.Name.Remove(urcb.Name.IndexOf("$")));
                    if (ln != null)
                    {
                        var tempNode = (NodeRCB)ln.LinkChildNodeByName(urcb);
                        if (!addressNodesPairs.ContainsKey(tempNode.IecAddress))
                        {
                            addressNodesPairs.Add(tempNode.IecAddress, tempNode);
                        }
                    }
                }
            }
            foreach (NodeLD ld in brcbs.GetChildNodes())      // LD level
            {
                foreach (NodeRCB brcb in ld.GetChildNodes())
                {
                    NodeBase ln = iec.FindNodeByAddress(ld.Name, brcb.Name.Remove(brcb.Name.IndexOf("$")));
                    if (ln != null)
                    {
                        var tempNode = (NodeRCB)ln.LinkChildNodeByName(brcb);
                        if (!addressNodesPairs.ContainsKey(tempNode.IecAddress))
                        {
                            addressNodesPairs.Add(tempNode.IecAddress, tempNode);
                        }
                    }
                }
            }
            // Add datasets to LNs
            foreach (NodeLD ld in lists.GetChildNodes())      // LD level
            {
                foreach (NodeVL vl in ld.GetChildNodes())
                {
                    NodeBase ln = iec.FindNodeByAddress(ld.Name, vl.Name.Remove(vl.Name.IndexOf("$")));
                    if (ln != null)
                    {
                        var tempNode = ln.LinkChildNodeByName(vl);
                        if (!addressNodesPairs.ContainsKey(tempNode.IecAddress))
                        {
                            addressNodesPairs.Add(tempNode.IecAddress, tempNode);
                        }
                    }
                }
            }

        }

        void recursiveLinkDA(NodeBase source, NodeBase target, NodeFC fc)
        {
            NodeBase linkedDa = target.LinkChildNodeByName(source);
            if (!addressNodesPairs.ContainsKey(linkedDa.IecAddress))
            {
                addressNodesPairs.Add(linkedDa.IecAddress, linkedDa);
            }
            // Set FC
            if (linkedDa is NodeData && !(linkedDa is NodeDO))
                (linkedDa as NodeData).SCL_FCDesc = fc.Name;
            // Check DO / DA types
            if (linkedDa != source)
            {
                // We are in a DA once again
                // That means this is a DO and not a DA
                // We have to create DO and add it to the iec model (target)
                // and replace linkedDa with this object
                NodeDO ido = new NodeDO(source.Name);
                ido.IsIecModel = true;
                target.RemoveChildNode(source);
                linkedDa = target.AddChildNode(ido);
            }
            foreach (NodeBase newSource in source.GetChildNodes())
            {
                recursiveLinkDA(newSource, linkedDa, fc);
            }
        }
    }
}
