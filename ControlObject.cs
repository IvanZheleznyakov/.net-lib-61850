using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class ControlObject
    {
        internal NodeDO self;
        internal LibraryManager libraryManager;
        internal CommandParams commandParams;

        public string Originator
        {
            get
            {
                return commandParams.orIdent;
            }
            set
            {
                commandParams.orIdent = value;
            }
        }

        public OriginatorCategoryEnum OrCat
        {
            get
            {
                return commandParams.orCat;
            }
            set
            {
                commandParams.orCat = value;
            }
        }

        public bool SynchroCheck
        {
            get
            {
                return commandParams.synchroCheck;
            }
            set
            {
                commandParams.synchroCheck = value;
            }
        }
        
        public bool InterlockCheck
        {
            get
            {
                return commandParams.interlockCheck;
            }
            set
            {
                commandParams.interlockCheck = value;
            }
        }

        public bool Test
        {
            get
            {
                return commandParams.Test;
            }
            set
            {
                commandParams.Test = value;
            }
        }

        public ControlModelEnum ControlModel { get; internal set; }
        public string ObjectReference { get; internal set; }
        
        public ControlObject(string objectReference, FunctionalConstraintEnum FC, LibraryManager manager)
        {
            libraryManager = manager;
            string mmsReference = IecToMmsConverter.ConvertIecAddressToMms(objectReference, FC);
            this.ObjectReference = objectReference;
            var node = manager.worker.iecs.DataModel.ied.FindNodeByAddress(mmsReference);
            self = (NodeDO)node;
            //manager.worker.iecs.DataModel.addressNodesPairs.TryGetValue(objectReference, out var outNode);
            //self = (NodeDO)outNode;
            commandParams = libraryManager.worker.iecs.Controller.PrepareSendCommand(node.FindChildNode("Oper").FindChildNode("ctlVal"));
            ControlModel = commandParams.CommandFlowFlag;
        }

        public void Select()
        {
            libraryManager.worker.iecs.Controller.ReadData(self.FindChildNode("SBO"));
        }

        public void SelectWithValue(object ctlVal)
        {
            var sendNode = (self.FindChildNode("SBOw").FindChildNode("ctlVal") as NodeData);
            commandParams.ctlVal = ctlVal;
            libraryManager.worker.iecs.Controller.SendCommandToIed(sendNode, commandParams, ActionRequested.WriteAsStructure);
        }

        public void Operate(object ctlVal)
        {
            var sendNode = (self.FindChildNode("Oper").FindChildNode("ctlVal") as NodeData);
            //(self.FindChildNode("Oper").FindChildNode("ctlVal") as NodeData).DataValue = ctlVal;
            commandParams.ctlVal = ctlVal;
            libraryManager.worker.iecs.Controller.SendCommandToIed(sendNode, commandParams, ActionRequested.WriteAsStructure);
        }
    }
}
