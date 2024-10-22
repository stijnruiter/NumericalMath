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

    private static T[] ElementwiseOperation<T>(T[] lhs, T[] rhs, Func<T, T, T> op) where T: INumber<T>
    {
        Assertions.AreSameLength(lhs, rhs);
        T[] result = new T[lhs.Length];
        for (var i  = 0; i < lhs.Length; i++)
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

    public static ColumnVector<T> Subtraction<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T: INumber<T>
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

    public static ColumnVector<T> ElementwiseProduct<T>(ColumnVector<T> lhs, ColumnVector<T> rhs) where T: INumber<T>
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
}
