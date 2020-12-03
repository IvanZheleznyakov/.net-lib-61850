using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    internal static class IecToMmsConverter
    {
        internal static string ConvertIecAddressToMms(string iecAddress, FunctionalConstraintEnum FC = FunctionalConstraintEnum.NONE)
        {
            string result = iecAddress.Replace('.', '$');
            int index = result.IndexOf('$');
            if (FC != FunctionalConstraintEnum.NONE)
            {
                if (index != -1)
                {
                    return result.Insert(index, "$" + ConvertFCToString(FC));
                }
                else
                {
                    return result + "$" + ConvertFCToString(FC) + "$";
                }
            }
            else
            {
                return result;
            }
        }

        internal static string ConvertFCToString(FunctionalConstraintEnum FC)
        {
            switch (FC)
            {
                case FunctionalConstraintEnum.BL: return "BL";
                case FunctionalConstraintEnum.CF: return "CF";
                case FunctionalConstraintEnum.CO: return "CO";
                case FunctionalConstraintEnum.DC: return "DC";
                case FunctionalConstraintEnum.EX: return "EX";
                case FunctionalConstraintEnum.MX: return "MX";
                case FunctionalConstraintEnum.OR: return "OR";
                case FunctionalConstraintEnum.SE: return "SE";
                case FunctionalConstraintEnum.SG: return "SG";
                case FunctionalConstraintEnum.SP: return "SP";
                case FunctionalConstraintEnum.SR: return "SR";
                case FunctionalConstraintEnum.ST: return "ST";
                case FunctionalConstraintEnum.SV: return "SV";
                case FunctionalConstraintEnum.BR: return "BR";
                case FunctionalConstraintEnum.RP: return "RP";
                default: return "";
            }
            // .net standard 2.1
            //return FC switch 
            //{
            //    FunctionalConstraintEnum.BL => "BL",
            //    FunctionalConstraintEnum.CF => "CF",
            //    FunctionalConstraintEnum.CO => "CO",
            //    FunctionalConstraintEnum.DC => "DC",
            //    FunctionalConstraintEnum.EX => "EX",
            //    FunctionalConstraintEnum.MX => "MX",
            //    FunctionalConstraintEnum.OR => "OR",
            //    FunctionalConstraintEnum.SE => "SE",
            //    FunctionalConstraintEnum.SG => "SG",
            //    FunctionalConstraintEnum.SP => "SP",
            //    FunctionalConstraintEnum.SR => "SR",
            //    FunctionalConstraintEnum.ST => "ST",
            //    FunctionalConstraintEnum.SV => "SV",
            //    FunctionalConstraintEnum.BR => "BR",
            //    FunctionalConstraintEnum.RP => "RP",
            //    _ => "",
            //};
        }
    }
}
