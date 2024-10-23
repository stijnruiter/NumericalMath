using System;

namespace LinearAlgebra.Exceptions;

public static class Assertions
{
    public static void AreSameLength<T>(T[] lhs, T[] rhs)
    {
        if (lhs.Length != rhs.Length)
            throw new DimensionMismatchException("Not the same length.", lhs.Length, rhs.Length);
    }

    public static void AreSameLength<T>(T[][] lhs, T[][] rhs)
    {
        if (lhs.Length != rhs.Length)
            throw new DimensionMismatchException("Number of rows do not match.", lhs.Length, rhs.Length);

        for (var i = 0; i < lhs.Length; i++)
        {
            if (lhs[i].Length != rhs[i].Length)
                throw new DimensionMismatchException($"Number of elements in row {i} do not match.", lhs[i].Length, rhs[i].Length);
        }
    }

    public static void IsRectangular<T>(T[][] values)
    {
        for (int i = 1; i < values.Length; i++)
            if (values[i].Length != values[0].Length)
                throw new DimensionMismatchException($"Column length mismatch. Column {i} has length {values[i].Length}, Column 0 has length {values[0].Length}.");
    }
}

public class DimensionMismatchException : Exception
{
    public DimensionMismatchException(string? message) : base(message) { }
    public DimensionMismatchException(string? message, int dim1, int dim2) : base($"{message}\n{dim1} != {dim2}") { }
}
