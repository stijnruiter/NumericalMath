using System;
using System.Numerics;

namespace LinearAlgebra.Structures;

public static class MatrixExtensions
{
    private static Matrix<Tout> Convert<Tin, Tout>(this Matrix<Tin> matrix, Func<Tin, Tout> converter) where Tin : struct, INumber<Tin> where Tout : struct, INumber<Tout>
    {
        Matrix<Tout> result = new Matrix<Tout>(matrix.RowCount, matrix.ColumnCount);
        for (int i = 0; i < matrix.RowCount; i++)
        {
            for (int j = 0; j < matrix.ColumnCount; j++)
            {
                result[i, j] = converter(matrix[i, j]);
            }
        }
        return result;
    }

    public static Matrix<double> ToDoubles(this Matrix<float> matrix) => matrix.Convert(System.Convert.ToDouble);
    public static Matrix<float> ToFloats(this Matrix<double> matrix) => matrix.Convert(System.Convert.ToSingle);

    public static ColumnVector<float> Solve(this Matrix<float> A, ColumnVector<float> b, float degenerateTolerance = Constants.DefaultFloatTolerance) 
        => PluFactorizationOperations.SolveUsingPLU(A, b, degenerateTolerance);
    public static ColumnVector<double> Solve(this Matrix<double> A, ColumnVector<double> b, double degenerateTolerance = Constants.DefaultDoubleTolerance) 
        => PluFactorizationOperations.SolveUsingPLU(A, b, degenerateTolerance);

    public static float Determinant(this Matrix<float> A, float degenerateTolerance = Constants.DefaultFloatTolerance) 
        => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);
    public static double Determinant(this Matrix<double> A, double degenerateTolerance = Constants.DefaultDoubleTolerance) 
        => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);

    public static Matrix<float> Inverse(this Matrix<float> A, float degenerateTolerance = Constants.DefaultFloatTolerance)
        => PluFactorizationOperations.SolveUsingPLU(A, Matrix<float>.Identity(A.RowCount), degenerateTolerance);

    public static Matrix<double> Inverse(this Matrix<double> A, double degenerateTolerance = Constants.DefaultDoubleTolerance)
        => PluFactorizationOperations.SolveUsingPLU(A, Matrix<double>.Identity(A.RowCount), degenerateTolerance);
}
