using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using LinearAlgebra.Exceptions;
using LinearAlgebra.Structures;

namespace LinearAlgebra.Test;

public static class Helpers
{
    public static bool AreApproxEquals<T>(this Structures.Vector<T> first, Structures.Vector<T> second, T? tolerance = default) where T : INumber<T>
        => AreApproxEquals((T[])first, (T[])second, tolerance);

    public static bool AreApproxEquals<T>(this T[] first, T[] second, T? tolerance = default) where T : INumber<T>
    {
        Assertions.AreSameLength(first, second);

        return first.SequenceEqual(second, new ToleranceComparer<T>(tolerance ?? T.Zero));
    }

    public static bool AreApproxEquals<T>(this T[][] first, T[][] second, T? tolerance = default) where T : INumber<T>
    {
        Assertions.AreSameLength(first, second);
        ToleranceComparer<T> comparer = new ToleranceComparer<T>(tolerance ?? T.Zero);

        for (int i = 0; i < first.Length; i++)
        {
            if (!first[i].SequenceEqual(second[i], comparer))
                return false;
        }
        return true;
    }
}



public class ToleranceComparer<T> : EqualityComparer<T> where T : INumber<T>
{
    private T _tolerance;

    public ToleranceComparer(T tolerance)
    {
        _tolerance = tolerance;
    }

    public override bool Equals(T? x, T? y)
    {
        if (x == null && y == null)
            return true;

        if (x == null || y == null)
            return false;

        return T.Abs(x - y) < _tolerance;
    }

    public override int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
}
