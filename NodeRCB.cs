using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class NodeRCB : NodeBase
    {
        //bool _defined = false;

        public NodeRCB(string Name, string reportName)
            : base(Name)
        {
            Activated = false;
            ReportName = reportName;
        }

        internal string ReportName { get; set; }

        public bool Activated { get; set; }

        public NodeVL dataset { get; set; }

        public bool isBuffered { get { if (isBufLock) return isBufSet; else return PurgeBuf_present; } set { isBufLock = true; isBufSet = value; } }
        bool isBufSet = false;
        bool isBufLock = false;

        public new string IecAddress
        {
            get
            {
                if (_addressLock)
                {
                    return _address;
                }

                _addressLock = true;
                _address = "";
                NodeBase tmpn = this;
                List<string> parts = new List<string>();
                bool iecModel = false;

                do
                {
                    if (!(tmpn is NodeFC))
                        parts.Add(tmpn.Name);
                    tmpn = tmpn.Parent;
                } while (tmpn != null && (!(tmpn is NodeIed) || iecModel));

                for (int i = parts.Count - 1; i >= 0; i--)
                {
                    //if (i == parts.Count - 4)
                    //    continue;
                    _address += parts[i];
                    if (iecModel)
                    {
                        if (i == parts.Count - 2)
                        {
                            if (i != 0)
                                _address += "/";
                        }
                        else if (i != 0 && i != parts.Count - 1)
                            if (parts[i - 1].Contains('/'))
                                _address += "->";
                            else
                                _address += ".";
                    }
                    else
                    {
                        if (i == parts.Count - 1)
                        {
                            if (i != 0)
                                _address += "/";
                        }
                        else if (i != 0)
                            _address += ".";
                    }
                }
                if (isBuffered)
                {
                    _address = _address.Replace("$BR$", ".");
                    return _address;
                }
                else
                {
                    _address = _address.Replace("$RP$", ".");
                    return _address;
                }
            }
            set
            {
                _address = value;
                _addressLock = true;
            }
        }

        NodeData _RptID;
        public string RptID
        {
            get
            {
                if (_RptID == null) _RptID = (NodeData)FindChildNode("RptID");
                if (_RptID != null)
                    return (string)_RptID.DataValue;
                else
                    return "";
            }
            set
            {
                if (_RptID == null) _RptID = (NodeData)FindChildNode("RptID");
                if (_RptID != null)
                    _RptID.DataValue = value;
            }
        }
        public bool RptID_present
        {
            get
            {
                if (_RptID != null) return true;
                _RptID = (NodeData)FindChildNode("RptID");
                if (_RptID != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _RptEna;
        public bool RptEna
        {
            get
            {
                if (_RptEna == null) _RptEna = (NodeData)FindChildNode("RptEna");
                if (_RptEna != null)
                    return (bool)_RptEna.DataValue;
                else
                    return false;
            }
            set
            {
                if (_RptEna == null) _RptEna = (NodeData)FindChildNode("RptEna");
                if (_RptEna != null)
                    _RptEna.DataValue = value;
            }
        }
        public bool RptEna_present
        {
            get
            {
                if (_RptEna != null) return true;
                _RptEna = (NodeData)FindChildNode("RptEna");
                if (_RptEna != null)
                    return true;
                else
                    return false;
            }
        }

        #region UnbufferedReport
        NodeData _Resv;
        public bool Resv
        {
            get
            {
                if (_Resv == null) _Resv = (NodeData)FindChildNode("Resv");
                if (_Resv != null)
                    return (bool)_Resv.DataValue;
                else
                    return false;
            }
            set
            {
                if (_Resv == null) _Resv = (NodeData)FindChildNode("Resv");
                if (_Resv != null)
                    _Resv.DataValue = value;
            }
        }
        public bool Resv_present
        {
            get
            {
                if (_Resv != null) return true;
                _Resv = (NodeData)FindChildNode("Resv");
                if (_Resv != null)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        NodeData _DatSet;
        public string DatSet
        {
            get
            {
                if (_DatSet == null) _DatSet = (NodeData)FindChildNode("DatSet");
                if (_DatSet != null)
                    return (string)_DatSet.DataValue;
                else
                    return "";
            }
            set
            {
                if (_DatSet == null) _DatSet = (NodeData)FindChildNode("DatSet");
                if (_DatSet != null)
                    _DatSet.DataValue = value;
            }
        }
        public bool DatSet_present
        {
            get
            {
                if (_DatSet != null) return true;
                _DatSet = (NodeData)FindChildNode("DatSet");
                if (_DatSet != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _ConfRev;
        public uint ConfRev
        {
            get
            {
                if (_ConfRev == null) _ConfRev = (NodeData)FindChildNode("ConfRev");
                if (_ConfRev != null)
                    return (uint)_ConfRev.DataValue;
                else
                    return 0;
            }
        }
        public bool ConfRev_present
        {
            get
            {
                if (_ConfRev != null) return true;
                _ConfRev = (NodeData)FindChildNode("ConfRev");
                if (_ConfRev != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _OptFlds;
        public ReportOptionsEnum OptFlds
        {
            get
            {
                if (_OptFlds == null) _OptFlds = (NodeData)FindChildNode("OptFlds");
                if (_OptFlds != null)
                {
                    byte[] val = (byte[])_OptFlds.DataValue;
                    ReportOptionsEnum ro = ReportOptionsEnum.NONE;
                    return ro.fromBytes(val);
                }
                else
                    return 0;
            }
            set
            {
                if (_OptFlds == null) _OptFlds = (NodeData)FindChildNode("OptFlds");
                if (_OptFlds != null)
                {
                    _OptFlds.DataValue = value.toBytes();
                }
            }
        }

        public bool OptFlds_present
        {
            get
            {
                if (_OptFlds != null) return true;
                _OptFlds = (NodeData)FindChildNode("OptFlds");
                if (_OptFlds != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _BufTm;
        public uint BufTm
        {
            get
            {
                if (_BufTm == null) _BufTm = (NodeData)FindChildNode("BufTm");
                if (_BufTm != null)
                    return Convert.ToUInt32(_BufTm.DataValue);
                else
                    return 0;
            }
            set
            {
                if (_BufTm == null) _BufTm = (NodeData)FindChildNode("BufTm");
                if (_BufTm != null)
                    _BufTm.DataValue = (long)value;
            }
        }
        public bool BufTm_present
        {
            get
            {
                if (_BufTm != null) return true;
                _BufTm = (NodeData)FindChildNode("BufTm");
                if (_BufTm != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _SqNum;
        public uint SqNum
        {
            get
            {
                if (_SqNum == null) _SqNum = (NodeData)FindChildNode("SqNum");
                if (_SqNum != null)
                    return (uint)_SqNum.DataValue;
                else
                    return 0;
            }
        }
        public bool SqNum_present
        {
            get
            {
                if (_SqNum != null) return true;
                _SqNum = (NodeData)FindChildNode("SqNum");
                if (_SqNum != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _TrgOps;
        public ReportTriggerOptionsEnum TrgOps
        {
            get
            {
                if (_TrgOps == null) _TrgOps = (NodeData)FindChildNode("TrgOps");
                if (_TrgOps != null)
                {
                    byte[] val = (byte[])_TrgOps.DataValue;
                    ReportTriggerOptionsEnum to = ReportTriggerOptionsEnum.NONE;
                    return to.fromBytes(val);
                }
                else
                    return 0;
            }
            set
            {
                if (_TrgOps == null) _TrgOps = (NodeData)FindChildNode("TrgOps");
                if (_TrgOps != null)
                {
                    _TrgOps.DataValue = value.toBytes();
                }
            }
        }

        public bool TrgOps_present
        {
            get
            {
                if (_TrgOps != null) return true;
                _TrgOps = (NodeData)FindChildNode("TrgOps");
                if (_TrgOps != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _IntgPd;
        public uint IntgPd
        {
            get
            {
                if (_IntgPd == null) _IntgPd = (NodeData)FindChildNode("IntgPd");
                if (_IntgPd != null)
                    return Convert.ToUInt32(_IntgPd.DataValue);
                else
                    return 0;
            }
            set
            {
                if (_IntgPd == null) _IntgPd = (NodeData)FindChildNode("IntgPd");
                if (_IntgPd != null)
                    _IntgPd.DataValue = (long)value;
            }
        }
        public bool IntgPd_present
        {
            get
            {
                if (_IntgPd != null) return true;
                _IntgPd = (NodeData)FindChildNode("IntgPd");
                if (_IntgPd != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _GI;
        public bool GI
        {
            get
            {
                if (_GI == null) _GI = (NodeData)FindChildNode("GI");
                if (_GI != null)
                    return (bool)_GI.DataValue;
                else
                    return false;
            }
            set
            {
                if (_GI == null) _GI = (NodeData)FindChildNode("GI");
                if (_GI != null)
                    _GI.DataValue = value;
            }
        }
        public bool GI_present
        {
            get
            {
                if (_GI != null) return true;
                _GI = (NodeData)FindChildNode("GI");
                if (_GI != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _Owner;
        public string Owner
        {
            get
            {
                if (_Owner == null) _Owner = (NodeData)FindChildNode("Owner");
                if (_Owner != null)
                    return (string)_Owner.DataValue;
                else
                    return "";
            }
        }
        public bool Owner_present
        {
            get
            {
                if (_Owner != null) return true;
                _Owner = (NodeData)FindChildNode("Owner");
                if (_Owner != null)
                    return true;
                else
                    return false;
            }
        }

        #region BufferedReport

        NodeData _PurgeBuf;
        public bool PurgeBuf
        {
            get
            {
                if (_PurgeBuf == null) _PurgeBuf = (NodeData)FindChildNode("PurgeBuf");
                if (_PurgeBuf != null)
                    return (bool)_PurgeBuf.DataValue;
                else
                    return false;
            }
            set
            {
                if (_PurgeBuf == null) _PurgeBuf = (NodeData)FindChildNode("PurgeBuf");
                if (_PurgeBuf != null)
                    _PurgeBuf.DataValue = value;
            }
        }
        public bool PurgeBuf_present
        {
            get
            {
                if (_PurgeBuf != null) return true;
                _PurgeBuf = (NodeData)FindChildNode("PurgeBuf");
                if (_PurgeBuf != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _EntryID;
        public string EntryID
        {
            get
            {
                if (_EntryID == null) _EntryID = (NodeData)FindChildNode("EntryID");
                if (_EntryID != null)
                    return (string)_EntryID.StringValue;
                else
                    return "";
            }
            set
            {
                if (_EntryID == null) _EntryID = (NodeData)FindChildNode("EntryID");
                if (_EntryID != null)
                    _EntryID.StringValue = value;
            }
        }
        public bool EntryID_present
        {
            get
            {
                if (_EntryID != null) return true;
                _EntryID = (NodeData)FindChildNode("EntryID");
                if (_EntryID != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _TimeOfEntry;
        public string TimeOfEntry
        {
            get
            {
                if (_TimeOfEntry == null) _TimeOfEntry = (NodeData)FindChildNode("TimeOfEntry");
                if (_TimeOfEntry != null)
                    return (string)_TimeOfEntry.StringValue;
                else
                    return "";
            }
        }
        public bool TimeOfEntry_present
        {
            get
            {
                if (_TimeOfEntry != null) return true;
                _TimeOfEntry = (NodeData)FindChildNode("TimeOfEntry");
                if (_TimeOfEntry != null)
                    return true;
                else
                    return false;
            }
        }

        NodeData _ResvTms;
        public uint ResvTms
        {
            get
            {
                if (_ResvTms == null) _ResvTms = (NodeData)FindChildNode("ResvTms");
                if (_ResvTms != null)
                    return Convert.ToUInt32(_ResvTms.DataValue);
                else
                    return 0;
            }
            set
            {
                if (_ResvTms == null) _ResvTms = (NodeData)FindChildNode("ResvTms");
                if (_ResvTms != null)
                    _ResvTms.DataValue = (long)value;
            }
        }
        public bool ResvTms_present
        {
            get
            {
                if (_ResvTms != null) return true;
                _ResvTms = (NodeData)FindChildNode("ResvTms");
                if (_ResvTms != null)
                    return true;
                else
                    return false;
            }
        }

        public bool Segmentation_present
        {
            get
            {
                return (NodeData)FindChildNode("Segmentation") != null;
            }
        }
        #endregion
    }

    public static class OptionsEnumExtensions
    {
        public static ReportOptionsEnum fromBytes(this ReportOptionsEnum res, byte[] value)
        {
            res = ReportOptionsEnum.NONE;
            if (value == null || value.Length < 1) return res;
            if ((value[0] & Scsm_MMS.OptFldsSeqNum) == Scsm_MMS.OptFldsSeqNum) res |= ReportOptionsEnum.SEQ_NUM;
            if ((value[0] & Scsm_MMS.OptFldsTimeOfEntry) == Scsm_MMS.OptFldsTimeOfEntry) res |= ReportOptionsEnum.TIME_STAMP;
            if ((value[0] & Scsm_MMS.OptFldsReasonCode) == Scsm_MMS.OptFldsReasonCode) res |= ReportOptionsEnum.REASON_FOR_INCLUSION;
            if ((value[0] & Scsm_MMS.OptFldsDataSet) == Scsm_MMS.OptFldsDataSet) res |= ReportOptionsEnum.DATA_SET;
            if ((value[0] & Scsm_MMS.OptFldsDataReference) == Scsm_MMS.OptFldsDataReference) res |= ReportOptionsEnum.DATA_REFERENCE;
            if ((value[0] & Scsm_MMS.OptFldsOvfl) == Scsm_MMS.OptFldsOvfl) res |= ReportOptionsEnum.BUFFER_OVERFLOW;
            if ((value[0] & Scsm_MMS.OptFldsEntryID) == Scsm_MMS.OptFldsEntryID) res |= ReportOptionsEnum.ENTRY_ID;
            if (value.Length < 2) return res;
            if ((value[1] & Scsm_MMS.OptFldsConfRev) == Scsm_MMS.OptFldsConfRev) res |= ReportOptionsEnum.CONF_REV;
            if ((value[1] & Scsm_MMS.OptFldsMoreSegments) == Scsm_MMS.OptFldsMoreSegments) res |= ReportOptionsEnum.SEGMENTATION;
            return res;
        }

        public static byte[] toBytes(this ReportOptionsEnum inp)
        {
            byte[] res = new byte[2];

            if ((inp & ReportOptionsEnum.SEQ_NUM) == ReportOptionsEnum.SEQ_NUM) res[0] |= Scsm_MMS.OptFldsSeqNum;
            if ((inp & ReportOptionsEnum.TIME_STAMP) == ReportOptionsEnum.TIME_STAMP) res[0] |= Scsm_MMS.OptFldsTimeOfEntry;
            if ((inp & ReportOptionsEnum.REASON_FOR_INCLUSION) == ReportOptionsEnum.REASON_FOR_INCLUSION) res[0] |= Scsm_MMS.OptFldsReasonCode;
            if ((inp & ReportOptionsEnum.DATA_SET) == ReportOptionsEnum.DATA_SET) res[0] |= Scsm_MMS.OptFldsDataSet;
            if ((inp & ReportOptionsEnum.DATA_REFERENCE) == ReportOptionsEnum.DATA_REFERENCE) res[0] |= Scsm_MMS.OptFldsDataReference;
            if ((inp & ReportOptionsEnum.BUFFER_OVERFLOW) == ReportOptionsEnum.BUFFER_OVERFLOW) res[0] |= Scsm_MMS.OptFldsOvfl;
            if ((inp & ReportOptionsEnum.ENTRY_ID) == ReportOptionsEnum.ENTRY_ID) res[0] |= Scsm_MMS.OptFldsEntryID;
            if ((inp & ReportOptionsEnum.CONF_REV) == ReportOptionsEnum.CONF_REV) res[1] |= Scsm_MMS.OptFldsConfRev;
            if ((inp & ReportOptionsEnum.SEGMENTATION) == ReportOptionsEnum.SEGMENTATION) res[1] |= Scsm_MMS.OptFldsMoreSegments;
            return res;
        }

        public static ReportTriggerOptionsEnum fromBytes(this ReportTriggerOptionsEnum res, byte[] value)
        {
            res = ReportTriggerOptionsEnum.NONE;
            if (value == null || value.Length < 1) return res;
            if ((value[0] & Scsm_MMS.TrgOpsDataChange) == Scsm_MMS.TrgOpsDataChange) res |= ReportTriggerOptionsEnum.DATA_CHANGED;
            if ((value[0] & Scsm_MMS.TrgOpsQualChange) == Scsm_MMS.TrgOpsQualChange) res |= ReportTriggerOptionsEnum.QUALITY_CHANGED;
            if ((value[0] & Scsm_MMS.TrgOpsDataActual) == Scsm_MMS.TrgOpsDataActual) res |= ReportTriggerOptionsEnum.DATA_UPDATE;
            if ((value[0] & Scsm_MMS.TrgOpsIntegrity) == Scsm_MMS.TrgOpsIntegrity) res |= ReportTriggerOptionsEnum.INTEGRITY;
            if ((value[0] & Scsm_MMS.TrgOpsGI) == Scsm_MMS.TrgOpsGI) res |= ReportTriggerOptionsEnum.GI;
            return res;
        }

        public static byte[] toBytes(this ReportTriggerOptionsEnum inp)
        {
            byte[] res = new byte[1];

            if ((inp & ReportTriggerOptionsEnum.DATA_CHANGED) == ReportTriggerOptionsEnum.DATA_CHANGED) res[0] |= Scsm_MMS.TrgOpsDataChange;
            if ((inp & ReportTriggerOptionsEnum.QUALITY_CHANGED) == ReportTriggerOptionsEnum.QUALITY_CHANGED) res[0] |= Scsm_MMS.TrgOpsQualChange;
            if ((inp & ReportTriggerOptionsEnum.DATA_UPDATE) == ReportTriggerOptionsEnum.DATA_UPDATE) res[0] |= Scsm_MMS.TrgOpsDataActual;
            if ((inp & ReportTriggerOptionsEnum.INTEGRITY) == ReportTriggerOptionsEnum.INTEGRITY) res[0] |= Scsm_MMS.TrgOpsIntegrity;
            if ((inp & ReportTriggerOptionsEnum.GI) == ReportTriggerOptionsEnum.GI) res[0] |= Scsm_MMS.TrgOpsGI;
            return res;
        }
    }

}
