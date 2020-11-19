using MMS_ASN1_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IEDExplorer
{
    public class MmsValue
    {
        public MmsValue(Data data)
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

        private Data asn1Data;

        public MmsTypeEnum MmsType { get; internal set; }

        public List<MmsValue> GetMmsArray()
        {
            List<MmsValue> result = new List<MmsValue>();
            foreach (var data in asn1Data.Array)
            {
                result.Add(new MmsValue(data));
            }

            return result;
        }

        public List<MmsValue> GetMmsStructure()
        {
            List<MmsValue> result = new List<MmsValue>();
            foreach (var data in asn1Data.Structure)
            {
                result.Add(new MmsValue(data));
            }

            return result;
        }

        public long GetBcd()
        {
            return asn1Data.Bcd;
        }

        public DateTime GetBinaryTime()
        {
            return TestDecoder.DecodeMmsBinaryTime(asn1Data.Binary_time.Value);
        }

        public byte[] GetBitString()
        {
            return asn1Data.Bit_string.Value;
        }

        public bool GetBoolean()
        {
            return asn1Data.Boolean;
        }

        public float GetFloat()
        {
            return TestDecoder.DecodeMmsFloat(asn1Data.Floating_point.Value);
        }

        public double GetDouble()
        {
            return TestDecoder.DecodeMmsDouble(asn1Data.Floating_point.Value);
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

        public long GetUnsigned()
        {
            return asn1Data.Unsigned;
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
