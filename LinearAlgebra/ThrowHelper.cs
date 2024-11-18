using LinearAlgebra.Structures;
using LinearAlgebra.Structures.MatrixStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra;

internal static class ThrowHelper
{
    internal static void ThrowIfDifferentLength<T>(ReadOnlySpan<T> span1, ReadOnlySpan<T> span2)
    {
        if (span1.Length != span2.Length)
            throw new ArgumentException($"Span lengths do not match. {span1.Length} != {span2.Length}");
    }

    internal static void ThrowIfDifferentLength<T>(in AbstractVector<T> vector1, in AbstractVector<T> vector2) where T : struct, INumber<T>
    {
        if (vector1.Length != vector2.Length)
            throw new ArgumentException($"Vector lengths do not match. {vector1.Length} != {vector2.Length}");
    }

    internal static void ThrowIfDifferentSize<T>(in Matrix<T> matrix1, in Matrix<T> matrix2) where T : struct, INumber<T>
    {
        if (matrix1.ColumnCount != matrix2.ColumnCount || matrix1.RowCount != matrix2.RowCount)
            throw new ArgumentException($"MatrixDimensions do not match. {matrix1.RowCount}x{matrix1.ColumnCount} != {matrix2.RowCount}x{matrix2.ColumnCount}");
    }

    internal static void ThrowIfOutOfRange<T>(int i, int j, in IMatrixStorage<T> matrix) where T : struct, INumber<T>
    {
        if (i < 0 || j < 0)
            throw new IndexOutOfRangeException($"Index ({i}, {j}) cannot be negative");

        if (i >= matrix.RowCount || j >= matrix.ColumnCount)
            throw new IndexOutOfRangeException($"Index ({i}, {j}) is not in {matrix.RowCount}x{matrix.ColumnCount} matrix");
    }

    internal static void ThrowIfOutOfRange<T>(int i, in AbstractVector<T> vector) where T : struct, INumber<T>
    {
        if (i < 0)
            throw new IndexOutOfRangeException($"Index {i} cannot be negative");

        if (i >= vector.Length)
            throw new IndexOutOfRangeException($"Index {i} is not in {vector.Length}-vectrox");
    }

    internal static void ThrowIfRowOutOfRange<T>(int i, in IMatrixStorage<T> matrix) where T : struct, INumber<T>
    {
        if (i < 0)
            throw new IndexOutOfRangeException($"Row {i} cannot be negative");

        if (i >= matrix.RowCount)
            throw new IndexOutOfRangeException($"Row {i} not in {matrix.RowCount}x{matrix.ColumnCount} matrix");
    }

    internal static void ThrowIfColumnOutOfRange<T>(int j, in IMatrixStorage<T> matrix) where T : struct, INumber<T>
    {
        if (j < 0)
            throw new IndexOutOfRangeException($"Column {j} cannot be negative");

        if (j >= matrix.ColumnCount)
            throw new IndexOutOfRangeException($"Column {j} not in {matrix.RowCount}x{matrix.ColumnCount} matrix");
    }

    internal static void ThrowIfEmpty<T>(IRectanglarMatrix<T> matrix) where T : struct, INumber<T>
    {
        if (matrix.ColumnCount == 0 || matrix.RowCount == 0)
            throw new Exception($"RectanglarMatrix is empty. {matrix.RowCount}x{matrix.ColumnCount}");
    }
}
