using MMS_ASN1_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class MmsValue
    {

        internal List<MmsValue> childs = null;
        public MmsValue()
        {

        }

        internal MmsValue(MmsValue mmsValue)
        {
            CopyFrom(mmsValue);
        }

        internal MmsValue(Data data)
        {
            asn1Data = data;
            if (asn1Data.isArraySelected())
            {
                MmsType = MmsTypeEnum.ARRAY;
            }
            else if (asn1Data.isBcdSelected())
            {
                MmsType = MmsTypeEnum.BCD;
            }
            else if (asn1Data.isBinary_timeSelected())
            {
                MmsType = MmsTypeEnum.BINARY_TIME;
            }
            else if (asn1Data.isBit_stringSelected())
            {
                MmsType = MmsTypeEnum.BIT_STRING;
            }
            else if (asn1Data.isBooleanSelected())
            {
                MmsType = MmsTypeEnum.BOOLEAN;
            }
            else if (asn1Data.isFloating_pointSelected())
            {
                if (asn1Data.Floating_point.Value.Length == 5)
                {
                    MmsType = MmsTypeEnum.FLOATING_POINT;
                }
                else
                {
                    MmsType = MmsTypeEnum.DOUBLE;
                }
            }
            else if (asn1Data.isGeneralized_timeSelected())
            {
                MmsType = MmsTypeEnum.GENERALIZED_TIME;
            }
            else if (asn1Data.isIntegerSelected())
            {
                MmsType = MmsTypeEnum.INTEGER;
            }
            else if (asn1Data.isMMSStringSelected())
            {
                MmsType = MmsTypeEnum.MMS_STRING;
            }
            else if (asn1Data.isObjIdSelected())
            {
                MmsType = MmsTypeEnum.OBJ_ID;
            }
            else if (asn1Data.isOctet_stringSelected())
            {
                MmsType = MmsTypeEnum.OCTET_STRING;
            }
            else if (asn1Data.isStructureSelected())
            {
                MmsType = MmsTypeEnum.STRUCTURE;
            }
            else if (asn1Data.isUnsignedSelected())
            {
                MmsType = MmsTypeEnum.UNSIGNED;
            }
            else if (asn1Data.isUtc_timeSelected())
            {
                MmsType = MmsTypeEnum.UTC_TIME;
            }
            else if (asn1Data.isVisible_stringSelected())
            {
                MmsType = MmsTypeEnum.VISIBLE_STRING;
            }
            else
            {
                MmsType = MmsTypeEnum.UNKNOWN;
            }
        }

        internal void CopyFrom(MmsValue mmsValue)
        {
            this.TypeOfError = mmsValue.TypeOfError;
            this.asn1Data = mmsValue.asn1Data;
            this.MmsType = mmsValue.MmsType;
        }

        internal Data asn1Data;

        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public MmsTypeEnum MmsType { get; internal set; }

        private int size = -1;
        public int Size
        {
            get
            {
                if (size == -1)
                {
                    if (MmsType == MmsTypeEnum.ARRAY)
                    {
                        size = GetMmsArray().Count;
                    }
                    else if (MmsType == MmsTypeEnum.STRUCTURE)
                    {
                        size = GetMmsStructure().Count;
                    }
                }
                return size;
            }
            internal set
            {
                size = value;
            }
        }

        public MmsValue GetChildByIndex(int index)
        {
            if ((MmsType != MmsTypeEnum.ARRAY && MmsType != MmsTypeEnum.STRUCTURE) || index < 0 || index >= Size)
            {
                return null;
            }

            if (MmsType == MmsTypeEnum.STRUCTURE)
            {
                return GetMmsStructure()[index];
            }

            return GetMmsArray()[index];
        }

        public List<MmsValue> GetMmsArray()
        {
            if (childs == null && (MmsType == MmsTypeEnum.ARRAY || MmsType == MmsTypeEnum.STRUCTURE))
            {
                childs = new List<MmsValue>();
                foreach (var data in asn1Data.Array)
                {
                    childs.Add(new MmsValue(data));
                }
            }

            return childs;
        }

        public List<MmsValue> GetMmsStructure()
        {
            if (childs == null && (MmsType == MmsTypeEnum.ARRAY || MmsType == MmsTypeEnum.STRUCTURE))
            {
                childs = new List<MmsValue>();
                foreach (var data in asn1Data.Structure)
                {
                    childs.Add(new MmsValue(data));
                }
            }

            return childs;
        }

        public long GetBcd()
        {
            return asn1Data.Bcd;
        }

        public DateTime GetBinaryTime()
        {
            return MmsDecoder.DecodeMmsBinaryTime(asn1Data.Binary_time.Value);
        }

        public byte[] GetBitString()
        {
            return asn1Data.Bit_string.Value;
        }

        public UInt32 GetBitStringAsInteger()
        {
            if (MmsType != MmsTypeEnum.BIT_STRING)
            {
                throw new Exception("Value type is not bit string");
            }

            byte[] bitString = GetBitString();

            UInt32 value = 0;

            int bitPos;

            for (bitPos = 0; bitPos < bitString.Length; bitPos++)
            {
                if (GetBitStringBit(bitPos))
                {
                    value += (UInt32)(1 << bitPos);
                }
            }

            return value;

        }

        public bool GetBitStringBit(int bitPos)
        {
            if (MmsType != MmsTypeEnum.BIT_STRING)
            {
                throw new Exception("Value type is not bit string");
            }

            byte[] bitString = GetBitString();
            return MmsDecoder.GetBitStringFromMmsValue(bitString, bitString.Length, bitPos);
        }

        public bool GetBoolean()
        {
            return asn1Data.Boolean;
        }

        public float GetFloat()
        {
            return MmsDecoder.DecodeMmsFloat(asn1Data.Floating_point.Value);
        }

        public double GetDouble()
        {
            return MmsDecoder.DecodeMmsDouble(asn1Data.Floating_point.Value);
        }

        public string GetGeneralizedTime()
        {
            return asn1Data.Generalized_time;
        }

        public long GetInteger()
        {
            return asn1Data.Integer;
        }

        public string GetMmsString()
        {
            return asn1Data.MMSString.Value;
        }

        public string GetObjId()
        {
            return asn1Data.ObjId.Value;
        }

        public byte[] GetOctetString()
        {
            return asn1Data.Octet_string;
        }

        public uint GetUnsigned()
        {
            return (uint)asn1Data.Unsigned;
        }

        public DateTime GetUtcTime()
        {
            return Scsm_MMS.ConvertFromUtcTime(asn1Data.Utc_time.Value, null);
        }

        public string GetVisibleString()
        {
            return asn1Data.Visible_string;
        }
    }
}
