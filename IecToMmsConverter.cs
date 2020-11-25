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
            if (index != -1)
            {
                return result.Insert(index, "$" + ConvertFCToString(FC));
            }
            else if (FC != FunctionalConstraintEnum.NONE)
            {
                return result + "$" + ConvertFCToString(FC) + "$";
            }
            else
            {
                return result;
            }
        }

        internal static string ConvertFCToString(FunctionalConstraintEnum FC)
        {
            return FC switch
            {
                FunctionalConstraintEnum.BL => "BL",
                FunctionalConstraintEnum.CF => "CF",
                FunctionalConstraintEnum.CO => "CO",
                FunctionalConstraintEnum.DC => "DC",
                FunctionalConstraintEnum.EX => "EX",
                FunctionalConstraintEnum.MX => "MX",
                FunctionalConstraintEnum.OR => "OR",
                FunctionalConstraintEnum.SE => "SE",
                FunctionalConstraintEnum.SG => "SG",
                FunctionalConstraintEnum.SP => "SP",
                FunctionalConstraintEnum.SR => "SR",
                FunctionalConstraintEnum.ST => "ST",
                FunctionalConstraintEnum.SV => "SV",
                _ => "",
            };
        }
    }
}
