using NumericalMath.Comparers;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Tests;

[TestFixture]
internal class MatrixExtensionsTests
{
    [TestCaseSource(typeof(PluFactorizationTests), nameof(PluFactorizationTests.LinearSystemSets))]
    public void SolveMatrixVectorExtension(Matrix<float> A, ColumnVector<float> b, ColumnVector<float> result)
    {
        ColumnVector<float> x = A.Solve(b);
        Assert.That(x, Is.EqualTo(result).Using<AbstractVector<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    [TestCaseSource(typeof(PluFactorizationTests), nameof(PluFactorizationTests.InverseMatricesSets))]
    public void InverseMatrixExtension(Matrix<float> A, Matrix<float> invA)
    {
        Assert.That(A.Inverse(), Is.EqualTo(invA).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-6f)));
        Assert.That(A.ToDoubles().Inverse().ToSingles(), Is.EqualTo(invA).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-6f)));
    }

    [TestCaseSource(typeof(MatrixTests), nameof(MatrixTests.MatrixPropertySets))]
    public void MatrixDeterminant(Matrix<float> A, float trace, float determinant, double determinant2)
    {
        Assert.That(A.Determinant(), Is.EqualTo(determinant).Within(1e-5f));
        Assert.That(A.ToDoubles().Determinant(), Is.EqualTo(determinant2).Within(1e-6d));
    }
}
