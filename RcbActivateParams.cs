﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEDExplorer
{
    public class RcbActivateParams
    {
        internal NodeRCB self;

        public bool sendRptID = false;
        public bool sendRptEna = true;
        public bool sendResv = true;
        public bool sendDatSet = true;
        public bool sendOptFlds = false;
        public bool sendBufTm = false;
        public bool sendTrgOps = true;
        public bool sendIntgPd = false;
        public bool sendGI = true;
        public bool sendPurgeBuf = false;
        public bool sendResvTms = false;
        public bool sendEntryID = false;

        public bool SetRptId(string rptId)
        {
            if (self.FindChildNode("RptID") != null)
            {
                self.RptID = rptId;
                sendRptID = true;
                return true;
            }

            return false;
        }

        public bool SetRptEna(bool rptEna)
        {
            if (self.FindChildNode("RptEna") != null)
            {
                self.RptEna = rptEna;
                sendRptEna = true;
                return true;
            }

            return false;
        }

        public bool SetResv(bool resv)
        {
            if (self.FindChildNode("Resv") != null)
            {
                self.Resv = resv;
                sendResv = true;
                return true;
            }

            return false;
        }

        public bool SetDatSet(string datSet)
        {
            if (self.DatSet_present)
            {
                self.DatSet = datSet;
                sendDatSet = true;
                return true;
            }

            return false;
        }

        public bool SetBufTm(uint bufTm)
        {
            if (self.BufTm_present)
            {
                self.BufTm = bufTm;
                sendBufTm = true;
                return true;
            }

            return false;
        }

        public NodeData[] getWriteArray()
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
