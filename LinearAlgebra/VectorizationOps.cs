using LinearAlgebra.Structures;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace LinearAlgebra;


// Vectorization of basic operations
// No checks on array lengths are performed. It is assumed that the calling method handles it.
internal static class VectorizationOps
{
    internal static void Addition<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs, Span<T> result) where T : struct, INumberBase<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] + rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs.Length; i++)
        {
            result[i] = lhs[i] + rhs[i];
        }
    }

    internal static void Subtraction<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs, Span<T> result) where T : struct, INumberBase<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] - rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs.Length; i++)
        {
            result[i] = lhs[i] - rhs[i];
        }
    }

    internal static T DotProduct<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs) where T : struct, INumberBase<T>
    {
        T result = T.Zero;
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs);

        for (int i = 0; i < leftVec.Length; i++)
        {
            result += Vector.Dot(leftVec[i], rightVec[i]);
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs.Length; i++)
        {
            result += lhs[i] * rhs[i];
        }
        return result;
    }

    internal static void ScalarProduct<T>(T scalar, ReadOnlySpan<T> rhs, Span<T> result) where T : struct, INumber<T>
    {
        Vector<T> scalarVec = new Vector<T>(scalar);
        ReadOnlySpan<Vector<T>> vectors = MemoryMarshal.Cast<T, Vector<T>>(rhs);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result);

        for (int i = 0; i < vectors.Length; i++)
        {
            resultVec[i] = vectors[i] * scalarVec;
        }
        for (int i = vectors.Length * Vector<T>.Count; i < rhs.Length; i++)
        {
            result[i] = rhs[i] * scalar;
        }
    }
}
