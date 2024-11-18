using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

namespace LinearAlgebra.Comparers;

public class SequenceToleranceEqualityComparer<T> : EqualityComparer<IEnumerable<T>> where T : INumber<T>
{
    private ToleranceEqualityComparer<T> _comparer;

    public SequenceToleranceEqualityComparer(T tolerance)
    {
        _comparer = new ToleranceEqualityComparer<T>(tolerance);
    }

    public override bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (x is null && y is null)
            return true;

        if (x is null || y is null)
            return false;

        return x.SequenceEqual(y, _comparer);
    }

    public override int GetHashCode([DisallowNull] IEnumerable<T> obj) => obj.GetHashCode();
}
    
public class ToleranceEqualityComparer<T> : EqualityComparer<T> where T : INumber<T>
{
    private T _tolerance;

    public ToleranceEqualityComparer(T tolerance)
    {
        _tolerance = tolerance;
    }

    public override bool Equals(T? x, T? y)
    {
        if (x is null && y is null)
            return true;
        
        if (x is null || y is null) 
            return false;

        return T.Abs(x - y) < _tolerance;
    }

    public override int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
}
