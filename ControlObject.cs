using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public class ControlObject
    {
        internal NodeDO self;
        internal LibraryManager libraryManager;
        internal string originator;
        internal OriginatorCategoryEnum orCat;
        internal objectReference;

        public ControlModelEnum ControlModel { get; internal set; }
        public string ObjectReference { get; internal set; }
        

        public void Select()
        {
            libraryManager.worker.iecs.Controller.ReadData(self.FindChildNode("SBO"));
        }

        public void SetOriginatorCategory(string originator, OriginatorCategoryEnum orCat)
        {
            this.originator = originator;
            this.orCat = orCat;
        }

        public void 
    }
}
