using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using MMS_ASN1_Model;
using System.Collections.Concurrent;

namespace lib61850net
{
    internal class Iec61850State: TcpState
    {
        /// <summary>
        /// Size of data buffer.
        /// </summary>
        internal const int dataBufferSize = 2048;
        /// <summary>
        /// Index of receive buffer.
        /// </summary>
        internal int dataBufferIndex = 0;
        /// <summary>
        /// TPKT Length
        /// </summary>
        internal int TpktLen = 0;
        /// <summary>
        /// TPKT Datagram buffer.
        /// </summary>
        internal byte[] dataBuffer = new byte[dataBufferSize];
        /// <summary>
        /// Upper level protocol state
        /// </summary>
        internal Iec61850lStateEnum istate = Iec61850lStateEnum.IEC61850_STATE_START;
        /// <summary>
        /// TPKT Receive state
        /// </summary>
        internal IsoTpktState kstate = IsoTpktState.TPKT_RECEIVE_START;
        /// <summary>
        /// OSI Receive state
        /// </summary>
        internal IsoProtocolState ostate = IsoProtocolState.OSI_STATE_START;
        /// <summary>
        /// MMS File service state
        /// </summary>
        internal FileTransferState fstate = FileTransferState.FILE_NO_ACTION;
        /*       /// <summary>
               /// OSI Protocol emulation
               /// </summary>
               public OsiEmul osi = new OsiEmul();*/
        /// <summary>
        /// ISO Protocol layers (new implementation)
        /// </summary>
        internal IsoLayers iso;
        /// <summary>
        /// ISO Layers connection parameters
        /// </summary>
        internal IsoConnectionParameters cp;
        /// <summary>
        /// MMS Protocol
        /// </summary>
        internal Scsm_MMS mms = new Scsm_MMS();
        /// <summary>
        /// Input stream of MMS parsing
        /// </summary>
        internal MemoryStream msMMS = new MemoryStream();
        /// <summary>
        /// Output stream of MMS coding
        /// </summary>
        internal MemoryStream msMMSout;
        /// <summary>
        /// Memory for continuation of requests
        /// </summary>
        internal Identifier continueAfter;
        /// <summary>
        /// Memory for continuation of file directory requests
        /// </summary>
        internal FileName continueAfterFileDirectory;
        /// <summary>
        /// Server data
        /// </summary>
        internal Iec61850Model DataModel;
        /// <summary>
        /// Queue for sending data from another threads
        /// </summary>
        internal ConcurrentQueue<WriteQueueElement> SendQueue = new ConcurrentQueue<WriteQueueElement>();
        internal ManualResetEvent sendQueueWritten = new ManualResetEvent(false);
        internal NodeBase[] lastFileOperationData = null;
        internal ConcurrentDictionary<int, NodeBase[]> OutstandingCalls;

        internal MMSCaptureDb CaptureDb;
        internal Iec61850Controller Controller;

        internal Iec61850State()
        {
            DataModel = new Iec61850Model(this);
            CaptureDb = new MMSCaptureDb(this);
            iso = new IsoLayers(this);
            OutstandingCalls = new ConcurrentDictionary<int, NodeBase[]>(2, 10);
            Controller = new Iec61850Controller(this, DataModel);
        }

        internal void NextState()
        {
        }

        internal void Send(NodeBase[] Data, CommAddress Address, ActionRequested Action, AutoResetEvent responseEvent = null, object param = null)
        {
            WriteQueueElement el = new WriteQueueElement(Data, Address, Action, responseEvent, param);
            SendQueue.Enqueue(el);
            sendQueueWritten.Set();
        }
    }

}
