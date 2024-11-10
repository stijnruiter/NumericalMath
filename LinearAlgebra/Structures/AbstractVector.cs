using LinearAlgebra.Exceptions;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace LinearAlgebra.Structures;

public abstract class AbstractVector<T> : IRectanglarMatrix<T>, IEquatable<AbstractVector<T>> where T : struct, INumber<T>
{
    protected T[] values;
    public int Length => values.Length;
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }

    public abstract T this[int rowIndex, int columnIndex] { get; set; }

    public AbstractVector(int count)
    {
        values = new T[count];
    }

    public AbstractVector(T[] values) => this.values = values;

    public T this[int index]
    {
        get => values[index];
        set => values[index] = value;
    }

    public bool Equals(AbstractVector<T>? other)
    {
        if (other is null)
            return false;

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && values.SequenceEqual(other.values);
    }

    public static implicit operator T[](AbstractVector<T> d) => d.values;

    public ReadOnlySpan<T> AsSpan() => values.AsSpan();

    public override string ToString() => $"Vec{ColumnCount}x{RowCount} [{string.Join(", ", values)}]";

    protected static T DotProduct<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs) where T : struct, INumber<T>
    {
        Assertions.AreSameLength(lhs, rhs);
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
}
