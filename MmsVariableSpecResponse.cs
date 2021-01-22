using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public class MmsVariableSpecResponse : IResponse
    {
        internal MmsVariableSpecResponse() { }

        public DataAccessErrorEnum TypeOfError { get; internal set; } = DataAccessErrorEnum.none;
        public MmsVariableSpecification MmsVariableSpecification { get; internal set; }


    }
}
