namespace lib61850net
{
    public class CommandTerminationReport
    {
        public string ObjectReference { get; internal set; }
        public ControlErrorEnum ControlError { get; internal set; } = ControlErrorEnum.NoError;
        public ControlAddCauseEnum ControlAddCause { get; internal set; } = ControlAddCauseEnum.ADD_CAUSE_NONE;
    }
}
