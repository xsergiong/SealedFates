namespace SealedFates
{
    public struct Int128
    {
        public ulong Low;
        public long High;

        //Constructor
        public Int128(ulong low, long high)
        {
            Low = low;
            High = high;
        }

        public static readonly Int128 Zero = new Int128(0, 0);
        public static readonly Int128 MinValue = new Int128(ulong.MaxValue, long.MinValue);
        public static readonly Int128 MaxValue = new Int128(ulong.MaxValue, long.MaxValue);

        #region ARITHMETICAL OPERATORS
        public static Int128 operator +(Int128 a, Int128 b)
        {
            ulong low = a.Low + b.Low;
            long overflow = (low < a.Low) ? 1 : 0;

            long high = a.High + b.High + overflow;

            return new Int128(low, high);
        }
        public void operator +=(Int128 b)
        {
            ulong low = Low + b.Low;
            long overflow = (low < Low) ? 1 : 0;

            long high = High + b.High + overflow;

            Low = low;
            High = high;
        }
        public static Int128 operator -(Int128 a, Int128 b)
        {
            ulong low = a.Low - b.Low;
            long overflow = (low > a.Low) ? 1 : 0;

            long high = a.High - b.High - overflow;

            return new Int128(low, high);
        }
        public void operator -=(Int128 b)
        {
            ulong low = Low - b.Low;
            long overflow = (low > Low) ? 1 : 0;

            long high = High - b.High - overflow;

            Low = low;
            High = high;
        }
        public void operator ++()
        {
            ulong low = Low + 1;
            long overflow = (low < Low) ? 1 : 0;

            long high = High + overflow;

            Low = low;
            High = high;
        }
        public void operator --()
        {
            ulong low = Low - 1;
            long overflow = (low > Low) ? 1 : 0;

            long high = High - overflow;

            Low = low;
            High = high;
        }
        public static Int128 operator *(Int128 a, Int128 b)
        {
            ulong aLow = a.Low;
            ulong bLow = b.Low;

            ulong aHigh = (ulong)a.High;
            ulong bHigh = (ulong)b.High;

            ulong p0Low = aLow * bLow;
            ulong p0High = MulHigh(aLow, bLow);

            ulong p1Low = aLow * bHigh;
            ulong p1High = MulHigh(aLow, bHigh);

            ulong p2Low = aHigh * bLow;
            ulong p2High = MulHigh(aHigh, bLow);

            ulong p3 = aHigh * bHigh;

            ulong low = p0Low;

            ulong carry = p0High;

            ulong mid = p1Low + p2Low + carry;

            //Overflow detection
            if (mid < p1Low)
                carry++;
            if (mid < p2Low)
                carry++;
            if (mid < carry)
                carry++;

            ulong high = p3 + p1High + p2High + carry;

            return new Int128(low, (long)high);
        }
        public void operator *=(Int128 b)
        {
            ulong aLow = Low;
            ulong bLow = b.Low;

            ulong aHigh = (ulong)High;
            ulong bHigh = (ulong)b.High;

            ulong p0Low = aLow * bLow;
            ulong p0High = MulHigh(aLow, bLow);

            ulong p1Low = aLow * bHigh;
            ulong p1High = MulHigh(aLow, bHigh);

            ulong p2Low = aHigh * bLow;
            ulong p2High = MulHigh(aHigh, bLow);

            ulong p3 = aHigh * bHigh;

            ulong low = p0Low;

            ulong carry = p0High;

            ulong mid = p1Low + p2Low + carry;

            //Overflow detection
            if (mid < p1Low)
                carry++;
            if (mid < p2Low)
                carry++;
            if (mid < carry)
                carry++;

            ulong high = p3 + p1High + p2High + carry;

            Low = low;
            High = (long)high;
        }

        public static ulong MulHigh(ulong a, ulong b)
        {
            ulong a0 = (uint)a;
            ulong a1 = a >> 32;
            ulong b0 = (uint)b;
            ulong b1 = b >> 32;

            ulong p0 = a0 * b0;
            ulong p1 = a0 * b1;
            ulong p2 = a1 * b0;
            ulong p3 = a1 * b1;

            ulong mid = (p0 >> 32) + (uint)p1 + (uint)p2;

            return p3 + (mid >> 32);
        }

        public static Int128 operator /(Int128 dividend, Int128 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int128 quotient = Zero;
            Int128 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= 1;

                if (remainder >= divisor)
                {
                    remainder -= divisor;
                    quotient.SetBit(i);
                }
            }

            return quotient;
        }
        public void operator /=(Int128 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int128 dividend = this;

            Int128 quotient = Zero;
            Int128 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= 1;

                if (remainder >= divisor)
                {
                    remainder -= divisor;
                    quotient.SetBit(i);
                }
            }

            Low = quotient.Low;
            High = quotient.High;
        }
        public static Int128 operator %(Int128 dividend, Int128 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int128 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= 1;

                if (remainder >= divisor)
                    remainder -= divisor;
            }

            return remainder;
        }
        public void operator %=(Int128 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            Int128 dividend = this;

            Int128 remainder = Zero;

            for (int i = 127; i >= 0; i--)
            {
                remainder <<= 1;

                if (dividend.GetBit(i))
                    remainder.Low |= 1;

                if (remainder >= divisor)
                    remainder -= divisor;
            }

            Low = remainder.Low;
            High = remainder.High;
        }
        #endregion

        #region EQUALITY OPERATORS
        public static bool operator ==(Int128 a, Int128 b)
        {
            ulong aL = a.Low;
            long aH = a.High;
            ulong bL = b.Low;
            long bH = b.High;

            if (aL == bL && aH == bH)
                return true;
            else
                return false;
        }
        public static bool operator !=(Int128 a, Int128 b)
        {
            ulong aL = a.Low;
            long aH = a.High;
            ulong bL = b.Low;
            long bH = b.High;

            if (aL != bL || aH != bH)
                return true;
            else
                return false;
        }
        public static bool operator >(Int128 a, Int128 b)
        {
            ulong aL = a.Low;
            long aH = a.High;
            ulong bL = b.Low;
            long bH = b.High;

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
        public static bool operator >=(Int128 a, Int128 b)
        {
            ulong aL = a.Low;
            long aH = a.High;
            ulong bL = b.Low;
            long bH = b.High;

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
        public static bool operator <(Int128 a, Int128 b)
        {
            ulong aL = a.Low;
            long aH = a.High;
            ulong bL = b.Low;
            long bH = b.High;

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
        public static bool operator <=(Int128 a, Int128 b)
        {
            ulong aL = a.Low;
            long aH = a.High;
            ulong bL = b.Low;
            long bH = b.High;

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
            if (obj?.GetType() != typeof(Int128))
                return false;

            Int128 newObj = (Int128)obj;

            if (newObj != this)
                return false;

            return true;
        }
        #endregion

        #region BITWISE OPERATORS
        public static Int128 operator &(Int128 a, Int128 b)
        {
            ulong low = a.Low & b.Low;
            long high = a.High & b.High;

            return new Int128(low, high);
        }
        public void operator &=(Int128 b)
        {
            ulong low = Low & b.Low;
            long high = High & b.High;

            Low = low;
            High = high;
        }
        public static Int128 operator |(Int128 a, Int128 b)
        {
            ulong low = a.Low | b.Low;
            long high = a.High | b.High;

            return new Int128(low, high);
        }
        public void operator |=(Int128 b)
        {
            ulong low = Low | b.Low;
            long high = High | b.High;

            Low = low;
            High = high;
        }
        public static Int128 operator ^(Int128 a, Int128 b)
        {
            ulong low = a.Low ^ b.Low;
            long high = a.High ^ b.High;

            return new Int128(low, high);
        }
        public void operator ^=(Int128 b)
        {
            ulong low = Low ^ b.Low;
            long high = High ^ b.High;

            Low = low;
            High = high;
        }
        public static Int128 operator >>(Int128 a, int b)
        {
            int maxOp = b > 128 ? 128 : b;
            int lowOp = maxOp > 64 ? 64 : b;
            int highOp = maxOp - 64;

            ulong low = a.Low >> lowOp;
            long high = a.High >> b - 64;

            ulong carry = (ulong)((a.High & highOp) << (64 - highOp));

            return new Int128(low | carry, high);
        }
        public static Int128 operator <<(Int128 a, int b)
        {
            int lowOp = b > 64 ? 64 : b;
            int highOp = b > 128 ? 128 : b;

            ulong low = a.Low << lowOp;
            long high = a.High << b;

            long carry = (long)a.Low >> (Math.Abs(lowOp - 64));

            return new Int128(low, high | carry);
        }
        public static Int128 operator ~(Int128 value)
        {
            return new Int128(~value.Low, ~value.High);
        }
        public bool GetBit(int index)
        {
            if (index > 127 || index < 0)
                throw new ArgumentOutOfRangeException();

            Int128 a = this;

            byte[] byteArray = a.ToByteArray();

            System.Collections.BitArray bitArray = new System.Collections.BitArray(byteArray);

            return bitArray.Get(index);
        }
        public Int128 SetBit(int index)
        {
            if (index > 127 || index < 0)
                throw new ArgumentOutOfRangeException();

            Int128 a = this;

            long targetBit = ~(1 << index);

            Int128 convertedTargetBit;

            if (index > 63)
                convertedTargetBit = new Int128(0, targetBit);
            else
                convertedTargetBit = new Int128((ulong)targetBit, 0);

            return a & convertedTargetBit;
        }
        #endregion

        #region CONVERSION OPERATORS
        public static explicit operator Int128(ulong value)
        {
            return new Int128(value, 0);
        }
        public static explicit operator Int128(long value)
        {
            return new Int128((ulong)value, 0);
        }
        public static explicit operator Int128(uint value)
        {
            return new Int128(value, 0);
        }
        public static explicit operator Int128(int value)
        {
            return new Int128((ulong)value, 0);
        }
        public static explicit operator Int128(uInt128 value)
        {
            return new Int128(value.Low , (long)value.High);
        }
        #endregion

        #region CONVERSION FUNCTIONS
        public readonly byte[] ToByteArray()
        {
            ulong low = Low;
            long high = High;

            var bytes = new Stack<byte>();

            for (; low > 0; low >>= 8)
                bytes.Push((byte)low);
            for (; high > 0; high >>= 10)
                bytes.Push((byte)high);

            return bytes.ToArray();
        }
        public readonly int[] ToBitArray()
        {
            Int128 number = this;

            byte[] byteArray = number.ToByteArray();

            System.Collections.BitArray bitArray = new(byteArray);

            int[] bits = bitArray.Cast<bool>().Select(bit => bit ? 1 : 0).ToArray();

            return bits;
        }
        public readonly string ToBitString()
        {
            Int128 number = this;

            int[] bitArray = number.ToBitArray();

            string bitString = "";

            for (int i = bitArray.Length; i > 0; i--)
                bitString += bitArray[i].ToString();

            return bitString;
        }
        public override readonly string ToString()
        {
            System.Numerics.BigInteger high = (System.Numerics.BigInteger)High;
            System.Numerics.BigInteger low = (System.Numerics.BigInteger)Low;

            System.Numerics.BigInteger value = high * (System.Numerics.BigInteger)Math.Pow(2, 64) + low;

            return value.ToString();
        }
        public readonly string ToHex()
        {
            Int128 number = this;

            string lowString = number.Low.ToString("X");
            string highString = number.High.ToString("X");

            return highString + lowString;
        }
        #endregion

        public override int GetHashCode()
        {
            return (int)(Low ^ (ulong)High);
        }
    }
}