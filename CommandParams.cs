using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEDExplorer
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
        public CommandType CommType;
        public MmsTypeEnum DataType;
        public ControlModelEnum CommandFlowFlag;
        public object ctlVal;
        public OriginatorCategoryEnum orCat;
        public string orIdent;
        public DateTime T;
        public bool Test;
        public bool interlockCheck;
        public bool synchroCheck;
        public string Address;
        public bool SBOrun;
        public bool SBOdiffTime;
        public int SBOtimeout;
    }
}
