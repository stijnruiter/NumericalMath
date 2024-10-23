using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAlgebra.Comparers;

public class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>
{
    public override bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return false;
        return x.SequenceEqual(y);
    }

    public override int GetHashCode(IEnumerable<T> obj) => obj.GetHashCode();
}