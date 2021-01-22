using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class Iec61850Model
    {
        /// <summary>
        /// Server data
        /// </summary>
        internal NodeIed ied;
        /// <summary>
        /// Server named variable lists
        /// </summary>
        internal NodeIed datasets;
        /// <summary>
        /// Server RP blocks (reports)
        /// </summary>
        internal NodeIed urcbs;
        /// <summary>
        /// Server BR blocks (reports)
        /// </summary>
        internal NodeIed brcbs;
        /// <summary>
        /// Server files
        /// </summary>
        internal NodeIed files;
        /// Enum types
        /// </summary>
        internal NodeIed enums;

        internal Iec61850Model(Iec61850State iecs)
        {
            ied = new NodeIed("ied", this);
            datasets = new NodeIed("datasets", this);
            urcbs = new NodeIed("urcbs", this);
            brcbs = new NodeIed("brcbs", this);
            files = new NodeIed("files", this);
            enums = new NodeIed("enums", this);
            ied.iecs = iecs;
            datasets.iecs = iecs;
            files.iecs = iecs;
            urcbs.iecs = iecs;
            brcbs.iecs = iecs;
            enums.iecs = iecs;
        }

        private void recursiveLinkDA(NodeBase source, NodeBase target, NodeFC fc)
        {
            NodeBase linkedDa = target.LinkChildNodeByName(source);
            // Set FC
            if (linkedDa is NodeData && !(linkedDa is NodeDO))
            {
                (linkedDa as NodeData).FC = (FunctionalConstraintEnum)NodeData.MapLibiecFC(fc.Name);
            }
            // Check DO / DA types
            if (linkedDa != source)
            {
                // We are in a DA once again
                // That means this is a DO and not a DA
                // We have to create DO and add it to the iec model (target)
                // and replace linkedDa with this object
                NodeDO ido = new NodeDO(source.Name);
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
