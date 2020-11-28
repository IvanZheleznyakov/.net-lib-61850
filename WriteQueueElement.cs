using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace lib61850net
{
    internal class WriteQueueElement
    {
        internal WriteQueueElement(NodeBase[] Data, CommAddress Address, ActionRequested Action, AutoResetEvent responseEvent = null, object param = null)
        {
            this.Data = Data;
            this.Address = Address;
            this.Action = Action;
            this.ResponseEvent = responseEvent;
            this.Param = param;
        }

        internal NodeBase[] Data { get; private set; }
        internal CommAddress Address { get; private set; }
        internal ActionRequested Action { get; private set; }
        internal AutoResetEvent ResponseEvent { get; private set; }
        internal object Param { get; private set; }
    }
}
