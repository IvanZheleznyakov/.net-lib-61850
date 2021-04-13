using MMS_ASN1_Model;
using org.bn.types;
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
        private object value;
        private uint uValue;
        public MmsValue(MmsTypeEnum mmsType, object value, int size = 0)
        {
            MmsType = mmsType;
            this.value = value;
            this.size = size;
        }

        internal MmsValue()
        {

        }

        internal MmsValue(MmsValue mmsValue)
        {
            CopyFrom(mmsValue);
        }

        internal MmsValue(Data data)
        {
            if (data.isArraySelected())
            {
                MmsType = MmsTypeEnum.ARRAY;
                childs = new List<MmsValue>();
                foreach (var newChild in data.Array)
                {
                    childs.Add(new MmsValue(newChild));
                }
            }
            else if (data.isBcdSelected())
            {
                value = data.Bcd;
                MmsType = MmsTypeEnum.BCD;
            }
            else if (data.isBinary_timeSelected())
            {
                value = MmsDecoder.DecodeMmsBinaryTime(data.Binary_time.Value);
                MmsType = MmsTypeEnum.BINARY_TIME;
            }
            else if (data.isBit_stringSelected())
            {
                value = data.Bit_string;
                MmsType = MmsTypeEnum.BIT_STRING;
            }
            else if (data.isBooleanSelected())
            {
                value = data.Boolean;
                MmsType = MmsTypeEnum.BOOLEAN;
            }
            else if (data.isFloating_pointSelected())
            {
                if (data.Floating_point.Value.Length == 5)
                {
                    value = MmsDecoder.DecodeMmsFloat(data.Floating_point.Value);
                    MmsType = MmsTypeEnum.FLOATING_POINT;
                }
                else
                {
                    value = MmsDecoder.DecodeMmsDouble(data.Floating_point.Value);
                    MmsType = MmsTypeEnum.DOUBLE;
                }
            }
            else if (data.isGeneralized_timeSelected())
            {
                value = data.Generalized_time;
                MmsType = MmsTypeEnum.GENERALIZED_TIME;
            }
            else if (data.isIntegerSelected())
            {
                value = data.Integer;
                MmsType = MmsTypeEnum.INTEGER;
            }
            else if (data.isMMSStringSelected())
            {
                value = data.MMSString.Value;
                MmsType = MmsTypeEnum.MMS_STRING;
            }
            else if (data.isObjIdSelected())
            {
                value = data.ObjId.Value;
                MmsType = MmsTypeEnum.OBJ_ID;
            }
            else if (data.isOctet_stringSelected())
            {
                value = data.Octet_string;
                MmsType = MmsTypeEnum.OCTET_STRING;
            }
            else if (data.isStructureSelected())
            {
                MmsType = MmsTypeEnum.STRUCTURE;
                childs = new List<MmsValue>();
                foreach (var newChild in data.Structure)
                {
                    childs.Add(new MmsValue(newChild));
                }
            }
            else if (data.isUnsignedSelected())
            {
                value = data.Unsigned;
                MmsType = MmsTypeEnum.UNSIGNED;
            }
            else if (data.isUtc_timeSelected())
            {
                value = Scsm_MMS.ConvertFromUtcTime(data.Utc_time.Value, null);
                MmsType = MmsTypeEnum.UTC_TIME;
            }
            else if (data.isVisible_stringSelected())
            {
                value = data.Visible_string;
                MmsType = MmsTypeEnum.VISIBLE_STRING;
            }
            else
            {
                MmsType = MmsTypeEnum.UNKNOWN;
            }
        }

        internal object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        internal void CopyFrom(MmsValue mmsValue)
        {
            this.TypeOfError = mmsValue.TypeOfError;
            this.MmsType = mmsValue.MmsType;
        }

        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public MmsTypeEnum MmsType { get; internal set; }

        private int size = 0;
        public int Size
        {
            get
            {
                if (size == 0)
                {
                    if (MmsType == MmsTypeEnum.ARRAY)
                    {
                        size = GetMmsArray().Count;
                    }
                    else if (MmsType == MmsTypeEnum.STRUCTURE)
                    {
                        size = GetMmsStructure().Count;
                    }
                    else if (MmsType == MmsTypeEnum.BIT_STRING)
                    {
                        size = 8 - (value as BitString).TrailBitsCnt;
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
            return childs;
        }

        public List<MmsValue> GetMmsStructure()
        {
            return childs;
        }

        public long GetBcd()
        {
            return (long)value;
        }

        public DateTime GetBinaryTime()
        {
            return (DateTime)value;
        }

        private bool isBitStringReversed = false;

        public BitString GetBitString()
        {
            return (BitString)value;
        }

        public byte[] GetBitStringAsArray()
        {
            byte[] result = (byte[])value;
            //if (!isBitStringReversed)
            //{
            //    for (int i = 0; i != result.Length; ++i)
            //    {
            //        result[i] = Reverse(result[i]);
            //    }

            //    isBitStringReversed = true;
            //}

            return result;
        }

        private UInt32 BitReverse(UInt32 value)
        {
            UInt32 left = (UInt32)1 << 7;
            UInt32 right = 1;
            UInt32 result = 0;

            for (int i = 7; i >= 1; i -= 2)
            {
                result |= (value & left) >> i;
                result |= (value & right) << i;
                left >>= 1;
                right <<= 1;
            }
            return result;
        }

        byte Reverse(byte value)
        {
            byte reverse = 0;
            for (int bit = 0; bit < 7; bit++)
            {
                reverse <<= 1;
                reverse |= (byte)(value & 1);
                value >>= 1;
            }

            return reverse;
        }

        public UInt32 GetBitStringAsInteger()
        {
            if (MmsType != MmsTypeEnum.BIT_STRING)
            {
                throw new Exception("Value type is not bit string");
            }

            //byte[] bitString = GetBitString();

            //ushort intValue = 0;
            //for (int k = Size; k >= 0; k--)
            //{
            //    intValue <<= 1;
            //    if (GetBitStringBit(Size - k))
            //    {
            //        intValue |= 0x01;
            //    }
            //}

            //byte[] bitStringB = GetBitString();

            //int[] bitString = new int[bitStringB.Length];

            //for (int i = 0; i != bitString.Length; ++i)
            //{
            //    bitString[i] = ~bitStringB[i];
            //}

            UInt32 intValue = 0;

            int bitPos;

            for (bitPos = 0; bitPos < Size; bitPos++)
            {
                if (GetBitStringBit(bitPos))
                {
                    intValue += (UInt32)(1 << Size - bitPos - 1);
                }
            }


            return intValue;

            //return BitReverse(intValue);

        }

        public bool GetBitStringBit(int bitPos)
        {
            if (MmsType != MmsTypeEnum.BIT_STRING)
            {
                throw new Exception("Value type is not bit string");
            }

            BitString bitString = GetBitString();
            return MmsDecoder.GetBitStringBitFromMmsValue(bitString.Value, Size, bitPos);
        }

        public bool GetBoolean()
        {
            return (bool)value;
        }

        public float GetFloat()
        {
            return (float)value;
        }

        public double GetDouble()
        {
            return (double)value;
        }

        public string GetGeneralizedTime()
        {
            return (string)value;
        }

        public long GetInteger()
        {
            return (long)value;
        }

        public string GetMmsString()
        {
            return (string)value;
        }

        public string GetObjId()
        {
            return (string)value; 
        }

        public byte[] GetOctetString()
        {
            return (byte[])value; 
        }

        public uint GetUnsigned()
        {
            return Convert.ToUInt32(value);
        }

        public DateTime GetUtcTime()
        {
            return (DateTime)value;
        }

        public string GetVisibleString()
        {
            try
            {
                return (string)value;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
