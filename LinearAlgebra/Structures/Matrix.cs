using LinearAlgebra.Structures.MatrixStorage;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace LinearAlgebra.Structures;

public partial class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : struct, INumber<T>
{
    // Row-major Matrix
    public IMatrixStorage<T> Storage { get; }

    //protected Memory<T> _values;
    internal Span<T> Span => Storage.Span;

    public int RowCount => Storage.RowCount;

    public int ColumnCount => Storage.ColumnCount;

    public Matrix(IMatrixStorage<T> storage)
    {
        Storage = storage;
    }

    internal Matrix(int rowCount, int columnCount, Memory<T> memory)
    {
        Storage = new RowMajorMatrixStorage<T>(rowCount, columnCount, memory);
    }

    public Matrix(int rowCount, int columnCount)
    {
        Storage = new RowMajorMatrixStorage<T>(rowCount, columnCount);
    }

    public Matrix(int rowCount, int columnCount, T scalar)
    {
        Storage = new RowMajorMatrixStorage<T>(rowCount, columnCount, scalar);
    }

    public Matrix(T[,] values) 
    {
        Storage = new RowMajorMatrixStorage<T>(values);
    }

    public T this[int rowIndex, int columnIndex]
    {
        // Matrix is row-major oriented
        get => Storage.GetElement(rowIndex, columnIndex);
        set => Storage.SetElement(rowIndex, columnIndex, value);
    }

    public ColumnVector<T> Column(int columnIndex) => Storage.GetColumn(columnIndex);

    public ColumnVector<T> ColumnSlice(int columnIndex, int start) => Storage.GetColumnSlice(columnIndex, start);

    public ColumnVector<T> ColumnSlice(int columnIndex, int start, int length) => Storage.GetColumnSlice(columnIndex, start, length);

    public RowVector<T> Row(int rowIndex) => Storage.GetRow(rowIndex);

    public bool Equals(Matrix<T>? other)
    {
        if (other is null)
            return false;

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && Storage.Span.SequenceEqual(other.Storage.Span);
    }

    public override string ToString()
    {
        return $"Mat{RowCount}x{ColumnCount}{Environment.NewLine}{string.Join(",\r\n", Enumerable.Range(0, RowCount).Select(x => WriteRow(Row(x))))}";
    }

    private static string WriteRow(RowVector<T> row)
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(T element in row.Span)
        {
            stringBuilder.Append($"{element, 10:f5}, ");
        }
        return stringBuilder.ToString();
    }

    public Matrix<T> Copy() => new Matrix<T>(Storage.Copy());

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

    public static Matrix<T> Random(int rows, int columns)
    {
        Random random = new Random();
        Matrix<T> matrix = new Matrix<T>(rows, columns, random.RandomNumbers<T>(rows * columns));
        return matrix;
    }

    public static Matrix<T> Zero(int size) => Zero(size, size);

    public static Matrix<T> Zero(int rowCount, int columnCount) => new Matrix<T>(rowCount, columnCount, T.Zero);

    public static Matrix<T> One(int size) => One(size, size);

    public static Matrix<T> One(int rowCount, int columnCount) => new Matrix<T>(rowCount, columnCount, T.One);

    public static Matrix<T> Identity(int size) => Matrix<T>.Diagonal(size, T.One);

    public static Matrix<T> Tridiagonal(int size, T a_left, T a_center, T a_right)
    {
        Matrix<T> result = Matrix<T>.Zero(size);
        for (int i = 0; i < size; i++)
        {
            if (i > 0)
            {
                result[i, i - 1] = a_left;
            }
            result[i, i] = a_center;
            if (i < size - 1)
            {
                result[i, i + 1] = a_right;
            }
        }

        return result;
    }

    public static Matrix<T> Diagonal(int size, T diagonal)
    {
        Matrix<T> result = Matrix<T>.Zero(size);
        for (int i = 0; i < size; i++)
        {
            result[i, i] = diagonal;
        }
        return result;
    }

    public T DiagonalProduct()
    {
        ReadOnlySpan<T> values = Storage.Span;
        T product = T.MultiplicativeIdentity;
        for (int i = 0; i < ColumnCount; i++)
        {
            product *= values[(i * ColumnCount) + i];
        }
        return product;
    }
}
