using System.Collections.Generic;

namespace lib61850net
{
    public class MmsVariableSpecification
    {
        //    internal NodeData self;
        public string Name { get; internal set; } = string.Empty;
        //    public string ObjectReference { get; internal set; }
        public MmsTypeEnum MmsType { get; internal set; }
        internal List<MmsVariableSpecification> childs;
        //     internal object DataValue { get; set; }
        private int size = -1;
        public int Size
        {
            get
            {
                if (size == -1)
                {
                    if (MmsType == MmsTypeEnum.ARRAY)
                    {
                        size = childs.Count;
                    }
                    else if (MmsType == MmsTypeEnum.STRUCTURE)
                    {
                        size = childs.Count;
                    }
                }
                return size;
            }
        }

        internal MmsVariableSpecification(MmsTypeEnum mmsType)
        {
            MmsType = mmsType;
            if (mmsType == MmsTypeEnum.ARRAY || mmsType == MmsTypeEnum.STRUCTURE)
            {
                childs = new List<MmsVariableSpecification>();
            }
        }

        internal void AddChild(MmsVariableSpecification newChild)
        {
            childs.Add(newChild);
        }

        public MmsVariableSpecification GetChildByIndex(int index)
        {
            if ((MmsType != MmsTypeEnum.ARRAY && MmsType != MmsTypeEnum.STRUCTURE) || index < 0 || index >= Size)
            {
                return null;
            }

            if (MmsType == MmsTypeEnum.STRUCTURE)
            {
                return childs[index];
            }

            return childs[index];
        }

        public List<MmsVariableSpecification> GetMmsArray()
        {
            return childs;
        }

        public List<MmsVariableSpecification> GetMmsStructure()
        {
            return childs;
        }

        //public long GetBcd()
        //{
        //    return (long)DataValue;
        //}

        //public DateTime GetBinaryTime()
        //{
        //    return MmsDecoder.DecodeMmsBinaryTime((byte[])DataValue);
        //}

        //public byte[] GetBitString()
        //{
        //    return (byte[])DataValue;
        //}

        //public bool GetBoolean()
        //{
        //    return (bool)DataValue;
        //}

        //public float GetFloat()
        //{
        //    return MmsDecoder.DecodeMmsFloat((byte[])DataValue);
        //}

        //public double GetDouble()
        //{
        //    return MmsDecoder.DecodeMmsDouble((byte[])DataValue);
        //}

        //public string GetGeneralizedTime()
        //{
        //    return (string)DataValue;
        //}

        //public long GetInteger()
        //{
        //    return (long)DataValue;
        //}

        //public string GetMmsString()
        //{
        //    return (string)DataValue;
        //}

        //public string GetObjId()
        //{
        //    return (string)DataValue;
        //}

        //public byte[] GetOctetString()
        //{
        //    return (byte[])DataValue;
        //}

        //public long GetUnsigned()
        //{
        //    return (long)DataValue;
        //}

        //public DateTime GetUtcTime()
        //{
        //    return Scsm_MMS.ConvertFromUtcTime((byte[])DataValue, null);
        //}

        //public string GetVisibleString()
        //{
        //    return (string)DataValue;
        //}
    }
}
