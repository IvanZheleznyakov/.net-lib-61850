using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public class LibraryManager
    {
        private Scsm_MMS_Worker worker;
        public int Start(string hostName, int port)
        {
            worker = new Scsm_MMS_Worker();
            return worker.Start(hostName, port);
        }

        public void Stop()
        {
            worker.Stop();
        }

        public void WriteRcb(RcbActivateParams rcbPar, bool reRead)
        {
            worker.iecs.Controller.WriteRcb(rcbPar, reRead);
        }
    }
}
