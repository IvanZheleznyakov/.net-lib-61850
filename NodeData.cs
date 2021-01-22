using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib61850net
{
    internal class NodeData : NodeBase
    {
        private MmsTypeEnum _dataType = MmsTypeEnum.STRUCTURE;
        private Object _dataValue = null;
        private Object _dataParam = null;
        private Object _valueTag = null;

        public NodeData(string Name)
            : base(Name)
        {
        }

        public MmsTypeEnum DataType
        {
            get
            {
                return _dataType;
            }
            set
            {
                _dataType = value;
            }
        }

        public Object DataValue
        {
            get
            {
                lock (this)
                    return _dataValue;
            }
            set
            {
                bool fire = false;

                lock (this)
                {
                    if (_dataValue == null || !_dataValue.Equals(value))
                    {
                        _dataValue = value;
                        fire = true;
                    }
                }
            }
        }

        public Object DataParam
        {
            get
            {
                return _dataParam;
            }
            set
            {
                _dataParam = value;
            }
        }

        public object ValueTag
        {
            get
            {
                return _valueTag;
            }
            set
            {
                _valueTag = value;
            }
        }

        internal override NodeBase FindNodeByValue(MmsTypeEnum dataType, object dataValue, ref NodeBase ContinueAfter)
        {
            if (dataValue == null)
                return null;
            NodeBase res = null;
            if (_childNodes.Count > 0)
            {
                foreach (NodeBase b in _childNodes)
                {
                    res = b.FindNodeByValue(dataType, dataValue, ref ContinueAfter);
                    if (res != null && ContinueAfter == null)
                        return res;
                    if (res != null && ContinueAfter != null && res == ContinueAfter)
                        ContinueAfter = null;
                }
            }
            else
            {
                if (dataType == this.DataType &&
                    this.DataValue != null && dataValue.ToString() == DataValue.ToString())
                {
                    return this;
                }
            }
            return null;
        }

        protected bool isFCCalculated = false;
        protected FunctionalConstraintEnum _FC = FunctionalConstraintEnum.NONE;

        public FunctionalConstraintEnum FC
        {
            get
            {
                if (_FC == FunctionalConstraintEnum.NONE && !isFCCalculated)
                {
                    isFCCalculated = true;
                    NodeBase nb = Parent;
                    if (nb != null) do
                        {
                            if (nb is NodeFC)
                            {
                                isFCCalculated = true;
                                _FC = (FunctionalConstraintEnum)MapLibiecFC(nb.Name);
                                return _FC;
                            }
                            nb = nb.Parent;
                        } while (nb != null);
                    return FunctionalConstraintEnum.NONE;
                }
                else
                {
                    return _FC;
                }
            }
            set
            {
                isFCCalculated = true;
                _FC = value;
            }
        }

        public string StringValue
        {
            get
            {
                string val = "";
                if (DataValue != null)
                {
                    switch (DataType)
                    {
                        case MmsTypeEnum.UTC_TIME:
                            if (!(DataValue is DateTime)) break;
                            if (DataValue != null) val = DataValue.ToString() + "." + ((DateTime)(DataValue)).Millisecond.ToString("D3") + " [LOC]";
                            if (DataParam != null)
                            {
                                bool close = false;
                                if (((byte)(DataParam) & 0x20) > 0)
                                {
                                    val += "[ClockNotSynchronised";
                                    close = true;
                                }
                                if (((byte)(DataParam) & 0x40) > 0)
                                {
                                    if (close)
                                        val += ", ";
                                    else
                                        val += "[";
                                    val += "ClockFailure";
                                    close = true;
                                }
                                if (((byte)(DataParam) & 0x80) > 0)
                                {
                                    if (close)
                                        val += ", ";
                                    else
                                        val += "[";
                                    val += "LeapSecondKnown";
                                    close = true;
                                }
                                if (close) val += "]";
                            }
                            break;
                        case MmsTypeEnum.BIT_STRING:
                            if (DataParam != null)
                            {
                                byte[] bbval = (byte[])DataValue;
                                int blen = bbval.Length;
                                int trail = (int)DataParam;

                                StringBuilder sb = new StringBuilder(32);
                                for (int i = 0; i < blen * 8 - trail; i++)
                                {
                                    if (((bbval[(i / 8)] << (i % 8)) & 0x80) > 0)
                                        sb.Append(1);     //.Insert(0, 1);
                                    else
                                        sb.Append(0);     //.Insert(0, 0);
                                }

                                switch (Name)    
                                {
                                    case "q":       // Quality descriptor
                                        DataQuality dq = DataQuality.NONE;
                                        dq = dq.FromBytes(bbval);
                                        sb.Append(" [");
                                        sb.Append(dq.ToString());
                                        sb.Append("]");
                                        break;
                                    case "TrgOps":  // Trigger Options
                                        ReportTriggerOptionsEnum tr = ReportTriggerOptionsEnum.NONE;
                                        tr = tr.fromBytes(bbval);
                                        sb.Append(" [");
                                        sb.Append(tr.ToString());
                                        sb.Append("]");
                                        break;
                                    case "OptFlds":  // Optional fields
                                        ReportOptionsEnum ro = ReportOptionsEnum.NONE;
                                        ro = ro.fromBytes(bbval);
                                        sb.Append(" [");
                                        sb.Append(ro.ToString());
                                        sb.Append("]");
                                        break;
                                }
                                val = sb.ToString();
                            }
                            break;
                        case MmsTypeEnum.BINARY_TIME:
                            if (DataValue != null) val = DataValue.ToString() + "." + ((DateTime)(DataValue)).Millisecond.ToString() + " [LOC]";
                            break;
                        case MmsTypeEnum.OCTET_STRING:
                            if (DataValue != null)
                            {
                                byte[] ba = System.Text.Encoding.ASCII.GetBytes(DataValue.ToString());
                                switch (Name)
                                {
                                    case "Owner":
                                        foreach (byte ipb in ba)
                                        {
                                            val += ipb.ToString();
                                            val += ".";
                                        }
                                        if (val.Length > 0) val = val.Substring(0, val.Length - 1);
                                        break;
                                    default:
                                        bool nonAscii = false;
                                        foreach (byte ipb in ba)
                                        {
                                            char c = Convert.ToChar(ipb);
                                            if (Char.IsControl(c))
                                            {
                                                nonAscii = true;
                                                break;
                                            }
                                            val += c;
                                        }
                                        if (nonAscii) val = BitConverter.ToString(ba);
                                        break;
                                }
                            }
                            break;
                        default:
                            val = DataValue.ToString();
                            break;
                    }
                }
                return val;
            }
            set
            {
                if (value != null && value != "")
                {
                    try
                    {
                        switch (DataType)
                        {
                            case MmsTypeEnum.UTC_TIME:
                                string tms = value;
                                DateTime tval;

                                if (tms.Contains('['))
                                    tms = tms.Substring(0, tms.IndexOf('['));

                                byte[] btm = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
                                DataValue = btm;

                                if (DateTime.TryParse(tms, out tval))
                                {
                                    int t = (int)Scsm_MMS.ConvertToUnixTimestamp(tval);
                                    byte[] uib = BitConverter.GetBytes(t);
                                    btm[0] = uib[3];
                                    btm[1] = uib[2];
                                    btm[2] = uib[1];
                                    btm[3] = uib[0];
                                    // Milliseconds
                                    int fractionOfSecond = (tval.Millisecond) * 16777 + ((tval.Millisecond * 216) / 1000);
	                                /* encode fraction of second */
                                    btm[4] = (byte)((fractionOfSecond >> 16) & 0xff);
                                    btm[5] = (byte)((fractionOfSecond >> 8) & 0xff);
                                    btm[6] = (byte)(fractionOfSecond & 0xff);

	                                /* encode time quality */
                                    btm[7] = 0x0a; /* 10 bit sub-second time accuracy */
                                }
                                else
                                {
                                    Logger.getLogger().LogError("NodeData.StringValue - cannot parse '" + tms + "' to DateTime");
                                }
                                break;
                            case MmsTypeEnum.BIT_STRING:
                                byte[] bbval = (byte[])DataValue;
                                int blen = bbval.Length;
                                int trail = (int)DataParam;

                                for (int i = 0; i < blen * 8 - trail && i < value.Length; i++)
                                {
                                    //if (((bbval[(i / 8)] << (i % 8)) & 0x80) > 0)
                                    if (value[i] == '1')
                                        bbval[(i / 8)] |= (byte)(0x80 >> (i % 8));
                                    else
                                        bbval[(i / 8)] &= (byte)~(0x80 >> (i % 8));
                                }
                                //val = sb.ToString();
                                break;
                            case MmsTypeEnum.BOOLEAN:
                                if (value.StartsWith("0") || value.StartsWith("f", StringComparison.CurrentCultureIgnoreCase))
                                    DataValue = false;
                                if (value.StartsWith("1") || value.StartsWith("t", StringComparison.CurrentCultureIgnoreCase))
                                    DataValue = true;
                                break;
                            case MmsTypeEnum.VISIBLE_STRING:
                                DataValue = value;
                                break;
                            case MmsTypeEnum.OCTET_STRING:
                                DataValue = Encoding.ASCII.GetBytes(value);
                                break;
                            case MmsTypeEnum.UNSIGNED:
                                long uns;
                                if (long.TryParse(value, out uns))
                                {
                                    DataValue = uns;
                                }
                                else
                                {
                                    Logger.getLogger().LogError("NodeData.StringValue - cannot parse '" + value + "' to unsigned (internally int64)");
                                }
                                break;
                            case MmsTypeEnum.INTEGER:
                                long sint;
                                if (long.TryParse(value, out sint))
                                {
                                    DataValue = sint;
                                }
                                else
                                {
                                    Logger.getLogger().LogError("NodeData.StringValue - cannot parse '" + value + "' to integer (internally int64)");
                                }
                                break;
                            case MmsTypeEnum.FLOATING_POINT:
                                float fval;
                                if (float.TryParse(value, out fval))
                                {
                                    DataValue = fval;
                                }
                                else
                                {
                                    Logger.getLogger().LogError("NodeData.StringValue - cannot parse '" + value + "' to float");
                                }
                                break;
                            default:
                                Logger.getLogger().LogError("NodeData.StringValue - type '" + DataType.ToString() + "' not implemented");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.getLogger().LogError("NodeData.StringValue - cannot parse '" + value + "' to type '" + DataType.ToString() + "', exception: " + e.Message);
                    }
                }
            }
        }

        public int sAddr { get; set; }

        internal static int MapLibiecFC(string FC)
        {
            int fco = 0;
            foreach (string s in Enum.GetNames(typeof(FunctionalConstraintEnum)))
            {
                if (s.Substring(s.LastIndexOf("_") + 1) == FC)
                {
                    return (int)Enum.GetValues(typeof(FunctionalConstraintEnum)).GetValue(fco);
                }
                fco++;
            }
            return -1;
        }

        enum LibIecDataAttributeType
        {
	        BOOLEAN = 0,/* int */
	        INT8 = 1,   /* int8_t */
	        INT16 = 2,  /* int16_t */
	        INT32 = 3,  /* int32_t */
	        INT64 = 4,  /* int64_t */
	        INT128 = 5, /* no native mapping! */
	        INT8U = 6,  /* uint8_t */
	        INT16U = 7, /* uint16_t */
	        INT24U = 8, /* uint32_t */
	        INT32U = 9, /* uint32_t */
	        FLOAT32 = 10, /* float */
	        FLOAT64 = 11, /* double */
	        ENUMERATED = 12,
	        OCTET_STRING_64 = 13,
	        OCTET_STRING_6 = 14,
	        OCTET_STRING_8 = 15,
	        VISIBLE_STRING_32 = 16,
	        VISIBLE_STRING_64 = 17,
	        VISIBLE_STRING_65 = 18,
	        VISIBLE_STRING_129 = 19,
	        VISIBLE_STRING_255 = 20,
	        UNICODE_STRING_255 = 21,
	        TIMESTAMP = 22,
	        QUALITY = 23,
	        CHECK = 24,
	        CODEDENUM = 25,
	        GENERIC_BITSTRING = 26,
	        CONSTRUCTED = 27,
	        ENTRY_TIME = 28,
	        PHYCOMADDR = 29
        }
    }   // class NodeData

    [Flags]
    public enum DataQuality
    {
        NONE = 0,
        VALIDITY0 = 0x01, // BIT "0" IN MMS INTERPRETATION
        VALIDITY1 = 0x02,
        OVERFLOW = 0x04,
        OUT_OF_RANGE = 0x08,
        BAD_REFERENCE = 0x10,
        OSCILLATORY = 0x20,
        FAILURE = 0x40,
        OLD_DATA = 0x80,
        // byte border
        INCONSISTENT = 0x100,
        INACCURATE = 0x200,
        SOURCE = 0x400,
        TEST = 0x800,
        OPERATOR_BLOCKED = 0x1000, // BIT "12" IN MMS INTERPRETATION
    }

    public static class DataEnumExtensions
    {
        public static DataQuality FromBytes(this DataQuality res, byte[] value)
        {
            res = DataQuality.NONE;
            if (value == null || value.Length < 1) return res;
            if ((value[0] & Scsm_MMS.DatQualValidity0) == Scsm_MMS.DatQualValidity0) res |= DataQuality.VALIDITY0;
            if ((value[0] & Scsm_MMS.DatQualValidity1) == Scsm_MMS.DatQualValidity1) res |= DataQuality.VALIDITY1;
            if ((value[0] & Scsm_MMS.DatQualOverflow) == Scsm_MMS.DatQualOverflow) res |= DataQuality.OVERFLOW;
            if ((value[0] & Scsm_MMS.DatQualOutOfRange) == Scsm_MMS.DatQualOutOfRange) res |= DataQuality.OUT_OF_RANGE;
            if ((value[0] & Scsm_MMS.DatQualBadReference) == Scsm_MMS.DatQualBadReference) res |= DataQuality.BAD_REFERENCE;
            if ((value[0] & Scsm_MMS.DatQualOscillatory) == Scsm_MMS.DatQualOscillatory) res |= DataQuality.OSCILLATORY;
            if ((value[0] & Scsm_MMS.DatQualFailure) == Scsm_MMS.DatQualFailure) res |= DataQuality.FAILURE;
            if ((value[0] & Scsm_MMS.DatQualOldData) == Scsm_MMS.DatQualOldData) res |= DataQuality.OLD_DATA;
            if (value.Length < 2) return res;
            if ((value[1] & Scsm_MMS.DatQualInconsistent) == Scsm_MMS.DatQualInconsistent) res |= DataQuality.INCONSISTENT;
            if ((value[1] & Scsm_MMS.DatQualInaccurate) == Scsm_MMS.DatQualInaccurate) res |= DataQuality.INACCURATE;
            if ((value[1] & Scsm_MMS.DatQualSource) == Scsm_MMS.DatQualSource) res |= DataQuality.SOURCE;
            if ((value[1] & Scsm_MMS.DatQualTest) == Scsm_MMS.DatQualTest) res |= DataQuality.TEST;
            if ((value[1] & Scsm_MMS.DatQualOperatorBlocked) == Scsm_MMS.DatQualOperatorBlocked) res |= DataQuality.OPERATOR_BLOCKED;
            return res;
        }

        public static byte[] ToBytes(this DataQuality inp)
        {
            byte[] res = new byte[2];

            if ((inp & DataQuality.VALIDITY0) == DataQuality.VALIDITY0) res[0] |= Scsm_MMS.DatQualValidity0;
            if ((inp & DataQuality.VALIDITY1) == DataQuality.VALIDITY1) res[0] |= Scsm_MMS.DatQualValidity1;
            if ((inp & DataQuality.OVERFLOW) == DataQuality.OVERFLOW) res[0] |= Scsm_MMS.DatQualOverflow;
            if ((inp & DataQuality.OUT_OF_RANGE) == DataQuality.OUT_OF_RANGE) res[0] |= Scsm_MMS.DatQualOutOfRange;
            if ((inp & DataQuality.BAD_REFERENCE) == DataQuality.BAD_REFERENCE) res[0] |= Scsm_MMS.DatQualBadReference;
            if ((inp & DataQuality.OSCILLATORY) == DataQuality.OSCILLATORY) res[0] |= Scsm_MMS.DatQualOscillatory;
            if ((inp & DataQuality.FAILURE) == DataQuality.FAILURE) res[0] |= Scsm_MMS.DatQualFailure;
            if ((inp & DataQuality.OLD_DATA) == DataQuality.OLD_DATA) res[0] |= Scsm_MMS.DatQualOldData;
            if ((inp & DataQuality.INCONSISTENT) == DataQuality.INCONSISTENT) res[1] |= Scsm_MMS.DatQualInconsistent;
            if ((inp & DataQuality.INACCURATE) == DataQuality.INACCURATE) res[1] |= Scsm_MMS.DatQualInaccurate;
            if ((inp & DataQuality.SOURCE) == DataQuality.SOURCE) res[1] |= Scsm_MMS.DatQualSource;
            if ((inp & DataQuality.TEST) == DataQuality.TEST) res[1] |= Scsm_MMS.DatQualTest;
            if ((inp & DataQuality.OPERATOR_BLOCKED) == DataQuality.OPERATOR_BLOCKED) res[1] |= Scsm_MMS.DatQualOperatorBlocked;
            return res;
        }
    }
}
