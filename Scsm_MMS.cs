using System;
using System.Collections.Generic;
using org.bn.attributes;
using org.bn.attributes.constraints;
using org.bn.coders;
using org.bn.types;
using org.bn;
using MMS_ASN1_Model;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;

namespace lib61850net
{
    internal class Scsm_MMS
    {
        // Protocol IEC6850 - definitions
        // OptFlds - report Optional Fields
        // 1st Byte
        internal const byte OptFldsReserved = 0x80; // bit "0" in MMS interpretation
        internal const byte OptFldsSeqNum = 0x40;
        internal const byte OptFldsTimeOfEntry = 0x20;
        internal const byte OptFldsReasonCode = 0x10;
        internal const byte OptFldsDataSet = 0x08;
        internal const byte OptFldsDataReference = 0x04;
        internal const byte OptFldsOvfl = 0x02;
        internal const byte OptFldsEntryID = 0x01;
        // 2nd Byte
        internal const byte OptFldsConfRev = 0x80;
        internal const byte OptFldsMoreSegments = 0x40; // bit "10" in MMS interpretation

        // TrgOps - report Trigger Options
        internal const byte TrgOpsReserved = 0x80;	// bit "0" in MMS interpretation
        internal const byte TrgOpsDataChange = 0x40;
        internal const byte TrgOpsQualChange = 0x20;
        internal const byte TrgOpsDataActual = 0x10;
        internal const byte TrgOpsIntegrity = 0x08;
        internal const byte TrgOpsGI = 0x04; // bit "6" in MMS interpretation

        // DatQual - Data Quality Codes
        internal const byte DatQualValidity0 = 0x80; // bit "0" in MMS interpretation
        internal const byte DatQualValidity1 = 0x40;
        internal const byte DatQualOverflow = 0x20;
        internal const byte DatQualOutOfRange = 0x10;
        internal const byte DatQualBadReference = 0x08;
        internal const byte DatQualOscillatory = 0x04;
        internal const byte DatQualFailure = 0x02;
        internal const byte DatQualOldData = 0x01;
        // 2nd Byte
        internal const byte DatQualInconsistent = 0x80;
        internal const byte DatQualInaccurate = 0x40;
        internal const byte DatQualSource = 0x20;
        internal const byte DatQualTest = 0x10;
        internal const byte DatQualOperatorBlocked = 0x08; // bit "12" in MMS interpretation

        // TimQual - Time Quality Codes
        internal const byte TimQualExtraSeconds = 0x80; // bit "0" in MMS interpretation
        internal const byte TimQualTimeBaseErr = 0x40;
        internal const byte TimQualNotSynchronized = 0x20; // bit "2". Bits 3-7=time precision encoding

        // Report Reading Phases
        const int phsRptID = 0;
        const int phsOptFlds = 1;
        const int phsSeqNum = 2;
        const int phsTimeOfEntry = 3;
        const int phsDatSet = 4;
        const int phsBufOvfl = 5;
        const int phsEntryID = 6;
        const int phsConfRev = 7;
        const int phsSubSeqNr = 8;
        const int phsMoreSegmentsFollow = 9;
        const int phsInclusionBitstring = 10;
        const int phsDataReferences = 11;
        const int phsValues = 12;
        const int phsReasonCodes = 13;

        IDecoder decoder = CoderFactory.getInstance().newDecoder("BER");
        IEncoder encoder = CoderFactory.getInstance().newEncoder("BER");

        internal int InvokeID { get; private set; } = 0;
        internal ConcurrentDictionary<int, NodeBase> dictionaryOfControlBlocks = new ConcurrentDictionary<int, NodeBase>();
        int MaxCalls = int.MaxValue;

        bool[] ServiceSupportOptions = new bool[96];
        enum ServiceSupportOptionsEnum
        {
            status = 0,
            getNameList = 1,
            identify = 2,
            rename = 3,
            read = 4,
            write = 5,
            getVariableAccessAttributes = 6,
            defineNamedVariable = 7,
            defineScatteredAccess = 8,
            getScatteredAccessAttributes = 9,
            deleteVariableAccess = 10,
            defineNamedVariableList = 11,
            getNamedVariableListAttributes = 12,
            deleteNamedVariableList = 13,
            defineNamedType = 14,
            getNamedTypeAttributes = 15,
            deleteNamedType = 16,
            input = 17,
            output = 18,
            takeControl = 19,
            relinquishControl = 20,
            defineSemaphore = 21,
            deleteSemaphore = 22,
            reportSemaphoreStatus = 23,
            reportPoolSemaphoreStatus = 24,
            reportSemaphoreEntryStatus = 25,
            initiateDownloadSequence = 26,
            downloadSegment = 27,
            terminateDownloadSequence = 28,
            initiateUploadSequence = 29,
            uploadSegment = 30,
            terminateUploadSequence = 31,
            requestDomainDownload = 32,
            requestDomainUpload = 33,
            loadDomainContent = 34,
            storeDomainContent = 35,
            deleteDomain = 36,
            getDomainAttributes = 37,
            createProgramInvocation = 38,
            deleteProgramInvocation = 39,
            start = 40,
            stop = 41,
            resume = 42,
            reset = 43,
            kill = 44,
            getProgramInvocationAttributes = 45,
            obtainFile = 46,
            defineEventCondition = 47,
            deleteEventCondition = 48,
            getEventConditionAttributes = 49,
            reportEventConditionStatus = 50,
            alterEventConditionMonitoring = 51,
            triggerEvent = 52,
            defineEventAction = 53,
            deleteEventAction = 54,
            getEventActionAttributes = 55,
            reportEventActionStatus = 56,
            defineEventEnrollment = 57,
            deleteEventEnrollment = 58,
            alterEventEnrollment = 59,
            reportEventEnrollmentStatus = 60,
            getEventEnrollmentAttributes = 61,
            acknowledgeEventNotification = 62,
            getAlarmSummary = 63,
            getAlarmEnrollmentSummary = 64,
            readJournal = 65,
            writeJournal = 66,
            initializeJournal = 67,
            reportJournalStatus = 68,
            createJournal = 69,
            deleteJournal = 70,
            getCapabilityList = 71,
            fileOpen = 72,
            fileRead = 73,
            fileClose = 74,
            fileRename = 75,
            fileDelete = 76,
            fileDirectory = 77,
            unsolicitedStatus = 78,
            informationReport = 79,
            eventNotification = 80,
            attachToEventCondition = 81,
            attachToSemaphore = 82,
            conclude = 83,
            cancel = 84
        }

        enum RejectPDU_rejectReason_confirmedRequestPDU
        {
            other = 0,
            unrecognizedService = 1,
            unrecognizedModifier = 2,
            invalidInvokeID = 3,
            invalidArgument = 4,
            invalidModifier = 5,
            maxServOutstandingEexceeded = 6,
            maxRecursionExceeded = 8,
            valueOutOfRange = 9
        }
        enum RejectPDU_rejectReason_confirmedResponsePDU
        {
            other = 0,
            unrecognizedService = 1,
            invalidInvokeID = 2,
            invalidResult = 3,
            maxRecursionExceeded = 5,
            valueOutOfRange = 6
        }
        enum RejectPDU_rejectReason_confirmedErrorPDU
        {
            other = 0,
            unrecognizedService = 1,
            invalidInvokeID = 2,
            invalidServiceError = 3,
            valueOutOfRange = 4
        }
        enum RejectPDU_rejectReason_unconfirmedPDU
        {
            other = 0,
            unrecognizedService = 1,
            invalidArgument = 2,
            maxRecursionExceeded = 3,
            valueOutOfRange = 4
        }
        enum RejectPDU_rejectReason_pduError
        {
            unknownPduType = 0,
            invalidPdu = 1,
            illegalAcseMapping = 2
        }
        enum RejectPDU_rejectReason_concludeRequestPDU
        {
            other = 0,
            invalidArgument = 1
        }
        enum RejectPDU_rejectReason_concludeResponsePDU
        {
            other = 0,
            invalidResult = 1
        }
        enum RejectPDU_rejectReason_concludeErrorPDU
        {
            other = 0,
            invalidServiceError = 1,
            valueOutOfRange = 2
        }

        enum ServiceError_errorClass_vmdstate
        {
            other = 0,
            vmdstateconflict = 1,
            vmdoperationalproblem = 2,
            domaintransferproblem = 3,
            statemachineidinvalid = 4
        }
        enum ServiceError_errorClass_applicationreference
        {
            other = 0,
            aplicationunreachable = 1,
            connectionlost = 2,
            applicationreferenceinvalid = 3,
            contextunsupported = 4
        }
        enum ServiceError_errorClass_definition
        {
            other = 0,
            objectundefined = 1,
            invalidaddress = 2,
            typeunsupported = 3,
            typeinconsistent = 4,
            objectexists = 5,
            objectattributeinconsistent = 6
        }
        enum ServiceError_errorClass_resource
        {
            other = 0,
            memoryunavailable = 1,
            processorresourceunavailable = 2,
            massstorageunavailable = 3,
            capabilityunavailable = 4,
            capabilityunknown = 5
        }
        enum ServiceError_errorClass_service
        {
            other = 0,
            primitivesoutofsequence = 1,
            objectstateconflict = 2,
            pdusize = 3,
            continuationinvalid = 4,
            objectconstraintconflict = 5
        }
        enum ServiceError_errorClass_servicepreempt
        {
            other = 0,
            timeout = 1,
            deadlock = 2,
            cancel = 3
        }
        enum ServiceError_errorClass_timeresolution
        {
            other = 0,
            unsupportabletimeresolution = 1
        }
        enum ServiceError_errorClass_access
        {
            other = 0,
            objectaccessunsupported = 1,
            objectnonexistent = 2,
            objectaccessdenied = 3,
            objectinvalidated = 4
        }
        enum ServiceError_errorClass_initiate
        {
            other = 0,
            versionincompatible = 1,
            maxsegmentinsufficient = 2,
            maxservicesoutstandingcallinginsufficient = 3,
            maxservicesoutstandingcalledinsufficient = 4,
            serviceCBBinsufficient = 5,
            parameterCBBinsufficient = 6,
            nestinglevelinsufficient = 7
        }
        enum ServiceError_errorClass_conclude
        {
            other = 0,
            furthercommunicationrequired = 1
        }
        enum ServiceError_errorClass_cancel
        {
            other = 0,
            invokeidunknown = 1,
            cancelnotpossible = 2
        }

        static Env _env = Env.getEnv();

        private int saveInvokeIdUntilFileIsRead = -1;

        private Dictionary<int, (Task, IResponse)> waitingMmsPdu;

        internal List<ControlObject> listOfControlObjects = new List<ControlObject>();

        internal LibraryManager.newReportReceivedEventHandler reportReceivedEventHandler;
        internal ConcurrentQueue<Report> queueOfReports = new ConcurrentQueue<Report>();

        internal delegate void readFileStateChangedEventHandler(bool isReading);
        internal event readFileStateChangedEventHandler ReadFileStateChanged;

        //  internal Dictionary<string, NodeBase> addressNodesPairs = new Dictionary<string, NodeBase>();

