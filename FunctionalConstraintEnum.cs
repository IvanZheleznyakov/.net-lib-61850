namespace lib61850net
{
    public enum FunctionalConstraintEnum
    {
        /** Status information */
        ST = 0,
        /** Measurands - analog values */
        MX = 1,
        /** Setpoint */
        SP = 2,
        /** Substitution */
        SV = 3,
        /** Configuration */
        CF = 4,
        /** Description */
        DC = 5,
        /** Setting group */
        SG = 6,
        /** Setting group editable */
        SE = 7,
        /** Service response / Service tracking */
        SR = 8,
        /** Operate received */
        OR = 9,
        /** Blocking */
        BL = 10,
        /** Extended definition */
        EX = 11,
        /** Control */
        CO = 12,
        ALL = 99,
        NONE = -1
    }
}
