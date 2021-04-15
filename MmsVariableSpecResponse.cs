namespace lib61850net
{
    public class MmsVariableSpecResponse : IResponse
    {
        internal MmsVariableSpecResponse() { }

        public DataAccessErrorEnum TypeOfError { get; internal set; } = DataAccessErrorEnum.none;
        public MmsVariableSpecification MmsVariableSpecification { get; internal set; }


    }
}
