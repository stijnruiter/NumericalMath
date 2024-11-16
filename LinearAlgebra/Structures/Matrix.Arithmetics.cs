using LinearAlgebra.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LinearAlgebra.Structures;

public partial class Matrix<T> : IRectanglarMatrix<T>, IEquatable<Matrix<T>> where T : struct, INumber<T>
{
    public static Matrix<T> operator +(Matrix<T> lhs, Matrix<T> rhs)
    {
        Assertions.AreSameSize(lhs, rhs);
        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        VectorizationOps.Addition(lhs.Span, rhs.Span, result.Span);
        return result;
    }

    public static Matrix<T> operator -(Matrix<T> lhs, Matrix<T> rhs)
    {
        Assertions.AreSameSize(lhs, rhs);
        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        VectorizationOps.Subtraction(lhs.Span, rhs.Span, result.Span);
        return result;
    }

    public static Matrix<T> operator *(Matrix<T> lhs, Matrix<T> rhs)
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("Unable to compute product, lhs matrix columns do not match rhs matrix rows.", lhs.ColumnCount, rhs.RowCount);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, rhs.ColumnCount);
        Product(lhs, rhs, result);
        return result;
    }

    public static ColumnVector<T> operator *(Matrix<T> lhs, ColumnVector<T> rhs)
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("RowVector and Matrix dimensions do not match", lhs.ColumnCount, rhs.RowCount);

        ColumnVector<T> result = new ColumnVector<T>(lhs.RowCount);
        Product(lhs, rhs, result);
        return result;
    }

    public static RowVector<T> operator *(RowVector<T> lhs, Matrix<T> rhs)
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("RowVector and Matrix dimensions do not match", lhs.ColumnCount, rhs.RowCount);

        RowVector<T> result = new RowVector<T>(rhs.ColumnCount);
        Product(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator *(T lhs, Matrix<T> rhs)
    {
        Matrix<T> result = new Matrix<T>(rhs.RowCount, rhs.ColumnCount);
        VectorizationOps.ScalarProduct(lhs, rhs.Span, result.Span);
        return result;
    }

    public static Matrix<T> operator *(Matrix<T> lhs, T rhs)
    {
        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        VectorizationOps.ScalarProduct(rhs, lhs.Span, result.Span);
        return result;
    }

    private static void Product<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        for (int j = 0; j < rhs.ColumnCount; j++)
        {
            ColumnVector<T> column = rhs.Column(j);
            for (int i = 0; i < lhs.RowCount; i++)
            {
                result[i, j] = lhs.Row(i) * column;
            }
        }
    }

    private static void Product<T>(Matrix<T> lhs, ColumnVector<T> rhs, ColumnVector<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < lhs.RowCount; i++)
        {
            result[i] = lhs.Row(i) * rhs;
        }
    }

    private static void Product<T>(RowVector<T> lhs, Matrix<T> rhs, RowVector<T> result) where T : struct, INumber<T>
    {
        for (int i = 0; i < rhs.ColumnCount; i++)
        {
            result[i] = lhs * rhs.Column(i);
        }
    }

}
