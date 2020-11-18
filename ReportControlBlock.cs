using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEDExplorer
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

        public bool IsBuffered { get { return self.isBuffered; } internal set { IsBuffered = value; } }
        public string Name { get { return self.IecAddress; } internal set { Name = value; } }

        public bool IsRptIDPresent { get { return self.RptID_present; } internal set { IsRptIDPresent = value; } }
        public bool IsRptEnaPresent { get { return self.RptEna_present; } internal set { IsRptEnaPresent = value; } }
        public bool IsResvPresent { get { return self.Resv_present; } internal set { IsResvPresent = value; } }
        public bool IsDatSetPresent { get { return self.DatSet_present; } internal set { IsDatSetPresent = value; } }
        public bool IsOptFldsPresent { get { return self.OptFlds_present; } internal set { IsOptFldsPresent = value; } }
        public bool IsBufTmPresent { get { return self.BufTm_present; } internal set { IsBufTmPresent = value; } }
        public bool IsTrgOpsPresent { get { return self.TrgOps_present; } internal set { IsTrgOpsPresent = value; } }
        public bool IsIntgPdPresent { get { return self.IntgPd_present; } internal set { IsIntgPdPresent = value; } }
        public bool IsGIPresent { get { return self.GI_present; } internal set { IsGIPresent = value; } }
        public bool IsPurgeBufPresent { get { return self.PurgeBuf_present; } internal set { IsPurgeBufPresent = value; } }
        public bool IsResvTmsPresent { get { return self.ResvTms_present; } internal set { IsResvTmsPresent = value; } }
        public bool IsEntryIDPresent { get { return self.EntryID_present; } internal set { IsEntryIDPresent = value; } }

        public void SetRptId(string rptId)
        {
            self.RptID = rptId;
            sendRptID = true;
        }

        public void SetRptEna(bool rptEna)
        {
            self.RptEna = rptEna;
            sendRptEna = true;
        }

        public void SetResv(bool resv)
        {
            self.Resv = resv;
            sendResv = true;
        }

        public void SetDatSet(string datSet)
        {
            self.DatSet = datSet;
            sendDatSet = true;
        }

        public void SetOptFlds(ReportOptionsEnum optFlds)
        {
            self.OptFlds = optFlds;
            sendOptFlds = true;
        }

        public void SetBufTm(uint bufTm)
        {
            self.BufTm = bufTm;
            sendBufTm = true;
        }

        public void SetTrgOps(ReportTriggerOptionsEnum trgOps)
        {
            self.TrgOps = trgOps;
            sendTrgOps = true;
        }

        public void SetIntgPd(uint intgPg)
        {
            self.IntgPd = intgPg;
            sendIntgPd = true;
        }

        public void SetGI(bool gi)
        {
            self.GI = gi;
            sendGI = true;
        }

        public void SetPurgeBuf(bool purgeBuf)
        {
            self.PurgeBuf = purgeBuf;
            sendPurgeBuf = true;
        }

        public void SetResvTms(uint resvtms)
        {
            self.ResvTms = resvtms;
            sendResvTms = true;
        }

        public void SetEntryID(string entryID)
        {
            self.EntryID = entryID;
            sendEntryID = true;
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
