using LinearAlgebra.Structures;
using System;
using System.Numerics;

namespace LinearAlgebra.Exceptions;

public static class Assertions
{
    public static void AreSameLength<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs)
    {
        if (lhs.Length != rhs.Length)
            throw new DimensionMismatchException("Not the same length.", lhs.Length, rhs.Length);
    }

    public static void AreSameSize<T>(Matrix<T> lhs, Matrix<T> rhs) where T : struct, INumber<T>
    {
        if (lhs.RowCount != rhs.RowCount)
            throw new DimensionMismatchException("Number of rows do not match.", lhs.RowCount, rhs.RowCount);

        if (lhs.ColumnCount != rhs.ColumnCount)
            throw new DimensionMismatchException("Number of columns do not match.", lhs.ColumnCount, rhs.ColumnCount);
    }
}

public class DimensionMismatchException : Exception
{
    public DimensionMismatchException(string? message) : base(message) { }
    public DimensionMismatchException(string? message, int dim1, int dim2) : base($"{message}\n{dim1} != {dim2}") { }
}
