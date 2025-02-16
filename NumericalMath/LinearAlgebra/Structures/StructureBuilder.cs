using NumericalMath.LinearAlgebra.Structures.MatrixStorage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace NumericalMath.LinearAlgebra.Structures;

public static class StructureBuilder
{
    public static RowVector<T> CreateRowVector<T>(ReadOnlySpan<T> values) where T : struct, INumber<T>
    {
        return new RowVector<T>(values.ToArray());
    }
    public static ColumnVector<T> CreateColumnVector<T>(ReadOnlySpan<T> values) where T : struct, INumber<T>
    {
        return new ColumnVector<T>(values.ToArray());
    }
    public static Matrix<T> CreateMatrix<T>(ReadOnlySpan<RowVector<T>> rows) where T : struct, INumber<T>
    {
        if (rows.Length == 0)
            return Matrix<T>.Empty;

        return new Matrix<T>(new RowMajorMatrixStorage<T>(rows));
    }
}

public class VectorEnumerator<T> : IEnumerator<T> where T : struct, INumber<T>
{
    private int position;
    private readonly AbstractVector<T> vector;
    public VectorEnumerator(AbstractVector<T> vec)
    {
        vector = vec;
        position = -1;
    }

    public T Current => vector[position];

    object IEnumerator.Current => vector[position];

    public void Dispose() { }

    public bool MoveNext()
    {
        position += 1;
        if (position < vector.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset()
    {
        position = -1;
    }
}

public class MatrixEnumerator<T> : IEnumerator<RowVector<T>> where T : struct, INumber<T>
{
    private int position;
    private readonly Matrix<T> matrix;
    public MatrixEnumerator(Matrix<T> matrix)
    {
        this.matrix = matrix;
        position = -1;
    }

    public RowVector<T> Current => matrix.Row(position);

    object IEnumerator.Current => matrix.Row(position);

    public void Dispose() { }

    public bool MoveNext()
    {
        position += 1;
        if (position < matrix.RowCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Reset()
    {
        position = -1;
    }
}