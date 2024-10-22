﻿using System.Numerics;
using System.Threading.Tasks;

namespace LinearAlgebra.Structures;


public class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : INumber<T>
{
    // Row-major Matrix
    protected T[][] values;

    public Matrix(int rowCount, int columnCount)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;

        // Initialize the rows
        values = new T[rowCount][];
        for (var i = 0; i < rowCount; i++)
        {
            // Init each row with {columnCount} elements
            values[i] = new T[columnCount];
        }
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
        AssertIsRectangular(values);

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

    private static void AssertIsRectangular(T[][] values)
    {
        for (int i = 1; i < values.Length; i++)
            if (values[i].Length != values[0].Length)
                throw new ArgumentException($"Column length mismatch. Column {i} has length {values[i].Length}, Column 0 has length {values[0].Length}.");
    }

    private static bool IsRectangular(T[][] values)
    {
        for (int i = 1; i < values.Length; i++)
            if (values[i].Length != values[0].Length)
                return false;
        return true;
    }

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

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && values.SequenceEqual(other.values, new SequenceEqualityComparer<T>());
    }

    public override string ToString()
    {
        return $"Mat{ColumnCount}x{RowCount}{Environment.NewLine}{string.Join(",\r\n", values.Select(WriteRow))}";
    }

    private static string WriteRow(T[] row) => string.Join(", ", row.Select(v => $"{v,10:f5}"));


    public static Matrix<T> operator +(Matrix<T> lhs, Matrix<T> rhs) => Arithmetics.Addition(lhs, rhs);
    public static Matrix<T> operator -(Matrix<T> lhs, Matrix<T> rhs) => Arithmetics.Subtraction(lhs, rhs);

    public static Matrix<T> operator *(Matrix<T> lhs, Matrix<T> rhs) => Arithmetics.Product(lhs, rhs);

    //public static Matrix<T> operator *(T lhs, Matrix<T> rhs) => Arithmetics.ScalarProduct(lhs, rhs);
    //public static Matrix<T> operator *(Matrix<T> lhs, T rhs) => Arithmetics.ScalarProduct(rhs, lhs);

    //public ColumnVector<T> Solve(ColumnVector<T> b) => MatrixOperations.SolveUsingLU(this, b);

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
        T trace = T.Zero;
        for (int i = 0; i < RowCount; i++)
        {
            trace += this[i, i];
        }
        return trace;
    }

    public static Matrix<T> Zero<T>(int size) where T : INumber<T> => Zero<T>(size, size);

    public static Matrix<T> Zero<T>(int rowCount, int columnCount) where T : INumber<T> => new Matrix<T>(rowCount, columnCount);
    
    public static Matrix<T> Identity<T>(int size) where T : INumber<T>
    {
        Matrix<T> result = Zero<T>(size);
        for (var i = 0; i < size; i++)
        {
            result[i, i] = T.One;
        }
        return result;
    }
}

internal class SequenceEqualityComparer<T> : EqualityComparer<T[]> where T : INumber<T>
{
    public override bool Equals(T[]? x, T[]? y)
    {
        if (x is null && y is null)
            return true;
        if (x is null || y is null)
            return false;
        return x.SequenceEqual(y);
    }

    public override int GetHashCode(T[] obj) => obj.GetHashCode();
}
