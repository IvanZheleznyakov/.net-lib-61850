using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public class FileDirectory
    {
        public DateTime TimeOfLastModification { get; internal set; }
        public int Size { get; internal set; }
        public string Name { get; internal set; }
        public byte[] FileData { get; internal set; }
    }
}