        internal int ReceiveData(Iec61850State iecs)
        {
            try
            {
                if (iecs == null)
                {
                    return -1;
                }

                iecs.logger.LogDebugBuffer("mms.ReceiveData", iecs.msMMS.GetBuffer(), iecs.msMMS.Position, iecs.msMMS.Length - iecs.msMMS.Position);

                MMSpdu mymmspdu = null;
                try
                {
                    //MMSCapture cap = null;
                //    byte[] pkt = iecs.msMMS.ToArray();
                    //if (iecs.CaptureDb.CaptureActive) cap = new MMSCapture(pkt, iecs.msMMS.Position, pkt.Length, MMSCapture.CaptureDirection.In);
                    ////////////////// Decoding
                    mymmspdu = decoder.decode<MMSpdu>(iecs.msMMS);
                    ////////////////// Decoding
                    //if (iecs.CaptureDb.CaptureActive && mymmspdu != null)
                    //{
                    //    cap.MMSPdu = mymmspdu;
                    //    iecs.CaptureDb.AddPacket(cap);
                    //}
                }
                catch (Exception e)
                {
                    int? vtu = Scsm_MMS_Worker.GetVTUIndex();
                    if (vtu != null)
                    {
                        try
                        {
                            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Malformed Pdu");
                            if (!di.Exists)
                            {
                                di.Create();
                            }

                            string fileName = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " Device_" + vtu + ".err";

                            var fs = File.Create(di.FullName + "\\" + fileName);
                            fs.Close();
                            byte[] buf = new byte[iecs.msMMS.Length];
                            iecs.msMMS.Read(buf, 0, (int)iecs.msMMS.Length);
                            File.WriteAllBytes(di.FullName + "\\" + fileName, buf);
                            //using (StreamWriter sw = new StreamWriter(fs))
                            //{

                            //    sw.Write(buf);
                            //    sw.Flush();
                            //}

                            //      fs.Close();
                        }
                        catch { }
                    }
                    iecs.sourceLogger?.SendError("lib61850net: mms.ReceiveData: Malformed MMS Packet received!!!: " + e.Message);
                    iecs.logger.LogError("mms.ReceiveData: Malformed MMS Packet received!!!: " + e.Message);
                }

                if (mymmspdu == null)
                {
                    iecs.sourceLogger?.SendError("lib61850net: mms.ReceiveData: Parsing Error!");
                    iecs.logger.LogError("mms.ReceiveData: Parsing Error!");

                    // Workaround - we can continue when reading-in the model also if one read fails
                    if (iecs.istate == Iec61850lStateEnum.IEC61850_READ_MODEL_DATA_WAIT)
                    {
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                        NodeBase logNode = iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().GetActualChildNode();
                        if (logNode != null)
                        {
                            iecs.sourceLogger?.SendWarning("lib61850net: mms.ReceiveData: Error reading " + logNode.IecAddress + " in IEC61850_READ_MODEL_DATA_WAIT, data values not actual in the subtree!");
                            iecs.logger.LogWarning("mms.ReceiveData: Error reading " + logNode.IecAddress + " in IEC61850_READ_MODEL_DATA_WAIT, data values not actual in the subtree!");
                        }
                        // Should be possible in this phase: only 1 request can be pending in the discovery phase
                        iecs.OutstandingCalls.Clear();
                        // Set up the state variable
                        if (iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().NextActualChildNode() == null)
                        {
                            if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                            {
                                if (iecs.DataModel.ied.NextActualChildNode() == null)
                                {
                                    // End of loop
                                    iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                                    iecs.logger.LogInfo("Reading named variable lists: [IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                                    iecs.DataModel.ied.ResetAllChildNodes();
                                }
                            }
                        }
                    }
                    return -1;
                }
                else if (mymmspdu.Initiate_ResponsePDU != null)
                {
                    removeCall(iecs, -1);
                    ReceiveInitiate(iecs, mymmspdu.Initiate_ResponsePDU);
                }
                else if (mymmspdu.Confirmed_ResponsePDU != null && mymmspdu.Confirmed_ResponsePDU.Service != null)
                {
                    iecs.logger.LogDebug("mymmspdu.Confirmed_ResponsePDU.Service exists!");
                    NodeBase[] operData = removeCall(iecs, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);

                    if (mymmspdu.Confirmed_ResponsePDU.Service.Identify != null)
                    {
                        ReceiveIdentify(iecs, mymmspdu.Confirmed_ResponsePDU.Service.Identify);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.GetNameList != null)
                    {
                        ReceiveGetNameList(iecs, mymmspdu.Confirmed_ResponsePDU.Service.GetNameList, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.GetVariableAccessAttributes != null)
                    {
                        ReceiveGetVariableAccessAttributes(iecs, mymmspdu.Confirmed_ResponsePDU.Service.GetVariableAccessAttributes, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.GetNamedVariableListAttributes != null)
                    {
                        ReceiveGetNamedVariableListAttributes(iecs, mymmspdu.Confirmed_ResponsePDU.Service.GetNamedVariableListAttributes);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.Read != null)
                    {
                        ReceiveRead(iecs, mymmspdu.Confirmed_ResponsePDU.Service.Read, operData, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.Write != null)
                    {
                        ReceiveWrite(iecs, mymmspdu.Confirmed_ResponsePDU.Service.Write, operData, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);
                        //iecs.logger.LogError("Not implemented PDU Write response received!!");
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.DefineNamedVariableList != null)
                    {
                        ReceiveDefineNamedVariableList(iecs, mymmspdu.Confirmed_ResponsePDU.Service.DefineNamedVariableList, operData);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.FileDirectory != null)
                    {
                        ReceiveFileDirectory(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileDirectory, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.FileOpen != null)
                    {
                        ReceiveFileOpen(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileOpen);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.FileRead != null)
                    {
                        ReceiveFileRead(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileRead, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.FileClose != null)
                    {
                        ReceiveFileClose(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileClose);
                    }
                    else if (mymmspdu.Confirmed_ResponsePDU.Service.DeleteNamedVariableList != null)
                    {
                        ReceiveDeleteNamedVariableList(iecs, mymmspdu.Confirmed_ResponsePDU.Service.DeleteNamedVariableList, operData);
                    }
                }
                else if (mymmspdu.Unconfirmed_PDU != null && mymmspdu.Unconfirmed_PDU.Service != null && mymmspdu.Unconfirmed_PDU.Service.InformationReport != null)
                {
                    ReceiveInformationReport(iecs, mymmspdu.Unconfirmed_PDU.Service.InformationReport);
                }
                else if (mymmspdu.RejectPDU != null)
                {
                    NodeBase[] operData = removeCall(iecs, mymmspdu.RejectPDU.OriginalInvokeID.Value);
                    ReceiveRejectPDU(iecs, mymmspdu);
                }
                else if (mymmspdu.Confirmed_ErrorPDU != null)
                {
                    NodeBase[] operData = removeCall(iecs, mymmspdu.Confirmed_ErrorPDU.InvokeID.Value);

                    if (waitingMmsPdu.ContainsKey(mymmspdu.Confirmed_ErrorPDU.InvokeID.Value))
                    {
                        waitingMmsPdu.TryGetValue(mymmspdu.Confirmed_ErrorPDU.InvokeID.Value, out (Task, IResponse) responseEventWithArg);
                        if (responseEventWithArg.Item2 is FileDirectoryResponse)
                        {
                            (responseEventWithArg.Item2 as FileDirectoryResponse).TypeOfError = (FileErrorResponseEnum)mymmspdu.Confirmed_ErrorPDU.ServiceError.ErrorClass.File;
                            responseEventWithArg.Item1?.Start();
                        }
                        else if (responseEventWithArg.Item2 is FileResponse)
                        {
                            (responseEventWithArg.Item2 as FileResponse).TypeOfError = (FileErrorResponseEnum)mymmspdu.Confirmed_ErrorPDU.ServiceError.ErrorClass.File;
                            responseEventWithArg.Item1?.Start();
                        }
                        //else if (responseEventWithArg.Item2 is ReadDataSetResponse)
                        //{
                        //    (responseEventWithArg.Item2 as ReadDataSetResponse).TypeOfErrors = new List<DataAccessErrorEnum>()
                        //    { (DataAccessErrorEnum)mymmspdu.Confirmed_ErrorPDU.ServiceError.AdditionalCode };
                        //    responseEventWithArg.Item1?.Start();
                        //}
                        waitingMmsPdu.Remove(mymmspdu.Confirmed_ErrorPDU.InvokeID.Value);
                    }

                    iecs.sourceLogger?.SendError("lib61850net: Confirmed_ErrorPDU received - requested operation not possible!!");
                    iecs.logger.LogError("Confirmed_ErrorPDU received - requested operation not possible!!");
                }
                else
                {
                    iecs.sourceLogger?.SendError("lib61850net: Not implemented PDU received!!");
                    iecs.logger.LogError("Not implemented PDU received!!");
                }

                return 0;
            }
            catch (Exception ex)
            {
                iecs.sourceLogger?.SendError("lib61850net: ERROR in parsing received mmspdu: " + ex.Message);
            //    Console.WriteLine("lib61850net: ERROR in parsing received mmspdu: " + ex.Message);
                iecs.tstate = TcpProtocolState.TCP_STATE_SHUTDOWN;
                return -1;
            }
        }

        private void ReceiveRejectPDU(Iec61850State iecs, MMSpdu mymmspdu)
        {
            string operation = "unknown", reason = "unknown";

            if (mymmspdu.RejectPDU.RejectReason.isCancel_errorPDUSelected())
            {
                operation = "Cancel_errorPDU";
            }
            else if (mymmspdu.RejectPDU.RejectReason.isCancel_requestPDUSelected())
            {
                operation = "Cancel_requestPDU";
            }
            else if (mymmspdu.RejectPDU.RejectReason.isCancel_responsePDUSelected())
            {
                operation = "Cancel_responsePDU";
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConclude_errorPDUSelected())
            {
                operation = "Conclude_errorPDU";
                reason = ((RejectPDU_rejectReason_concludeErrorPDU)mymmspdu.RejectPDU.RejectReason.Conclude_errorPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConclude_requestPDUSelected())
            {
                operation = "Conclude_requestPDU";
                reason = ((RejectPDU_rejectReason_concludeRequestPDU)mymmspdu.RejectPDU.RejectReason.Conclude_requestPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConclude_responsePDUSelected())
            {
                operation = "Conclude_responsePDU";
                reason = ((RejectPDU_rejectReason_concludeResponsePDU)mymmspdu.RejectPDU.RejectReason.Conclude_responsePDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConfirmed_errorPDUSelected())
            {
                operation = "Confirmed_errorPDU";
                reason = ((RejectPDU_rejectReason_confirmedErrorPDU)mymmspdu.RejectPDU.RejectReason.Confirmed_errorPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConfirmed_requestPDUSelected())
            {
                operation = "Confirmed_requestPDU";
                reason = ((RejectPDU_rejectReason_confirmedRequestPDU)mymmspdu.RejectPDU.RejectReason.Confirmed_requestPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConfirmed_responsePDUSelected())
            {
                operation = "Confirmed_responsePDU";
                reason = ((RejectPDU_rejectReason_confirmedResponsePDU)mymmspdu.RejectPDU.RejectReason.Confirmed_responsePDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isPdu_errorSelected())
            {
                operation = "Pdu_error";
                reason = ((RejectPDU_rejectReason_pduError)mymmspdu.RejectPDU.RejectReason.Pdu_error).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isUnconfirmedPDUSelected())
            {
                operation = "UnconfirmedPDU";
                reason = ((RejectPDU_rejectReason_unconfirmedPDU)mymmspdu.RejectPDU.RejectReason.UnconfirmedPDU).ToString();
            }
            iecs.sourceLogger?.SendError("lib61850net: RejectPDU received - requested operation " + operation + " rejected!! Reason code: " + reason);
            iecs.logger.LogError("RejectPDU received - requested operation " + operation + " rejected!! Reason code: " + reason);

            if (iecs.istate == Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR_WAIT)
            {
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR;
                if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.ied.NextActualChildNode() == null)
                    {
                        // End of loop
                        //   iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                        iecs.logger.LogInfo("Reading variable values: [IEC61850_READ_MODEL_DATA]");
                        iecs.DataModel.ied.ResetAllChildNodes();
                    }
                }
            }
            else if (iecs.istate == Iec61850lStateEnum.IEC61850_READ_MODEL_DATA_WAIT)
            {
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                if (iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                    {
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                            // End of loop
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                            iecs.logger.LogInfo("Reading named variable lists: [IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                            iecs.DataModel.ied.ResetAllChildNodes();
                        }
                    }
                }
            }
        }

        private void ReceiveFileDirectory(Iec61850State iecs, FileDirectory_Response dir, int invokeId)
        {
            List<FileDirectory> listOfFileDirectory = new List<FileDirectory>();
            iecs.sourceLogger?.SendInfo("FileDirectory PDU received!!");
            iecs.logger.LogInfo("FileDirectory PDU received!!");
            if (dir.ListOfDirectoryEntry != null)
            {
                foreach (DirectoryEntry de in dir.ListOfDirectoryEntry)
                {
                    if (de.FileName != null)
                    {
                        FileDirectory fileDirectory = new FileDirectory();
                        IEnumerator<string> en = de.FileName.Value.GetEnumerator();
                        en.MoveNext();
                        string name = en.Current;

                        fileDirectory.Name = name;

                        bool absolutePath = false;
                        bool isdir = false;
                        NodeFile nf = null;
                        NodeFile nfbase = null;

                        if (name.StartsWith("/") || name.StartsWith("\\"))
                        {
                            absolutePath = true;
                        }
                        if (name.EndsWith("/") || name.EndsWith("\\"))
                        {
                            isdir = true;
                        }

                        string[] names = name.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < names.Length; i++)
                        {
                            bool isdirResult = isdir || (i < (names.Length - 1));
                            if (i == 0) nfbase = nf = new NodeFile(names[i], isdirResult);
                            else nf = (NodeFile)nf.AddChildNode(new NodeFile(names[i], isdirResult));
                        }
                        if (!nf.isDir)
                        {
                            nf.ReportedSize = de.FileAttributes.SizeOfFile.Value;
                            nf.ReportedTime = MmsDecoder.DecodeAsn1Time(de.FileAttributes.LastModified);

                            fileDirectory.Size = de.FileAttributes.SizeOfFile.Value;
                            fileDirectory.TimeOfLastModification = nf.ReportedTime;
                        }
                        if (absolutePath)
                        {
                            (iecs.lastFileOperationData[0]).GetIedNode().AddChildNode(nfbase, true);
                            (iecs.lastFileOperationData[0]).GetIedNode().AddChildNode(nfbase, true);
                        }
                        else
                        {
                            (iecs.lastFileOperationData[0]).AddChildNode(nfbase, true);
                        }

                        listOfFileDirectory.Add(fileDirectory);
                    }
                    else
                    {
                        iecs.sourceLogger?.SendInfo("lib61850net: FileDirectory PDU received!!");
                        iecs.logger.LogInfo("Empty FileName in FileDirectory PDU!!");
                    }
                }
                if (iecs.lastFileOperationData[0] is NodeFile)
                    (iecs.lastFileOperationData[0] as NodeFile).FileReady = true;
                iecs.fstate = FileTransferState.FILE_DIRECTORY;
                if (waitingMmsPdu.ContainsKey(invokeId))
                {
                    waitingMmsPdu.TryGetValue(invokeId, out (Task, IResponse) responseEventWithArg);
                    (responseEventWithArg.Item2 as FileDirectoryResponse).FileDirectories = listOfFileDirectory;
                    (responseEventWithArg.Item2 as FileDirectoryResponse).TypeOfError = FileErrorResponseEnum.none;
                    responseEventWithArg.Item1?.Start();
                    //    handlerWithParam.Item1?.Invoke(response, null);
                    //     waitingMmsPdu[invokeId]?.Invoke(response, null);
                    waitingMmsPdu.Remove(invokeId);
                }
            }
            else
            {
                iecs.sourceLogger?.SendInfo("lib61850net: No file in FileDirectory PDU!!");
                iecs.logger.LogInfo("No file in FileDirectory PDU!!");
            }
        }

        private void ReceiveFileOpen(Iec61850State iecs, FileOpen_Response fileopn)
        {
            iecs.logger.LogInfo("FileOpen PDU received!!");
            iecs.sourceLogger?.SendInfo("lib61850net: FileOpen PDU received!!");
            if (iecs.lastFileOperationData[0] is NodeFile)
            {
                ReadFileStateChanged?.Invoke(true);
                (iecs.lastFileOperationData[0] as NodeFile).frsmId = fileopn.FrsmID.Value;
                iecs.fstate = FileTransferState.FILE_OPENED;
                iecs.sourceLogger?.SendInfo("lib61850net: FileOpened: " + (iecs.lastFileOperationData[0] as NodeFile).FullName +
                    " Size: " + fileopn.FileAttributes.SizeOfFile.Value.ToString());
                iecs.logger.LogInfo("FileOpened: " + (iecs.lastFileOperationData[0] as NodeFile).FullName +
                    " Size: " + fileopn.FileAttributes.SizeOfFile.Value.ToString());
            }
        }

        private byte[] tempFileBuffer;

        private void AppendDataToTempFileBuffer(byte[] newData)
        {
            if (tempFileBuffer == null)
            {
                tempFileBuffer = newData;
            }
            else
            {
                int origLen = tempFileBuffer.Length;
                Array.Resize<byte>(ref tempFileBuffer, origLen + newData.Length);
                Array.Copy(newData, 0, tempFileBuffer, origLen, newData.Length);
            }
        }

        private void ReceiveFileRead(Iec61850State iecs, FileRead_Response filerd, int invokeId)
        {
            iecs.logger.LogInfo("FileRead PDU received!!");
            if (iecs.lastFileOperationData[0] is NodeFile)
            {
                AppendDataToTempFileBuffer(filerd.FileData);
                //(iecs.lastFileOperationData[0] as NodeFile).AppendData(filerd.FileData);

                if (filerd.MoreFollows)
                {
                    iecs.fstate = FileTransferState.FILE_READ;
                    ReadFileStateChanged?.Invoke(true);
                }
                else
                {
                    if (waitingMmsPdu.ContainsKey(saveInvokeIdUntilFileIsRead))
                    {
                        waitingMmsPdu.TryGetValue(saveInvokeIdUntilFileIsRead, out (Task, IResponse) responseWithArg);
                        (responseWithArg.Item2 as FileResponse).FileBuffer = new FileBuffer()
                        {
                            Buffer = tempFileBuffer,
                        };
                        (responseWithArg.Item2 as FileResponse).TypeOfError = FileErrorResponseEnum.none;

                        responseWithArg.Item1?.Start();
                        waitingMmsPdu.Remove(saveInvokeIdUntilFileIsRead);
                        tempFileBuffer = null;
                    }
                    saveInvokeIdUntilFileIsRead = -1;
                    iecs.fstate = FileTransferState.FILE_COMPLETE;
                    (iecs.lastFileOperationData[0] as NodeFile).FileReady = true;
                    ReadFileStateChanged?.Invoke(false);
                }
            }
        }

        private void ReceiveFileClose(Iec61850State iecs, FileClose_Response filecls)
        {
            iecs.logger.LogInfo("FileClose PDU received!!");
            iecs.fstate = FileTransferState.FILE_NO_ACTION;
        }

        private void ReceiveDefineNamedVariableList(Iec61850State iecs, DefineNamedVariableList_Response dnvl, NodeBase[] lastOperationData)
        {
            NodeVL nvl = (lastOperationData[0] as NodeVL);
            nvl.Defined = true;
            //nvl.OnDefinedSuccess?.Invoke(nvl, null);
        }

        private void ReceiveDeleteNamedVariableList(Iec61850State iecs, DeleteNamedVariableList_Response dnvl, NodeBase[] lastOperationData)
        {
            NodeVL nvl = (lastOperationData[0] as NodeVL);
            if (dnvl.NumberMatched.Value == 0)
            {
                Logger.getLogger().LogWarning("NVL name did not match any list on server: " + nvl.Name);
            }
            if (dnvl.NumberDeleted.Value > 0)
            {
                nvl.Defined = false;
                //if (nvl.OnDeleteSuccess != null)
                //    nvl.OnDeleteSuccess(nvl, null);
            }
            else
                Logger.getLogger().LogWarning("NVL Not deleted on server: " + nvl.Name);
        }

        private void ReceiveWrite(Iec61850State iecs, Write_Response write, NodeBase[] lastOperationData, int invokeId)
        {
            int i = 0;
            try
            {
                bool isInvokeIdContains = waitingMmsPdu.TryGetValue(invokeId, out (Task, IResponse) response);
                if (isInvokeIdContains)
                {
                    (response.Item2 as WriteResponse).TypeOfErrors = new List<DataAccessErrorEnum>();
                    (response.Item2 as WriteResponse).Names = new List<string>();
                }
                foreach (Write_Response.Write_ResponseChoiceType wrc in write.Value)
                {
                    if (wrc.isFailureSelected())
                    {
                        if (isInvokeIdContains)
                        {
                            (response.Item2 as WriteResponse).Names.Add(lastOperationData[i].IecAddress);
                            (response.Item2 as WriteResponse).TypeOfErrors.Add((DataAccessErrorEnum)wrc.Failure.Value);
                        }
                        Logger.getLogger().LogWarning("Write failed for " + lastOperationData[i++].IecAddress + ", failure: " + wrc.Failure.Value.ToString()
                            + ", (" + Enum.GetName(typeof(DataAccessErrorEnum), ((DataAccessErrorEnum)wrc.Failure.Value)) + ")");
                    }
                    if (wrc.isSuccessSelected())
                    {
                        if (isInvokeIdContains)
                        {
                            (response.Item2 as WriteResponse).Names.Add(lastOperationData[i++].IecAddress);
                            (response.Item2 as WriteResponse).TypeOfErrors.Add(DataAccessErrorEnum.none);
                            Logger.getLogger().LogInfo("Write succeeded for " + lastOperationData[i - 1].IecAddress);
                        }
                        else
                        {
                            Logger.getLogger().LogInfo("Write succeeded for " + lastOperationData[i++].IecAddress);
                        }
                    }
                }

                if (isInvokeIdContains)
                {
                    response.Item1?.Start();
                    waitingMmsPdu.Remove(invokeId);
                }
            }
            catch { }
        }

        private void ReceiveInformationReport(Iec61850State iecs, InformationReport Report)
        {
            string rptName, datName = null, varName;
            byte[] rptOpts = new byte[] { 0, 0 };
            byte[] incBstr = new byte[] { 0 };
            List<AccessResult> list;
            int phase = phsRptID;
            int datanum = 0;
            int dataReferenceCount = 0;
            int dataValuesCount = 0;
            int reasoncnt = 0;
            int[] listmap = null;

            Report report = new Report();

            iecs.logger.LogDebug("Report != null");
            if (Report.VariableAccessSpecification != null && Report.VariableAccessSpecification.VariableListName != null &&
                Report.VariableAccessSpecification.VariableListName.Vmd_specific != null)
            {
                iecs.logger.LogDebug("Report.VariableAccessSpecification.VariableListName.Vmd_specific = " + Report.VariableAccessSpecification.VariableListName.Vmd_specific.Value);

                if (Report.VariableAccessSpecification.VariableListName.Vmd_specific.Value == "RPT")
                {
                    if (Report.ListOfAccessResult != null)
                    {
                     //   iecs.sourceLogger?.SendInfo("Пришел отчёт");
                        iecs.logger.LogDebug("Report.ListOfAccessResult != null");
                        list = (List<AccessResult>)Report.ListOfAccessResult;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].Success != null)
                            {
                                if (phase == phsRptID)
                                { // Is this phase active??
                                    phase++;
                                    if (list[i].Success.isVisible_stringSelected())
                                    {
                                        rptName = list[i].Success.Visible_string;
                                        iecs.logger.LogDebug("Report Name = " + rptName);
                                   //     iecs.logger.LogInfo("Report Name = " + rptName);
                                        report.RptId = rptName;
                                        continue;
                                    }
                                }
                                if (phase == phsOptFlds)
                                { // Is this phase active??
                                    phase++;
                                    if (list[i].Success.isBit_stringSelected())
                                    {
                                        rptOpts = list[i].Success.Bit_string.Value;
                                        iecs.logger.LogDebug("Report Optional Fields = " + rptOpts[0].ToString());
                                 //       iecs.logger.LogInfo("Report Optional Fields = " + rptOpts[0].ToString());
                                        continue;
                                    }
                                }
                                if (phase == phsSeqNum)
                                {
                                    phase++;
                                    // Is this phase active, e.g. is this bit set in OptFlds??
                                    if ((rptOpts[0] & OptFldsSeqNum) != 0)
                                    {
                                        report.HasSequenceNumber = true;
                                        report.SeqNum = list[i].Success.Unsigned;
                                        // No evaluation of Sequence Number
                                        continue;
                                    }
                                }
                                if (phase == phsTimeOfEntry)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsTimeOfEntry) != 0)
                                    {
                                        report.HasTimeOfEntry = true;
                                        ulong millis;
                                        ulong days = 0;
                                        DateTime origin;

                                        millis = (ulong)(list[i].Success.Binary_time.Value[0] << 24) +
                                                 (ulong)(list[i].Success.Binary_time.Value[1] << 16) +
                                                 (ulong)(list[i].Success.Binary_time.Value[2] << 8) +
                                                 (ulong)(list[i].Success.Binary_time.Value[3]);
                                        if (list[i].Success.Binary_time.Value.Length == 6)
                                        {
                                            days = (ulong)(list[i].Success.Binary_time.Value[4] << 8) +
                                                   (ulong)(list[i].Success.Binary_time.Value[5]);
                                            origin = new DateTime(1984, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                                            //          millis *= 1000;
                                        }
                                        else
                                        {
                                            origin = DateTime.UtcNow.Date;
                                        }

                                        double dMillis = (double)(millis + days * 24 * 3600 * 1000);
                                        report.TimeOfEntry = origin.AddMilliseconds(dMillis);

                                        // No evaluation of Time Of Entry
                                        continue;
                                    }
                                }
                                if (phase == phsDatSet)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsDataSet) != 0)
                                    {
                                        if (list[i].Success.isVisible_stringSelected())
                                        {
                                            report.HasDataSetName = true;
                                            datName = list[i].Success.Visible_string;
                                            report.DataSetName = datName;
                                            iecs.logger.LogDebug("Report Data Set Name = " + datName);
                                         //   iecs.logger.LogInfo("Report Data Set Name = " + datName);
                                            continue;
                                        }
                                    }
                                }

                                if (phase == phsBufOvfl)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsOvfl) != 0)
                                    {
                                        report.HasBufferOverFlow = true;
                                        report.BufferOverflow = list[i].Success.Boolean;
                                        // No evaluation of rptOptsOvfl
                                        continue;
                                    }
                                }
                                if (phase == phsEntryID)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsEntryID) != 0)
                                    {
                                        report.EntryID = list[i].Success.Octet_string;
                                        // No evaluation of OptFldsEntryID
                                        continue;
                                    }
                                }
                                if (phase == phsConfRev)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[1] & OptFldsConfRev) != 0)
                                    {
                                        report.HasConfigurationRevision = true;
                                        report.ConfigurationRevision = list[i].Success.Unsigned;
                                        // No evaluation of OptFldsConfRev
                                        continue;
                                    }
                                }
                                if (phase == phsSubSeqNr)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[1] & OptFldsMoreSegments) != 0)
                                    {
                                        // No evaluation of OptFldsMoreSegments
                                        continue;
                                    }
                                }
                                if (phase == phsMoreSegmentsFollow)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[1] & OptFldsMoreSegments) != 0)
                                    {

                                        // No evaluation of OptFldsMoreSegments
                                        continue;
                                    }
                                }
                                if (phase == phsInclusionBitstring)
                                { // Is this phase active??
                                    phase++;
                                    if (list[i].Success.isBit_stringSelected())
                                    {
                                        incBstr = list[i].Success.Bit_string.Value;
                                        int effbytes = incBstr.Length - (int)(list[i].Success.Bit_string.TrailBitsCnt / 8);
                                        int effpadding = list[i].Success.Bit_string.TrailBitsCnt % 8;
                                        listmap = new int[incBstr.Length * 8];   // for each data in report includes its index into variable list
                                        int listidx = 0;
                                        for (int j = 0; j < effbytes; j++)
                                        {
                                            int l = 0x0;
                                            if ((effpadding > 0) && (j + 1 == effbytes))
                                                l = 1 << (effpadding - 1);
                                            for (int k = 0x80; k > l; k = k >> 1)
                                            {
                                                if ((incBstr[j] & k) > 0)
                                                {
                                                    listmap[datanum] = listidx;
                                                    datanum++;
                                                }
                                                listidx++;
                                            }
                                        }
                                        report.DataSetSize = datanum;
                                        report.DataReferences = new string[datanum];
                                        report.DataIndices = new int[datanum];
                                        report.DataValues = new MmsValue[datanum];
                                        report.ReasonForInclusion = new ReasonForInclusionEnum[datanum];
                                        report.HasReasonForInclusion = true;
                                        for (int k = 0; k != datanum; ++k)
                                        {
                                            report.DataIndices[k] = listmap[k];
                                        }
                                        if (datanum < 3)
                                        {
                                            int a = "test".Length;
                                        }
                                        iecs.logger.LogDebug("Report Inclusion Bitstring = " + datanum.ToString());
                                        continue;
                                    }
                                }
