using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IEDExplorer
{
    public static class TestDecoder
    {
        public static bool GetBitStringFromMmsValue(byte[] buf, int size, int bitPos)
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
    }
}
