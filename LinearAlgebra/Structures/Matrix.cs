using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace LinearAlgebra.Structures;

public partial class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : struct, INumber<T>
{
    // Row-major Matrix
    protected Memory<T> _values;
    internal Span<T> Span => _values.Span;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public Matrix(int rowCount, int columnCount, Memory<T> values)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = values;
    }

    public Matrix(int rowCount, int columnCount)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = new T[rowCount * columnCount];
    }

    public Matrix(int rowCount, int columnCount, T scalar) : this(rowCount, columnCount)
    {
        _values.Span.Fill(scalar);
    }

    public Matrix(T[,] values) 
    {
        RowCount = values.GetLength(0);
        ColumnCount = values.GetLength(1);
        T[] destination = new T[RowCount * ColumnCount];
        Buffer.BlockCopy(values, 0, destination, 0, values.Length * Unsafe.SizeOf<T>());
        _values = destination;
    }

    public T this[int rowIndex, int columnIndex]
    {
        // Matrix is column-major oriented
        get
        {
            AssertIndexInRange(rowIndex, columnIndex);
            return _values.Span[(rowIndex * ColumnCount) + columnIndex];
        }

        set
        {
            AssertIndexInRange(rowIndex, columnIndex);
            _values.Span[(rowIndex * ColumnCount) + columnIndex] = value;
        }
    }

    public ColumnVector<T> Column(int columnIndex)
    {
        AssertColumnInRange(columnIndex);
        return new ColumnVector<T>(_values.Slice(columnIndex), stride: ColumnCount);
    }

    public ColumnVector<T> ColumnSlice(int columnIndex, int start)
    {
        AssertColumnInRange(columnIndex);

        if (start == RowCount)
            return new ColumnVector<T>(0);

        return new ColumnVector<T>(_values.Slice(start * ColumnCount + columnIndex), stride: ColumnCount);
    }

    public ColumnVector<T> ColumnSlice(int columnIndex, int start, int length)
    {
        AssertColumnInRange(columnIndex);
        if (start == RowCount || length == 0)
            return new ColumnVector<T>(0);
        return new ColumnVector<T>(_values.Slice(start * ColumnCount + columnIndex, length * ColumnCount), stride: ColumnCount);
    }


    public RowVector<T> Row(int rowIndex)
    {
        AssertRowInRange(rowIndex);
        return new RowVector<T>(_values.Slice(rowIndex * ColumnCount, ColumnCount), stride: 1);
    }

    public bool Equals(Matrix<T>? other)
    {
        if (other is null)
            return false;

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && _values.Span.SequenceEqual(other._values.Span);
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

    public Matrix<T> Copy()
    {
        T[] copy = new T[_values.Length];
        _values.CopyTo(copy);
        return new Matrix<T>(RowCount, ColumnCount, copy);
    }

    public void SwapRows(int row1, int row2)
    {
        Span<T> rowView1 = Row(row1).Span;
        Span<T> rowView2 = Row(row2).Span;
        Span<T> tempCopy = new T[rowView1.Length];
        rowView1.CopyTo(tempCopy);
        rowView2.CopyTo(rowView1);
        tempCopy.CopyTo(rowView2);
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

    public static Matrix<T>? Random(int rows, int columns)
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

    private void AssertIndexInRange(int i, int j)
    {
        if (i < 0 || j < 0 || i >= RowCount|| j >= ColumnCount)
            throw new IndexOutOfRangeException($"({i},{j}) not an index in {ColumnCount}x{RowCount} matrix.");
    }

    private void AssertRowInRange(int i)
    {
        if (i < 0 || i >= RowCount)
            throw new IndexOutOfRangeException($"Row {i} not in {ColumnCount}x{RowCount} matrix.");
    }

    private void AssertColumnInRange(int j)
    {
        if (j < 0 || j >= ColumnCount)
            throw new IndexOutOfRangeException($"Column {j} not in {ColumnCount}x{RowCount} matrix.");
    }

    /// <summary>
    /// Get largest absolute column value, e.g. max(abs(matrix[startIndex.. , columnIndex]))
    /// </summary>
    /// <param name="matrix">The searchmatrix</param>
    /// <returns>The max abs value and its index</returns>
    public (T value, int index) GetAbsMaxElementInColumn(int columnIndex, int startIndex = 0) 
    {
        // Start with first element
        ReadOnlySpan<T> values = Span;
        int maxIndex = startIndex;
        T maxValue = T.Abs(values[(startIndex * ColumnCount) + columnIndex]);
        T nextValue;

        // Loop over the rest of the rows
        for (int i = startIndex + 1; i < RowCount; i++)
        {
            if ((nextValue = T.Abs(values[(i * ColumnCount) + columnIndex])) > maxValue)
            {
                maxValue = nextValue;
                maxIndex = i;
            }
        }
        return (maxValue, maxIndex);
    }

    public T DiagonalProduct()
    {
        ReadOnlySpan<T> values = _values.Span;
        T product = T.MultiplicativeIdentity;
        for (int i = 0; i < ColumnCount; i++)
        {
            product *= values[(i * ColumnCount) + i];
        }
        return product;
    }
}
