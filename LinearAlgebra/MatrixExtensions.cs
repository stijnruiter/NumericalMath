using LinearAlgebra.Structures;

namespace LinearAlgebra;

public static class MatrixExtensions
{
    public static ColumnVector<float> Solve(this Matrix<float> A, ColumnVector<float> b, float degenerateTolerance = 1e-5f) => PluFactorizationOperations.SolveUsingPLU(A, b, degenerateTolerance);
    public static ColumnVector<double> Solve(this Matrix<double> A, ColumnVector<double> b, double degenerateTolerance = 1e-6d) => PluFactorizationOperations.SolveUsingPLU(A, b, degenerateTolerance);

    public static float Determinant(this Matrix<float> A, float degenerateTolerance = 1e-5f) => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);
    public static double Determinant(this Matrix<double> A, double degenerateTolerance = 1e-6d) => PluFactorizationOperations.PluDeterminant(A, degenerateTolerance);
}
