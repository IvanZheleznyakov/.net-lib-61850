using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    public class ReportControlBlock
    {
        internal ReportControlBlock()
        {

        }

        internal NodeRCB self;

        internal bool sendRptID = false;
        internal bool sendRptEna = false;
        internal bool sendResv = false;
        internal bool sendDatSet = false;
        internal bool sendOptFlds = false;
        internal bool sendBufTm = false;
        internal bool sendTrgOps = false;
        internal bool sendIntgPd = false;
        internal bool sendGI = false;
        internal bool sendPurgeBuf = false;
        internal bool sendResvTms = false;
        internal bool sendEntryID = false;

        private string objReference;

        public string ObjectReference
        {
            get
            {
                return objReference;
            }
            internal set
            {
                objReference = value;
            }
        }

        public DataAccessErrorEnum TypeOfError { get; internal set; }

        public bool IsBuffered { get { return self.isBuffered; } }
        public string Name { get { return self.IecAddress; } }

        public bool IsRptIDPresent { get { return self.RptID_present; } }
        public bool IsRptEnaPresent { get { return self.RptEna_present; } }
        public bool IsResvPresent { get { return self.Resv_present; } }
        public bool IsDatSetPresent { get { return self.DatSet_present; } }
        public bool IsOptFldsPresent { get { return self.OptFlds_present; } }
        public bool IsBufTmPresent { get { return self.BufTm_present; } }
        public bool IsTrgOpsPresent { get { return self.TrgOps_present; } }
        public bool IsIntgPdPresent { get { return self.IntgPd_present; } }
        public bool IsGIPresent { get { return self.GI_present; } }
        public bool IsPurgeBufPresent { get { return self.PurgeBuf_present; } }
        public bool IsResvTmsPresent { get { return self.ResvTms_present; } }
        public bool IsEntryIDPresent { get { return self.EntryID_present; } }

        DateTime m_tmTimeLastDataSetReports = DateTime.Now;
        public DateTime TimeLastDataSetReports
        {
            set
            {
                m_tmTimeLastDataSetReports = DateTime.Now;
            }
        }

        int m_nLastExchangeTimeOutErrorCount;
        public int LastExchangeTimeOutErrorCount
        {
            get { return m_nLastExchangeTimeOutErrorCount; }
        }

        public void LastExchangeTimeOutErrorCountClear()
        {
            m_nLastExchangeTimeOutErrorCount = 0;
        }

        public void LastExchangeTimeOutErrorCountInc()
        {
            m_nLastExchangeTimeOutErrorCount++;
        }

        uint m_nSavedIntegrityPeriod = 0;

        public bool BadTimeoutInterval
        {
            get
            {
                TimeSpan tmsDelta = DateTime.Now - m_tmTimeLastDataSetReports;
                return tmsDelta.TotalMilliseconds > m_nSavedIntegrityPeriod + 3000;
            }
        }

        public string RptId
        {
            get
            {
                return self.RptID;
            }
            set
            {
                self.RptID = value;
                sendRptID = true;
            }
        }

        public bool RptEna
        {
            get
            {
                return self.RptEna;
            }
            set
            {
                self.RptEna = value;
                sendRptEna = true;
            }
        }

        public bool Resv
        {
            get
            {
                return self.Resv;
            }
            set
            {
                self.Resv = value;
                sendResv = true;
            }
        }

        public string DatSet
        {
            get
            {
                return self.DatSet;
            }
            set
            {
                self.DatSet = value;
                sendDatSet = true;
            }
        }

        public ReportOptionsEnum OptFlds
        {
            get
            {
                return self.OptFlds;
            }
            set
            {
                self.OptFlds = value;
                sendOptFlds = true;
            }
        }

        public uint BufTm
        {
            get
            {
                return self.BufTm;
            }
            set
            {
                self.BufTm = value;
                sendBufTm = true;
            }
        }

        public ReportTriggerOptionsEnum TrgOps
        {
            get
            {
                return self.TrgOps;
            }
            set
            {
                self.TrgOps = value;
                sendTrgOps = true;
            }
        }

        public uint IntgPd
        {
            get
            {
                return self.IntgPd;
            }
            set
            {
                self.IntgPd = value;
                sendIntgPd = true;
            }
        }

        public bool GI
        { 
            get
            {
                return self.GI;
            }
            set
            {
                self.GI = value;
                sendGI = true;
            }
        }

        public bool PurgeBuf
        {
            get
            {
                return self.PurgeBuf;
            }
            set
            {
                self.PurgeBuf = value;
                sendPurgeBuf = true;
            }
        }

        public uint ResvTms
        {
            get
            {
                return self.ResvTms;
            }
            set
            {
                self.ResvTms = value;
                sendResvTms = true;
            }
        }

        public string EntryID
        {
            get
            {
                return self.EntryID;
            }
            set
            {
                self.EntryID = value;
                sendEntryID = true;
            }
        }

        internal void ResetFlags()
        {
            sendRptID = false;
            sendRptEna = false;
            sendResv = false;
            sendDatSet = false;
            sendOptFlds = false;
            sendBufTm = false;
            sendTrgOps = false;
            sendIntgPd = false;
            sendGI = false;
            sendPurgeBuf = false;
            sendResvTms = false;
            sendEntryID = false;
        }

        internal NodeData[] GetWriteArray()
        {
            List<NodeData> nlst = new List<NodeData>();
            // Reservation - Must go first!!!
            NodeBase fcn = self.FindChildNode("Resv");
            if (sendResv && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("ResvTms");
            if (sendResvTms && fcn != null) nlst.Add((NodeData)fcn);
            // Normal members
            fcn = self.FindChildNode("RptID");
            if (sendRptID && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("DatSet");
            if (sendDatSet && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("OptFlds");
            if (sendOptFlds && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("BufTm");
            if (sendBufTm && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("TrgOps");
            if (sendTrgOps && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("IntgPd");
            if (sendIntgPd && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("PurgeBuf");
            if (sendPurgeBuf && fcn != null) nlst.Add((NodeData)fcn);
            fcn = self.FindChildNode("EntryID");
            if (sendEntryID && fcn != null) nlst.Add((NodeData)fcn);
            // Activation - Must go last!!!
            fcn = self.FindChildNode("RptEna");
            if (sendRptEna && fcn != null) nlst.Add((NodeData)fcn);
            // GI - Must go after activation!!!
            fcn = self.FindChildNode("GI");
            if (sendGI && fcn != null) nlst.Add((NodeData)fcn);
            return nlst.ToArray();
        }
    }

}
