using System;
using System.Numerics;

namespace LinearAlgebra;

internal static class ElementwiseOps
{
    internal static T DotProduct<T>(ReadOnlySpan<T> lhs, int leftStride, ReadOnlySpan<T> rhs, int rightStride) where T : struct, INumberBase<T>
    {
        T result = T.AdditiveIdentity;
        for (int i = 0; i * leftStride < lhs.Length; i++)
        {
            result += lhs[i * leftStride] * rhs[i * rightStride];
        }
        return result;
    }
}
