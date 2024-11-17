using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LinearAlgebra.Structures.MatrixStorage;

public class RowMajorMatrixStorage<T> : IMatrixStorage<T> where T : struct, INumber<T>
{
    private Memory<T> _values;

    public int RowCount { get; }

    public int ColumnCount { get; }

    public Span<T> Span => _values.Span;

    public RowMajorMatrixStorage(int rowCount, int columnCount)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = new T[rowCount * columnCount];
    }

    public RowMajorMatrixStorage(int rowCount, int columnCount, Memory<T> values)
    {
        RowCount = rowCount;
        ColumnCount = columnCount;
        _values = values;
    }

    public RowMajorMatrixStorage(int rowCount, int columnCount, T scalar) : this(rowCount, columnCount)
    {
        _values.Span.Fill(scalar);
    }

    public RowMajorMatrixStorage(T[,] values)
    {
        RowCount = values.GetLength(0);
        ColumnCount = values.GetLength(1);
        T[] destination = new T[RowCount * ColumnCount];
        Buffer.BlockCopy(values, 0, destination, 0, values.Length * Unsafe.SizeOf<T>());
        _values = destination;
    }

    public ColumnVector<T> GetColumn(int j)
    {
        AssertColumnInRange(j);
        return new ColumnVector<T>(_values.Slice(j), stride: ColumnCount);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start)
    {
        AssertColumnInRange(j);

        if (start == RowCount)
            return new ColumnVector<T>(0);

        return new ColumnVector<T>(_values.Slice(start * ColumnCount + j), stride: ColumnCount);
    }

    public ColumnVector<T> GetColumnSlice(int j, int start, int length)
    {
        AssertColumnInRange(j);

        if (start == RowCount || length == 0)
            return new ColumnVector<T>(0);

        return new ColumnVector<T>(_values.Slice(start * ColumnCount + j, length * ColumnCount), stride: ColumnCount);
    }

    public T GetElement(int i, int j)
    {
        AssertIndexInRange(i, j);
        return _values.Span[(i * ColumnCount) + j];
    }

    public RowVector<T> GetRow(int i)
    {
        AssertRowInRange(i);
        return new RowVector<T>(_values.Slice(i * ColumnCount, ColumnCount), stride: 1);
    }

    public RowVector<T> GetRowSlice(int i, int start)
    {
        throw new NotImplementedException();
    }

    public RowVector<T> GetRowSlice(int i, int start, int length)
    {
        throw new NotImplementedException();
    }

    public void SetElement(int i, int j, T value)
    {
        AssertIndexInRange(i, j);
        _values.Span[(i * ColumnCount) + j] = value;
    }

    public void SwapRows(int row1, int row2)
    {
        Memory<T> rowView1 = _values.Slice(row1 * ColumnCount, ColumnCount);
        Memory<T> rowView2 = _values.Slice(row2 * ColumnCount, ColumnCount);
        Memory<T> tempCopy = new T[rowView1.Length];
        rowView1.CopyTo(tempCopy);
        rowView2.CopyTo(rowView1);
        tempCopy.CopyTo(rowView2);
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

    public IMatrixStorage<T> Copy()
    {
        T[] copyValues = new T[_values.Length];
        _values.CopyTo(copyValues);
        return new RowMajorMatrixStorage<T>(RowCount, ColumnCount, copyValues);
    }
}
