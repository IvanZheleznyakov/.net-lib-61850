using System;
using System.Collections.Generic;
using System.Text;

namespace lib61850net
{
    public enum Validity
    {
        GOOD = 0,
        RESERVED = 1,
        INVALID = 2,
        QUESTIONABLE = 3
    }
    public class Quality
    {
        private UInt16 value;

        private const UInt16 QUALITY_DETAIL_OVERFLOW = 4;
        private const UInt16 QUALITY_DETAIL_OUT_OF_RANGE = 8;
        private const UInt16 QUALITY_DETAIL_BAD_REFERENCE = 16;
        private const UInt16 QUALITY_DETAIL_OSCILLATORY = 32;
        private const UInt16 QUALITY_DETAIL_FAILURE = 64;
        private const UInt16 QUALITY_DETAIL_OLD_DATA = 128;
        private const UInt16 QUALITY_DETAIL_INCONSISTENT = 256;
        private const UInt16 QUALITY_DETAIL_INACCURATE = 512;
        private const UInt16 QUALITY_SOURCE_SUBSTITUTED = 1024;
        private const UInt16 QUALITY_TEST = 2048;
        private const UInt16 QUALITY_OPERATOR_BLOCKED = 4096;
        private const UInt16 QUALITY_DERIVED = 8192;

        public ushort Value => value;

        public override string ToString()
        {
            return GetValidity().ToString();

        }

        public Quality(int bitStringValue)
        {
            value = (UInt16)bitStringValue;
        }

        public Quality()
        {
            value = 0;
        }

        public Validity GetValidity()
        {
            int qualityVal = value & 0x3;

            return (Validity)qualityVal;
        }

        public void SetValidity(Validity validity)
        {
            value = (UInt16)(value & 0xfffc);

            value += (ushort)validity;
        }

        public Validity Validity
        {
            get { return GetValidity(); }
            set { SetValidity(value); }
        }

        public bool Overflow
        {
            get { return ((this.value & QUALITY_DETAIL_OVERFLOW) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_OVERFLOW;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_OVERFLOW));
            }
        }

        public bool OutOfRange
        {
            get { return ((this.value & QUALITY_DETAIL_OUT_OF_RANGE) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_OUT_OF_RANGE;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_OUT_OF_RANGE));
            }
        }

        public bool BadReference
        {
            get { return ((this.value & QUALITY_DETAIL_BAD_REFERENCE) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_BAD_REFERENCE;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_BAD_REFERENCE));
            }
        }

        public bool Oscillatory
        {
            get { return ((this.value & QUALITY_DETAIL_OSCILLATORY) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_OSCILLATORY;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_OSCILLATORY));
            }
        }

        public bool Failure
        {
            get { return ((this.value & QUALITY_DETAIL_FAILURE) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_FAILURE;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_FAILURE));
            }
        }

        public bool OldData
        {
            get { return ((this.value & QUALITY_DETAIL_OLD_DATA) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_OLD_DATA;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_OLD_DATA));
            }
        }

        public bool Inconsistent
        {
            get { return ((this.value & QUALITY_DETAIL_INCONSISTENT) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_INCONSISTENT;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_INCONSISTENT));
            }
        }

        public bool Inaccurate
        {
            get { return ((this.value & QUALITY_DETAIL_INACCURATE) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DETAIL_INACCURATE;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DETAIL_INACCURATE));
            }
        }

        public bool Substituted
        {
            get { return ((this.value & QUALITY_SOURCE_SUBSTITUTED) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_SOURCE_SUBSTITUTED;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_SOURCE_SUBSTITUTED));
            }
        }

        public bool Test
        {
            get { return ((this.value & QUALITY_TEST) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_TEST;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_TEST));
            }
        }

        public bool OperatorBlocked
        {
            get { return ((this.value & QUALITY_OPERATOR_BLOCKED) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_OPERATOR_BLOCKED;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_OPERATOR_BLOCKED));
            }
        }

        public bool Derived
        {
            get { return ((this.value & QUALITY_DERIVED) != 0); }
            set
            {
                if (value)
                    this.value |= QUALITY_DERIVED;
                else
                    this.value = (ushort)((int)this.value & (~QUALITY_DERIVED));
            }
        }

        public static implicit operator Quality(Validity v)
        {
            throw new NotImplementedException();
        }
    }
}
