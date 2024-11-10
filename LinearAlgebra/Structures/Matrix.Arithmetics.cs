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
        Addition(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator -(Matrix<T> lhs, Matrix<T> rhs)
    {
        Assertions.AreSameSize(lhs, rhs);
        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        Subtraction(lhs, rhs, result);
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
        ScalarProduct(lhs, rhs, result);
        return result;
    }

    public static Matrix<T> operator *(Matrix<T> lhs, T rhs)
    {
        Matrix<T> result = new Matrix<T>(lhs.RowCount, lhs.ColumnCount);
        ScalarProduct(rhs, lhs, result);
        return result;
    }

    private static void Addition<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs._values);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs._values);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result._values);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] + rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs._values.Length; i++)
        {
            result._values[i] = lhs._values[i] + rhs._values[i];
        }
    }

    private static void Subtraction<T>(Matrix<T> lhs, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        ReadOnlySpan<Vector<T>> leftVec = MemoryMarshal.Cast<T, Vector<T>>(lhs._values);
        ReadOnlySpan<Vector<T>> rightVec = MemoryMarshal.Cast<T, Vector<T>>(rhs._values);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result._values);

        for (int i = 0; i < leftVec.Length; i++)
        {
            resultVec[i] = leftVec[i] - rightVec[i];
        }
        for (int i = leftVec.Length * Vector<T>.Count; i < lhs._values.Length; i++)
        {
            result._values[i] = lhs._values[i] - rhs._values[i];
        }
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

    private static void ScalarProduct<T>(T scalar, Matrix<T> rhs, Matrix<T> result) where T : struct, INumber<T>
    {
        Vector<T> scalarVec = new Vector<T>(scalar);
        ReadOnlySpan<Vector<T>> vectors = MemoryMarshal.Cast<T, Vector<T>>(rhs._values);
        Span<Vector<T>> resultVec = MemoryMarshal.Cast<T, Vector<T>>(result._values);

        for (int i = 0; i < vectors.Length; i++)
        {
            resultVec[i] = vectors[i] * scalarVec;
        }
        for (int i = vectors.Length * Vector<T>.Count; i < rhs._values.Length; i++)
        {
            result._values[i] = rhs._values[i] * scalar;
        }
    }
}
