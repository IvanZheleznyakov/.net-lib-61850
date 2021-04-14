using System;
using System.IO;
using org.bn.attributes;
using org.bn.attributes.constraints;
using org.bn.coders;
using org.bn.types;
using org.bn;
using MMS_ASN1_Model;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using DLL_Log;

namespace lib61850net
{
    internal class Scsm_MMS_Worker
    {
        private Thread _workerThread;
        private bool _run;
        private IsoConnectionParameters isoParameters;
        private Env _env;
        private bool restart_allowed = false;
        WaitHandle[] _waitHandles = new WaitHandle[5];
        internal Iec61850State iecs;
        private static int? indexVTU;

        internal static int? GetVTUIndex()
        {
            return indexVTU;
        }

        internal bool IsFileReadingNow { get; set; } = false;

        internal Task connectionCreatedTask;
        internal Task connectionClosedTask;

        internal Scsm_MMS_Worker(SourceMsg_t logger = null, int? vtu = null)
        {
            _env = Env.getEnv();
            iecs = new Iec61850State(logger);
            iecs.mms.ReadFileStateChanged += Mms_ReadFileStateChanged;
            Scsm_MMS_Worker.indexVTU = vtu;
        }

        private void Mms_ReadFileStateChanged(bool isReading)
        {
            IsFileReadingNow = isReading;
        }

        internal bool Start(string hostName, int port, Task connectClosedTask, Task connectStartedTask = null)
        {
            isoParameters = new IsoConnectionParameters(hostName, port);
            restart_allowed = false;
            connectionCreatedTask = connectStartedTask;
            connectionClosedTask = connectClosedTask;
            return Start();
        }

        private bool Start()
        {
            //// Run Thread
            if (!_run && _workerThread == null)
            {
                _run = true;
                _workerThread = new Thread(new ParameterizedThreadStart(WorkerThreadProc));
                iecs.sourceLogger?.SendInfo(String.Format("Starting new communication, hostname = {0}, port = {1}.", isoParameters.hostname, isoParameters.port));
                _workerThread.Start(this);
            }
            else
            {
                throw new Exception("Соединение уже установлено!");
            }
            return true;
        }

        internal void Stop()
        {
            if (iecs != null)
            { 
                iecs.mms.SendConclude(iecs);
                iecs.iso.SendReleaseAcse(iecs);
                iecs.receiveDone.WaitOne(500);

                Stop(false);
            }
        }

        internal void Stop(bool restart_enable)
        {
            restart_allowed = restart_enable;
            if (_workerThread != null)
            {
                (_waitHandles[3] as ManualResetEvent).Set();
                _workerThread = null;
                _run = false;
                iecs.sourceLogger?.SendInfo(String.Format("Communication to hostname = {0}, port = {1} stopped.", isoParameters.hostname, isoParameters.port));
            }

            TcpRw.StopClient(iecs);
            try
            {
                if (connectionClosedTask?.Status == TaskStatus.Created)
                {
                    connectionClosedTask?.Start();
                }
            }
            catch { }
        }

