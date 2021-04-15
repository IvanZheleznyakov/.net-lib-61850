using System.Collections.Generic;

namespace lib61850net
{
    public class DeviceDirectoryResponse : IResponse
    {
        public List<string> DirectoryNames { get; internal set; }
        public DataAccessErrorEnum TypeOfError { get; internal set; }
    }
}
