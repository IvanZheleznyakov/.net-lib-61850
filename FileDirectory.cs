using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class FileDirectory
    {
        public DateTime TimeOfLastModification { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public byte[] FileData { get; set; }
    }
}
