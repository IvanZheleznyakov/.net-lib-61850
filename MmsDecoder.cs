using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    internal static class MmsDecoder
    {
        internal static bool GetBitStringFromMmsValue(byte[] buf, int size, int bitPos)
        {
            if (bitPos < size)
            {
                int bytePos = bitPos / 8;

                int bitPosInByte = 7 - (bitPos % 8);

                int bitMask = (1 << bitPosInByte);

                if ((buf[bytePos] & bitMask) > 0)
                    return true;
                else
                    return false;

            }
            else
                return false;
        }

        internal static DateTime DecodeMmsBinaryTime(byte[] binTimeBuf)
        {
            ulong millis;
            ulong days = 0;
            DateTime origin;

            millis = (ulong)(binTimeBuf[0] << 24) +
                     (ulong)(binTimeBuf[1] << 16) +
                     (ulong)(binTimeBuf[2] << 8) +
                     (ulong)(binTimeBuf[3]);
            if (binTimeBuf.Length == 6)
            {
                days = (ulong)(binTimeBuf[4] << 8) +
                       (ulong)(binTimeBuf[5]);
                origin = new DateTime(1984, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            }
            else
            {
                origin = DateTime.UtcNow.Date;
            }

            double dMillis = (double)(millis + days * 24 * 3600 * 1000);
            origin = origin.AddMilliseconds(dMillis);

            return origin.ToLocalTime();
        }

        internal static float DecodeMmsFloat(byte[] floatBuf)
        {
            byte[] tmp = new byte[4];
            tmp[0] = floatBuf[4];
            tmp[1] = floatBuf[3];
            tmp[2] = floatBuf[2];
            tmp[3] = floatBuf[1];
            return BitConverter.ToSingle(tmp, 0);
        }

        internal static double DecodeMmsDouble(byte[] doubleBuf)
        {
            byte[] tmp = new byte[8];
            tmp[0] = doubleBuf[8];
            tmp[1] = doubleBuf[7];
            tmp[2] = doubleBuf[6];
            tmp[3] = doubleBuf[5];
            tmp[4] = doubleBuf[4];
            tmp[5] = doubleBuf[3];
            tmp[6] = doubleBuf[2];
            tmp[7] = doubleBuf[1];
            return BitConverter.ToDouble(tmp, 0);
        }

        internal static DateTime DecodeAsn1Time(string stringTime)
        {
            stringTime = stringTime.Insert(4, "-");
            stringTime = stringTime.Insert(7, "-");
            stringTime = stringTime.Insert(10, "T");
            stringTime = stringTime.Insert(13, ":");
            stringTime = stringTime.Insert(16, ":");

            return DateTime.Parse(stringTime);
        }
    }
}
