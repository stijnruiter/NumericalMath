using System;
using System.Numerics;

namespace LinearAlgebra.Structures;

public class ColumnVector<T> : Vector<T> where T : INumber<T>
{
    public ColumnVector(int count) : base(count) { }

    public ColumnVector(T[] values) : base(values) { }

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

    public static ColumnVector<T> operator +(ColumnVector<T> lhs, ColumnVector<T> rhs) => Arithmetics.Addition(lhs, rhs);
    public static ColumnVector<T> operator -(ColumnVector<T> lhs, ColumnVector<T> rhs) => Arithmetics.Subtraction(lhs, rhs);

    public static ColumnVector<T> operator *(T lhs, ColumnVector<T> rhs) => Arithmetics.ScalarProduct(lhs, rhs);
    public static ColumnVector<T> operator *(ColumnVector<T> lhs, T rhs) => Arithmetics.ScalarProduct(rhs, lhs);

    public static Matrix<T> operator *(ColumnVector<T> lhs, RowVector<T> rhs) => Arithmetics.OuterProduct(lhs, rhs);

    public static ColumnVector<T> Zero(int size) => new ColumnVector<T>(size);
}