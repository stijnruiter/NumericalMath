using System;
using System.Numerics;

namespace LinearAlgebra.Structures.MatrixStorage;

public interface IMatrixStorage<T> where T : struct, INumber<T>
{
    Span<T> Span { get; }

    int RowCount { get; }
    int ColumnCount { get; }

    T GetElement(int i, int j);
    void SetElement(int i, int j, T value);

    RowVector<T> GetRow(int i);
    RowVector<T> GetRowSlice(int i, int start);
    RowVector<T> GetRowSlice(int i, int start, int length);

    ColumnVector<T> GetColumn(int j);
    ColumnVector<T> GetColumnSlice(int j, int start);
    ColumnVector<T> GetColumnSlice(int j, int start, int length);

    IMatrixStorage<T> Copy();
}