        private void WorkerThreadProc(object obj)
        {
            try
            {
                iecs.sourceLogger?.SendInfo("lib61850net: Start WorkerThreadProc");
                Scsm_MMS_Worker self = (Scsm_MMS_Worker)obj;

                iecs.hostname = self.isoParameters.hostname;
                iecs.port = self.isoParameters.port;           // due to tcps inheritance
                iecs.cp = self.isoParameters;                  // due to tcps inheritance
                iecs.logger = Logger.getLogger();

                _waitHandles[0] = iecs.connectDone;
                _waitHandles[1] = iecs.receiveDone;
                _waitHandles[2] = iecs.sendDone;
                _waitHandles[3] = new ManualResetEvent(false);   // end thread
                _waitHandles[4] = iecs.sendQueueWritten;
                //DateTime tout = null;



                CommAddress ad = new CommAddress();
                DateTime IdentifyTimeoutBase = new DateTime();
                TimeSpan IdentifyTimeout = new TimeSpan(0, 0, 5);   // 5 sec

                while (self._run)
                {
                    switch (iecs.tstate)
                    {
                        case TcpProtocolState.TCP_STATE_START:
                            iecs.sourceLogger?.SendInfo("[TCP_STATE_START]");
                            iecs.logger.LogInfo("[TCP_STATE_START]");
                            iecs.kstate = IsoTpktState.TPKT_RECEIVE_START;
                            iecs.ostate = IsoProtocolState.OSI_STATE_START;
                            iecs.istate = Iec61850lStateEnum.IEC61850_STATE_START;
                            TcpRw.StartClient(iecs);
                            break;
                        case TcpProtocolState.TCP_STATE_SHUTDOWN:
                            iecs.sourceLogger?.SendInfo("[TCP_STATE_SHUTDOWN]");
                            iecs.logger.LogInfo("[TCP_STATE_SHUTDOWN]");
                            Stop();
                            //   Thread.Sleep(5000);
                            //      iecs.tstate = TcpProtocolState.TCP_STATE_START;
                            iecs.tstate = TcpProtocolState.TCP_CONNECT_WAIT;
                            break;
                        case TcpProtocolState.TCP_CONNECTED:
                            switch (iecs.ostate)
                            {
                                case IsoProtocolState.OSI_CONNECT_COTP:
                                    iecs.sourceLogger?.SendInfo("[OSI_CONNECT_COTP]");
                                    iecs.logger.LogInfo("[OSI_CONNECT_COTP]");
                                    iecs.ostate = IsoProtocolState.OSI_CONNECT_COTP_WAIT;
                                    iecs.iso.SendCOTPSessionInit(iecs);
                                    break;
                                case IsoProtocolState.OSI_CONNECT_PRES:
                                    iecs.sourceLogger?.SendInfo("[OSI_CONNECT_PRES]");
                                    iecs.logger.LogInfo("[OSI_CONNECT_PRES]");
                                    // This cannot be before Send, but is issued inside Send chain before TCP send
                                    // iecs.ostate = IsoProtocolState.OSI_CONNECT_PRES_WAIT;
                                    iecs.mms.SendInitiate(iecs);
                                    break;
                                case IsoProtocolState.OSI_CONNECTED:
                                    switch (iecs.istate)
                                    {
                                        case Iec61850lStateEnum.IEC61850_STATE_START:
                                            if (iecs.DataModel.ied.Identify)
                                            {
                                                iecs.sourceLogger?.SendInfo("[IEC61850_STATE_START] (Send IdentifyRequest)");
                                                iecs.logger.LogInfo("[IEC61850_STATE_START] (Send IdentifyRequest)");
                                                iecs.istate = Iec61850lStateEnum.IEC61850_CONNECT_MMS_WAIT;
                                                iecs.mms.SendIdentify(iecs);
                                                IdentifyTimeoutBase = DateTime.UtcNow;
                                            }
                                            else
                                            {
                                                iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN;
                                            }
                                            break;
                                        case Iec61850lStateEnum.IEC61850_CONNECT_MMS_WAIT:
                                            // If we wait for Identify response too long, continue without it
                                            if (DateTime.UtcNow.Subtract(IdentifyTimeout).CompareTo(IdentifyTimeoutBase) > 0)
                                            {
                                                // Timeout expired
                                                iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN;
                                                iecs.sourceLogger?.SendWarning("lib61850net: MMS Identify message not supported by server, although declared in ServicesSupportedCalled bitstring");
                                                iecs.logger.LogWarning("MMS Identify message not supported by server, although declared in ServicesSupportedCalled bitstring");
                                            }
                                            break;
                                        case Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN:
                                            iecs.sourceLogger?.SendInfo("[IEC61850_READ_NAMELIST_DOMAIN]");
                                            iecs.logger.LogInfo("[IEC61850_READ_NAMELIST_DOMAIN]");
                                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN_WAIT;
                                            iecs.mms.SendGetNameListDomain(iecs);
                                            break;
                                        case Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR:
                                            iecs.sourceLogger?.SendInfo("[IEC61850_READ_NAMELIST_VAR]");
                                            iecs.logger.LogInfo("[IEC61850_READ_NAMELIST_VAR]");
                                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR_WAIT;
                                            iecs.mms.SendGetNameListVariables(iecs);
                                            break;
                                        case Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR:
                                            iecs.sourceLogger?.SendInfo("[IEC61850_READ_ACCESSAT_VAR]");
                                            iecs.logger.LogInfo("[IEC61850_READ_ACCESSAT_VAR]");
                                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR_WAIT;
                                            iecs.mms.SendGetVariableAccessAttributes(iecs);
                                            //iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                                            break;
                                        case Iec61850lStateEnum.IEC61850_READ_MODEL_DATA:
                                            if (_env.dataReadOnStartup)
                                            {
                                                iecs.sourceLogger?.SendInfo("[IEC61850_READ_MODEL_DATA]");
                                                iecs.logger.LogInfo("[IEC61850_READ_MODEL_DATA]");
                                                CommAddress adr = new CommAddress
                                                {
                                                    Domain = null,
                                                    Variable = null,
                                                };
                                                NodeBase[] data = new NodeBase[1];
                                                // Issue reads by FC level
                                                data[0] = iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().GetActualChildNode();
                                                WriteQueueElement wqel = new WriteQueueElement(data, adr, ActionRequested.Read);
                                                iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA_WAIT;
                                                iecs.mms.SendRead(iecs, wqel);
                                            }
                                            else
                                            {
                                                iecs.sourceLogger?.SendInfo("[IEC61850_READ_MODEL_DATA] - Skipped due to a presetting");
                                                iecs.logger.LogInfo("[IEC61850_READ_MODEL_DATA] - Skipped due to a presetting");
                                                iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                                            }
                                            break;
                                        case Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST:
                                            iecs.sourceLogger?.SendInfo("[IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                                            iecs.logger.LogInfo("[IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST_WAIT;
                                            iecs.mms.SendGetNameListNamedVariableList(iecs);
                                            break;
                                        case Iec61850lStateEnum.IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST:
                                            iecs.sourceLogger?.SendInfo("[IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST]");
                                            iecs.logger.LogInfo("[IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST]");
                                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST_WAIT;
                                            if (iecs.mms.SendGetNamedVariableListAttributes(iecs) != 0)
                                            {
                                                // No VarLists
                                                iecs.sourceLogger?.SendInfo("Init end: [IEC61850_FREILAUF]");
                                                iecs.logger.LogInfo("Init end: [IEC61850_FREILAUF]");
                                                iecs.istate = Iec61850lStateEnum.IEC61850_MAKEGUI;
                                            }
                                            break;
                                        case Iec61850lStateEnum.IEC61850_MAKEGUI:
                                            if (connectionCreatedTask?.Status == TaskStatus.Created)
                                            {
                                                iecs.sourceLogger?.SendInfo("[IEC61850_MAKEGUI]");
                                                iecs.logger.LogInfo("[IEC61850_MAKEGUI]");
                                                if (connectionCreatedTask?.Status == TaskStatus.Created)
                                                {
                                                    connectionCreatedTask?.Start();
                                                }
                                            }
                                            iecs.istate = Iec61850lStateEnum.IEC61850_FREILAUF;
                                            break;
                                        case Iec61850lStateEnum.IEC61850_FREILAUF:
                                            // File service handling
                                            switch (iecs.fstate)
                                            {
                                                case FileTransferState.FILE_DIRECTORY:
                                                    iecs.fstate = FileTransferState.FILE_NO_ACTION;
                                                    break;
                                                case FileTransferState.FILE_OPENED:
                                                case FileTransferState.FILE_READ:
                                                    // issue a read
                                                    iecs.Send(iecs.lastFileOperationData, ad, ActionRequested.ReadFile);
                                                    iecs.fstate = FileTransferState.FILE_NO_ACTION;
                                                    break;
                                                case FileTransferState.FILE_COMPLETE:
                                                    // issue a close
                                                    // file can be saved from context menu
                                                    iecs.Send(iecs.lastFileOperationData, ad, ActionRequested.CloseFile);
                                                    iecs.fstate = FileTransferState.FILE_NO_ACTION;
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                                case IsoProtocolState.OSI_STATE_SHUTDOWN:
                                    TcpRw.StopClient(iecs);
                                    try
                                    {
                                        if (connectionClosedTask?.Status == TaskStatus.Created)
                                        {
                                            connectionClosedTask?.Start();
                                        }
                                    }
                                    catch { }
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    int waitres = WaitHandle.WaitAny(_waitHandles, 500);
                    switch (waitres)
                    {
                        case 0:     // connect
                            {
                                if (iecs.ostate == IsoProtocolState.OSI_STATE_START)
                                    iecs.ostate = IsoProtocolState.OSI_CONNECT_COTP;
                                iecs.connectDone.Reset();
                                TcpRw.Receive(iecs);    // issue a Receive call
                                break;
                            }
                        case 1:     // receive
                            {
                                iecs.receiveDone.Reset();
                                TcpRw.Receive(iecs);    // issue a new Receive call
                                break;
                            }
                        case 2:     // send
                            {
                                iecs.sendDone.Reset();
                                break;
                            }
                        case 3:     // endthread
                            {
                                self._run = false;
                                break;
                            }
                        case 4:     // send data
                            {
                                iecs.sendQueueWritten.Reset();
                                Logger.getLogger().LogDebug("SendQueue Waiting for lock in Worker!");
                                WriteQueueElement el;
                                while (iecs.SendQueue.TryDequeue(out el))
                                {
                                    switch (el.Action)
                                    {
                                        case ActionRequested.Write:
                                            {
                                                if (el.MmsValue == null)
                                                {
                                                    iecs.mms.SendWrite(iecs, el, el.ResponseTask, (WriteResponse)el.Response);
                                                }
                                                else
                                                {
                                                    iecs.mms.SendWrite(iecs, el, el.MmsValue, el.ResponseTask, (WriteResponse)el.Response);
                                                }
                                            }
                                            break;
                                        case ActionRequested.WriteAsStructure:
                                            iecs.mms.SendWriteAsStructure(iecs, el, el.ResponseTask, (WriteResponse)el.Response);
                                            break;
                                        case ActionRequested.Read:
                                            if (el.Data[0] is NodeVL)
                                            {
                                                iecs.mms.SendReadVL(iecs, el, el.ResponseTask, el.Response);
                                            }
                                            else
                                            {
                                                iecs.mms.SendRead(iecs, el, el.ResponseTask, el.Response);
                                            }
                                            break;
                                        case ActionRequested.DefineNVL:
                                            iecs.mms.SendDefineNVL(iecs, el);
                                            break;
                                        case ActionRequested.DeleteNVL:
                                            iecs.mms.SendDeleteNVL(iecs, el);
                                            break;
                                        case ActionRequested.GetDirectory:
                                            iecs.mms.SendFileDirectory(iecs, el, el.ResponseTask, (FileDirectoryResponse)el.Response);
                                            break;
                                        case ActionRequested.OpenFile:
                                            iecs.mms.SendFileOpen(iecs, el, el.ResponseTask, (FileResponse)el.Response);
                                            break;
                                        case ActionRequested.ReadFile:
                                            iecs.mms.SendFileRead(iecs, el);
                                            break;
                                        case ActionRequested.CloseFile:
                                            iecs.mms.SendFileClose(iecs, el);
                                            break;
                                        case ActionRequested.FileDelete:
                                            iecs.mms.SendFileDelete(iecs, el);
                                            break;
                                    }
                                }
                                break;
                            }
                        case WaitHandle.WaitTimeout:
                            {
                                break;
                            }
                    }
                }
                TcpRw.StopClient(iecs);
                try
                {
                    if (connectionClosedTask?.Status == TaskStatus.Created)
                    {
                        connectionClosedTask?.Start();
                    }
                }
                catch { }
                if (restart_allowed)
                {
                    Start();
                    try
                    {

                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                iecs?.sourceLogger?.SendError("lib61850net: критическая ошибка в workerthreadproc: " + ex.Message);
                try
                {
                    if (connectionClosedTask?.Status == TaskStatus.Created)
                    {
                        connectionClosedTask?.Start();
                    }
                }
                catch { }
            }

        }
    }
}
