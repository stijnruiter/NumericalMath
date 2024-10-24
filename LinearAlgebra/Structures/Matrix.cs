using LinearAlgebra.Comparers;
using LinearAlgebra.Exceptions;
using System;
using System.Linq;
using System.Numerics;

namespace LinearAlgebra.Structures;

public class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : INumber<T>
{
    // Row-major Matrix
    protected T[][] values;

    public Matrix(int rowCount, int columnCount)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        values = InitValues(rowCount, columnCount);
    }

    public Matrix(T[,] values) : this(values.GetLength(0), values.GetLength(1))
    {
        for (var i = 0; i < RowCount; i++)
        {
            for (var j = 0; j < ColumnCount; j++)
            {
                this[i, j] = values[i, j];
            }
        }
    }

    public Matrix(T[][] values)
    {
        Assertions.IsRectangular(values);

        RowCount = values.Length;
        ColumnCount = values[0].Length;
        this.values = values;
    }

    public T this[int index]
    {
        get => values[index / ColumnCount][index % ColumnCount];
        set => values[index / ColumnCount][index % ColumnCount] = value;
    }

    public T this[int rowIndex, int columnIndex]
    {
        // Matrix is column-major oriented
        get => values[rowIndex][columnIndex];
        set => values[rowIndex][columnIndex] = value;
    }

    public ColumnVector<T> Column(int columnIndex) => (ColumnVector<T>)ColumnArray(columnIndex);
    public T[] ColumnArray(int columnIndex)
    {
        T[] col = new T[RowCount];

        for (int i = 0; i < RowCount; i++)
        {
            col[i] = this[i, columnIndex];
        }

        return col;
    }

    public RowVector<T> Row(int rowIndex) => (RowVector<T>)RowArray(rowIndex);
    public T[] RowArray(int rowIndex) => values[rowIndex];

    public int RowCount { get; }

    public int ColumnCount { get; }

    private static T[][] InitValues(int rows, int columns)
    {
        T[][] values = new T[rows][];
        for (var i = 0; i < rows; i++)
        {
            values[i] = new T[columns];
        }
        return values;
    }

    public T[][] Elements => values;

    public bool Equals(Matrix<T>? other)
    {
        if (other is null)
            return false;

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && values.JaggedSequenceEqual(other.values);
    }

    public override string ToString()
    {
        return $"Mat{ColumnCount}x{RowCount}{Environment.NewLine}{string.Join(",\r\n", values.Select(WriteRow))}";
    }

    private static string WriteRow(T[] row) => string.Join(", ", row.Select(v => $"{v,10:f5}"));


    public static Matrix<T> operator +(Matrix<T> lhs, Matrix<T> rhs) => Arithmetics.Addition(lhs, rhs);
    public static Matrix<T> operator -(Matrix<T> lhs, Matrix<T> rhs) => Arithmetics.Subtraction(lhs, rhs);

    public static Matrix<T> operator *(Matrix<T> lhs, Matrix<T> rhs) => Arithmetics.Product(lhs, rhs);
    public static ColumnVector<T> operator *(Matrix<T> lhs, ColumnVector<T> rhs) => Arithmetics.Product(lhs, rhs);
    public static RowVector<T> operator *(RowVector<T> lhs, Matrix<T> rhs) => Arithmetics.Product(lhs, rhs);

    public static Matrix<T> operator *(T lhs, Matrix<T> rhs) => Arithmetics.ScalarProduct(lhs, rhs);
    public static Matrix<T> operator *(Matrix<T> lhs, T rhs) => Arithmetics.ScalarProduct(rhs, lhs);

    public Matrix<T> Copy()
    {
        T[][] copy = new T[values.Length][];
        for (int i = 0; i < copy.Length; i++)
        {
            copy[i] = (T[])values[i].Clone();
        }
        return new Matrix<T>(copy);
    }

    public void SwapRows(int row1, int row2)
    {
        T[] temp = values[row2];
        values[row2] = values[row1];
        values[row1] = temp;
    }


    public ColumnVector<T> Diagonal()
    {
        T[] values = new T[RowCount];
        for (int i = 0; i < RowCount; i++)
        {
            values[i] = this[i, i];
        }
        return new ColumnVector<T>(values);
    }

    public T Trace()
    {
        T trace = T.AdditiveIdentity;
        for (int i = 0; i < RowCount; i++)
        {
            trace += this[i, i];
        }
        return trace;
    }

    public Matrix<T> Transpose()
    {
        Matrix<T> result = new Matrix<T>(ColumnCount, RowCount);
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                result[j, i] = this[i, j];
            }
        }
        return result;
    }

    public static Matrix<T> Zero(int size) => Zero(size, size);

    public static Matrix<T> Zero(int rowCount, int columnCount) => new Matrix<T>(rowCount, columnCount);
    
    public static Matrix<T> Identity(int size)
    {
        Matrix<T> result = Zero(size);
        for (int i = 0; i < size; i++)
        {
            result[i, i] = T.One;
        }
        return result;
    }

    public static Matrix<T> Tridiagonal(int size, T a_left, T a_center, T a_right)
    {
        Matrix<T> result = Matrix<T>.Zero(size);
        for(int i = 0; i < size; i++)
        {
            if (i > 0)
            {
                result[i, i - 1] = a_left;
            }    
            result[i, i]     = a_center;
            if (i < size - 1)
            {
                result[i, i + 1] = a_right;
            }
        }

        return result;
    }
}
