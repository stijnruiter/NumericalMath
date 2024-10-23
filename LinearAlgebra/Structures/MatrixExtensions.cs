namespace LinearAlgebra.Structures;

public static class MatrixExtensions
{
    public static ColumnVector<float> Solve(this Matrix<float> A, ColumnVector<float> b, float degenerateTolerance = Constants.DefaultFloatTolerance) => PluFactorizationOperations.SolveUsingPLU(A, b, degenerateTolerance);
    public static ColumnVector<double> Solve(this Matrix<double> A, ColumnVector<double> b, double degenerateTolerance = Constants.DefaultDoubleTolerance) => PluFactorizationOperations.SolveUsingPLU(A, b, degenerateTolerance);

    public static float Determinant(this Matrix<float> A, float degenerateTolerance = Constants.DefaultFloatTolerance) => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);
    public static double Determinant(this Matrix<double> A, double degenerateTolerance = Constants.DefaultDoubleTolerance) => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);

    public static Matrix<float> Inverse(this Matrix<float> A, float degenerateTolerance = Constants.DefaultFloatTolerance)
        => PluFactorizationOperations.SolveUsingPLU(A, Matrix<float>.Identity(A.RowCount), degenerateTolerance);

    public static Matrix<double> Inverse(this Matrix<double> A, double degenerateTolerance = Constants.DefaultDoubleTolerance)
        => PluFactorizationOperations.SolveUsingPLU(A, Matrix<double>.Identity(A.RowCount), degenerateTolerance);
}
