namespace lib61850net
{
    public class SelectResponse: IResponse
    {
        public bool IsSelected { get; internal set; }
        public DataAccessErrorEnum TypeOfError { get; internal set; }
    }
}
