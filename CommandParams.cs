using System;

namespace lib61850net
{
    internal enum CommandType
    {
        SingleCommand,
        DoubleCommand,
        IntegerCommand,
        EnumCommand,
        BinaryStepCommand,
        AnalogueSetpoint,
        AnalogueByBinary,
    }

    internal class CommandParams
    {
        internal CommandType CommType;
        internal MmsTypeEnum DataType;
        internal ControlModelEnum CommandFlowFlag;
        internal object ctlVal;
        internal OriginatorCategoryEnum orCat;
        internal string orIdent;
        internal DateTime T;
        internal bool Test;
        internal bool interlockCheck;
        internal bool synchroCheck;
        internal string Address;
        internal bool SBOrun;
        internal bool SBOdiffTime;
        internal int SBOtimeout;
    }
}
