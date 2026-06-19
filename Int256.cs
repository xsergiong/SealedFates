namespace SealedFates
{
    public struct Int256
    {
        public uInt128 Low;
        public Int128 High;

        //Constructor
        public Int256(uInt128 low, Int128 high)
        {
            Low = low;
            High = high;
        }

        public static readonly Int256 Zero = new Int256(new uInt128(0, 0), new Int128(0, 0));
        public static readonly Int256 MinValue = new Int256(uInt128.MaxValue, Int128.MinValue);
        public static readonly Int256 MaxValue = new Int256(uInt128.MaxValue, Int128.MaxValue);

        #region ARITHMETICAL OPERATORS
        public static Int256 operator +(Int256 a, Int256 b)
        {
            uInt128 low = a.Low + b.Low;
            Int128 overflow = (low < a.Low) ? new Int128(1, 0) : Int128.Zero;

            Int128 high = a.High + b.High + overflow;

            return new Int256(low, high);
        }
        public void operator +=(Int256 b)
        {
            uInt128 low = Low + b.Low;
            Int128 overflow = (low < Low) ? new Int128(1, 0) : Int128.Zero;

            Int128 high = High + b.High + overflow;

            Low = low;
            High = high;
        }
        public static Int256 operator -(Int256 a, Int256 b)
        {
            uInt128 low = a.Low - b.Low;
            Int128 overflow = (low > a.Low) ? new Int128(1, 0) : Int128.Zero;

            Int128 high = a.High - b.High - overflow;

            return new Int256(low, high);
        }
        public void operator -=(Int256 b)
        {
            uInt128 low = Low - b.Low;
            Int128 overflow = (low > Low) ? new Int128(1, 0) : Int128.Zero;

            Int128 high = High - b.High - overflow;

            Low = low;
            High = high;
        }
        public void operator ++()
        {
            uInt128 low = Low + new uInt128(1, 0);
            Int128 overflow = (low < Low) ? new Int128(1, 0) : Int128.Zero;

            Int128 high = High + overflow;

            Low = low;
            High = high;
        }
        public void operator --()
        {
            uInt128 low = Low - new uInt128(1, 0);
            Int128 overflow = (low > Low) ? new Int128(1, 0) : Int128.Zero;

            Int128 high = High - overflow;

            Low = low;
            High = high;
        }
        public static Int256 operator *(Int256 a, Int256 b)
        {
            uInt128 aLow = a.Low;
            uInt128 bLow = b.Low;

            uInt128 aHigh = new uInt128(a.High.Low, (ulong)a.High.High);
            uInt128 bHigh = new uInt128(b.High.Low, (ulong)b.High.High);

            uInt128 p0Low = aLow * bLow;
            uInt128 p0High = MulHigh(aLow, bLow);

            uInt128 p1Low = aLow * bHigh;
            uInt128 p1High = MulHigh(aLow, bHigh);

            uInt128 p2Low = aHigh * bLow;
            uInt128 p2High = MulHigh(aHigh, bLow);

            uInt128 p3 = aHigh * bHigh;

            uInt128 low = p0Low;

            uInt128 carry = p0High;

            uInt128 mid = p1Low + p2Low + carry;

            //Overflow detection
            if (mid < p1Low)
                carry++;
            if (mid < p2Low)
                carry++;
            if (mid < carry)
                carry++;

            uInt128 high = p3 + p1High + p2High + carry;

            return new Int256(low, new Int128(high.Low, (long)high.High));
        }
        public void operator *=(Int256 b)
        {
            uInt128 aLow = Low;
            uInt128 bLow = b.Low;

            uInt128 aHigh = new uInt128(High.Low, (ulong)High.High);
            uInt128 bHigh = new uInt128(b.High.Low, (ulong)b.High.High);

            uInt128 p0Low = aLow * bLow;
            uInt128 p0High = MulHigh(aLow, bLow);

            uInt128 p1Low = aLow * bHigh;
            uInt128 p1High = MulHigh(aLow, bHigh);

            uInt128 p2Low = aHigh * bLow;
            uInt128 p2High = MulHigh(aHigh, bLow);

            uInt128 p3 = aHigh * bHigh;

            uInt128 low = p0Low;

            uInt128 carry = p0High;

            uInt128 mid = p1Low + p2Low + carry;

            //Overflow detection
            if (mid < p1Low)
                carry++;
            if (mid < p2Low)
                carry++;
            if (mid < carry)
                carry++;

            uInt128 high = p3 + p1High + p2High + carry;

            Low = low;
            High = (Int128)high;
        }

        public static uInt128 MulHigh(uInt128 a, uInt128 b)
        {
            uInt128 a0 = (uInt128)a.Low;
            uInt128 a1 = (uInt128)a.High;
            uInt128 b0 = (uInt128)b.Low;
            uInt128 b1 = (uInt128)b.High;

            uInt128 p0 = a0 * b0;
            uInt128 p1 = a0 * b1;
            uInt128 p2 = a1 * b0;
            uInt128 p3 = a1 * b1;

            uInt128 mid = (p0 >> 64) + (uInt128)p1.Low + (uInt128)p2.Low;

            return p3 + (mid >> 32);
        }

        public static Int256 operator /(Int256 dividend, Int256 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int256 quotient = Zero;
            Int256 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= (uInt128)1;

                if (remainder >= divisor)
                {
                    remainder -= divisor;
                    quotient.SetBit(i);
                }
            }

            return quotient;
        }
        public void operator /=(Int256 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int256 dividend = this;

            Int256 quotient = Zero;
            Int256 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= (uInt128)1;

                if (remainder >= divisor)
                {
                    remainder -= divisor;
                    quotient.SetBit(i);
                }
            }

            Low = quotient.Low;
            High = quotient.High;
        }
        public static Int256 operator %(Int256 dividend, Int256 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int256 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= (uInt128)1;

                if (remainder >= divisor)
                    remainder -= divisor;
            }

            return remainder;
        }
        public void operator %=(Int256 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int256 dividend = this;

            Int256 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= (uInt128)1;

                if (remainder >= divisor)
                    remainder -= divisor;
            }

            Low = remainder.Low;
            High = remainder.High;
        }
        #endregion

        #region EQUALITY OPERATORS
        public static bool operator ==(Int256 a, Int256 b)
        {
            uInt128 aL = a.Low;
            Int128 aH = a.High;
            uInt128 bL = b.Low;
            Int128 bH = b.High;

            if (aL == bL && aH == bH)
                return true;
            else
                return false;
        }
        public static bool operator !=(Int256 a, Int256 b)
        {
            uInt128 aL = a.Low;
            Int128 aH = a.High;
            uInt128 bL = b.Low;
            Int128 bH = b.High;

            if (aL != bL || aH != bH)
                return true;
            else
                return false;
        }
        public static bool operator >(Int256 a, Int256 b)
        {
            uInt128 aL = a.Low;
            Int128 aH = a.High;
            uInt128 bL = b.Low;
            Int128 bH = b.High;

            if (aH > bH)
                return true;
            else if (aH == bH)
            {
                if (aL > bL)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public static bool operator >=(Int256 a, Int256 b)
        {
            uInt128 aL = a.Low;
            Int128 aH = a.High;
            uInt128 bL = b.Low;
            Int128 bH = b.High;

            if (aH > bH)
                return true;
            else if (aH == bH)
            {
                if (aL > bL)
                    return true;
                else if (aL == bL)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public static bool operator <(Int256 a, Int256 b)
        {
            uInt128 aL = a.Low;
            Int128 aH = a.High;
            uInt128 bL = b.Low;
            Int128 bH = b.High;

            if (aH < bH)
                return true;
            else if (aH == bH)
            {
                if (aL < bL)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        public static bool operator <=(Int256 a, Int256 b)
        {
            uInt128 aL = a.Low;
            Int128 aH = a.High;
            uInt128 bL = b.Low;
            Int128 bH = b.High;

            if (aH < bH)
                return true;
            else if (aH == bH)
            {
                if (aL < bL)
                    return true;
                else if (aL == bL)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj?.GetType() != typeof(Int256))
                return false;

            Int256 newObj = (Int256)obj;

            if (newObj != this)
                return false;

            return true;
        }
        #endregion

        #region BITWISE OPERATORS
        public static Int256 operator &(Int256 a, Int256 b)
        {
            uInt128 low = a.Low & b.Low;
            Int128 high = a.High & b.High;

            return new Int256(low, high);
        }
        public void operator &=(Int256 b)
        {
            uInt128 low = Low & b.Low;
            Int128 high = High & b.High;

            Low = low;
            High = high;
        }
        public static Int256 operator |(Int256 a, Int256 b)
        {
            uInt128 low = a.Low | b.Low;
            Int128 high = a.High | b.High;

            return new Int256(low, high);
        }
        public void operator |=(Int256 b)
        {
            uInt128 low = Low | b.Low;
            Int128 high = High | b.High;

            Low = low;
            High = high;
        }
        public static Int256 operator ^(Int256 a, Int256 b)
        {
            uInt128 low = a.Low ^ b.Low;
            Int128 high = a.High ^ b.High;

            return new Int256(low, high);
        }
        public void operator ^=(Int256 b)
        {
            uInt128 low = Low ^ b.Low;
            Int128 high = High ^ b.High;

            Low = low;
            High = high;
        }
        public static Int256 operator >>(Int256 a, int b)
        {
            int maxOp = b > 128 ? 128 : b;
            int lowOp = maxOp > 64 ? 64 : b;
            int highOp = maxOp - 64;

            uInt128 low = a.Low >> lowOp;
            Int128 high = a.High >> b - 64;

            uInt128 carry = (uInt128)((a.High & (Int128)highOp) << (64 - highOp));

            return new Int256(low | carry, high);
        }
        public static Int256 operator <<(Int256 a, int b)
        {
            int lowOp = b > 64 ? 64 : b;
            int highOp = b > 128 ? 128 : b;

            uInt128 low = a.Low << lowOp;
            Int128 high = a.High << b;

            Int128 carry = (Int128)a.Low >> (Math.Abs(lowOp - 64));

            return new Int256(low, high | carry);
        }
        public static Int256 operator ~(Int256 value)
        {
            return new Int256(~value.Low, ~value.High);
        }
        public bool GetBit(int index)
        {
            if (index > 127 || index < 0)
                throw new ArgumentOutOfRangeException();

            Int256 a = this;

            byte[] byteArray = a.ToByteArray();

            System.Collections.BitArray bitArray = new System.Collections.BitArray(byteArray);

            return bitArray.Get(index);
        }
        public Int256 SetBit(int index)
        {
            if (index > 127 || index < 0)
                throw new ArgumentOutOfRangeException();

            Int256 a = this;

            Int128 targetBit = ~((Int128)1 << index);

            Int256 convertedTargetBit;

            if (index > 63)
                convertedTargetBit = new Int256((uInt128)0, targetBit);
            else
                convertedTargetBit = new Int256((uInt128)targetBit, (Int128)0);

            return a & convertedTargetBit;
        }
        #endregion

        public static explicit operator Int256(uInt128 value)
        {
            return new Int256(value, (Int128)0);
        }
        public static explicit operator Int256(Int128 value)
        {
            return new Int256((uInt128)value, (Int128)0);
        }
        public static explicit operator Int256(ulong value)
        {
            return new Int256((uInt128)value, (Int128)0);
        }
        public static explicit operator Int256(long value)
        {
            return new Int256((uInt128)value, (Int128)0);
        }

        #region CONVERSION FUNCTIONS
        public readonly byte[] ToByteArray()
        {
            uInt128 low = Low;
            Int128 high = High;

            var bytes = new Stack<byte>();

            for (; low > (uInt128)0; low >>= 8)
                bytes.Push((byte)low.Low);
            for (; high > (Int128)0; high >>= 10)
                bytes.Push((byte)high.Low);

            return bytes.ToArray();
        }
        public readonly int[] ToBitArray()
        {
            Int256 number = this;

            byte[] byteArray = number.ToByteArray();

            System.Collections.BitArray bitArray = new(byteArray);

            int[] bits = bitArray.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();

            return bits;
        }
        public readonly string ToBitString()
        {
            Int256 number = this;

            int[] bitArray = number.ToBitArray();

            string bitString = "";

            for (int i = bitArray.Length; i > 0; i--)
                bitString += bitArray[i].ToString();

            return bitString;
        }
        public override readonly string ToString()
        {
            System.Numerics.BigInteger p0 = (System.Numerics.BigInteger)Low.Low;
            System.Numerics.BigInteger p1 = (System.Numerics.BigInteger)Low.High;
            System.Numerics.BigInteger p2 = (System.Numerics.BigInteger)High.Low;
            System.Numerics.BigInteger p3 = (System.Numerics.BigInteger)High.High;

            System.Numerics.BigInteger value =
                p3 * (System.Numerics.BigInteger)Math.Pow(2, 192)
                + p2 * (System.Numerics.BigInteger)Math.Pow(2, 128)
                + p1 * (System.Numerics.BigInteger)Math.Pow(2, 64)
                + p0;

            return value.ToString();
        }
        public readonly string ToHex()
        {
            string p0String = Low.Low.ToString("X");
            string p1String = Low.High.ToString("X");
            string p2String = High.Low.ToString("X");
            string p3String = High.High.ToString("X");

            return p3String + p2String + p1String + p0String;
        }
        #endregion

        public override int GetHashCode()
        {
            return (int)((long)Low.Low ^ (long)Low.High ^ (long)High.Low ^ High.High);
        }
    }
}