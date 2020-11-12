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

        public string RptId { get; set; }
        public long SeqNum { get; set; }
        public List<ReasonForInclusionEnum> ReasonForInclusion { get; set; } = new List<ReasonForInclusionEnum>();
        public string DataSetName { get; set; }
        public DateTime TimeOfEntry { get; set; }
        public bool BufferOverflow { get; set; }
        public byte[] EntryID { get; set; }
        public long ConfigurationRevision { get; set; }
        public int DataSetSize { get; set; }
    }
}
