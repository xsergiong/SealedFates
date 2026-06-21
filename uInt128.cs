namespace SealedFates
{
    public struct uInt128
    {
        public ulong Low;
        public ulong High;

        //Constructor
        public uInt128(ulong low, ulong high)
        {
            Low = low;
            High = high;
        }

        public static readonly uInt128 Zero = new uInt128(0, 0);
        public static readonly uInt128 MinValue = new uInt128(ulong.MinValue, ulong.MinValue);
        public static readonly uInt128 MaxValue = new uInt128(ulong.MaxValue, long.MaxValue);

        #region ARITHMETICAL OPERATORS
        public static uInt128 operator +(uInt128 a, uInt128 b)
        {
            ulong low = a.Low + b.Low;
            ulong overflow = (low < a.Low) ? (ulong)1 : 0;

            ulong high = a.High + b.High + overflow;

            return new uInt128(low, high);
        }
        public static uInt128 operator -(uInt128 a, uInt128 b)
        {
            ulong low = a.Low - b.Low;
            ulong overflow = (low > a.Low) ? (ulong)1 : 0;

            ulong high = a.High - b.High - overflow;

            return new uInt128(low, high);
        }
        public static uInt128 operator *(uInt128 a, uInt128 b)
        {
            ulong aLow = a.Low;
            ulong bLow = b.Low;

            ulong aHigh = a.High;
            ulong bHigh = b.High;

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

            return new uInt128(low, high);
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

        public static uInt128 operator /(uInt128 dividend, uInt128 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            uInt128 quotient = Zero;
            uInt128 remainder = Zero;

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
        public static uInt128 operator %(uInt128 dividend, uInt128 divisor)
        {
            if (divisor == Zero)
                throw new DivideByZeroException();

            uInt128 remainder = Zero;

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
        #endregion

        #region EQUALITY OPERATORS
        public static bool operator ==(uInt128 a, uInt128 b)
        {
            ulong aL = a.Low;
            ulong aH = a.High;
            ulong bL = b.Low;
            ulong bH = b.High;

            if (aL == bL && aH == bH)
                return true;
            else
                return false;
        }
        public static bool operator !=(uInt128 a, uInt128 b)
        {
            ulong aL = a.Low;
            ulong aH = a.High;
            ulong bL = b.Low;
            ulong bH = b.High;

            if (aL != bL || aH != bH)
                return true;
            else
                return false;
        }
        public static bool operator >(uInt128 a, uInt128 b)
        {
            ulong aL = a.Low;
            ulong aH = a.High;
            ulong bL = b.Low;
            ulong bH = b.High;

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
        public static bool operator >=(uInt128 a, uInt128 b)
        {
            ulong aL = a.Low;
            ulong aH = a.High;
            ulong bL = b.Low;
            ulong bH = b.High;

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
        public static bool operator <(uInt128 a, uInt128 b)
        {
            ulong aL = a.Low;
            ulong aH = a.High;
            ulong bL = b.Low;
            ulong bH = b.High;

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
        public static bool operator <=(uInt128 a, uInt128 b)
        {
            ulong aL = a.Low;
            ulong aH = a.High;
            ulong bL = b.Low;
            ulong bH = b.High;

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
            if (obj?.GetType() != typeof(uInt128))
                return false;

            uInt128 newObj = (uInt128)obj;

            if (newObj != this)
                return false;

            return true;
        }
        #endregion

        #region BITWISE OPERATORS
        public static uInt128 operator &(uInt128 a, uInt128 b)
        {
            ulong low = a.Low & b.Low;
            ulong high = a.High & b.High;

            return new uInt128(low, high);
        }
        public static uInt128 operator |(uInt128 a, uInt128 b)
        {
            ulong low = a.Low | b.Low;
            ulong high = a.High | b.High;

            return new uInt128(low, high);
        }
        public static uInt128 operator ^(uInt128 a, uInt128 b)
        {
            ulong low = a.Low ^ b.Low;
            ulong high = a.High ^ b.High;

            return new uInt128(low, high);
        }
        public static uInt128 operator >>(uInt128 a, int b)
        {
            int maxOp = b > 128 ? 128 : b;
            int lowOp = maxOp > 64 ? 64 : b;
            int highOp = maxOp - 64;

            ulong low = a.Low >> lowOp;
            ulong high = a.High >> b - 64;

            ulong carry = (a.High & (ulong)highOp) << (64 - highOp);

            return new uInt128(low | carry, high);
        }
        public static uInt128 operator <<(uInt128 a, int b)
        {
            int lowOp = b > 64 ? 64 : b;
            int highOp = b > 128 ? 128 : b;

            ulong low = a.Low << lowOp;
            ulong high = a.High << b;

            ulong carry = a.Low >> (System.Math.Abs(lowOp - 64));

            return new uInt128(low, high | carry);
        }
        public static uInt128 operator ~(uInt128 value)
        {
            return new uInt128(~value.Low, ~value.High);
        }
        public bool GetBit(int index)
        {
            if (index > 127 || index < 0)
                throw new ArgumentOutOfRangeException();

            if (index < 64)
                return (Low & (1UL << index)) != 0;
            else
                return (High & (1UL << (index - 64))) != 0;
        }
        public void SetBit(int index)
        {
            if (index > 127 || index < 0)
                throw new ArgumentOutOfRangeException();

            if (index < 64)
                Low |= (1UL << index);
            else
                High |= (1UL << (index - 64));
        }
        #endregion

        #region CONVERSION OPERATORS
        public static explicit operator uInt128(ulong value)
        {
            return new uInt128(value, 0);
        }
        public static explicit operator uInt128(long value)
        {
            return new uInt128((ulong)value, 0);
        }
        public static explicit operator uInt128(uint value)
        {
            return new uInt128(value, 0);
        }
        public static explicit operator uInt128(int value)
        {
            return new uInt128((ulong)value, 0);
        }
        public static explicit operator uInt128(Int128 value)
        {
            return new uInt128(value.Low, (ulong)value.High);
        }
        #endregion

        #region CONVERSION FUNCTIONS
        public override readonly string ToString()
        {
            System.Numerics.BigInteger high = (System.Numerics.BigInteger)High;
            System.Numerics.BigInteger low = (System.Numerics.BigInteger)Low;

            System.Numerics.BigInteger value = high * (System.Numerics.BigInteger)System.Math.Pow(2, 64) + low;

            return value.ToString();
        }
        public readonly string ToHex()
        {
            uInt128 number = this;

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