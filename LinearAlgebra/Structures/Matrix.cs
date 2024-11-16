using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace LinearAlgebra.Structures;

public partial class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : struct, INumber<T>
{
    // Row-major Matrix
    protected Memory<T> _values;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public Span<T> Elements => _values.Span;

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

    //public ColumnVector<T> Column(int columnIndex) => (ColumnVector<T>)ColumnArray(columnIndex);

    public ColumnVector<T> Column(int columnIndex)
    {
        AssertColumnInRange(columnIndex);

        T[] col = new T[RowCount];
        for (int i = 0; i < RowCount; i++)
        {
            col[i] = this[i, columnIndex];
        }

        return new ColumnVector<T>(col) ;
    }

    public ColumnVector<T> ColumnSlice(int columnIndex, int start, int length)
    {
        AssertColumnInRange(columnIndex);

        T[] col = new T[length];
        for (int i = start; i < start + length; i++)
        {
            col[i - start] = this[i, columnIndex];
        }

        return new ColumnVector<T>(col);
    }

    public ColumnVector<T> ColumnSlice(int columnIndex, int start)
    {
        AssertColumnInRange(columnIndex);

        T[] col = new T[RowCount - start];
        for (int i = start; i < RowCount; i++)
        {
            col[i - start] = this[i, columnIndex];
        }

        return new ColumnVector<T>(col);
    }

    public RowVector<T> RowView(int rowIndex)
    {
        AssertRowInRange(rowIndex);
        return new RowVector<T>(_values.Slice(rowIndex * ColumnCount, ColumnCount));
    }

    public bool Equals(Matrix<T>? other)
    {
        if (other is null)
            return false;

        return RowCount == other.RowCount && ColumnCount == other.ColumnCount && _values.Span.SequenceEqual(other._values.Span);
    }

    public override string ToString()
    {
        return $"Mat{RowCount}x{ColumnCount}{Environment.NewLine}{string.Join(",\r\n", Enumerable.Range(0, RowCount).Select(x => WriteRow(RowView(x))))}";
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
        Span<T> rowView1 = RowView(row1).Span;
        Span<T> rowView2 = RowView(row2).Span;
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

    public static Matrix<T> Zero(int size) => Zero(size, size);

    public static Matrix<T> Zero(int rowCount, int columnCount) => new Matrix<T>(rowCount, columnCount, T.Zero);

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

    internal Span<T> Span => _values.Span;
}
