using LinearAlgebra.Exceptions;
using System;
using System.Numerics;

namespace LinearAlgebra.Structures;

public class ColumnVector<T> : AbstractVector<T> where T : struct, INumber<T>
{
    public ColumnVector(T[] values) : this (new Memory<T>(values))
    { }

    public ColumnVector(int count) : base(count) { }

    public ColumnVector(Memory<T> values) : base(values) { }

    public ColumnVector(int count, T scalar) : base(count)
    {
        values.Span.Fill(scalar);
    }

    public override int RowCount => Length;

    public override int ColumnCount => 1;

    public override T this[int rowIndex, int columnIndex]
    {
        get
        {
            if (columnIndex != 0)
                throw new IndexOutOfRangeException();

            return this[rowIndex];
        }

        set
        {
            if (columnIndex != 0)
                throw new IndexOutOfRangeException();

            this[rowIndex] = value;
        }
    }

    public RowVector<T> Transpose() => new RowVector<T>(values);

    public static explicit operator ColumnVector<T>(T[] vector) => new ColumnVector<T>(vector);

    public static ColumnVector<T> operator +(ColumnVector<T> lhs, ColumnVector<T> rhs)
    {
        Assertions.AreSameLength<T>(lhs.Span, rhs.Span);
        ColumnVector<T> result = new ColumnVector<T>(lhs.Length);
        VectorizationOps.Addition<T>(lhs.Span, rhs.Span, result.Span);
        return result;
    }

    public static ColumnVector<T> operator -(ColumnVector<T> lhs, ColumnVector<T> rhs)
    {
        Assertions.AreSameLength<T>(lhs.Span, rhs.Span);
        ColumnVector<T> result = new ColumnVector<T>(lhs.Length);
        VectorizationOps.Subtraction<T>(lhs.Span, rhs.Span, result.Span);
        return result;
    }

    public static ColumnVector<T> operator *(T lhs, ColumnVector<T> rhs)
    {
        ColumnVector<T> result = new ColumnVector<T>(rhs.Length);
        VectorizationOps.ScalarProduct(lhs, rhs.Span, result.Span);
        return result;
    }

    public static ColumnVector<T> operator *(ColumnVector<T> lhs, T rhs)
    {
        ColumnVector<T> result = new ColumnVector<T>(lhs.Length);
        VectorizationOps.ScalarProduct(rhs, lhs.Span, result.Span);
        return result;
    }

    public static Matrix<T> operator *(ColumnVector<T> lhs, RowVector<T> rhs) => Arithmetics.OuterProduct(lhs, rhs);

    public static ColumnVector<T> Zero(int size) => new ColumnVector<T>(size);
    
    public ColumnVector<T> Copy()
    {
        T[] copy = new T[values.Length];
        values.CopyTo(copy);
        return new ColumnVector<T>(copy);
    }
}