#if false
                                if (phase == phsDataReferences)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    //phase++;
                                    if ((rptOpts[0] & OptFldsDataReference) != 0)
                                    {
                                        if (list[i].Success.isVisible_stringSelected())
                                        {
                                            varName = list[i].Success.Visible_string;
                                            iecs.logger.LogDebug("Report Variable Name = " + varName);
                                            NodeBase b = iecs.DataModel.ied.FindNodeByAddress(varName);
                                            Data dataref = list[i + datanum].Success;
                                            if (!(b is NodeFC))
                                                // dataref = (dataref.Structure as List<Data>)[0];
                                                if (list[i + datanum].Success != null)
                                                    recursiveReadData(iecs, dataref, b, NodeState.Reported);
                                            datacnt++;
                                        }
                                        // Evaluation of OptFldsDataReference
                                        if (datacnt == datanum)
                                            phase += 2;   // phsReasonCodes
                                        else
                                            continue;
                                    }
                                    else
                                        phase++;
                                }
#endif
                                if (phase == phsDataReferences)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    //phase++;
                                    if ((rptOpts[0] & OptFldsDataReference) != 0)
                                    {
                                        report.HasDataReference = true;
                                        if (list[i].Success.isVisible_stringSelected())
                                        {
                                            varName = list[i].Success.Visible_string;
                                            report.DataReferences[dataReferenceCount] = varName;
                                            iecs.logger.LogDebug("Report Variable Name = " + varName);
                                            NodeBase b = iecs.DataModel.ied.FindNodeByAddress(varName);
                                            Data dataref = list[i + datanum].Success;
                                            if (!(b is NodeFC))
                                            {
                                                // dataref = (dataref.Structure as List<Data>)[0];
                                                if (list[i + datanum].Success != null)
                                                {
                                                    RecursiveReadData(iecs, dataref, b, NodeState.Reported);
                                                }
                                            }
                                            dataReferenceCount++;
                                        }
                                        // Evaluation of OptFldsDataReference
                                        if (dataReferenceCount == datanum)
                                        // break; // iedexplorer
                                        {
                                            phase++; // zheleznyakov
                                            continue;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                        phase++;
                                }
                                // here we will be only if report is received without variable names
                                if (phase == phsValues)
                                {
                                    // Report WITHOUT references:
                                    // Need to investigate report members
                                    NodeBase lvb = iecs.DataModel.datasets.FindNodeByAddress(datName, true);
                                    report.DataValues[dataValuesCount] = new MmsValue(list[i].Success);
                                    if (lvb != null)
                                    {
                                        NodeBase[] nba = lvb.GetChildNodes();

                                        if (dataValuesCount < nba.Length)
                                        {
                                            if (!(nba[dataValuesCount] is NodeFC))
                                            {
                                                Data dataref = list[i].Success;
                                                if (list[i].Success != null)
                                                {
                                                    RecursiveReadData(iecs, dataref, nba[listmap[dataValuesCount]], NodeState.Reported);
                                                    varName = nba[listmap[dataValuesCount]].CommAddress.Domain + "/" + nba[listmap[dataValuesCount]].CommAddress.Variable;
                                                }
                                            }
                                            dataValuesCount++;
                                        }
                                        else
                                        {
                                            dataValuesCount++;
                                        }
                                        // End or continue?
                                        if (dataValuesCount == datanum)
                                        {
                                            phase++; // phsReasonCodes
                                            continue;
                                        }
                                        else
                                            continue;
                                    }
                                }
                                if (phase == phsReasonCodes)
                                {
                                    if ((rptOpts[0] & OptFldsReasonCode) != 0)
                                    {
                                        byte[] bitStringValue = list[i].Success.Bit_string.Value;
                                        int size = list[i].Success.Bit_string.getLengthInBits();
                                        if (MmsDecoder.GetBitStringFromMmsValue(bitStringValue, size, 1))
                                        {
                                            report.ReasonForInclusion[reasoncnt] = ReasonForInclusionEnum.DATA_CHANGE;
                                        }
                                        else if (MmsDecoder.GetBitStringFromMmsValue(bitStringValue, size, 2))
                                        {
                                            report.ReasonForInclusion[reasoncnt] = ReasonForInclusionEnum.QUALITY_CHANGE;
                                        }
                                        else if (MmsDecoder.GetBitStringFromMmsValue(bitStringValue, size, 3))
                                        {
                                            report.ReasonForInclusion[reasoncnt] = ReasonForInclusionEnum.DATA_UPDATE;
                                        }
                                        else if (MmsDecoder.GetBitStringFromMmsValue(bitStringValue, size, 4))
                                        {
                                            report.ReasonForInclusion[reasoncnt] = ReasonForInclusionEnum.INTEGRITY;
                                        }
                                        else if (MmsDecoder.GetBitStringFromMmsValue(bitStringValue, size, 5))
                                        {
                                            report.ReasonForInclusion[reasoncnt] = ReasonForInclusionEnum.GI;
                                        }
                                        reasoncnt++;
                                        // End or continue?
                                        if (reasoncnt == datanum)
                                            break;  // phsReasonCodes
                                        else
                                            continue;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                 //   iecs.sourceLogger?.SendInfo("Обработка отчёта завершена: rptid " + report.RptId + " ds: " + report.DataSetName + " count: " + report.DataValues.Length);
                    Task newReportTask = new Task(() => reportReceivedEventHandler(report));
                 //   iecs.sourceLogger?.SendInfo("Создали таск для " + report.RptId + ": " + newReportTask.Status + " handler: " + reportReceivedEventHandler.Method.Name);
                    newReportTask.Start();
                 //   iecs.sourceLogger?.SendInfo("Отправили сигнал " + report.RptId + " status task: " + newReportTask.Status);
                }
            }
            else if (Report != null && Report.VariableAccessSpecification != null && Report.VariableAccessSpecification.isListOfVariableSelected())
            {
                foreach (VariableAccessSpecification.ListOfVariableSequenceType lvs in Report.VariableAccessSpecification.ListOfVariable)
                {
                    string lstErr = (lvs.VariableSpecification.isNameSelected() ? (lvs.VariableSpecification.Name.isVmd_specificSelected() ? lvs.VariableSpecification.Name.Vmd_specific.Value : "??") : "??");
                    if (lstErr == "LastApplError")
                    {
                        int phs = 0;
                        string cntrlObj = "";
                        ControlErrorEnum error = ControlErrorEnum.NoError;
                        OriginatorCategoryEnum originOrCat = OriginatorCategoryEnum.NOT_SUPPORTED;
                        string originOrStr = "";
                        long ctlNum = 0;
                        ControlAddCauseEnum addCause = ControlAddCauseEnum.ADD_CAUSE_UNKNOWN;
                        ControlObject controlObject = null;
                        CommandTerminationReport comTermReport = new CommandTerminationReport();

                        foreach (AccessResult ara in Report.ListOfAccessResult)
                        {
                            if (ara.isSuccessSelected() && ara.Success.isStructureSelected())
                            {
                                foreach (Data data in ara.Success.Structure)
                                {
                                    switch (phs++)
                                    {
                                        case 0:
                                            if (data.isVisible_stringSelected())
                                            {
                                                cntrlObj = data.Visible_string;
                                                controlObject = listOfControlObjects.Find(x => cntrlObj.Contains(x.mmsReference));
                                                comTermReport.ObjectReference = cntrlObj;
                                            }
                                            break;
                                        case 1:
                                            if (data.isIntegerSelected())
                                            {
                                                error = (ControlErrorEnum)data.Integer;
                                                comTermReport.ControlError = error;
                                            }
                                            break;
                                        case 2:
                                            if (data.isStructureSelected())
                                            {
                                                int j = 0;
                                                foreach (Data d in data.Structure)
                                                {
                                                    if (j == 0)
                                                    {
                                                        if (d.isIntegerSelected())
                                                            originOrCat = (OriginatorCategoryEnum)d.Integer;
                                                    }
                                                    if (j == 1)
                                                    {
                                                        if (d.isOctet_stringSelected())
                                                        {
                                                            originOrStr = System.Text.Encoding.ASCII.GetString(d.Octet_string);
                                                        }
                                                    }
                                                    ++j;
                                                }
                                            }
                                            break;
                                        case 3:
                                            if (data.isIntegerSelected())
                                                ctlNum = data.Integer;
                                            break;
                                        case 4:
                                            if (data.isIntegerSelected())
                                            {
                                                addCause = (ControlAddCauseEnum)data.Integer;
                                                comTermReport.ControlAddCause = addCause;
                                            }
                                            break;
                                    } // switch
                                }
                            } // if
                        } // foreach
                        if (controlObject != null)
                        {
                            controlObject.ControlAddCause = addCause;
                            controlObject.ControlError = error;
                            controlObject.responseComTerTask?.Start();
                        }
                        Logger.getLogger().LogWarning("Have got LastApplError:" +
                            ", Control Object: " + cntrlObj +
                            ", Error: " + ((int)error).ToString() + " (" + Enum.GetName(typeof(ControlErrorEnum), error) + ")" +
                            ", Originator: " + ((int)originOrCat).ToString() + " (" + Enum.GetName(typeof(OriginatorCategoryEnum), originOrCat) + "), Id = " + originOrStr +
                            ", CtlNum: " + ctlNum.ToString() +
                            ", addCause: " + ((int)addCause).ToString() + " (" + Enum.GetName(typeof(ControlAddCauseEnum), addCause) + ")"
                             );
                    }
                    else
                        iecs.logger.LogDebug("Have unknown Unconfirmed PDU: " + lstErr);
                }
            }
        }

        private void ReceiveRead(Iec61850State iecs, Read_Response Read, NodeBase[] lastOperationData, int receivedInvokeId)
        {
            iecs.logger.LogDebug("Read != null");
            if (receivedInvokeId == 255)
            {
                int a = receivedInvokeId;
            }
            if (Read.VariableAccessSpecification != null)
            {
                iecs.logger.LogDebug("Read.VariableAccessSpecification != null");

                if (Read.VariableAccessSpecification.ListOfVariable != null)
                {
                    iecs.logger.LogDebug("Read.VariableAccessSpecification.ListOfVariable != null");
                    if (Read.ListOfAccessResult != null)
                    {
                        if (Read.ListOfAccessResult.Count == Read.VariableAccessSpecification.ListOfVariable.Count)
                        {
                            bool isInvokeIdExists = waitingMmsPdu.ContainsKey(receivedInvokeId);
                            (Task, IResponse) response = (null, null);
                            if (isInvokeIdExists)
                            {
                                waitingMmsPdu.TryGetValue(receivedInvokeId, out response);
                            }
                            if (response.Item2 is ReadDataSetResponse)
                            {
                                (response.Item2 as ReadDataSetResponse).MmsValues = new List<MmsValue>();
                                (response.Item2 as ReadDataSetResponse).TypeOfErrors = new List<DataAccessErrorEnum>();
                            }
                            IEnumerator<AccessResult> are = Read.ListOfAccessResult.GetEnumerator();
                            IEnumerator<VariableAccessSpecification.ListOfVariableSequenceType> vase = Read.VariableAccessSpecification.ListOfVariable.GetEnumerator();
                            while (are.MoveNext() && vase.MoveNext())
                            {
                                iecs.logger.LogDebug("Reading variable: " + vase.Current.VariableSpecification.Name.Domain_specific.ItemID.Value);
                                NodeBase b = (iecs.DataModel.ied as NodeIed).FindNodeByAddress(vase.Current.VariableSpecification.Name.Domain_specific.DomainID.Value, vase.Current.VariableSpecification.Name.Domain_specific.ItemID.Value);
                                if (b != null)
                                {
                                    iecs.logger.LogDebug("Node address: " + b.IecAddress);
                                    bool isNeededToBeRead = false;
                                    if (b is NodeDO)
                                    {
                                        isNeededToBeRead = (b as NodeDO).FC == FunctionalConstraintEnum.BR || (b as NodeDO).FC == FunctionalConstraintEnum.RP || (b as NodeDO).FC == FunctionalConstraintEnum.CO || (b as NodeDO).FC == FunctionalConstraintEnum.CF;
                                    }
                                    RecursiveReadData(iecs, are.Current.Success, b, NodeState.Read, isNeededToBeRead);
                                    if (isInvokeIdExists)
                                    {
                                        ReportControlBlock rcb = null;
                                        bool isRcbRequested = response.Item2 is RCBResponse;
                                        if (b is NodeDO)
                                        {
                                            if (isRcbRequested && ((b as NodeDO).FC == FunctionalConstraintEnum.BR))
                                            {
                                                string mmsRef = IecToMmsConverter.ConvertIecAddressToMms(b.IecAddress, FunctionalConstraintEnum.BR);
                                                rcb = new ReportControlBlock()
                                                {
                                                    self = (NodeRCB)iecs.DataModel.brcbs.FindNodeByAddress(mmsRef),
                                                    TypeOfError = DataAccessErrorEnum.none
                                                };
                                            }
                                            else if (isRcbRequested && ((b as NodeDO).FC == FunctionalConstraintEnum.RP))
                                            {
                                                string mmsRef = IecToMmsConverter.ConvertIecAddressToMms(b.IecAddress, FunctionalConstraintEnum.RP);
                                                rcb = new ReportControlBlock()
                                                {
                                                    self = (NodeRCB)iecs.DataModel.urcbs.FindNodeByAddress(mmsRef),
                                                    TypeOfError = DataAccessErrorEnum.none
                                                };
                                            }
                                            if (isRcbRequested)
                                            {
                                            //    Console.WriteLine("rcb requested on invokeid " + receivedInvokeId);
                                                (response.Item2 as RCBResponse).ReportControlBlock = new ReportControlBlock();
                                                (response.Item2 as RCBResponse).ReportControlBlock.self = rcb.self;
                                                (response.Item2 as RCBResponse).ReportControlBlock.TypeOfError = rcb.TypeOfError;
                                                (response.Item2 as RCBResponse).TypeOfError = rcb.TypeOfError;
                                                (response.Item2 as RCBResponse).ReportControlBlock.ResetFlags();
                                            }
                                            else if (response.Item2 is ReadResponse)
                                            {
                                                (response.Item2 as ReadResponse).MmsValue = new MmsValue(are.Current.Success);
                                                (response.Item2 as ReadResponse).TypeOfError = DataAccessErrorEnum.none;
                                                response.Item1?.Start();
                                                waitingMmsPdu.Remove(receivedInvokeId);
                                            }
                                            else if (response.Item2 is SelectResponse)
                                            {
                                                (response.Item2 as SelectResponse).IsSelected = true;
                                                (response.Item2 as SelectResponse).TypeOfError = DataAccessErrorEnum.none;
                                            }
                                            else if (response.Item2 is ReadDataSetResponse)
                                            {
                                                (response.Item2 as ReadDataSetResponse).MmsValues.Add(new MmsValue(are.Current.Success));
                                                (response.Item2 as ReadDataSetResponse).TypeOfErrors.Add(DataAccessErrorEnum.none);
                                            }
                                            if (isRcbRequested || response.Item2 is SelectResponse)
                                            {
                                                response.Item1?.Start();
                                                waitingMmsPdu.Remove(receivedInvokeId);
                                            }
                                        }
                                        else if (response.Item2 is ReadDataSetResponse && b is NodeData)
                                        {
                                            (response.Item2 as ReadDataSetResponse).MmsValues.Add(new MmsValue(are.Current.Success));
                                            (response.Item2 as ReadDataSetResponse).TypeOfErrors.Add(DataAccessErrorEnum.none);
                                        }
                                        else if (response.Item2 is ReadResponse && b is NodeData)
                                        {
                                            (response.Item2 as ReadResponse).MmsValue = new MmsValue(are.Current.Success);
                                            (response.Item2 as ReadResponse).TypeOfError = DataAccessErrorEnum.none;
                                            response.Item1?.Start();
                                            waitingMmsPdu.Remove(receivedInvokeId);
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            // Error
                            //     Console.WriteLine("Error reading variables, different count of Specifications and AccessResults!");
                            iecs.sourceLogger?.SendError("lib61850net: Error reading variables, different count of Specifications and AccessResults!");
                            iecs.logger.LogError("Error reading variables, different count of Specifications and AccessResults!");
                        }
                    }
                }
                else if (Read.VariableAccessSpecification.VariableListName != null)
                {
                    // reading a NVL read
                    iecs.logger.LogDebug("Read.VariableAccessSpecification.ListOfVariable != null");

                    if (Read.ListOfAccessResult != null)
                    {

                        // Find the NVL by name
                        if (Read.VariableAccessSpecification.VariableListName.Domain_specific != null)
                        {

                            string domain = Read.VariableAccessSpecification.VariableListName.Domain_specific.DomainID.Value;
                            string address = Read.VariableAccessSpecification.VariableListName.Domain_specific.ItemID.Value;
                            NodeBase nb = iecs.DataModel.datasets.FindNodeByAddress(domain, address, true);
                            bool isInvokeIdExists = (waitingMmsPdu.ContainsKey(receivedInvokeId));
                            ReadDataSetResponse response = new ReadDataSetResponse()
                            {
                                TypeOfErrors = new List<DataAccessErrorEnum>(),
                                MmsValues = new List<MmsValue>(),
                            };
                            // TODO
                            if (nb != null && nb.GetChildCount() == Read.ListOfAccessResult.Count)
                            {
                                NodeBase[] data = nb.GetChildNodes();

                                for (int i = 0; i < data.Length; i++)
                                {
                                    iecs.logger.LogDebug("Reading variable: " + data[i].IecAddress);
                                    RecursiveReadData(iecs, (Read.ListOfAccessResult as List<AccessResult>)[i].Success, data[i], NodeState.Read);
                                    if (isInvokeIdExists)
                                    {
                                        if ((Read.ListOfAccessResult as List<AccessResult>)[i].Success != null)
                                        {
                                            response.TypeOfErrors.Add(DataAccessErrorEnum.none);
                                            response.MmsValues.Add(new MmsValue((Read.ListOfAccessResult as List<AccessResult>)[i].Success));
                                        }
                                        else
                                        {
                                            response.TypeOfErrors.Add(DataAccessErrorEnum.objectAccessDenied);
                                            response.MmsValues.Add(null);
                                        }
                                    }
                                }

                                if (isInvokeIdExists)
                                {
                                    waitingMmsPdu.TryGetValue(receivedInvokeId, out (Task, IResponse) userResponse);
                                    (userResponse.Item2 as ReadDataSetResponse).MmsValues = new List<MmsValue>(response.MmsValues);
                                    (userResponse.Item2 as ReadDataSetResponse).TypeOfErrors = new List<DataAccessErrorEnum>(response.TypeOfErrors);
                                    userResponse.Item1?.Start();
                                    waitingMmsPdu.Remove(receivedInvokeId);
                                }
                            }
                        }
                    }
                }
            }
            else    // When VariableAccessSpecification is missing, try to read to actual variable
            {

                if (Read.ListOfAccessResult != null)
                {

                    int i = 0;
                    // libiec61850 correction
                    // one read of a node is equal to separate reads to node children
                    if (Read.ListOfAccessResult.Count > lastOperationData.Length)
                    {
                        if (Read.ListOfAccessResult.Count == lastOperationData[0].GetChildNodes().Length)
                        {
                            lastOperationData = lastOperationData[0].GetChildNodes();
                        }
                    }
                    // libiec61850 correction end
                    bool isInvokeIdExists = waitingMmsPdu.ContainsKey(receivedInvokeId);
                    (Task, IResponse) response = (null, null);
                    if (isInvokeIdExists)
                    {
                        waitingMmsPdu.TryGetValue(receivedInvokeId, out response);
                    }
                    if (response.Item2 is ReadDataSetResponse)
                    {
                        (response.Item2 as ReadDataSetResponse).MmsValues = new List<MmsValue>();
                        (response.Item2 as ReadDataSetResponse).TypeOfErrors = new List<DataAccessErrorEnum>();
                    }
                    else if (response.Item2 is ReadResponse)
                    {
                        (response.Item2 as ReadResponse).MmsValue = new MmsValue();
                    }
                    foreach (AccessResult ar in Read.ListOfAccessResult)
                    {
                        if (i <= lastOperationData.GetUpperBound(0))
                        {
                            if (ar.Success != null)
                            {
                                iecs.logger.LogDebug("Reading Actual variable value: " + lastOperationData[i].IecAddress);
                                bool isNeededToBeRead = false;
                                if (lastOperationData[i] is NodeFC)
                                {
                                    isNeededToBeRead = (lastOperationData[i] as NodeFC).Name == "RP" || (lastOperationData[i] as NodeFC).Name == "BR" || (lastOperationData[i] as NodeFC).Name == "CO" || (lastOperationData[i] as NodeFC).Name == "CF";
                                }
                                else if (lastOperationData[i] is NodeDO)
                                {
                                    isNeededToBeRead = (lastOperationData[i] as NodeDO).FC == FunctionalConstraintEnum.BR || (lastOperationData[i] as NodeDO).FC == FunctionalConstraintEnum.RP || (lastOperationData[i] as NodeDO).FC == FunctionalConstraintEnum.CO || (lastOperationData[i] as NodeDO).FC == FunctionalConstraintEnum.CF;
                                }
                                else if (lastOperationData[i] is NodeData)
                                {
                                    isNeededToBeRead = (lastOperationData[i] as NodeData).FC == FunctionalConstraintEnum.BR || (lastOperationData[i] as NodeData).FC == FunctionalConstraintEnum.RP || (lastOperationData[i] as NodeData).FC == FunctionalConstraintEnum.CO || (lastOperationData[i] as NodeData).FC == FunctionalConstraintEnum.CF;
                                }
                                RecursiveReadData(iecs, ar.Success, lastOperationData[i], NodeState.Read, isNeededToBeRead);
                                if (isInvokeIdExists)
                                {
                                    MmsValue mmsValue = new MmsValue(ar.Success)
                                    {
                                        TypeOfError = DataAccessErrorEnum.none,
                                    };
                                    ReportControlBlock rcb = null;
                                    bool isRcbRequested = response.Item2 is RCBResponse;
                                    if (lastOperationData[i] is NodeDO || lastOperationData[i] is NodeData)
                                    {

                                        if (isRcbRequested && ((lastOperationData[i] as NodeDO).FC == FunctionalConstraintEnum.BR))
                                        {
                                            string mmsRef = IecToMmsConverter.ConvertIecAddressToMms(lastOperationData[i].IecAddress, FunctionalConstraintEnum.BR);
                                            rcb = new ReportControlBlock()
                                            {
                                                self = (NodeRCB)iecs.DataModel.brcbs.FindNodeByAddress(mmsRef),
                                                TypeOfError = DataAccessErrorEnum.none
                                            };
                                        }
                                        else if (isRcbRequested && ((lastOperationData[i] as NodeDO).FC == FunctionalConstraintEnum.RP))
                                        {
                                            string mmsRef = IecToMmsConverter.ConvertIecAddressToMms(lastOperationData[i].IecAddress, FunctionalConstraintEnum.RP);
                                            rcb = new ReportControlBlock()
                                            {
                                                self = (NodeRCB)iecs.DataModel.urcbs.FindNodeByAddress(mmsRef),
                                                TypeOfError = DataAccessErrorEnum.none
                                            };
                                        }
                                        //  responseEventWithArg.Item2 = isRcbRequested ? rcb : mmsValue;
                                        if (isRcbRequested)
                                        {
                                       //     Console.WriteLine("rcb requested on invokeid " + receivedInvokeId);
                                            (response.Item2 as RCBResponse).ReportControlBlock = new ReportControlBlock();
                                            (response.Item2 as RCBResponse).ReportControlBlock.self = rcb.self;
                                            (response.Item2 as RCBResponse).ReportControlBlock.ObjectReference = rcb.self.IecAddress;
                                            (response.Item2 as RCBResponse).ReportControlBlock.TypeOfError = rcb.TypeOfError;
                                            (response.Item2 as RCBResponse).TypeOfError = rcb.TypeOfError;
                                            (response.Item2 as RCBResponse).ReportControlBlock.ResetFlags();
                                        }
                                        else if (response.Item2 is ReadResponse)
                                        {
                                            (response.Item2 as ReadResponse).MmsValue = mmsValue;
                                            (response.Item2 as ReadResponse).TypeOfError = DataAccessErrorEnum.none;
                                        }
                                        else if (response.Item2 is SelectResponse)
                                        {
                                            (response.Item2 as SelectResponse).IsSelected = true;
                                            (response.Item2 as SelectResponse).TypeOfError = DataAccessErrorEnum.none;
                                        }
                                        else if (response.Item2 is ReadDataSetResponse)
                                        {
                                            (response.Item2 as ReadDataSetResponse).MmsValues.Add(new MmsValue(ar.Success));
                                            (response.Item2 as ReadDataSetResponse).TypeOfErrors.Add(DataAccessErrorEnum.none);
                                        }
                                        if (isRcbRequested || response.Item2 is SelectResponse)
                                        {

                                            response.Item1?.Start();
                                            waitingMmsPdu.Remove(receivedInvokeId);
                                        }
                                    }
                                    else if (response.Item2 is ReadDataSetResponse && lastOperationData[i] is NodeData)
                                    {
                                        (response.Item2 as ReadDataSetResponse).MmsValues.Add(new MmsValue(ar.Success));
                                        (response.Item2 as ReadDataSetResponse).TypeOfErrors.Add(DataAccessErrorEnum.none);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isInvokeIdExists)
                            {
                                if (response.Item2 is RCBResponse)
                                {
                                    (response.Item2 as RCBResponse).TypeOfError = (DataAccessErrorEnum)ar.Failure.Value;
                                }
                                else if (response.Item2 is ReadResponse)
                                {
                                    (response.Item2 as ReadResponse).TypeOfError = (DataAccessErrorEnum)ar.Failure.Value;
                                }
                                else if (response.Item2 is SelectResponse)
                                {
                                    (response.Item2 as SelectResponse).TypeOfError = (DataAccessErrorEnum)ar.Failure.Value;
                                }
                                //     waitingMmsPdu[receivedInvokeId]?.Invoke(response, null);
                            }
                            iecs.sourceLogger?.SendError("lib61850net: Not matching read structure in ReceiveRead");
                            iecs.logger.LogError("Not matching read structure in ReceiveRead");
                        }
                        i++;
                    }
                    try
                    {
                        if (response.Item1?.Status == TaskStatus.Created)
                        {
                            response.Item1?.Start();
                            waitingMmsPdu.Remove(receivedInvokeId);
                        }
                    }
                    catch { }
                    lastOperationData = null;
                }
            }
            if (iecs.istate == Iec61850lStateEnum.IEC61850_READ_MODEL_DATA_WAIT)
            {
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                if (iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                    {
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                            // End of loop
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                            iecs.logger.LogInfo("Reading named variable lists: [IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                            iecs.DataModel.ied.ResetAllChildNodes();
                        }
                    }
                }
            }
        }

        private void ReceiveGetNamedVariableListAttributes(Iec61850State iecs, GetNamedVariableListAttributes_Response GetNamedVariableListAttributes)
        {
            iecs.logger.LogDebug("GetNamedVariableListAttributes != null");
            if (GetNamedVariableListAttributes.MmsDeletable)
                (iecs.DataModel.datasets.GetActualChildNode().GetActualChildNode() as NodeVL).Deletable = true;

            if (GetNamedVariableListAttributes.ListOfVariable != null)
            {
                iecs.logger.LogDebug("GetVariableAccessAttributes.ListOfVariable != null");
                foreach (GetNamedVariableListAttributes_Response.ListOfVariableSequenceType v in GetNamedVariableListAttributes.ListOfVariable)
                {
                    iecs.logger.LogDebug(String.Format("GetNameList.ListOfIdentifier: {0}/{1}", v.VariableSpecification.Name.Domain_specific.DomainID.Value, v.VariableSpecification.Name.Domain_specific.ItemID.Value));
                    NodeBase b = (iecs.DataModel.ied as NodeIed).FindNodeByAddress(v.VariableSpecification.Name.Domain_specific.DomainID.Value, v.VariableSpecification.Name.Domain_specific.ItemID.Value);
                    if (b != null)
                    {
                        iecs.DataModel.datasets.GetActualChildNode().GetActualChildNode().LinkChildNodeByAddress(b);
                    }
                }
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST;
                if (iecs.DataModel.datasets.GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.datasets.NextActualChildNode() == null)
                    {
                        // End of loop
                        iecs.istate = Iec61850lStateEnum.IEC61850_MAKEGUI;
                        iecs.logger.LogInfo("Init end: [IEC61850_FREILAUF]");
                        iecs.DataModel.datasets.ResetAllChildNodes();
                        //iecs.
                    }
                }
            }
        }

        private MmsVariableSpecification MatchDescriptionWithMms(TypeDescription typeD, string name = null)
        {
            MmsVariableSpecification newMVS = new MmsVariableSpecification(typeD.MmsType)
            {
                Name = name
            };
            if (typeD.MmsType == MmsTypeEnum.STRUCTURE)
            {
                foreach (var child in typeD.Structure.Components)
                {
                    newMVS.AddChild(MatchDescriptionWithMms(child.ComponentType.TypeDescription, child.ComponentName.Value));
                }
            }

            return newMVS;
        }

        private void RecursiveCommandParDesription(Iec61850State iecs, NodeBase actualNode, TypeDescription t)
        {
            if (t == null) return;
            if (t.Structure != null)
            {
                iecs.logger.LogDebug("t.Structure != null");
                if (t.Structure.Components != null) foreach (TypeDescription.StructureSequenceType.ComponentsSequenceType s in t.Structure.Components)
                    {
                        iecs.logger.LogDebug(s.ComponentName.Value);
                        var newActNode = actualNode.FindChildNode(s.ComponentName.Value);
                        //NodeBase newActualNode;
                        //// DO or DA?
                        //bool isDO = false;
                        //if (actualNode is NodeFC) isDO = true;  // Safe to say under FC must be a DO
                        //if (isDO)
                        //    newActualNode = new NodeDO(s.ComponentName.Value);
                        //else
                        //    newActualNode = new NodeData(s.ComponentName.Value);
                        //newActualNode = actualNode.AddChildNode(newActualNode);
                        RecursiveCommandParDesription(iecs, newActNode, s.ComponentType.TypeDescription);
                    }
            }
            else if (t.Array != null)
            {
                //iecs.logger.LogDebug("t.Array != null");
                //if (actualNode is NodeData && !(actualNode is NodeDO))
                //    (actualNode as NodeData).DataType = MmsTypeEnum.ARRAY;
                //for (int i = 0; i < t.Array.NumberOfElements.Value; i++)
                //{
                //    NodeData newActualNode = new NodeData("[" + i.ToString() + "]");
                //    actualNode.AddChildNode(newActualNode);
                //    RecursiveReadTypeDescription(iecs, newActualNode, t.Array.ElementType.TypeDescription);
                //}
            }
            else if (t.Integer != null)
            {
                iecs.logger.LogDebug("t.Integer != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.INTEGER;
            }
            else if (t.Bcd != null)
            {
                iecs.logger.LogDebug("t.Bcd != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BCD;
            }
            else if (t.Boolean != null)
            {
                iecs.logger.LogDebug("t.Boolean != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BOOLEAN;
            }
            else if (t.Floating_point != null)
            {
                iecs.logger.LogDebug("t.Floating_point != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.FLOATING_POINT;
            }
            else if (t.Generalized_time != null)
            {
                iecs.logger.LogDebug("t.Generalized_time != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.GENERALIZED_TIME;
            }
            else if (t.MMSString != null)
            {
                iecs.logger.LogDebug("t.MMSString != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.MMS_STRING;
            }
            else if (t.ObjId != null)
            {
                iecs.logger.LogDebug("t.ObjId != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.OBJ_ID;
            }
            else if (t.Octet_string != null)
            {
                iecs.logger.LogDebug("t.Octet_string != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.OCTET_STRING;
            }
            else if (t.Unsigned != null)
            {
                iecs.logger.LogDebug("t.Unsigned != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.UNSIGNED;
            }
            else if (t.Utc_time != null)
            {
                iecs.logger.LogDebug("t.Utc_time != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.UTC_TIME;
            }
            else if (t.Visible_string != null)
            {
                iecs.logger.LogDebug("t.Visible_string != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.VISIBLE_STRING;
            }
            else if (t.isBinary_timeSelected())
            {
                iecs.logger.LogDebug("t.Binary_time != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BINARY_TIME;
            }
            else if (t.isBit_stringSelected())
            {
                iecs.logger.LogDebug("t.Bit_string != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BIT_STRING;
            }
        }

        private ConcurrentQueue<NodeFC> queueOfActualNodeFC = new ConcurrentQueue<NodeFC>();

        private void ReceiveGetVariableAccessAttributes(Iec61850State iecs, GetVariableAccessAttributes_Response GetVariableAccessAttributes, int invokeId)
        {
            iecs.logger.LogDebug("GetVariableAccessAttributes != null");
            bool isInvokeIdExists = waitingMmsPdu.ContainsKey(invokeId);


            if (GetVariableAccessAttributes.TypeDescription != null)
            {
                iecs.logger.LogDebug("GetVariableAccessAttributes.TypeDescription != null");
                // если запросили спецификацию самостоятельно
                if (isInvokeIdExists)
                {
                    if (dictionaryOfControlBlocks.ContainsKey(invokeId))
                    {
                        NodeBase comNode;
                        dictionaryOfControlBlocks.TryRemove(invokeId, out comNode);
                        RecursiveCommandParDesription(iecs, comNode, GetVariableAccessAttributes.TypeDescription);
                    }
                    waitingMmsPdu.TryGetValue(invokeId, out (Task, IResponse) taskWithResponse);
                    (taskWithResponse.Item2 as MmsVariableSpecResponse).MmsVariableSpecification = MatchDescriptionWithMms(GetVariableAccessAttributes.TypeDescription);
                    (taskWithResponse.Item2 as MmsVariableSpecResponse).TypeOfError = DataAccessErrorEnum.none;
                    taskWithResponse.Item1?.Start();
                    waitingMmsPdu.Remove(invokeId);
                }
                else //автоматический запрос при составлении модели
                {
                    NodeFC nodeFCToRead;
                    queueOfActualNodeFC.TryDequeue(out nodeFCToRead);
                    RecursiveReadTypeDescription(iecs, nodeFCToRead,
                                             GetVariableAccessAttributes.TypeDescription);
                    iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR;
                    if (queueOfReportsFC.Count == 0)
                    {
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                        iecs.logger.LogInfo("Reading variable values: [IEC61850_READ_MODEL_DATA]");
                    }
                }
            }
        }

        private void LinkRcb(Iec61850State iecs)
        {
            foreach (var (nb, rcb) in listOfRCB)
            {
                foreach (var child in nb.GetChildNodes())
                {
                    rcb.LinkChildNodeByAddress(child);
                }
            }
        }

        private void ReceiveGetNameList(Iec61850State iecs, GetNameList_Response GetNameList, int receivedInvokeId)
        {
            switch (iecs.istate)
            {
                case Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN_WAIT:
                    foreach (Identifier i in GetNameList.ListOfIdentifier)
                    {
                        iecs.logger.LogDebug(String.Format("GetNameList.ListOfIdentifier: {0}", i.Value));
                        iecs.DataModel.ied.AddChildNode(new NodeLD(i.Value));
                        iecs.continueAfter = null;
                    }
                    iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR;
                    iecs.logger.LogInfo("Reading variable names: [IEC61850_READ_NAMELIST_VAR]");
                    break;
                case Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR_WAIT:
                    foreach (Identifier i in GetNameList.ListOfIdentifier)
                    {
                        iecs.logger.LogDebug("GetNameList.ListOfIdentifier: " + i.Value.ToString());
                        AddIecAddress(iecs, i.Value);
                        iecs.continueAfter = i;
                    }
                    iecs.logger.LogDebug("GetNameList.MoreFollows: " + GetNameList.MoreFollows.ToString());
                    if (GetNameList.MoreFollows)
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR;
                    else
                    {
                        iecs.continueAfter = null;
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                                iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR;    // next state
                       //     LinkRcb(iecs);
                            //iecs.istate = Iec61850lStateEnum.IEC61850_MAKEGUI;
                        //    iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                            iecs.logger.LogInfo("Reading variable specifications: [IEC61850_READ_ACCESSAT_VAR]");
                            iecs.DataModel.ied.ResetAllChildNodes();
                        }
                        else
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR;         // next logical device
                    }
                    break;
                case Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST_WAIT:
                    if (GetNameList.ListOfIdentifier != null)
                        foreach (Identifier i in GetNameList.ListOfIdentifier)
                        {
                            iecs.logger.LogDebug(String.Format("GetNameList.ListOfIdentifier: {0}", i.Value));
                            NodeBase nld = iecs.DataModel.datasets.AddChildNode(new NodeLD(iecs.DataModel.ied.GetActualChildNode().Name));
                            NodeVL vl = new NodeVL(i.Value);
                            vl.Defined = true;
                            nld.AddChildNode(vl);
                            iecs.continueAfter = null;
                        }
                    if (GetNameList.MoreFollows)
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                    else
                    {
                        iecs.continueAfter = null;
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                            iecs.logger.LogInfo("Reading variable lists attributes: [IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST]");    // next state
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST; // next state
                            iecs.DataModel.ied.ResetAllChildNodes();
                            iecs.DataModel.datasets.ResetAllChildNodes();
                        }
                        else
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;         // next logical device
                    }
                    break;
                default:
                    {
                        if (waitingMmsPdu.ContainsKey(receivedInvokeId))
                        {
                            (Task, IResponse) responseWithTask;
                            waitingMmsPdu.TryGetValue(receivedInvokeId, out responseWithTask);
                            (responseWithTask.Item2 as DeviceDirectoryResponse).DirectoryNames = new List<string>();
                            if (GetNameList.ListOfIdentifier != null)
                            {
                                foreach (Identifier i in GetNameList.ListOfIdentifier)
                                {
                                    (responseWithTask.Item2 as DeviceDirectoryResponse).DirectoryNames.Add(i.Value);
                                    iecs.continueAfter = null;
                                }
                                (responseWithTask.Item2 as DeviceDirectoryResponse).TypeOfError = DataAccessErrorEnum.none;
                            }
                            else
                            {
                                (responseWithTask.Item2 as DeviceDirectoryResponse).TypeOfError = DataAccessErrorEnum.invalidAddress;
                            }

                            responseWithTask.Item1?.Start();
                            waitingMmsPdu.Remove(receivedInvokeId);
                        }
                        break;
                    }
            }
        }

        private void ReceiveIdentify(Iec61850State iecs, Identify_Response Identify)
        {
            iecs.logger.LogInfo(String.Format("Received Identify: {0}, {1}, {2}",
                Identify.VendorName.Value,
                Identify.ModelName.Value,
                Identify.Revision.Value
                ));
            (iecs.DataModel.ied as NodeIed).VendorName = Identify.VendorName.Value;
            (iecs.DataModel.ied as NodeIed).ModelName = Identify.ModelName.Value;
            (iecs.DataModel.ied as NodeIed).Revision = Identify.Revision.Value;
            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN;
            iecs.logger.LogInfo("Reading domain (LD) names: [IEC61850_READ_NAMELIST_DOMAIN]");
        }

        private void RecursiveReadData(Iec61850State iecs, Data data, NodeBase actualNode, NodeState s, bool isNeededToBeRead = false)
        {
            if (data == null)
            {
                iecs.logger.LogDebug("data = null, returning from recursiveReadData");
                return;
            }
            if (actualNode == null)
            {
                iecs.logger.LogDebug("actualNode = null, returning from recursiveReadData");
                return;
            }
            if (isNeededToBeRead)
            {
                iecs.logger.LogDebug("recursiveReadData: nodeAddress=" + actualNode.IecAddress + ", state=" + s.ToString());
                if (data.Structure != null)
                {
                    iecs.logger.LogDebug("data.Structure != null");
                    NodeBase[] nb = actualNode.GetChildNodes();
                    int i = 0;
                    foreach (Data d in data.Structure)
                    {
                        if (i <= nb.GetUpperBound(0))
                        {
                            //     var newActNode = actualNode.FindChildNode()
                            RecursiveReadData(iecs, d, nb[i], s, isNeededToBeRead);
                        }
                        else
                        {
                            iecs.logger.LogError("Not matching read structure: Node=" + actualNode.IecAddress);
                            iecs.sourceLogger?.SendError("lib61850net: Not matching read structure: Node=" + actualNode.IecAddress);
                        }
                        i++;
                    }
                }
                else if (data.Array != null)
                {
                    iecs.logger.LogDebug("data.Array != null");
                    int i = 0;
                    NodeBase[] nb = actualNode.GetChildNodes();
                    foreach (Data d in data.Array)
                    {
                        if (i <= nb.GetUpperBound(0))
                            RecursiveReadData(iecs, d, nb[i], s, isNeededToBeRead);
                        else
                        {
                            iecs.sourceLogger?.SendError("lib61850net: Not matching read array: Node=" + actualNode.Name);
                            iecs.logger.LogError("Not matching read array: Node=" + actualNode.Name);
                        }
                        i++;
                    }
                }
                else if (data.isIntegerSelected())
                {
                    iecs.logger.LogDebug("data.Integer != null");
                    (actualNode as NodeData).DataValue = data.Integer;
                }
                else if (data.isBcdSelected())
                {
                    iecs.logger.LogDebug("data.Bcd != null");
                    (actualNode as NodeData).DataValue = data.Bcd;
                }
                else if (data.isBooleanSelected())
                {
                    iecs.logger.LogDebug("data.Boolean != null");
                    (actualNode as NodeData).DataValue = data.Boolean;
                }
                else if (data.isFloating_pointSelected())
                {
                    iecs.logger.LogDebug("data.Floating_point != null");
                    if (data.Floating_point.Value.Length == 5)
                    {
                        float k = 0.0F;
                        byte[] tmp = new byte[4];
                        tmp[0] = data.Floating_point.Value[4];
                        tmp[1] = data.Floating_point.Value[3];
                        tmp[2] = data.Floating_point.Value[2];
                        tmp[3] = data.Floating_point.Value[1];
                        k = BitConverter.ToSingle(tmp, 0);
                        (actualNode as NodeData).DataValue = k;
                    }
                    else if (data.Floating_point.Value.Length == 9)
                    {
                        double k = 0.0;
                        byte[] tmp = new byte[8];
                        tmp[0] = data.Floating_point.Value[8];
                        tmp[1] = data.Floating_point.Value[7];
                        tmp[2] = data.Floating_point.Value[6];
                        tmp[3] = data.Floating_point.Value[5];
                        tmp[4] = data.Floating_point.Value[4];
                        tmp[5] = data.Floating_point.Value[3];
                        tmp[6] = data.Floating_point.Value[2];
                        tmp[7] = data.Floating_point.Value[1];
                        k = BitConverter.ToDouble(tmp, 0);
                        (actualNode as NodeData).DataValue = k;
                    }
                }
                else if (data.isGeneralized_timeSelected())
                {
                    iecs.logger.LogDebug("data.Generalized_time != null");
                    (actualNode as NodeData).DataValue = data.Generalized_time;
                }
                else if (data.isMMSStringSelected())
                {
                    iecs.logger.LogDebug("data.MMSString != null");
                    (actualNode as NodeData).DataValue = data.MMSString.Value;
                }
                else if (data.isObjIdSelected())
                {
                    iecs.logger.LogDebug("data.ObjId != null");
                    (actualNode as NodeData).DataValue = data.ObjId.Value;
                }
                else if (data.isOctet_stringSelected())
                {
                    iecs.logger.LogDebug("data.Octet_string != null");
                    (actualNode as NodeData).DataValue = System.Text.ASCIIEncoding.ASCII.GetString(data.Octet_string);
                }
                else if (data.isUnsignedSelected())
                {
                    iecs.logger.LogDebug("data.Unsigned != null");
                    (actualNode as NodeData).DataValue = data.Unsigned;
                }
                else if (data.isUtc_timeSelected())
                {
                    iecs.logger.LogDebug("data.Utc_time != null");

                    (actualNode as NodeData).DataValue = ConvertFromUtcTime(data.Utc_time.Value, (actualNode as NodeData).DataParam);
                }
                else if (data.isVisible_stringSelected())
                {
                    iecs.logger.LogDebug("data.Visible_string != null");
                    (actualNode as NodeData).DataValue = data.Visible_string;
                }
                else if (data.isBinary_timeSelected())
                {
                    iecs.logger.LogDebug("data.Binary_time != null");

                    ulong millis;
                    ulong days = 0;
                    DateTime origin;

                    millis = (ulong)(data.Binary_time.Value[0] << 24) +
                             (ulong)(data.Binary_time.Value[1] << 16) +
                             (ulong)(data.Binary_time.Value[2] << 8) +
                             (ulong)(data.Binary_time.Value[3]);
                    if (data.Binary_time.Value.Length == 6)
                    {
                        days = (ulong)(data.Binary_time.Value[4] << 8) +
                               (ulong)(data.Binary_time.Value[5]);
                        origin = new DateTime(1984, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        //millis *= 1000;
                    }
                    else
                    {
                        origin = DateTime.UtcNow.Date;
                    }

                    double dMillis = (double)(millis + days * 24 * 3600 * 1000);
                    origin = origin.AddMilliseconds(dMillis);

                    (actualNode as NodeData).DataValue = origin.ToLocalTime();
                    (actualNode as NodeData).DataParam = data.Binary_time.Value;

                }
                else if (data.isBit_stringSelected())
                {
                    iecs.logger.LogDebug("data.Bit_string != null");
                    (actualNode as NodeData).DataValue = data.Bit_string.Value;
                    (actualNode as NodeData).DataParam = data.Bit_string.TrailBitsCnt;
                }
                iecs.logger.LogDebug("recursiveReadData: successfull return");
            }
        }

        internal ConcurrentQueue<NodeBase> QueueOfCommandNodes { get; set; } = new ConcurrentQueue<NodeBase>();

        void RecursiveReadTypeDescription(Iec61850State iecs, NodeBase actualNode, TypeDescription t)
        {
            //  string address = actualNode.CommAddress.Domain + "/" + actualNode.CommAddress.Variable;
            //if (!addressNodesPairs.ContainsKey(address))
            //{
            //    addressNodesPairs.Add(address, actualNode);
            //}
            if (t == null) return;
            if (t.Structure != null)
            {
                iecs.logger.LogDebug("t.Structure != null");
                if (t.Structure.Components != null) foreach (TypeDescription.StructureSequenceType.ComponentsSequenceType s in t.Structure.Components)
                    {
                        iecs.logger.LogDebug(s.ComponentName.Value);
                        NodeBase newActualNode;
                        // DO or DA?
                        bool isDO = false;
                        if (actualNode is NodeFC) isDO = true;  // Safe to say under FC must be a DO
                        if (isDO)
                            newActualNode = new NodeDO(s.ComponentName.Value);
                        else
                            newActualNode = new NodeData(s.ComponentName.Value);
                        newActualNode = actualNode.AddChildNode(newActualNode);
                        RecursiveReadTypeDescription(iecs, newActualNode, s.ComponentType.TypeDescription);
                        if (actualNode is NodeFC && (actualNode.Name == "RP" || actualNode.Name == "BR"))
                        {
                            // Having RCB
                            NodeBase nrpied;
                            string actualName = actualNode.Parent.Parent.Name;
                            if (actualNode.Name == "RP")
                            {
                                nrpied = iecs.DataModel.urcbs.AddChildNode(new NodeLD(actualName));
                            }
                            else
                            {
                                nrpied = iecs.DataModel.brcbs.AddChildNode(new NodeLD(actualName));
                            }
                            NodeBase nrp = new NodeRCB(newActualNode.CommAddress.Variable, newActualNode.Name);
                            nrpied.AddChildNode(nrp);
                            foreach (NodeBase nb in newActualNode.GetChildNodes())
                            {
                                nrp.LinkChildNodeByAddress(nb);
                            }
                        }
                    }
            }
            else if (t.Array != null)
            {
                iecs.logger.LogDebug("t.Array != null");
                if (actualNode is NodeData && !(actualNode is NodeDO))
                    (actualNode as NodeData).DataType = MmsTypeEnum.ARRAY;
                for (int i = 0; i < t.Array.NumberOfElements.Value; i++)
                {
                    NodeData newActualNode = new NodeData("[" + i.ToString() + "]");
                    actualNode.AddChildNode(newActualNode);
                    RecursiveReadTypeDescription(iecs, newActualNode, t.Array.ElementType.TypeDescription);
                }
            }
            else if (t.Integer != null)
            {
                iecs.logger.LogDebug("t.Integer != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.INTEGER;
            }
            else if (t.Bcd != null)
            {
                iecs.logger.LogDebug("t.Bcd != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BCD;
            }
            else if (t.Boolean != null)
            {
                iecs.logger.LogDebug("t.Boolean != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BOOLEAN;
            }
            else if (t.Floating_point != null)
            {
                iecs.logger.LogDebug("t.Floating_point != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.FLOATING_POINT;
            }
            else if (t.Generalized_time != null)
            {
                iecs.logger.LogDebug("t.Generalized_time != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.GENERALIZED_TIME;
            }
            else if (t.MMSString != null)
            {
                iecs.logger.LogDebug("t.MMSString != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.MMS_STRING;
            }
            else if (t.ObjId != null)
            {
                iecs.logger.LogDebug("t.ObjId != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.OBJ_ID;
            }
            else if (t.Octet_string != null)
            {
                iecs.logger.LogDebug("t.Octet_string != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.OCTET_STRING;
            }
            else if (t.Unsigned != null)
            {
                iecs.logger.LogDebug("t.Unsigned != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.UNSIGNED;
            }
            else if (t.Utc_time != null)
            {
                iecs.logger.LogDebug("t.Utc_time != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.UTC_TIME;
            }
            else if (t.Visible_string != null)
            {
                iecs.logger.LogDebug("t.Visible_string != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.VISIBLE_STRING;
            }
            else if (t.isBinary_timeSelected())
            {
                iecs.logger.LogDebug("t.Binary_time != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BINARY_TIME;
            }
            else if (t.isBit_stringSelected())
            {
                iecs.logger.LogDebug("t.Bit_string != null");
                (actualNode as NodeData).DataType = MmsTypeEnum.BIT_STRING;
            }
        }

        internal int ReceiveInitiate(Iec61850State iecs, Initiate_ResponsePDU initiate_ResponsePDU)
        {
            if (initiate_ResponsePDU != null)
            {
                iecs.logger.LogDebug("mymmspdu.Initiate_ResponsePDU exists!");
                int cing = initiate_ResponsePDU.NegotiatedMaxServOutstandingCalling.Value;
                int ced = initiate_ResponsePDU.NegotiatedMaxServOutstandingCalled.Value;
                iecs.logger.LogDebug(String.Format("mymmspdu.Initiate_ResponsePDU.NegotiatedMaxServOutstandingCalling: {0}, Called: {1}",
                    cing, ced));

                //MaxCalls = cing < ced ? cing : ced;

                StringBuilder sb2 = new StringBuilder();
                int j = 0;
                foreach (byte b in initiate_ResponsePDU.InitResponseDetail.ServicesSupportedCalled.Value.Value)
                {
                    for (int i = 7; i >= 0; i--)
                    {
                        ServiceSupportOptions[j] = ((b >> i) & 1) == 1;
                        if (ServiceSupportOptions[j]) sb2.Append(Enum.GetName(typeof(ServiceSupportOptionsEnum), (ServiceSupportOptionsEnum)j) + ',');
                        j++;
                    }
                }
                iecs.logger.LogInfo("Services Supported: " + sb2.ToString());

                if (ServiceSupportOptions[(int)ServiceSupportOptionsEnum.defineNamedVariableList])
                {
                    iecs.DataModel.ied.DefineNVL = true;
                }
                if (ServiceSupportOptions[(int)ServiceSupportOptionsEnum.identify])
                {
                    iecs.DataModel.ied.Identify = true;
                }
            }
            else
            {
                iecs.logger.LogError("mms.ReceiveInitiate: not an Initiate_ResponsePDU");
                iecs.sourceLogger?.SendError("lib61850net: mms.ReceiveInitiate: not an Initiate_ResponsePDU");
                return -1;
            }
            return 0;
        }

        /*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

        internal int SendConclude(Iec61850State iecs)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Conclude_RequestPDU crreq = new Conclude_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Conclude_Request cnreq = new Conclude_Request();

            cnreq.initWithDefaults();

            csrreq.selectConclude(cnreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConclude_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendIdentify: Encoding Error!");
                iecs.logger.LogError("mms.SendIdentify: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendIdentify(Iec61850State iecs)
        {


            waitingMmsPdu = new Dictionary<int, (Task, IResponse)>();
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Identify_Request idreq = new Identify_Request();

            idreq.initWithDefaults();

            csrreq.selectIdentify(idreq);

            InvokeID = 0;
            crreq.InvokeID = new Unsigned32(InvokeID++);
            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendIdentify: Encoding Error!");
                iecs.logger.LogError("mms.SendIdentify: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendGetNameListDomain(Iec61850State iecs)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNameList_Request nlreq = new GetNameList_Request();

            nlreq.ObjectClass = new ObjectClass();
            nlreq.ObjectClass.selectBasicObjectClass(ObjectClass.ObjectClass__basicObjectClass_domain);
            nlreq.ObjectScope = new GetNameList_Request.ObjectScopeChoiceType();
            nlreq.ObjectScope.selectVmdSpecific();

            csrreq.selectGetNameList(nlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendGetNameListDomain: Encoding Error!");
                iecs.logger.LogError("mms.SendGetNameListDomain: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendGetNameListVariables(Iec61850State iecs, string ldName = null, Task responseTask = null, DeviceDirectoryResponse response = null)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNameList_Request nlreq = new GetNameList_Request();

            nlreq.ObjectClass = new ObjectClass();
            nlreq.ObjectClass.selectBasicObjectClass(ObjectClass.ObjectClass__basicObjectClass_namedVariable);
            nlreq.ObjectScope = new GetNameList_Request.ObjectScopeChoiceType();
            if (ldName != null)
            {
                nlreq.ObjectScope.selectDomainSpecific(new Identifier(ldName));
            }
            else
            {
                nlreq.ObjectScope.selectDomainSpecific(new Identifier(iecs.DataModel.ied.GetActualChildNode().Name));
            }
            nlreq.ContinueAfter = iecs.continueAfter;

            csrreq.selectGetNameList(nlreq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendGetNameListVariables: Encoding Error!");
                iecs.logger.LogError("mms.SendGetNameListVariables: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendGetNameListNamedVariableList(Iec61850State iecs, string ldName = null, Task responseTask = null, DeviceDirectoryResponse response = null)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNameList_Request nlreq = new GetNameList_Request();

            nlreq.ObjectClass = new ObjectClass();
            nlreq.ObjectClass.selectBasicObjectClass(ObjectClass.ObjectClass__basicObjectClass_namedVariableList);
            nlreq.ObjectScope = new GetNameList_Request.ObjectScopeChoiceType();
            if (ldName != null)
            {
                nlreq.ObjectScope.selectDomainSpecific(new Identifier(ldName));
            }
            else
            {
                nlreq.ObjectScope.selectDomainSpecific(new Identifier(iecs.DataModel.ied.GetActualChildNode().Name));
            }
            nlreq.ContinueAfter = iecs.continueAfter;

            csrreq.selectGetNameList(nlreq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendGetNameListVariables: Encoding Error!");
                iecs.logger.LogError("mms.SendGetNameListVariables: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendGetVariableAccessAttributes(Iec61850State iecs, NodeBase node = null, Task responseTask = null, MmsVariableSpecResponse response = null)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetVariableAccessAttributes_Request vareq = new GetVariableAccessAttributes_Request();
            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

            if (queueOfReportsFC.Count > 0)
            {
                NodeFC actNode;
                queueOfReportsFC.TryDequeue(out actNode);
                queueOfActualNodeFC.Enqueue(actNode);
                dst.DomainID = new Identifier(actNode.Parent.Parent.Name); // actual LD name
                dst.ItemID = new Identifier(actNode.Parent.Name + "$" + actNode.Name); // e.g. LLN0$RP
            }
            else if (node == null)
            {
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR;
                if (queueOfReportsFC.Count == 0)
                {
                    iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                    iecs.logger.LogInfo("Reading variable values: [IEC61850_READ_MODEL_DATA]");
                    return 0;
                }
            }
            else
            {
                dst.DomainID = new Identifier(node.CommAddress.Domain);
                dst.ItemID = new Identifier(node.CommAddress.Variable);
            }

            iecs.logger.LogDebug("SendGetVariableAccessAttributes: Get Attr for: " + dst.ItemID.Value);
            on.selectDomain_specific(dst);

            vareq.selectName(on);

            csrreq.selectGetVariableAccessAttributes(vareq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendGetVariableAccessAttributes: Encoding Error!");
                iecs.logger.LogError("mms.SendGetVariableAccessAttributes: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendGetNamedVariableListAttributes(Iec61850State iecs)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNamedVariableListAttributes_Request nareq = new GetNamedVariableListAttributes_Request();
            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

            NodeBase n = iecs.DataModel.datasets.GetActualChildNode();
            if (n == null)
            {
                iecs.logger.LogDebug("mms.SendGetNamedVariableListAttributes: No lists defined!");
                return -2;
            }

            dst.DomainID = new Identifier(n.Name);
            dst.ItemID = new Identifier(iecs.DataModel.datasets.GetActualChildNode().GetActualChildNode().Name);         // List name e.g. MMXU0$MX

            iecs.logger.LogDebug("GetNamedVariableListAttributes: Get Attr for: " + dst.ItemID.Value);
            on.selectDomain_specific(dst);

            nareq.Value = on;

            csrreq.selectGetNamedVariableListAttributes(nareq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendGetNamedVariableListAttributes: Encoding Error!");
                iecs.logger.LogError("mms.SendGetNamedVariableListAttributes: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);



            return 0;
        }

        internal int SendRead(Iec61850State iecs, WriteQueueElement el, Task responseTask = null, IResponse response = null)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Read_Request rreq = new Read_Request();

            List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();

            foreach (NodeBase b in el.Data)
            {
                VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();

                ObjectName on = new ObjectName();
                ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

                vas.VariableSpecification = new VariableSpecification();
                vas.VariableSpecification.selectName(on);

                dst.DomainID = new Identifier(b.CommAddress.Domain);
                dst.ItemID = new Identifier(b.CommAddress.Variable);

                iecs.logger.LogDebug("SendRead: Reading: " + dst.ItemID.Value);
                on.selectDomain_specific(dst);

                vasl.Add(vas);
            }

            //iecs.lastOperationData = el.Data;

            rreq.VariableAccessSpecification = new VariableAccessSpecification();
            rreq.VariableAccessSpecification.selectListOfVariable(vasl);
            rreq.SpecificationWithResult = true;

            csrreq.selectRead(rreq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendRead: Encoding Error!");
                iecs.logger.LogError("mms.SendRead: Encoding Error!");
             //   Console.WriteLine("error in sendread: encoding error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendReadVL(Iec61850State iecs, WriteQueueElement el, Task responseTask = null, IResponse response = null)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Read_Request rreq = new Read_Request();

            NodeBase b = el.Data[0];    // Must be NodeVL

            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

            dst.DomainID = new Identifier(b.CommAddress.Domain);
            dst.ItemID = new Identifier(b.CommAddress.Variable);
            iecs.logger.LogDebug("SendRead: Reading with NVL: " + dst.ItemID.Value);
            iecs.logger.LogInfo("SendRead: Reading with NVL: " + dst.ItemID.Value);
            on.selectDomain_specific(dst);

            rreq.VariableAccessSpecification = new VariableAccessSpecification();
            rreq.VariableAccessSpecification.selectVariableListName(on);
            rreq.SpecificationWithResult = true;

            csrreq.selectRead(rreq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendRead: Encoding Error!");
                iecs.logger.LogError("mms.SendRead: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendWrite(Iec61850State iecs, WriteQueueElement el, MmsValue[] mmsV, Task responseTask = null, WriteResponse response = null)
        {
            try
            {
                MMSpdu mymmspdu = new MMSpdu();
                iecs.msMMSout = new MemoryStream();

                Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
                ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
                Write_Request wreq = new Write_Request();

                List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();
                List<Data> datl = new List<Data>();

                for (int i = 0; i != el.Data.Length; ++i)/* NodeData d in el.Data)*/
                {
                    var d = (NodeData)el.Data[i];
                    var sendMmsValue = el.MmsValue[i];
                    if (d != null)
                    {
                        VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();
                        Data dat = new Data();

                        ObjectName on = new ObjectName();
                        ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();
                        dst.DomainID = new Identifier(el.Address.Domain);
                        dst.ItemID = new Identifier(el.Address.Variable + "$" + d.Name);
                        on.selectDomain_specific(dst);

                        vas.VariableSpecification = new VariableSpecification();
                        vas.VariableSpecification.selectName(on);

                        vasl.Add(vas);

                        switch (sendMmsValue.MmsType)
                        {
                            case MmsTypeEnum.BOOLEAN:
                                dat.selectBoolean((bool)sendMmsValue.Value);
                                break;
                            case MmsTypeEnum.VISIBLE_STRING:
                                dat.selectVisible_string((string)sendMmsValue.Value);
                                break;
                            case MmsTypeEnum.OCTET_STRING:
                                dat.selectOctet_string((byte[])sendMmsValue.Value);
                                break;
                            case MmsTypeEnum.UTC_TIME:
                                UtcTime val = new UtcTime((byte[])sendMmsValue.Value);
                                dat.selectUtc_time(val);
                                break;
                            case MmsTypeEnum.BIT_STRING:
                                {
                               //     dat.selectBit_string(new BitString((byte[])sendMmsValue.Value));
                                    //  dat.selectBit_string(new BitString((byte[])sendMmsValue.Value, MmsDecoder.GetPadding((byte[])sendMmsValue.Value)));
                                    if (d.Name.EndsWith("TrgOps"))
                                    {
                                        dat.selectBit_string(new BitString((byte[])sendMmsValue.Value, 2/*, (int)d.DataParam*/));
                                    }
                                    else if (d.Name.EndsWith("OptFlds"))
                                    {
                                        dat.selectBit_string(new BitString((byte[])sendMmsValue.Value, 6/*, (int)d.DataParam*/));
                                    }
                                    else
                                    {
                                        dat.selectBit_string(new BitString((byte[])sendMmsValue.Value, (int)d.DataParam));
                                    }
                                    break;
                                }
                            case MmsTypeEnum.UNSIGNED:
                                dat.selectUnsigned(Convert.ToInt64(sendMmsValue.Value));
                                break;
                            case MmsTypeEnum.INTEGER:
                                dat.selectInteger((long)sendMmsValue.Value);
                                break;
                            case MmsTypeEnum.FLOATING_POINT:
                                byte[] byteval;
                                byte[] tmp;
                                if (sendMmsValue.Value is float)
                                {
                                    byteval = new byte[5];
                                    tmp = BitConverter.GetBytes((float)sendMmsValue.Value);
                                    byteval[4] = tmp[0];
                                    byteval[3] = tmp[1];
                                    byteval[2] = tmp[2];
                                    byteval[1] = tmp[3];
                                    byteval[0] = 0x08;
                                }
                                else
                                {
                                    byteval = new byte[9];
                                    tmp = BitConverter.GetBytes((float)sendMmsValue.Value);
                                    byteval[8] = tmp[0];
                                    byteval[7] = tmp[1];
                                    byteval[6] = tmp[2];
                                    byteval[5] = tmp[3];
                                    byteval[4] = tmp[4];
                                    byteval[3] = tmp[5];
                                    byteval[2] = tmp[6];
                                    byteval[1] = tmp[7];
                                    byteval[0] = 0x08;      // ???????????? TEST
                                }
                                FloatingPoint fpval = new FloatingPoint(byteval);
                                dat.selectFloating_point(fpval);
                                break;
                            default:
                                iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Cannot send unknown datatype!");
                                iecs.logger.LogError("mms.SendWrite: Cannot send unknown datatype!");
                                return 1;
                        }
                        datl.Add(dat);

                        iecs.logger.LogDebug("SendWrite: Writing: " + dst.ItemID.Value);
                    }
                    else
                        iecs.logger.LogWarning("SendWrite: Null in data for write for: " + el.Address.Variable);
                }
                wreq.VariableAccessSpecification = new VariableAccessSpecification();
                wreq.VariableAccessSpecification.selectListOfVariable(vasl);
                wreq.ListOfData = datl;

                csrreq.selectWrite(wreq);

                if (responseTask != null)
                {
                    waitingMmsPdu.Add(InvokeID, (responseTask, response));
                }

                crreq.InvokeID = new Unsigned32(InvokeID++);

                crreq.Service = csrreq;

                mymmspdu.selectConfirmed_RequestPDU(crreq);

                encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

                if (iecs.msMMSout.Length == 0)
                {
                    iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Encoding Error!");
                    iecs.logger.LogError("mms.SendWrite: Encoding Error!");
                    return -1;
                }

                this.Send(iecs, mymmspdu, InvokeID, el.Data);



                return 0;
            }
            catch
            {
                if (responseTask != null)
                {
                    (response as WriteResponse).TypeOfErrors = new List<DataAccessErrorEnum>
                    {
                        DataAccessErrorEnum.typeInconsistent
                    };
                    responseTask?.Start();
                }

                return -1;
            }
        }

        internal int SendWrite(Iec61850State iecs, string name, Task responseTask, WriteResponse response)
        {
            //try
            //{
            //    MMSpdu mymmspdu = new MMSpdu();
            //    iecs.msMMSout = new MemoryStream();

            //    Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            //    ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            //    Write_Request wreq = new Write_Request();

            //    List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();
            //    List<Data> datl = new List<Data>();

            //    foreach (NodeData d in el.Data)
            //    {
            //        if (d != null)
            //        {
            //            VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();
            //            Data dat = new Data();

            //            ObjectName on = new ObjectName();
            //            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();
            //            dst.DomainID = new Identifier(el.Address.Domain);
            //            dst.ItemID = new Identifier(el.Address.Variable + "$" + d.Name);
            //            on.selectDomain_specific(dst);

            //            vas.VariableSpecification = new VariableSpecification();
            //            vas.VariableSpecification.selectName(on);

            //            vasl.Add(vas);

            //            switch (d.DataType)
            //            {
            //                case MmsTypeEnum.BOOLEAN:
            //                    dat.selectBoolean((bool)d.DataValue);
            //                    break;
            //                case MmsTypeEnum.VISIBLE_STRING:
            //                    dat.selectVisible_string((string)d.DataValue);
            //                    break;
            //                case MmsTypeEnum.OCTET_STRING:
            //                    dat.selectOctet_string((byte[])d.DataValue);
            //                    break;
            //                case MmsTypeEnum.UTC_TIME:
            //                    UtcTime val = new UtcTime((byte[])d.DataValue);
            //                    dat.selectUtc_time(val);
            //                    break;
            //                case MmsTypeEnum.BIT_STRING:
            //                    dat.selectBit_string(new BitString((byte[])d.DataValue, (int)d.DataParam));
            //                    break;
            //                case MmsTypeEnum.UNSIGNED:
            //                    dat.selectUnsigned((long)d.DataValue);
            //                    break;
            //                case MmsTypeEnum.INTEGER:
            //                    dat.selectInteger((long)d.DataValue);
            //                    break;
            //                case MmsTypeEnum.FLOATING_POINT:
            //                    byte[] byteval;
            //                    byte[] tmp;
            //                    if (d.DataValue is float)
            //                    {
            //                        byteval = new byte[5];
            //                        tmp = BitConverter.GetBytes((float)d.DataValue);
            //                        byteval[4] = tmp[0];
            //                        byteval[3] = tmp[1];
            //                        byteval[2] = tmp[2];
            //                        byteval[1] = tmp[3];
            //                        byteval[0] = 0x08;
            //                    }
            //                    else
            //                    {
            //                        byteval = new byte[9];
            //                        tmp = BitConverter.GetBytes((float)d.DataValue);
            //                        byteval[8] = tmp[0];
            //                        byteval[7] = tmp[1];
            //                        byteval[6] = tmp[2];
            //                        byteval[5] = tmp[3];
            //                        byteval[4] = tmp[4];
            //                        byteval[3] = tmp[5];
            //                        byteval[2] = tmp[6];
            //                        byteval[1] = tmp[7];
            //                        byteval[0] = 0x08;      // ???????????? TEST
            //                    }
            //                    FloatingPoint fpval = new FloatingPoint(byteval);
            //                    dat.selectFloating_point(fpval);
            //                    break;
            //                default:
            //                    iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Cannot send unknown datatype!");
            //                    iecs.logger.LogError("mms.SendWrite: Cannot send unknown datatype!");
            //                    return 1;
            //            }
            //            datl.Add(dat);

            //            iecs.logger.LogDebug("SendWrite: Writing: " + dst.ItemID.Value);
            //        }
            //        else
            //        {
            //            iecs.sourceLogger?.SendWarning("lib61850net: SendWrite: Null in data for write for: " + el.Address.Variable);
            //            iecs.logger.LogWarning("SendWrite: Null in data for write for: " + el.Address.Variable);
            //        }
            //    }
            //    wreq.VariableAccessSpecification = new VariableAccessSpecification();
            //    wreq.VariableAccessSpecification.selectListOfVariable(vasl);
            //    wreq.ListOfData = datl;

            //    csrreq.selectWrite(wreq);

            //    if (responseTask != null)
            //    {
            //        waitingMmsPdu.Add(InvokeID, (responseTask, response));
            //    }

            //    crreq.InvokeID = new Unsigned32(InvokeID++);

            //    crreq.Service = csrreq;

            //    mymmspdu.selectConfirmed_RequestPDU(crreq);

            //    encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            //    if (iecs.msMMSout.Length == 0)
            //    {
            //        iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Encoding Error!");
            //        iecs.logger.LogError("mms.SendWrite: Encoding Error!");
            //        return -1;
            //    }

            //    this.Send(iecs, mymmspdu, InvokeID, el.Data);



            //    return 0;
            //}
            //catch
            //{
            //    if (responseTask != null)
            //    {
            //        (response as WriteResponse).TypeOfErrors = new List<DataAccessErrorEnum>
            //        {
            //            DataAccessErrorEnum.typeInconsistent
            //        };
            //        responseTask?.Start();
            //    }

            return -1;
            //}
        }

        internal int SendWrite(Iec61850State iecs, WriteQueueElement el, Task responseTask = null, WriteResponse response = null)
        {
            try
            {
                MMSpdu mymmspdu = new MMSpdu();
                iecs.msMMSout = new MemoryStream();

                Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
                ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
                Write_Request wreq = new Write_Request();

                List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();
                List<Data> datl = new List<Data>();

                foreach (NodeData d in el.Data)
                {
                    if (d != null)
                    {
                        VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();
                        Data dat = new Data();

                        ObjectName on = new ObjectName();
                        ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();
                        dst.DomainID = new Identifier(el.Address.Domain);
                        dst.ItemID = new Identifier(el.Address.Variable + "$" + d.Name);
                        on.selectDomain_specific(dst);

                        vas.VariableSpecification = new VariableSpecification();
                        vas.VariableSpecification.selectName(on);

                        vasl.Add(vas);

                        switch (d.DataType)
                        {
                            case MmsTypeEnum.BOOLEAN:
                                dat.selectBoolean((bool)d.DataValue);
                                break;
                            case MmsTypeEnum.VISIBLE_STRING:
                                dat.selectVisible_string((string)d.DataValue);
                                break;
                            case MmsTypeEnum.OCTET_STRING:
                                dat.selectOctet_string((byte[])d.DataValue);
                                break;
                            case MmsTypeEnum.UTC_TIME:
                                UtcTime val = new UtcTime((byte[])d.DataValue);
                                dat.selectUtc_time(val);
                                break;
                            case MmsTypeEnum.BIT_STRING:
                                dat.selectBit_string(new BitString((byte[])d.DataValue, (int)d.DataParam));
                                break;
                            case MmsTypeEnum.UNSIGNED:
                                dat.selectUnsigned((long)d.DataValue);
                                break;
                            case MmsTypeEnum.INTEGER:
                                dat.selectInteger((long)d.DataValue);
                                break;
                            case MmsTypeEnum.FLOATING_POINT:
                                byte[] byteval;
                                byte[] tmp;
                                if (d.DataValue is float)
                                {
                                    byteval = new byte[5];
                                    tmp = BitConverter.GetBytes((float)d.DataValue);
                                    byteval[4] = tmp[0];
                                    byteval[3] = tmp[1];
                                    byteval[2] = tmp[2];
                                    byteval[1] = tmp[3];
                                    byteval[0] = 0x08;
                                }
                                else
                                {
                                    byteval = new byte[9];
                                    tmp = BitConverter.GetBytes((float)d.DataValue);
                                    byteval[8] = tmp[0];
                                    byteval[7] = tmp[1];
                                    byteval[6] = tmp[2];
                                    byteval[5] = tmp[3];
                                    byteval[4] = tmp[4];
                                    byteval[3] = tmp[5];
                                    byteval[2] = tmp[6];
                                    byteval[1] = tmp[7];
                                    byteval[0] = 0x08;      // ???????????? TEST
                                }
                                FloatingPoint fpval = new FloatingPoint(byteval);
                                dat.selectFloating_point(fpval);
                                break;
                            default:
                                iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Cannot send unknown datatype!");
                                iecs.logger.LogError("mms.SendWrite: Cannot send unknown datatype!");
                                return 1;
                        }
                        datl.Add(dat);

                        iecs.logger.LogDebug("SendWrite: Writing: " + dst.ItemID.Value);
                    }
                    else
                    {
                        iecs.sourceLogger?.SendWarning("lib61850net: SendWrite: Null in data for write for: " + el.Address.Variable);
                        iecs.logger.LogWarning("SendWrite: Null in data for write for: " + el.Address.Variable);
                    }
                }
                wreq.VariableAccessSpecification = new VariableAccessSpecification();
                wreq.VariableAccessSpecification.selectListOfVariable(vasl);
                wreq.ListOfData = datl;

                csrreq.selectWrite(wreq);

                if (responseTask != null)
                {
                    waitingMmsPdu.Add(InvokeID, (responseTask, response));
                }

                crreq.InvokeID = new Unsigned32(InvokeID++);

                crreq.Service = csrreq;

                mymmspdu.selectConfirmed_RequestPDU(crreq);

                encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

                if (iecs.msMMSout.Length == 0)
                {
                    iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Encoding Error!");
                    iecs.logger.LogError("mms.SendWrite: Encoding Error!");
                    return -1;
                }

                this.Send(iecs, mymmspdu, InvokeID, el.Data);



                return 0;
            }
            catch
            {
                if (responseTask != null)
                {
                    (response as WriteResponse).TypeOfErrors = new List<DataAccessErrorEnum>
                    {
                        DataAccessErrorEnum.typeInconsistent
                    };
                    responseTask?.Start();
                }

                return -1;
            }
        }

        internal int SendWriteAsStructure(Iec61850State iecs, WriteQueueElement el, Task responseTask = null, WriteResponse response = null)
        {
            try
            {
                MMSpdu mymmspdu = new MMSpdu();
                iecs.msMMSout = new MemoryStream();

                Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
                ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
                Write_Request wreq = new Write_Request();

                List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();
                List<Data> datList_Seq = new List<Data>();
                List<Data> datList_Struct = new List<Data>();

                VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();
                Data dat_Seq = new Data();
                ObjectName on = new ObjectName();
                ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();
                dst.DomainID = new Identifier(el.Address.Domain);
                dst.ItemID = new Identifier(el.Address.Variable);   // until Oper
                on.selectDomain_specific(dst);
                vas.VariableSpecification = new VariableSpecification();
                vas.VariableSpecification.selectName(on);
                vasl.Add(vas);

                MakeStruct(iecs, el.Data, datList_Struct);
                iecs.logger.LogDebug("SendWrite: Writing Command Structure: " + dst.ItemID.Value);

                dat_Seq.selectStructure(datList_Struct);
                datList_Seq.Add(dat_Seq);

                wreq.VariableAccessSpecification = new VariableAccessSpecification();
                wreq.VariableAccessSpecification.selectListOfVariable(vasl);
                wreq.ListOfData = datList_Seq;

                csrreq.selectWrite(wreq);

                if (responseTask != null)
                {
                    waitingMmsPdu.Add(InvokeID, (responseTask, response));
                }

                crreq.InvokeID = new Unsigned32(InvokeID++);

                crreq.Service = csrreq;

                mymmspdu.selectConfirmed_RequestPDU(crreq);

                encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

                if (iecs.msMMSout.Length == 0)
                {
                    iecs.sourceLogger?.SendError("lib61850net: mms.SendWriteAsStructure: Encoding Error!");
                    iecs.logger.LogError("mms.SendWriteAsStructure: Encoding Error!");
                    return -1;
                }

                this.Send(iecs, mymmspdu, InvokeID, el.Data);



                return 0;
            }
            catch
            {
                if (responseTask != null)
                {
                    (response as WriteResponse).TypeOfErrors = new List<DataAccessErrorEnum>
                    {
                        DataAccessErrorEnum.typeInconsistent
                    };
                    responseTask?.Start();
                }

                return -1;
            }
        }

        private static void MakeStruct(Iec61850State iecs, NodeBase[] data, List<Data> datList_Struct)
        {
            foreach (NodeData d in data)
            {
                Data dat_Struct = new Data();

                switch (d.DataType)
                {
                    case MmsTypeEnum.BOOLEAN:
                        dat_Struct.selectBoolean((bool)d.DataValue);
                        break;
                    case MmsTypeEnum.VISIBLE_STRING:
                        dat_Struct.selectVisible_string((string)d.DataValue);
                        break;
                    case MmsTypeEnum.OCTET_STRING:
                        dat_Struct.selectOctet_string((byte[])d.DataValue);
                        break;
                    case MmsTypeEnum.UTC_TIME:
                        UtcTime val = new UtcTime((byte[])d.DataValue);
                        dat_Struct.selectUtc_time(val);
                        break;
                    case MmsTypeEnum.BIT_STRING:
                        dat_Struct.selectBit_string(new BitString((byte[])d.DataValue, (int)d.DataParam));
                        break;
                    case MmsTypeEnum.UNSIGNED:
                        dat_Struct.selectUnsigned((long)d.DataValue);
                        break;
                    case MmsTypeEnum.INTEGER:
                        dat_Struct.selectInteger((long)d.DataValue);
                        break;
                    case MmsTypeEnum.STRUCTURE:
                        List<Data> datList_Struct2 = new List<Data>();
                        MakeStruct(iecs, d.GetChildNodes(), datList_Struct2);          // Recursive call
                        dat_Struct.selectStructure(datList_Struct2);
                        break;
                    default:
                        iecs.sourceLogger?.SendError("lib61850net: mms.SendWrite: Cannot send unknown datatype!");
                        iecs.logger.LogError("mms.SendWrite: Cannot send unknown datatype!");
                        //return 1;
                        break;
                }
                datList_Struct.Add(dat_Struct);

            }
        }

        internal int SendDefineNVL(Iec61850State iecs, WriteQueueElement el)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            DefineNamedVariableList_Request nvlreq = new DefineNamedVariableList_Request();
            List<DefineNamedVariableList_Request.ListOfVariableSequenceType> dnvl = new List<DefineNamedVariableList_Request.ListOfVariableSequenceType>();
            DefineNamedVariableList_Request.ListOfVariableSequenceType var;

            foreach (NodeBase d in el.Data)
            {
                var = new DefineNamedVariableList_Request.ListOfVariableSequenceType();
                var.VariableSpecification = new VariableSpecification();
                var.VariableSpecification.selectName(new ObjectName());
                var.VariableSpecification.Name.selectDomain_specific(new ObjectName.Domain_specificSequenceType());
                var.VariableSpecification.Name.Domain_specific.DomainID = new Identifier(d.CommAddress.Domain);
                var.VariableSpecification.Name.Domain_specific.ItemID = new Identifier(d.CommAddress.Variable);
                dnvl.Add(var);
            }

            NodeBase[] nvl = new NodeBase[1];
          //  nvl[0] = el.Address.owner;
            //iecs.lastOperationData = nvl;

            nvlreq.ListOfVariable = dnvl;
            nvlreq.VariableListName = new ObjectName();
            nvlreq.VariableListName.selectDomain_specific(new ObjectName.Domain_specificSequenceType());
            nvlreq.VariableListName.Domain_specific.DomainID = new Identifier(el.Address.Domain);
            nvlreq.VariableListName.Domain_specific.ItemID = new Identifier(el.Address.Variable);

            csrreq.selectDefineNamedVariableList(nvlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendDefineNVL: Encoding Error!");
                iecs.logger.LogError("mms.SendDefineNVL: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, nvl);



            return 0;
        }

        internal int SendDeleteNVL(Iec61850State iecs, WriteQueueElement el)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            DeleteNamedVariableList_Request dnvlreq = new DeleteNamedVariableList_Request();
            List<ObjectName> onl = new List<ObjectName>();
            ObjectName on;

            foreach (NodeBase d in el.Data)
            {
                on = new ObjectName();
                on.selectDomain_specific(new ObjectName.Domain_specificSequenceType());
                on.Domain_specific.DomainID = new Identifier(d.CommAddress.Domain);
                on.Domain_specific.ItemID = new Identifier(d.CommAddress.Variable);
                onl.Add(on);
            }

            dnvlreq.ListOfVariableListName = onl;
            //dnvlreq.DomainName = new Identifier(el.Address.Domain);
            dnvlreq.ScopeOfDelete = DeleteNamedVariableList_Request.scopeOfDelete_specific;

            csrreq.selectDeleteNamedVariableList(dnvlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendDeleteNVL: Encoding Error!");
                iecs.logger.LogError("mms.SendDeleteNVL: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendFileDirectory(Iec61850State iecs, WriteQueueElement el, Task responseTask = null, FileDirectoryResponse response = null)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileDirectory_Request filedreq = new FileDirectory_Request();
            FileName filename = new FileName();
            FileName conafter = new FileName();

            filename.initValue();
            if (el.Data[0] is NodeFile)
                filename.Add((el.Data[0] as NodeFile).FullName);
            else
                filename.Add(el.Address.Variable);
            filedreq.FileSpecification = filename;
            if (iecs.continueAfterFileDirectory != null && iecs.continueAfterFileDirectory.Value.Count == 1)
            {
                conafter.initValue();
                string[] name = new string[1];
                iecs.continueAfterFileDirectory.Value.CopyTo(name, 0);
                conafter.Add(name[0]);
                filedreq.ContinueAfter = conafter;
            }

            iecs.lastFileOperationData = el.Data;

            if (el.Data == null || el.Data[0] == null)
            {
                int a = "".Length;
            }

            csrreq.selectFileDirectory(filedreq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendFileDirectory: Encoding Error!");
                iecs.logger.LogError("mms.SendFileDirectory: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendFileOpen(Iec61850State iecs, WriteQueueElement el, Task responseTask = null, FileResponse response = null)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileOpen_Request fileoreq = new FileOpen_Request();
            FileName filename = new FileName();

            filename.initValue();
            if (el.Data[0] is NodeFile)
            {
                if ((el.Data[0] as NodeFile).FullName.EndsWith("/"))
                {
                    (el.Data[0] as NodeFile).FullName = (el.Data[0] as NodeFile).FullName.Remove((el.Data[0] as NodeFile).FullName.Length - 1);
                }
                filename.Add((el.Data[0] as NodeFile).FullName);
            }
            else
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendFileOpen: Request not a file!");
                iecs.logger.LogError("mms.SendFileOpen: Request not a file!");
                return -1;
            }
            fileoreq.FileName = filename;
            fileoreq.InitialPosition = new Unsigned32(0);

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileOpen(fileoreq);

            if (responseTask != null)
            {
                waitingMmsPdu.Add(InvokeID, (responseTask, response));
                saveInvokeIdUntilFileIsRead = InvokeID;
            }

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendFileOpen: Encoding Error!");
                iecs.logger.LogError("mms.SendFileOpen: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);

            return 0;
        }

        internal int SendFileRead(Iec61850State iecs, WriteQueueElement el)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileRead_Request filerreq = new FileRead_Request();
            if (el.Data[0] is NodeFile)
                filerreq.Value = new Integer32((el.Data[0] as NodeFile).frsmId);
            else
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendReadFile: Request not a file!");
                iecs.logger.LogError("mms.SendReadFile: Request not a file!");
                return -1;
            }

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileRead(filerreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendFileRead: Encoding Error!");
                iecs.logger.LogError("mms.SendFileRead: Encoding Error!");
                return -1;
            }



            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendFileDelete(Iec61850State iecs, WriteQueueElement el)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileDelete_Request filedreq = new FileDelete_Request();
            FileName filename = new FileName();

            filename.initValue();
            if (el.Data[0] is NodeFile)
                filename.Add((el.Data[0] as NodeFile).FullName);
            else
            {
                iecs.logger.LogError("mms.SendDeleteFile: Request not a file!");
                return -1;
            }
            filedreq.Value = filename;

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileDelete(filedreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendDeleteFile: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendFileClose(Iec61850State iecs, WriteQueueElement el)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileClose_Request filecreq = new FileClose_Request();

            if (el.Data[0] is NodeFile)
                filecreq.Value = new Integer32((el.Data[0] as NodeFile).frsmId);
            else
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendCloseFile: Request not a file!");
                iecs.logger.LogError("mms.SendCloseFile: Request not a file!");
                return -1;
            }

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileClose(filecreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendCloseFile: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);



            return 0;
        }

        internal int SendInitiate(Iec61850State iecs)
        {


            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Initiate_RequestPDU ireq = new Initiate_RequestPDU();
            Initiate_RequestPDU.InitRequestDetailSequenceType idet = new Initiate_RequestPDU.InitRequestDetailSequenceType();

            idet.ProposedVersionNumber = new Integer16(1);
            byte[] ppc = { 0xf1, 0x00 };
            idet.ProposedParameterCBB = new ParameterSupportOptions(new BitString(ppc, 5));

            byte[] ssc = { 0xee, 0x1c, 0x00, 0x00, 0x04, 0x08, 0x00, 0x00, 0x79, 0xef, 0x18 };
            idet.ServicesSupportedCalling = new MMS_ASN1_Model.ServiceSupportOptions(new BitString(ssc, 3));

            ireq.InitRequestDetail = idet;

            ireq.LocalDetailCalling = new Integer32(65000);
            ireq.ProposedMaxServOutstandingCalling = new Integer16(10);
            ireq.ProposedMaxServOutstandingCalled = new Integer16(10);
            ireq.ProposedDataStructureNestingLevel = new Integer8(5);

            mymmspdu.selectInitiate_RequestPDU(ireq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.sourceLogger?.SendError("lib61850net: mms.SendInitiate: Encoding Error!");
                iecs.logger.LogError("mms.SendInitiate: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, 0, null);



            return 0;
        }

        private void Send(Iec61850State iecs, MMSpdu pdu, int InvokeIdInc, NodeBase[] OperationData)
        {
            //if (iecs.CaptureDb.CaptureActive)
            //{
            //    MMSCapture cap;
            //    iecs.msMMSout.Seek(0, SeekOrigin.Begin);
            //    iecs.msMMSout.Read(iecs.sendBuffer, 0, (int)iecs.msMMSout.Length);
            //    cap = new MMSCapture(iecs.sendBuffer, 0, iecs.msMMSout.Length, MMSCapture.CaptureDirection.Out);
            //    cap.MMSPdu = pdu;
            //    iecs.CaptureDb.AddPacket(cap);
            //}
            insertCall(iecs, InvokeIdInc, OperationData);
            iecs.iso.Send(iecs);
        }


        private List<(NodeBase, NodeBase)> listOfRCB = new List<(NodeBase, NodeBase)>();
        
        /// <summary>
        /// Здесь хранятся узлы с функциональной связью RP или BR, которые ожидают очереди на получение спецификации при построении модели.
        /// </summary>
        private ConcurrentQueue<NodeFC> queueOfReportsFC = new ConcurrentQueue<NodeFC>();

        private void TestAdd(Iec61850State iecs, string[] addr, byte deep, NodeBase actNode)
        {
            switch (deep)
            {
                case 0:
                    {
                        return;
                    }
                case 1:
                    {
                        var curfc = actNode.AddChildNode(new NodeFC(addr[deep]));
                        if (addr.Length < 3)
                        {
                            return;
                        }
                        TestAdd(iecs, addr, 2, curfc);
                        break;
                    }
                case 2:
                    {
                        // здесь мы обрабатываем ситуацию, когда встречаем параметры отчёта или блоки команд управления,
                        // добавляем их в отдельную очередь, чтобы затем прочитать у них спецификацию
                        // для этого сравниваем имя узла, его логический узел и логическое устройство
                        if (actNode is NodeFC && (actNode.Name == "RP" || actNode.Name == "BR" || actNode.Name == "CO"))
                        {
                            if (queueOfReportsFC.Count(el => el.Name == actNode.Name && el.Parent.Name == actNode.Parent.Name && el.Parent.Parent.Name == actNode.Parent.Parent.Name) == 0)
                            {
                                queueOfReportsFC.Enqueue(actNode as NodeFC); 
                            }
                            // Having RCB
                            //NodeBase nrpied;
                            //if (actNode.Name == "RP") nrpied = iecs.DataModel.urcbs.AddChildNode(new NodeLD(iecs.DataModel.ied.GetActualChildNode().Name));
                            //else nrpied = iecs.DataModel.brcbs.AddChildNode(new NodeLD(iecs.DataModel.ied.GetActualChildNode().Name));
                            //NodeBase nrp = new NodeRCB(curdo.CommAddress.Variable, curdo.Name);
                            //nrpied.AddChildNode(nrp);
                            //listOfRCB.Add((curdo, nrp));
                            //foreach (NodeBase nb in curdo.GetChildNodes())
                            //{
                            //    nrp.LinkChildNodeByAddress(nb);
                            //}
                            return;
                        }
                        var curdo = actNode.AddChildNode(new NodeDO(addr[deep]));
                        if (addr.Length < 4)
                        {
                            return;
                        }
                        TestAdd(iecs, addr, 3, curdo);
                        break;
                    }
                default:
                    {
                        var curdt = actNode.AddChildNode(new NodeData(addr[deep]));
                        if (addr.Length < deep + 2)
                        {
                            return;
                        }
                        TestAdd(iecs, addr, ++deep, curdt);
                        break;
                    }
            }
        }

        private void TestGlobalAdd(Iec61850State iecs, string addr)
        {
            string[] parts = addr.Split(new char[] { '$' });

            NodeBase curld = iecs.DataModel.ied.GetActualChildNode();

            if (parts.Length < 1)
            {
                return;
            }

            var curln = curld.AddChildNode(new NodeLN(parts[0]));

            if (parts.Length < 2)
            {
                return;
            }

            TestAdd(iecs, parts, 1, curln);
        }

        private NodeBase currentNodeToAdd;

        private void NewTestAdd(string addr)
        {
            if (currentNodeToAdd is NodeLD)
            {
                var newNode = currentNodeToAdd.AddChildNode(new NodeLN(addr));
                currentNodeToAdd = newNode;
            }
            else if (currentNodeToAdd is NodeLN)
            {
                var newNode = currentNodeToAdd.AddChildNode(new NodeFC(addr));
                currentNodeToAdd = newNode;
            }
            else if (currentNodeToAdd is NodeFC)
            {
                if ((currentNodeToAdd.Name == "RP" || currentNodeToAdd.Name == "BR" || currentNodeToAdd.Name == "CO"))
                {
                    if (queueOfReportsFC.Count(el => el.Name == currentNodeToAdd.Name && el.Parent.Name == currentNodeToAdd.Parent.Name && el.Parent.Parent.Name == currentNodeToAdd.Parent.Parent.Name) == 0)
                    {
                        queueOfReportsFC.Enqueue(currentNodeToAdd as NodeFC);
                    }
                    return;
                }
                var newNode = currentNodeToAdd.AddChildNode(new NodeDO(addr));
                currentNodeToAdd = newNode;
            }
            else
            {
                var newNode = currentNodeToAdd.AddChildNode(new NodeData(addr));
                currentNodeToAdd = newNode;
            }
        }

        private void NewTestGlobalAdd(Iec61850State iecs, string addr)
        {
            string[] parts = addr.Split(new char[] { '$' });
            if (parts.Length < 2)
            {
                var curld = iecs.DataModel.ied.GetActualChildNode();
                currentNodeToAdd = curld.AddChildNode(new NodeLN(addr));
                return;
            }

            if (currentNodeToAdd != null)
            {
                if (addr.StartsWith(currentNodeToAdd.CommAddress.Variable))
                {
                    NewTestAdd(parts.Last());
                }
                else
                {
                    NodeBase cursor = currentNodeToAdd;
                    while (cursor.Parent != null)
                    {
                        cursor = cursor.Parent;
                        if (addr.StartsWith(cursor.CommAddress.Variable))
                        {
                            break;
                        }
                    }

                    if (cursor.Parent == null)
                    {
                        currentNodeToAdd = iecs.DataModel.ied.GetActualChildNode();
                    }
                    else
                    {
                        currentNodeToAdd = cursor;
                    }

                    NewTestAdd(parts.Last());
                }
            }
            //NodeBase curld = iecs.DataModel.ied.GetActualChildNode();
            //NodeBase curln;
            //if (!addr.Contains("$"))
            //{
            //    curln = curld.AddChildNode(new NodeLN(addr));
            //}
        }

        private void AddIecAddress(Iec61850State iecs, string addr)
        {
            //NewTestGlobalAdd(iecs, addr);
            TestGlobalAdd(iecs, addr);
            //string[] parts = addr.Split(new char[] { '$' });
            //NodeBase curld = iecs.DataModel.ied.GetActualChildNode();
            //NodeBase curln, curfc; //, curdt;
            //if (parts.Length < 1)
            //{
            //    return;
            //}
            //curln = curld.AddChildNode(new NodeLN(parts[0]));
            //if (parts.Length < 2)
            //{
            //    return;
            //}
            //curfc = curln.AddChildNode(new NodeFC(parts[1]));
            //if (parts.Length < 3)
            //{
            //    return;
            //}
        }

        internal static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        internal static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        internal static void ConvertToUtcTime(DateTime dt, byte[] utc_time)
        {
            int t = (int)Scsm_MMS.ConvertToUnixTimestamp(dt);
            byte[] uib = BitConverter.GetBytes(t);
            utc_time[0] = uib[3];
            utc_time[1] = uib[2];
            utc_time[2] = uib[1];
            utc_time[3] = uib[0];

            UInt32 remainder = (UInt32)dt.Millisecond;
            UInt32 fractionOfSecond = (remainder) * 16777 + ((remainder * 216) / 1000);
            /* encode fraction of second */
            utc_time[4] = (byte)((fractionOfSecond >> 16) & 0xff);
            utc_time[5] = (byte)((fractionOfSecond >> 8) & 0xff);
            utc_time[6] = (byte)(fractionOfSecond & 0xff);
            /* encode time quality */
            utc_time[7] = 0x0a; /* 10 bit sub-second time accuracy */
        }


        internal static DateTime ConvertFromUtcTime(byte[] utc_time, Object dataParam)
        {
            long seconds;
            long millis;

            if (utc_time != null && utc_time.Length == 8)
            {
                seconds = (utc_time[0] << 24) +
                          (utc_time[1] << 16) +
                          (utc_time[2] << 8) +
                          (utc_time[3]);

                millis = 0;
                for (int i = 0; i < 24; i++)
                {
                    if (((utc_time[(i / 8) + 4] << (i % 8)) & 0x80) > 0)
                    {
                        millis += 1000000 / (1 << (i + 1));
                    }
                }
                millis /= 1000;

                DateTime dt = ConvertFromUnixTimestamp(seconds);
                dt = dt.AddMilliseconds(millis);
                dataParam = utc_time[7];
                return dt.ToLocalTime();
            }
            else
            {
                dataParam = (byte)0xff;
                return DateTime.Now;
            }
        }

        internal static DateTime ConvertFromUtcTime2(byte[] utc_time, Object dataParam)
        {
            long seconds;
            long fractionOfSecond;

            if (utc_time != null && utc_time.Length == 8)
            {
                seconds = (utc_time[0] << 24) +
                          (utc_time[1] << 16) +
                          (utc_time[2] << 8) +
                          (utc_time[3]);

                fractionOfSecond = (utc_time[4] << 16);
                fractionOfSecond += (utc_time[5] << 8);
                fractionOfSecond += (utc_time[6]);

                UInt32 millis = (UInt32)(fractionOfSecond / 16777);

                DateTime dt = ConvertFromUnixTimestamp(seconds);
                dt = dt.AddMilliseconds(millis);
                dataParam = utc_time[7];
                return dt.ToLocalTime();
            }
            else
            {
                dataParam = (byte)0xff;
                return DateTime.Now;
            }
        }

        private int insertCall(Iec61850State iecs, int InvokeIdInc, NodeBase[] OperationData)
        {
            if (iecs.OutstandingCalls.Count >= MaxCalls)
            {
                Logger.getLogger().LogWarning("Cannot send a request to client, " + MaxCalls + " calls pending for InvokeId " + (InvokeIdInc - 1).ToString());
                //return -1;
                // Auto Purge
                Logger.getLogger().LogWarning("Auto Purge Activated, deleting all previous operations. Too fast cycle of requests???");
                NodeBase[] ret = null;
                foreach (int id in iecs.OutstandingCalls.Keys)
                {
                    //  iecs.OutstandingCalls.TryRemove(id, out ret);
                    Logger.getLogger().LogWarning("Auto Purging Id=" + id + ", Operation data: " + ((ret == null) ? "null" : (ret[0] == null) ? "null" : ret[0].Name));
                }
            }
            if (!iecs.OutstandingCalls.TryAdd(InvokeIdInc - 1, OperationData))
            {
                return -2;
            }
            return 0;
        }

        private NodeBase[] removeCall(Iec61850State iecs, int InvokeId)
        {
            NodeBase[] ret;
            if (!iecs.OutstandingCalls.TryRemove(InvokeId, out ret)) return null;
            return ret;
        }

        private void checkCalls(Iec61850State iecs, int InvokeId)
        {
            // If we do not believe we have caught all received ids
            NodeBase[] ret;
            foreach (int id in iecs.OutstandingCalls.Keys)
            {
                if (id + MaxCalls < InvokeId)
                    iecs.OutstandingCalls.TryRemove(id, out ret);
            }
        }

    }
}
