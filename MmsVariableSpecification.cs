using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class MmsVariableSpecification
    {
        internal NodeData self;
        public string Name { get; internal set; }
        public string ObjectReference { get; internal set; }
        public MmsTypeEnum MmsType { get; internal set; }
        internal object DataValue { get; set; }

        internal MmsVariableSpecification(NodeData node)
        {
            self = node;
            Name = self.Name;
            ObjectReference = self.IecAddress;
            MmsType = self.DataType;
            DataValue = self.DataValue;
        }

        public List<MmsVariableSpecification> GetMmsArray()
        {
            List<MmsVariableSpecification> result = new List<MmsVariableSpecification>();

            var childs = self.GetChildNodes();

            foreach (var node in childs)
            {
                result.Add(new MmsVariableSpecification((NodeData)node));
            }

            return result;
        }

        public List<MmsVariableSpecification> GetMmsStructure()
        {
            return GetMmsArray();
        }

        public long GetBcd()
        {
            return (long)DataValue;
        }

        public DateTime GetBinaryTime()
        {
            return MmsDecoder.DecodeMmsBinaryTime((byte[])DataValue);
        }

        public byte[] GetBitString()
        {
            return (byte[])DataValue;
        }

        public bool GetBoolean()
        {
            return (bool)DataValue;
        }

        public float GetFloat()
        {
            return MmsDecoder.DecodeMmsFloat((byte[])DataValue);
        }

        public double GetDouble()
        {
            return MmsDecoder.DecodeMmsDouble((byte[])DataValue);
        }

        public string GetGeneralizedTime()
        {
            return (string)DataValue;
        }

        public long GetInteger()
        {
            return (long)DataValue;
        }

        public string GetMmsString()
        {
            return (string)DataValue;
        }

        public string GetObjId()
        {
            return (string)DataValue;
        }

        public byte[] GetOctetString()
        {
            return (byte[])DataValue;
        }

        public long GetUnsigned()
        {
            return (long)DataValue;
        }

        public DateTime GetUtcTime()
        {
            return Scsm_MMS.ConvertFromUtcTime((byte[])DataValue, null);
        }

        public string GetVisibleString()
        {
            return (string)DataValue;
        }
    }
}
