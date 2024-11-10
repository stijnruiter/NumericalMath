using System;
using System.Numerics;

namespace LinearAlgebra.Structures;

public class RowVector<T> : AbstractVector<T> where T : struct, INumber<T>
{
    public RowVector(int count) : base(count)
    {
    }

    public RowVector(T[] values) : base(values)
    {
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

    public static RowVector<T> operator +(RowVector<T> lhs, RowVector<T> rhs) => Arithmetics.Addition(lhs, rhs);
    public static RowVector<T> operator -(RowVector<T> lhs, RowVector<T> rhs) => Arithmetics.Subtraction(lhs, rhs);

    public static RowVector<T> operator *(T lhs, RowVector<T> rhs) => Arithmetics.ScalarProduct(lhs, rhs);
    public static RowVector<T> operator *(RowVector<T> lhs, T rhs) => Arithmetics.ScalarProduct(rhs, lhs);

    public static T operator *(RowVector<T> lhs, ColumnVector<T> rhs) => DotProduct(lhs.AsSpan(), rhs.AsSpan());

}