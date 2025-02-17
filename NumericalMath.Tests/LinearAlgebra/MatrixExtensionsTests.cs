using NumericalMath.Comparers;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Tests.LinearAlgebra;

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

    [TestCase(0, 0, 0, 0, ExpectedResult = 0)]
    [TestCase(1, 2, 3, 4, ExpectedResult = -2)]
    [TestCase(1, 0, 0, 2, ExpectedResult = 2)]
    [TestCase(0, 1, 1, 0, ExpectedResult = -1)]
    [TestCase(1, 2, 3, 0, ExpectedResult = -6)]
    [TestCase(1, 2, 3, 0, ExpectedResult = -6)]
    [TestCase(1, 0, 3, 2, ExpectedResult = 2)]
    [TestCase(3, 2, 0, 5, ExpectedResult = 15)]
    public int Determinant2(int m11, int m12, int m21, int m22)
    {
        return Matrix<int>.Determinant(m11, m12, m21, m22);
    }

    [TestCase(0, 0, 0, 0, 0, 0, 0, 0, 0, ExpectedResult = 0)]
    [TestCase(1, 0, 0, 0, 2, 0, 0, 0, 3, ExpectedResult = 6)]
    [TestCase(1, 2, 3, 4, 5, 6, 7, 8, 9, ExpectedResult = 0)]
    [TestCase(2, 5, -3, 4, -8, 2, 0, 6, -1, ExpectedResult = -60)]
    [TestCase(1, 2, 3, 0, 1, 4, 2, 3, 5, ExpectedResult = 3)]
    public int Determinant3(
        int m11, int m12, int m13,
        int m21, int m22, int m23,
        int m31, int m32, int m33
        )
    {
        return Matrix<int>.Determinant(
            m11, m12, m13,
            m21, m22, m23,
            m31, m32, m33);
    }
}
