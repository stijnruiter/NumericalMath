using System;
using System.Linq;
using System.Numerics;
using LinearAlgebra.Exceptions;
using LinearAlgebra.Structures;

namespace LinearAlgebra;

/// <summary>
/// Internal class with all vector/matrix arithmetics
/// Note; this is just a draft version, implemented but with no optimizations
/// TODO: less data copying, simd operations, Span support, etc.
/// </summary>
public static class Arithmetics
{
    private static T[] ElementwiseOperation<T>(ReadOnlySpan<T> lhs, ReadOnlySpan<T> rhs, Func<T, T, T> op) where T : struct, INumber<T>
    {
        Assertions.AreSameLength(lhs, rhs);
        T[] result = new T[lhs.Length];
        for (var i = 0; i < lhs.Length; i++)
        {
            result[i] = op(lhs[i], rhs[i]);
        }
        return result;
    }

    private static T[] ElementwiseOperation<T>(T lhs, ReadOnlySpan<T> rhs, Func<T, T, T> op) where T : struct, INumber<T>
    {
        T[] result = new T[rhs.Length];
        for (var i = 0; i < rhs.Length; i++)
        {
            result[i] = op(lhs, rhs[i]);
        }
        return result;
    }

    public static RowVector<T> ElementwiseProduct<T>(RowVector<T> lhs, RowVector<T> rhs) where T : struct, INumber<T>
    {
        Memory<T> result = ElementwiseOperation<T>(lhs.Span, rhs.Span, (a, b) => a * b);
        return new RowVector<T>(result);
    }

    public static ColumnVector<T> ElementwiseProduct<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T : struct, INumber<T>
    {
        Memory<T> result = ElementwiseOperation<T>(lhs.Span, rhs.Span, (a, b) => a * b);
        return new ColumnVector<T>(result);
    }

    public static Matrix<T> ElementwiseProduct<T>(Matrix<T> lhs, Matrix<T> rhs) where T : struct, INumber<T>
    {
        Memory<T> result = ElementwiseOperation<T>(lhs.Span, rhs.Span, (a, b) => a * b);
        return new Matrix<T>(lhs.RowCount, lhs.ColumnCount, result);
    }

    public static RowVector<T> ElementwiseDivision<T>(RowVector<T> lhs, RowVector<T> rhs) where T : struct, INumber<T>
    {
        Memory<T> result = ElementwiseOperation<T>(lhs.Span, rhs.Span, (a, b) => a / b);
        return new RowVector<T>(result);
    }

    public static ColumnVector<T> ElementwiseDivision<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T : struct, INumber<T>
    {
        Memory<T> result = ElementwiseOperation<T>(lhs.Span, rhs.Span, (a, b) => a / b);
        return new ColumnVector<T>(result);
    }

    public static Matrix<T> ElementwiseDivision<T>(Matrix<T> lhs, Matrix<T> rhs) where T : struct, INumber<T>
    {
        Memory<T> result = ElementwiseOperation<T>(lhs.Span, rhs.Span, (a, b) => a / b);
        return new Matrix<T>(lhs.RowCount, lhs.ColumnCount, result);
    }

    public static Matrix<T> OuterProduct<T>(ColumnVector<T> lhs, RowVector<T> rhs) where T : struct, INumber<T>
    {
        Assertions.AreSameLength<T>(lhs.Span, rhs.Span);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, rhs.ColumnCount);
        for (var i = 0; i < lhs.Length; i++)
        {
            RowVector<T> resultRow = result.RowView(i);
            ElementwiseOperation(lhs[i], rhs.Span, (a, b) => a * b).CopyTo(resultRow.Span);
        }
        return result;
    }

    public static IRectanglarMatrix<T> TensorProduct<T>(IRectanglarMatrix<T> left, IRectanglarMatrix<T> right) where T : struct, INumber<T>
    {
        int rowCount = left.RowCount * right.RowCount;
        int columnCount = left.ColumnCount * right.ColumnCount;
        IRectanglarMatrix<T> result =
            rowCount == 1 ? new RowVector<T>(columnCount) :
            columnCount == 1 ? new ColumnVector<T>(rowCount) :
            new Matrix<T>(rowCount, columnCount);

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                result[i, j] = left[i / right.RowCount, j / right.ColumnCount] * right[i % right.RowCount, j % right.ColumnCount];
            }
        }
        return result;
    }

    public static T Norm2<T>(AbstractVector<T> vector) where T : struct, INumber<T>, IRootFunctions<T>
    {
        T result = T.AdditiveIdentity;

        for (int i = 0; i < vector.Length; i++)
        {
            result += vector[i] * vector[i];
        }
        return T.Sqrt(result);
    }

    public static T Norm1<T>(AbstractVector<T> vector) where T : struct, INumber<T>
    {
        T result = T.AdditiveIdentity;

        for (int i = 0; i < vector.Length; i++)
        {
            result += T.Abs(vector[i]);
        }
        return result;
    }

    public static T? NormInf<T>(AbstractVector<T> vector) where T : struct, INumber<T> => Max<T>(vector.Span, T.Abs);

    public static T? Max<T>(ReadOnlySpan<T> span, Func<T, T> op) where T : struct, IComparisonOperators<T, T, bool>
    {
        if (span.Length == 0)
            return null;

        T max = op(span[0]);
        T newValue;
        for (int i = 0; i < span.Length; i++)
        {
            if ((newValue = op(span[i])) > max)
                max = newValue;
        }
        return max;
    }
}
