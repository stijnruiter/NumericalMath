using LinearAlgebra.Exceptions;
using System;
using System.Numerics;

namespace LinearAlgebra.Structures;

public class RowVector<T> : AbstractVector<T> where T : struct, INumber<T>
{
    public RowVector(T[] values) : this(new Memory<T>(values))
    {

    }

    public RowVector(int count) : base(count)
    {
    }

    public RowVector(Memory<T> values) : base(values)
    {
    }

    public RowVector(int count, T scalar) : base(count)
    {
        values.Span.Fill(scalar);
    }

    public override int RowCount => 1;

    public override int ColumnCount => Length;

    public override T this[int rowIndex, int columnIndex]
    {
        get
        {
            if (rowIndex != 0)
                throw new IndexOutOfRangeException();

            return this[columnIndex];
        }

        set
        {
            if (rowIndex != 0)
                throw new IndexOutOfRangeException();

            this[columnIndex] = value;
        }
    }

    public ColumnVector<T> Transpose() => new ColumnVector<T>(values);

    public static explicit operator RowVector<T>(T[] vector) => new RowVector<T>(vector);

    public static RowVector<T> operator +(RowVector<T> lhs, RowVector<T> rhs)
    {
        Assertions.AreSameLength(lhs, rhs);
        RowVector<T> result = new RowVector<T>(lhs.Length);
        VectorizationOps.Addition<T>(lhs.Span, rhs.Span, result.Span);
        return result;
    }

    public static RowVector<T> operator -(RowVector<T> lhs, RowVector<T> rhs)
    {
        Assertions.AreSameLength(lhs, rhs);
        RowVector<T> result = new RowVector<T>(lhs.Length);
        VectorizationOps.Subtraction<T>(lhs.Span, rhs.Span, result.Span);
        return result;
    }

    public static RowVector<T> operator *(T lhs, RowVector<T> rhs)
    {
        RowVector<T> result = new RowVector<T>(rhs.Length);
        VectorizationOps.ScalarProduct(lhs, rhs.Span, result.Span);
        return result;
    }

    public static RowVector<T> operator *(RowVector<T> lhs, T rhs)
    {
        RowVector<T> result = new RowVector<T>(lhs.Length);
        VectorizationOps.ScalarProduct(rhs, lhs.Span, result.Span);
        return result;
    }

    public static T operator *(RowVector<T> lhs, ColumnVector<T> rhs) => VectorizationOps.DotProduct<T>(lhs.Span, rhs.Span);
    
    public RowVector<T> Copy()
    {
        T[] copy = new T[values.Length];
        values.CopyTo(copy);
        return new RowVector<T>(copy);
    }
}