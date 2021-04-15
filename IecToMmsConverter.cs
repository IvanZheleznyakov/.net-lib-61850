namespace lib61850net
{
    internal static class IecToMmsConverter
    {
        /// <summary>
        /// Изменение формата имени переменной из формата 61850 в формат MMS
        /// </summary>
        /// <param name="iecAddress">Полное имя переменной в терминах 61850</param>
        /// <param name="FC">Функциональная связь</param>
        /// <returns></returns>
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
            return FC == FunctionalConstraintEnum.NONE ? "" : FC.ToString();
        }
    }
}
