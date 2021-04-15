using System;

namespace lib61850net
{
    [Flags]
    public enum ReportOptionsEnum
    {
        NONE = 0,
        SEQ_NUM = 1,
        TIME_STAMP = 2,
        REASON_FOR_INCLUSION = 4,
        DATA_SET = 8,
        DATA_REFERENCE = 16,
        BUFFER_OVERFLOW = 32,
        ENTRY_ID = 64,
        CONF_REV = 128,
        SEGMENTATION = 256
    }
}
