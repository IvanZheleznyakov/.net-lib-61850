using System.Threading.Tasks;

namespace lib61850net
{
    internal class WriteQueueElement
    {
        internal WriteQueueElement(NodeBase[] Data, CommAddress Address, ActionRequested Action, 
            Task responseTask = null, IResponse response = null, MmsValue[] mmsValue = null)
        {
            this.Data = Data;
            this.Address = Address;
            this.Action = Action;
            this.ResponseTask = responseTask;            
            this.Response = response;
            this.MmsValue = mmsValue;
        }

        internal NodeBase[] Data { get; private set; }
        internal CommAddress Address { get; private set; }
        internal ActionRequested Action { get; private set; }
        internal Task ResponseTask { get; private set; }
        internal IResponse Response { get; private set; }
        internal MmsValue[] MmsValue { get; private set; }
    }
}
