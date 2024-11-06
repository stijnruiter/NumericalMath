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

    private static T[] ElementwiseOperation<T>(T[] lhs, T[] rhs, Func<T, T, T> op) where T : INumber<T>
    {
        Assertions.AreSameLength(lhs, rhs);
        T[] result = new T[lhs.Length];
        for (var i = 0; i < lhs.Length; i++)
        {
            result[i] = op(lhs[i], rhs[i]);
        }
        return result;
    }

    private static T[] ElementwiseOperation<T>(T lhs, T[] rhs, Func<T, T, T> op) where T : INumber<T>
    {
        T[] result = new T[rhs.Length];
        for (var i = 0; i < rhs.Length; i++)
        {
            result[i] = op(lhs, rhs[i]);
        }
        return result;
    }

    private static T[][] ElementwiseOperation<T>(T lhs, T[][] rhs, Func<T, T, T> op) where T : INumber<T>
    {
        T[][] result = new T[rhs.Length][];
        for (var i = 0; i < rhs.Length; i++)
        {
            result[i] = ElementwiseOperation(lhs, rhs[i], op);
        }
        return result;
    }

    private static T[][] ElementwiseOperation<T>(T[][] lhs, T[][] rhs, Func<T, T, T> op) where T : INumber<T>
    {
        Assertions.AreSameLength(lhs, rhs);

        T[][] result = new T[rhs.Length][];
        for (var i = 0; i < rhs.Length; i++)
        {
            result[i] = ElementwiseOperation(lhs[i], rhs[i], op);
        }
        return result;
    }

    public static RowVector<T> Addition<T>(RowVector<T> lhs, RowVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a + b);
        return (RowVector<T>)result;
    }

    public static ColumnVector<T> Addition<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a + b);
        return (ColumnVector<T>)result;
    }

    public static Matrix<T> Addition<T>(Matrix<T> lhs, Matrix<T> rhs) where T : INumber<T>
    {
        T[][] result = ElementwiseOperation<T>(lhs.Elements, rhs.Elements, (a, b) => a + b);
        return new Matrix<T>(result);
    }

    public static RowVector<T> Subtraction<T>(RowVector<T> lhs, RowVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a - b);
        return (RowVector<T>)result;
    }

    public static ColumnVector<T> Subtraction<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a - b);
        return (ColumnVector<T>)result;
    }

    public static Matrix<T> Subtraction<T>(Matrix<T> lhs, Matrix<T> rhs) where T : INumber<T>
    {
        T[][] result = ElementwiseOperation<T>(lhs.Elements, rhs.Elements, (a, b) => a - b);
        return new Matrix<T>(result);
    }


    public static RowVector<T> ElementwiseProduct<T>(RowVector<T> lhs, RowVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a * b);
        return (RowVector<T>)result;
    }

    public static ColumnVector<T> ElementwiseProduct<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a * b);
        return (ColumnVector<T>)result;
    }

    public static Matrix<T> ElementwiseProduct<T>(Matrix<T> lhs, Matrix<T> rhs) where T : INumber<T>
    {
        T[][] result = ElementwiseOperation<T>(lhs.Elements, rhs.Elements, (a, b) => a * b);
        return new Matrix<T>(result);
    }


    public static RowVector<T> ElementwiseDivision<T>(RowVector<T> lhs, RowVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a / b);
        return (RowVector<T>)result;
    }

    public static ColumnVector<T> ElementwiseDivision<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation<T>(lhs, rhs, (a, b) => a / b);
        return (ColumnVector<T>)result;
    }

    public static Matrix<T> ElementwiseDivision<T>(Matrix<T> lhs, Matrix<T> rhs) where T : INumber<T>
    {
        T[][] result = ElementwiseOperation<T>(lhs.Elements, rhs.Elements, (a, b) => a / b);
        return new Matrix<T>(result);
    }


    public static T DotProduct<T>(RowVector<T> lhs, ColumnVector<T> rhs) where T : INumber<T>
    {
        Assertions.AreSameLength((T[])lhs, (T[])rhs);
        T result = T.Zero;
        for (var i = 0; i < lhs.Length; i++)
        {
            result += lhs[i] * rhs[i];
        }
        return result;
    }

    public static Matrix<T> ScalarProduct<T>(T scalar, Matrix<T> rhs) where T : INumber<T>
    {
        T[][] result = ElementwiseOperation(scalar, rhs.Elements, (a, b) => a * b);
        return new Matrix<T>(result);
    }

    public static ColumnVector<T> ScalarProduct<T>(T scalar, ColumnVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation(scalar, rhs, (a, b) => a * b);
        return (ColumnVector<T>)result;
    }

    public static RowVector<T> ScalarProduct<T>(T scalar, RowVector<T> rhs) where T : INumber<T>
    {
        T[] result = ElementwiseOperation(scalar, rhs, (a, b) => a * b);
        return (RowVector<T>)result;
    }


    public static Matrix<T> OuterProduct<T>(ColumnVector<T> lhs, RowVector<T> rhs) where T : INumber<T>
    {
        Assertions.AreSameLength((T[])lhs, (T[])rhs);

        T[][] results = new T[lhs.Length][];
        for (var i = 0; i < lhs.Length; i++)
        {
            results[i] = ElementwiseOperation(lhs[i], rhs, (a, b) => a * b);
        }
        return new Matrix<T>(results);
    }

    public static Matrix<T> Product<T>(Matrix<T> lhs, Matrix<T> rhs) where T : INumber<T>
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("Unable to compute product, lhs matrix columns do not match rhs matrix rows.", lhs.ColumnCount, rhs.RowCount);

        Matrix<T> result = new Matrix<T>(lhs.RowCount, rhs.ColumnCount);
        for (int i = 0; i < lhs.RowCount; i++)
        {
            for (int j = 0; j < rhs.ColumnCount; j++)
            {
                result[i, j] = lhs.Row(i) * rhs.Column(j);
            }
        }
        return result;
    }

    public static ColumnVector<T> Product<T>(Matrix<T> lhs, ColumnVector<T> rhs) where T : INumber<T>
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("RowVector and Matrix dimensions do not match", lhs.ColumnCount, rhs.RowCount);

        ColumnVector<T> result = new ColumnVector<T>(lhs.RowCount);
        for (int i = 0; i < lhs.RowCount; i++)
        {
            result[i] = lhs.Row(i) * rhs;
        }
        return result;
    }

    public static RowVector<T> Product<T>(RowVector<T> lhs, Matrix<T> rhs) where T : INumber<T>
    {
        if (lhs.ColumnCount != rhs.RowCount)
            throw new DimensionMismatchException("RowVector and Matrix dimensions do not match", lhs.ColumnCount, rhs.RowCount);

        RowVector<T> result = new RowVector<T>(rhs.ColumnCount);
        for(int i = 0; i < rhs.ColumnCount; i++)
        {
            result[i] = lhs * rhs.Column(i);
        }
        return result;
    }

    public static IRectanglarMatrix<T> TensorProduct<T>(IRectanglarMatrix<T> left, IRectanglarMatrix<T> right) where T : INumber<T>
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

    public static T Norm2<T>(Structures.Vector<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T result = T.AdditiveIdentity;

        for (int i = 0; i < vector.Length; i++)
        {
            result += vector[i] * vector[i];
        }
        return T.Sqrt(result);
    }

    public static T Norm1<T>(Structures.Vector<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T result = T.AdditiveIdentity;

        for (int i = 0; i < vector.Length; i++)
        {
            result += T.Abs(vector[i]);
        }
        return result;
    }

    public static T? NormInf<T>(Structures.Vector<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return ((T[])vector).Max(T.Abs);
    }

}
