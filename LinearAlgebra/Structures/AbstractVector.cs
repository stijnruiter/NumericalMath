using System;
using System.Linq;
using System.Numerics;

namespace LinearAlgebra.Structures;

public abstract class AbstractVector<T> : IRectanglarMatrix<T>, IEquatable<AbstractVector<T>> where T : struct, INumber<T>
{
    protected Memory<T> values;
    internal Span<T> Span => values.Span;
    public int Length { get; }
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }

    public int Stride { get; protected set; }

    public abstract T this[int rowIndex, int columnIndex] { get; set; }

    protected AbstractVector(int count)
    {
        values = new T[count];
        Stride = 1;
        Length = count;
    }

    protected AbstractVector(Memory<T> memory, int stride)
    {
        Stride = stride;
        values = memory;
        Length = (memory.Length + stride - 1) / stride;
    }

    public T this[int index]
    {
        get => values.Span[index * Stride];
        set => values.Span[index * Stride] = value;
    }

    public bool Equals(AbstractVector<T>? other)
    {
        if (other is null)
            return false;

        if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
            return false;

        for (int i = 0; i < values.Length; i++)
        {
            if (this[i] != other[i]) 
                return false;
        }
        return true;
    }

    public override string ToString() => $"Vec{ColumnCount}x{RowCount} [{
        string.Join(", ", Enumerable.Range(0, Length).Select(i => this[i]))}]";
}
