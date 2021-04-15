using System.Collections.Generic;

namespace lib61850net
{
    public class WriteResponse: IResponse
    {
        public List<DataAccessErrorEnum> TypeOfErrors { get; internal set; }
        public List<string> Names { get; internal set; }

        public bool IsErrorExists()
        {
            if (TypeOfErrors == null)
            {
                return true;
            }

            foreach (var error in TypeOfErrors)
            {
                if (error != DataAccessErrorEnum.none)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
