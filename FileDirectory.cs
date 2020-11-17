using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public class FileDirectory
    {
        public string TimeOfLastModification { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public byte[] FileData { get; set; }
    }
}
