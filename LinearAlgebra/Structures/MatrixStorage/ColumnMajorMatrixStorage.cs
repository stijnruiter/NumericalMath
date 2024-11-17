using System;
using System.Numerics;

namespace LinearAlgebra.Structures.MatrixStorage;

internal class ColumnMajorMatrixStorage<T> : IMatrixStorage<T> where T : struct, INumber<T>
{
    private Memory<T> _values;

    public Span<T> Span => _values.Span;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public ColumnMajorMatrixStorage(int rowCount, int columnCount)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = new T[rowCount * columnCount];
    }

    public ColumnMajorMatrixStorage(int rowCount, int columnCount, Memory<T> values)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = values;
    }

    public ColumnMajorMatrixStorage(int rowCount, int columnCount, T scalar) : this(rowCount, columnCount)
    {
        _values.Span.Fill(scalar);
    }

    public ColumnMajorMatrixStorage(T[,] values)
    {
        RowCount = values.GetLength(0);
        ColumnCount = values.GetLength(1);
        _values = new T[RowCount * ColumnCount];
        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < ColumnCount; j++)
            {
                SetElement(i, j, values[i, j]);
            }
        }
    }

    public IMatrixStorage<T> Copy()
    {
        Memory<T> values = new T[_values.Length];
        _values.CopyTo(values);
        return new ColumnMajorMatrixStorage<T>(RowCount, ColumnCount, values);
    }

    public ColumnVector<T> GetColumn(int j)
    {
        AssertColumnInRange(j);
        return new ColumnVector<T>(_values.Slice(j * RowCount, RowCount), stride: 1);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start)
    {
        AssertColumnInRange(j);
        return new ColumnVector<T>(_values.Slice(j * RowCount, RowCount).Slice(start), stride: 1);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start, int length)
    {
        AssertColumnInRange(j);
        return new ColumnVector<T>(_values.Slice(j * RowCount, RowCount).Slice(start, length), stride: 1);
    }

    public T GetElement(int i, int j)
    {
        AssertIndexInRange(i, j);
        return _values.Span[i + j * RowCount];
    }

    public RowVector<T> GetRow(int i)
    {
        return new RowVector<T>(_values.Slice(i), stride: RowCount);
    }

    public RowVector<T> GetRowSlice(int i, int start)
    {
        AssertRowInRange(i);

        if (start == ColumnCount)
            return new RowVector<T>(0);

        return new RowVector<T>(_values.Slice(start * RowCount + i), stride: RowCount);
    }

    public RowVector<T> GetRowSlice(int i, int start, int length)
    {
        AssertColumnInRange(i);

        if (start == ColumnCount || length == 0)
            return new RowVector<T>(0);

        return new RowVector<T>(_values.Slice(start * RowCount + i, length * ColumnCount), stride: ColumnCount);
    }

    public void SetElement(int i, int j, T value)
    {
        AssertIndexInRange(i, j);
        _values.Span[i + j * RowCount] = value;
    }

    public void SwapRows(int row1, int row2)
    {
        Span<T> view = _values.Span;
        for (int j = 0; j < ColumnCount; j++)
        {
            (view[row1 + j * RowCount], view[row2 + j * RowCount]) = (view[row2 + j * RowCount], view[row1 + j * RowCount]);
        }
    }

    private void AssertIndexInRange(int i, int j)
    {
        if (i < 0 || j < 0 || i >= RowCount || j >= ColumnCount)
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

}
