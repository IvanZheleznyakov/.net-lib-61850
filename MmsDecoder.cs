using org.bn.types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib61850net
{
    public static class MmsDecoder
    {
        internal static BitString CreateBitStringFromInteger(int val)
        {
            BitArray ba = new BitArray(BitConverter.GetBytes(val));
            byte[] byteVal = null;
            byte lastBit = 0;
            for (int i = ba.Length - 1; i != -1; --i)
            {
                if (ba[i])
                {
                    lastBit = (byte)(i + 1);
                    break;
                }
            }
            BitString output = null;
            if (lastBit <= 8)
                byteVal = new byte[] { 0 };
            else if (lastBit <= 16)
                byteVal = new byte[] { 0, 0 };
            else if (lastBit <= 24)
                byteVal = new byte[] { 0, 0, 0 };
            else
                byteVal = new byte[] { 0, 0, 0, 0 };

            byte mask = 1;

            for (int i = 0; i < lastBit; i++)
            {
                if ((i % 8) == 0)
                    mask = 0x80;
                else
                    mask >>= 1;


                if (ba[i])
                {
                    byteVal[i / 8] |= mask;
                }
            }

            output = new BitString();
            output.Value = byteVal;
            output.TrailBitsCnt = byteVal.Length * 8 - ((lastBit / 8) * 8) - lastBit % 8;

            return output;
        }

        internal static int GetPadding(byte[] buf)
        {
            // int a = BitConverter.
            //  BitArray ba = new BitArray(BitConverter.GetBytes(a));
            BitArray ba = new BitArray(buf);
            byte lastBit = 0;
            for (int i = ba.Length - 1; i != -1; --i)
            {
                if (ba[i])
                {
                    lastBit = (byte)(i + 1);
                    break;
                }
            }

            return (buf.Length * 8 - ((lastBit / 8) * 8) - lastBit % 8);
        }

        internal static bool GetBitStringFromMmsValue(byte[] buf, int size, int bitPos)
        {
            if (bitPos < size)
            {
                int[] bitString = new int[buf.Length];

                for (int i = 0; i != buf.Length; ++i)
                {
                    bitString[i] = ~buf[i];
                }

                int bytePos = bitPos / 8;

                int bitPosInByte = 7 - (bitPos % 8);

                int bitMask = (1 << bitPosInByte);

                if ((bitString[bytePos] & bitMask) > 0)
                    return true;
                else
                    return false;

            }
            else
                return false;
        }

        private static void SetBitStringBit(ref byte[] buffer, int size, int bitPos, bool value)
        {
            if (bitPos < size)
            {
                int bytePos = bitPos / 8;
                int bitPosInByte = 7 - (bitPos % 8);
                int bitMask = (1 << bitPosInByte);
                
                if (value)
                {
                    buffer[bytePos] |= (byte)bitMask;
                }
                else
                {
                    buffer[bytePos] &= (byte)(~bitMask);
                }
            }
        }

        public static byte[] GetBitStringFromInteger(int size, ulong value)
        {
            byte[] result;
            if (size <= 8)
            {
                result = new byte[] { 0 };
            }
            else if (size <= 16)
            {
                result = new byte[] { 0, 0 };
            }
            else if (size <= 24)
            {
                result = new byte[] { 0, 0, 0 };
            }
            else
            {
                result = new byte[] { 0, 0, 0, 0 };
            }

            for (int bitPos = size - 1; bitPos != -1; --bitPos)
            {
                SetBitStringBit(ref result, size, bitPos, ((value & 1) == 1));
                value >>= 1;
            }

            return result;
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
            float result = 0.0F;
            byte[] tmp = new byte[4];
            tmp[0] = floatBuf[4];
            tmp[1] = floatBuf[3];
            tmp[2] = floatBuf[2];
            tmp[3] = floatBuf[1];
            result = BitConverter.ToSingle(tmp, 0);
            return result;
        }

        internal static double DecodeMmsDouble(byte[] doubleBuf)
        {
            double result = 0.0;
            byte[] tmp = new byte[8];
            tmp[0] = doubleBuf[8];
            tmp[1] = doubleBuf[7];
            tmp[2] = doubleBuf[6];
            tmp[3] = doubleBuf[5];
            tmp[4] = doubleBuf[4];
            tmp[5] = doubleBuf[3];
            tmp[6] = doubleBuf[2];
            tmp[7] = doubleBuf[1];
            result = BitConverter.ToDouble(tmp, 0);
            return result;
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
