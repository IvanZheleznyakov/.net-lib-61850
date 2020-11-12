using MMS_ASN1_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public class Report
    {
        public enum ReasonForInclusionEnum
        {
            NOT_INCLUDED = 0,
            DATA_CHANGE = 1,
            QUALITY_CHANGE = 2,
            DATA_UPDATE = 3,
            INTEGRITY = 4,
            GI = 5,
            UNKNOWN = 6,
        }

        public bool HasDataSetName { get; set; } = false;
        public bool HasReasonForInclusion { get; set; } = false;
        public bool HasSequenceNumber { get; set; } = false;
        public bool HasDataReference { get; set; } = false;
        public bool HasConfigurationRevision { get; set; } = false;
        public bool HasTimeOfEntry { get; set; } = false;
        public bool HasBufferOverFlow { get; set; } = false;

        public string RptId { get; set; }
        public long SeqNum { get; set; }
        public string DataSetName { get; set; }
        public DateTime TimeOfEntry { get; set; }
        public bool BufferOverflow { get; set; }
        public byte[] EntryID { get; set; }
        public long ConfigurationRevision { get; set; }
        public int DataSetSize { get; set; }
        public int[] DataIndices { get; set; }
        public string[] DataReferences { get; set; }
        public MmsValue[] DataValues { get; set; }
        public ReasonForInclusionEnum[] ReasonForInclusion { get; set; }
    }
}
