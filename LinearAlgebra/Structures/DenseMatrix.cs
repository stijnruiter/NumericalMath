using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace LinearAlgebra.Structures;

// Make a Row-Major matrix, but with a single array as data structure
public class DenseMatrix<T> where T : struct, INumber<T>
{
    private T[] _values;
    private int _rows;
    private int _columns;

    protected DenseMatrix(int rows, int columns)
    {
        _columns = columns;
        _rows = rows;
        _values = new T[rows * columns];
    }

    public DenseMatrix(int rows, int columns, T scalar) : this(rows, columns)
    {
        Array.Fill(_values, scalar);
    }

    public DenseMatrix(T[,] values) : this(values.GetLength(0), values.GetLength(1))
    {
        // 2D array T[,] is row major stored, thus a direct copy is possible
        Buffer.BlockCopy(values, 0, _values, 0, values.Length * Unsafe.SizeOf<T>());
    }

    public T At(int i, int j)
    {
        AssertIndexInRange(i, j);
        return _values[i * _columns + j];
    }

    public void At(int i, int j, T value)
    {
        AssertIndexInRange(i, j);
        _values[i * _columns + j] = value;
    }

    public ReadOnlySpan<T> GetRow(int i)
    {
        AssertRowInRange(i);
        return new ReadOnlySpan<T>(_values, i * _columns, _columns);
    }

    public void CopyRow(int i, Span<T> storage)
    {
        GetRow(i).CopyTo(storage);
    }

    public void SetRow(int i, ReadOnlySpan<T> values)
    {
        values.CopyTo(_values.AsSpan().Slice(i * _columns, _columns));
    }

    public void CopyColumn(int j, Span<T> storage)
    {
        AssertColumnInRange(j);
        for (int i = 0; i < _rows; i++)
        {
            storage[i] = At(i, j);
        }
    }

    public T[] GetColumn(int j)
    {
        T[] values = new T[_rows];
        CopyColumn(j, values);
        return values;
    }

    public void SetColumn(int j, ReadOnlySpan<T> values)
    {
        Span<T> valueSpan = _values.AsSpan();
        for (int i = 0; i < _rows; i++)
        {
            valueSpan[i * _columns + j] = values[i];
        }
    }

    public int RowCount => _rows;
    public int ColumnCount => _columns;

    private void AssertIndexInRange(int i, int j)
    {
        if (i < 0 || j < 0 || i >= _rows || j >= _columns)
            throw new IndexOutOfRangeException($"({i},{j}) not an index in {_columns}x{_rows} matrix.");
    }

    private void AssertRowInRange(int i)
    {
        if (i < 0 || i >= _rows)
            throw new IndexOutOfRangeException($"Row {i} not in {_columns}x{_rows} matrix.");
    }

    private void AssertColumnInRange(int j)
    {
        if (j < 0 || j >= _columns)
            throw new IndexOutOfRangeException($"Column {j} not in {_columns}x{_rows} matrix.");
    }

    public override string ToString()
    {
        return $"Mat{RowCount}x{ColumnCount}{Environment.NewLine}{string.Join(",\r\n",
            Enumerable.Range(0, _rows).Select(x => WriteRow(GetRow(x).ToArray())))}";
    }

    private static string WriteRow(T[] row) => string.Join(", ", row.Select(v => $"{v,10:f5}"));
}
