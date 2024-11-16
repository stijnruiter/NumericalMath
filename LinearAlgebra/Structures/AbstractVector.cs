using System;
using System.Linq;
using System.Numerics;

namespace LinearAlgebra.Structures;

public abstract class AbstractVector<T> : IRectanglarMatrix<T>, IEquatable<AbstractVector<T>> where T : struct, INumber<T>
{
    protected Memory<T> values;
    public int Length => values.Length;
    public abstract int RowCount { get; }
    public abstract int ColumnCount { get; }

    public abstract T this[int rowIndex, int columnIndex] { get; set; }

    public AbstractVector(int count)
    {
        values = new T[count];
    }

    public AbstractVector(Memory<T> values) => this.values = values;

    public T this[int index]
    {
        get => values.Span[index];
        set => values.Span[index] = value;
    }

    public bool Equals(AbstractVector<T>? other)
    {
        if (other is null)
            return false;

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && values.Span.SequenceEqual(other.values.Span);
    }

    public Span<T> Span => values.Span;

    public override string ToString() => $"Vec{ColumnCount}x{RowCount} [{string.Join(", ", values)}]";
}
