//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace lib61850net
//{
//    internal class MMSCaptureDb
//    {
//        Iec61850State iecs;
//        internal delegate void NewPacket(MMSCapture cap);
//        internal event NewPacket OnNewPacket;
//        int PacketNr;

//        internal MMSCaptureDb(Iec61850State _iecs)
//        {
//            iecs = _iecs;
//        }
//        /// <summary>
//        /// List of captured MMS packets (PDUs)
//        /// </summary>
//        List<MMSCapture> CapturedData = new List<MMSCapture>();
//        /// <summary>
//        /// Capture of MMS packets (PDUs) active
//        /// </summary>
//        internal bool CaptureActive = false;

//        internal void AddPacket(MMSCapture cap)
//        {
//            CapturedData.Add(cap);
//            cap.PacketNr = PacketNr++;
//            if (OnNewPacket != null) OnNewPacket(cap);
//        }

//        internal void Clear()
//        {
//            CapturedData.Clear();
//            PacketNr = 0;
//        }
//    }
//}
