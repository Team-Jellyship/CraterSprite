using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;
using Vector2 = Godot.Vector2;

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

    public static T ClampTowards<T>(T input, T min, T max, T rate)
        where T : ISignedNumber<T>, IComparable<T>
    {
        if (input.CompareTo(max) > 0)
        {
            var delta = input - max;
            if (delta.CompareTo(rate) < 0)
            {
                return max;
            }

            return input - rate;
        }
        if (input.CompareTo(min) < 0)
        {
            var delta = input - min;
            if (delta.CompareTo(-rate) > 0)
            {
                return min;
            }

            return input + rate;
        }

        return input;
    }

    public static T ChooseRandom<T>(List<T> items)
    {
        return items.Count == 0 ? default : items[GD.RandRange(0, items.Count - 1)];
    }

    /**
     * <summary>Helper function to get a direction vector from an angle</summary>
     * <param name="angle">Angle in degrees</param>
     */
    public static Vector2 VectorFromAngle(float angle)
    {
        var rads = Mathf.DegToRad(angle);
        return new Vector2(Mathf.Cos(rads), -Mathf.Sin(rads));
    }

    public static Vector2 RandomDirection(float negativeBounds = -MathF.PI, float positiveBounds = MathF.PI)
    {
        var angle = (float)GD.RandRange(negativeBounds, positiveBounds);
        return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
    }

    public static Vector2 ClampVectorLength(Vector2 vector, float length)
    {
        var vectorLengthSquared = vector.LengthSquared();
        if (vectorLengthSquared > length * length)
        {
            return vector * (length / MathF.Sqrt(vectorLengthSquared));
        }
        return vector;
    }

    public static Vector2 ScaleVectorLength(Vector2 vector, float delta)
    {
        var length = vector.Length();
        var normal = vector / length;

        if (-delta > length)
        {
            return Vector2.Zero;
        }

        return normal * (length + delta);
    }
}