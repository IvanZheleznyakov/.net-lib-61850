namespace lib61850net
{
    public class ReadResponse: IResponse
    {
        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public MmsValue MmsValue { get; internal set; }
    }
}
