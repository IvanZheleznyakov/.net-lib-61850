using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace lib61850net
{
    internal class NodeFile : NodeBase
    {
        public NodeFile(string Name, bool isDir)
            : base(Name)
        {
            this.isDir = isDir;
            this.files = new List<NodeFile>();
            frsmId = 0;
        }
        public event EventHandler DirectoryUpdated;

        public bool isDir { get; private set; }

        private bool fileReady;
        public bool FileReady
        {
            get
            {
                return fileReady;
            }
            set
            {
                fileReady = value;
                if (DirectoryUpdated != null)
                    DirectoryUpdated(this, new EventArgs());
            }
        }

        private bool fileSaved;
        public bool FileSaved
        {
            get
            {
                return fileSaved;
            }
            set
            {
                fileSaved = value;
                if (DirectoryUpdated != null)
                    DirectoryUpdated(this, new EventArgs());
            }
        }

        public int frsmId { get; set; }

        public int ReportedSize { get; set; }

        public DateTime ReportedTime { get; set; }

        public List<NodeFile> files { get; private set; }

        private byte[] data;
        public byte[] Data { get { return data; } set { data = value; } }

        //public int AppendData(byte[] chunk)
        //{
        //    if (data == null)
        //        data = chunk;
        //    else
        //    {
        //        int origLen = data.Length;
        //        Array.Resize<byte>(ref data, origLen + chunk.Length);
        //        Array.Copy(chunk, 0, data, origLen, chunk.Length);
        //    }
        //    return data.Length;
        //}

        private bool isFullNameCreated = false;
        private string fullName;

        public string FullName
        {
            get
            {
                if (isFullNameCreated)
                {
                    return fullName;
                }
                fullName = "";
                //NodeFile nf = this;
                NodeBase nb = this;
                do
                {
                    fullName = "/" + nb.Name + fullName;
                    if (nb is NodeFile && (nb as NodeFile).isDir && !fullName.EndsWith("/"))
                    {
                        fullName += "/";
                    }
                    nb = nb.Parent;
                }
                while (nb is NodeFile);
                isFullNameCreated = true;
                return fullName;
            }
            internal set
            {
                isFullNameCreated = true;
                fullName = value;
            }
        }

        public void SaveFile(string FileName)
        {
            File.Delete(FileName);
            FileStream outst = File.Create(FileName);
            outst.Write(data, 0, data.Length);
            outst.Close();
            FileSaved = true;
        }

        public void Reset()
        {
            data = null;
            FileSaved = false;
            FileReady = false;
        }
    }
}
