namespace lib61850net
{
    public class RCBResponse: IResponse
    {
        public DataAccessErrorEnum TypeOfError { get; internal set; }
        public ReportControlBlock ReportControlBlock { get; internal set; }
    }
}
