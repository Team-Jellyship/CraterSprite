using System;
using System.Numerics;

namespace CraterSprite;
public abstract class CraterMath
{
    /**
     * <summary>Get a number value, moved towards another value.
     * Prevents overshooting the value.
     * </summary>
     * <param name="input">Value to change</param>
     * <param name="destination">Value to reach</param>
     * <param name="rate">Amount to move input towards destination. Should be positive</param>
     */
    public static T MoveTo<T>(T input, T destination, T rate)
        where T : ISignedNumber<T>, IComparable<T>
    {
        var delta = destination - input;
        if (T.Abs(delta).CompareTo(rate) < 0)
        {
            return destination;
        }

        return input + (T.IsNegative(delta) ? -rate : rate);
    }
}