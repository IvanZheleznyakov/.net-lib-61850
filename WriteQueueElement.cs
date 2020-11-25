using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class WriteQueueElement
    {
        internal WriteQueueElement(NodeBase[] Data, CommAddress Address, ActionRequested Action, LibraryManager.responseReceivedHandler handler = null)
        {
            this.Data = Data;
            this.Address = Address;
            this.Action = Action;
            this.Handler = handler;
        }

        internal NodeBase[] Data { get; private set; }
        internal CommAddress Address { get; private set; }
        internal ActionRequested Action { get; private set; }
        internal LibraryManager.responseReceivedHandler Handler { get; private set; }
    }
}
