using MMS_ASN1_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class Report
    {
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
