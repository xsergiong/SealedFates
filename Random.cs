namespace SealedFates
{
    using System.Security.Cryptography;

    public class Random
	{
		public Int128 GetInt128(Int128 min, Int128 max)
		{
			ulong minLow = min.Low;
			long minHigh = min.High;
			ulong maxLow = max.Low;
			long maxHigh = max.High;

			int p3, p2, p1, p0;
            
			p3 = RandomNumberGenerator.GetInt32((int)(minHigh >> 32), (int)(maxHigh >> 32));

			if (p3 > (int)(minHigh >> 32))
			{
				p2 = RandomNumberGenerator.GetInt32(0, int.MaxValue);
                p1 = RandomNumberGenerator.GetInt32(0, int.MaxValue);
                p0 = RandomNumberGenerator.GetInt32(0, int.MaxValue);

				return new Int128
				(
					(ulong)(p1 * System.Math.Pow(2, 32) + p0),
					(long)(p3 * System.Math.Pow(2, 32) + p2)
				);
            }

            p2 = RandomNumberGenerator.GetInt32((int)minHigh, (int)maxHigh);

            if (p2 > (int)minHigh)
            {
                p1 = RandomNumberGenerator.GetInt32(0, int.MaxValue);
                p0 = RandomNumberGenerator.GetInt32(0, int.MaxValue);

                return new Int128
                (
                    (ulong)(p1 * System.Math.Pow(2, 32) + p0),
                    (long)(p3 * System.Math.Pow(2, 32) + p2)
                );
            }

            p1 = RandomNumberGenerator.GetInt32((int)(minLow >> 32), (int)(maxLow >> 32));

            if (p1 > (int)(minLow >> 32))
            {
                p0 = RandomNumberGenerator.GetInt32(0, int.MaxValue);

                return new Int128
                (
                    (ulong)(p1 * System.Math.Pow(2, 32) + p0),
                    (long)(p3 * System.Math.Pow(2, 32) + p2)
                );
            }

            p0 = RandomNumberGenerator.GetInt32((int)(minLow), (int)(maxLow));

            return new Int128
            (
               (ulong)(p1 * System.Math.Pow(2, 32) + p0),
               (long)(p3 * System.Math.Pow(2, 32) + p2)
            );
        }

        public Int128 Abs(Int128 value)
        {
            return new Int128(value.Low, System.Math.Abs(value.High));
        }
    }
}