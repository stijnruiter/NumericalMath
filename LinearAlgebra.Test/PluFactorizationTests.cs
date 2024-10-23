using LinearAlgebra.Structures;
using LinearAlgebra.Comparers;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using System;

namespace LinearAlgebra.Test;

[TestFixture]
public class PluFactorizationTests
{

    public static IEnumerable<TestCaseData> FactorizationMatrices
    {
        get
        {
            float[,] Avalues = new float[,] {{ 6, -7},
                                             { 0,  3 }};
            Matrix<float> A = new Matrix<float>(Avalues);
            int[] pivots = [0, 1];
            int permutations = 0;
            Matrix<float> lower = Matrix<float>.Identity(2);
            Matrix<float> upper = A.Copy();
            yield return new TestCaseData(A, pivots, permutations, lower, upper);

            Avalues = new float[,] {{ 1, 2, 3},
                                    { 3, 2, 1},
                                    { 2, 1, 3}};
            A = new Matrix<float>(Avalues);
            upper = new Matrix<float>(new float[,]
            {
                { 3,    2, 1 },
                { 0, 4f/3, 8f/3 },
                { 0,    0, 3 }
            });
            lower = new Matrix<float>(new float[,]
            {
                { 1,     0,    0 },
                { 1f/3,  1,    0 },
                { 2f/3, -1f/4, 1 }
            });
            pivots = [1, 0, 2];
            permutations = 1;
            yield return new TestCaseData(A, pivots, permutations, lower, upper);


            Avalues = new float[,] {{ 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                                    { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                                    { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                                    { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                                    { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                                    { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }};
            /**
             * [0, 1, 2, 3, 4, 5]
             * [0, 3, 2, 1, 4, 5]
             * [0, 3, 4, 1, 2, 5]
             * [0, 3, 4, 5, 2, 1]
             * [0, 3, 4, 5, 1, 2]
             **/
            A = new Matrix<float>(Avalues);

            upper = new Matrix<float>(new float[,]
            {
                { 1f,   5f,  3.2f    ,  4.3f    ,  8.2f    ,   6f },
                { 0,  -55f, -6.41f   , -1.3f    , -7.2f    ,  -6f },
                { 0,     0,  3.33309f,  4.04727f,  0.26182f,   0.21818f},
                { 0,     0,  0       , -4.89677f, -3.20329f, -11.08608f},
                { 0,     0,  0       ,  0       ,  4.60226f,   5.26332f},
                { 0,     0,  0       ,  0       ,  0       ,  -3.09968f}
            });
            lower = new Matrix<float>(new float[,]
            {
                { 1, 0,        0,         0,        0,        0 },
                { 1, 1,        0,         0,        0,        0 },
                { 0, 0.03636f, 1,         0,        0,        0 },
                { 1, 0.01273f, 0.74453f,  1,        0,        0 },
                { 0, 0,        0.93007f, -0.04815f, 1,        0 },
                { 0,-0.32727f,-0.59939f, -0.81695f, 0.03990f, 1 }
            });

            pivots = [0, 4, 5, 1, 2, 3];
            permutations = 4;
            yield return new TestCaseData(A, pivots, permutations, lower, upper);
        }
    }

    [TestCaseSource(nameof(FactorizationMatrices))]
    public void PluFactorization(Matrix<float> A, int[] expectedPivots, int expectedPermutations, Matrix<float> expectedL, Matrix<float> expectedU)
    {
        (Matrix<float> LU, int[] pivots, int permutations) = PluFactorizationOperations.PluFactorization(A, 1e-5f);

        Assert.That(pivots.SequenceEqual(expectedPivots), "Pivots not identical.\r\nExpected: [{0}]\r\nBut was: [{1}]", string.Join(",", expectedPivots), string.Join(",", pivots));
        Assert.That(permutations, Is.EqualTo(expectedPermutations));

        Matrix<float> lower = PluFactorizationOperations.ExtractLower(LU);
        Matrix<float> upper = PluFactorizationOperations.ExtractUpper(LU);

        Assert.That(lower, Is.EqualTo(expectedL).Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));
        Assert.That(upper, Is.EqualTo(expectedU).Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));


        // PA = LU
        Matrix<float> PA = new Matrix<float>(A.RowCount, A.ColumnCount);
        for (var i = 0; i < A.RowCount; i++)
        {
            for (var j = 0; j < A.ColumnCount; j++)
            {
                PA[i, j] = A[pivots[i], j];
            }
        }
        Assert.That(lower * upper, Is.EqualTo(PA).Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));
    }

    public static IEnumerable<TestCaseData> LinearSystemSets
    {
        get
        {
            float[,] Avalues = new float[,] {{ 6, -7},
                                             { 0,  3 }};
            Matrix<float> A = new Matrix<float>(Avalues);
            ColumnVector<float> b = new ColumnVector<float>([3, 4]);
            ColumnVector<float> result = new ColumnVector<float>([37f / 18f, 4f / 3f]);
            yield return new TestCaseData(A, b, result);
            Avalues = new float[,] {{ 1, 2, 3},
                                    { 3, 2, 1},
                                    { 2, 1, 3}};
            A = new Matrix<float>(Avalues);
            b = new ColumnVector<float>([1, 2, 2]);
            result = new ColumnVector<float>([0.75f, -0.25f, 0.25f]);
            yield return new TestCaseData(A, b, result);


            Avalues = new float[,] {{ 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                                    { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                                    { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                                    { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                                    { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                                    { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }};

            float[] bvalues = { 1.0f, 2.0f, 3.3f, 4.4f, 5.5f, 6.6f };
            float[] results =
            {
                -10.2777f,
                  0.214471f,
                 -3.5016f,
                  4.47097f,
                  1.8482f,
                 -2.16166f
            };
            A = new Matrix<float>(Avalues);
            b = new ColumnVector<float>(bvalues);
            result = new ColumnVector<float>(results);
            yield return new TestCaseData(A, b, result);
        }
    }

    [TestCaseSource(nameof(LinearSystemSets))]
    public void SolveMatrixVectorUsingLUDecomp(Matrix<float> A, ColumnVector<float> b, ColumnVector<float> result)
    {
        ColumnVector<float> x1 = PluFactorizationOperations.SolveUsingDoolittleLU(A, b);
        Assert.That(x1, Is.EqualTo(result).Using<Vector<float>>((a, b) => a.ApproxEquals(b, 5e-4f)));


        ColumnVector<float> x2 = PluFactorizationOperations.SolveUsingPLU(A, b, 1e-5f);
        Assert.That(x2, Is.EqualTo(result).Using<Vector<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    public static IEnumerable<TestCaseData> InverseMatricesSets
    {
        get
        {
            yield return new TestCaseData(
                new Matrix<float>(new float[,]
                {
                    { 1f, 2f },
                    { 3f, 4f }
                }),
                new Matrix<float>(new float[,]
                {
                    {-2.0f,  1.0f },
                    { 1.5f, -0.5f }
                }));

            yield return new TestCaseData(
                new Matrix<float>(new float[,]
                {
                    { 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                    { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                    { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                    { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                    { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                    { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }
                }),
                new Matrix<float>(new float[,]
                {
                    {  2.48269f,  -1.08299f,   -1.39254f,   -1.49448f,   -0.399701f,   0.420458f },
                    { -0.0065064f, 0.0108966f, -0.0450246f,  0.0454587f, -0.00439019f, 0.0260444f },
                    {  0.426931f, -0.234924f,   0.179534f,  -0.601676f,  -0.192006f,  -0.0526869f },
                    { -0.334124f,  0.187515f,  -0.161651f,   0.489028f,   0.14661f,    0.303855f },
                    { -0.435192f,  0.311171f,   0.202564f,   0.368953f,   0.124022f,  -0.198927f },
                    {  0.364829f, -0.262941f,   0.0128716f, -0.322614f,  -0.101888f,  -0.00957533f }
                }));
        }
    }

    [TestCaseSource(nameof(InverseMatricesSets))]
    public void InverseMatrixUsingLuDecomp(Matrix<float> A, Matrix<float> invA)
    {
        Assert.That(PluFactorizationOperations.SolveUsingDoolittleLU(A, Matrix<float>.Identity(A.RowCount)), Is.EqualTo(invA)
            .Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));

        Assert.That(PluFactorizationOperations.SolveUsingPLU(A, Matrix<float>.Identity(A.RowCount), Constants.DefaultFloatTolerance), Is.EqualTo(invA)
            .Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    [TestCaseSource(nameof(LinearSystemSets))]
    public void SolveMatrixMatrixUsingLuDecomp(Matrix<float> A, ColumnVector<float> b, ColumnVector<float> result)
    {
        Assert.That(PluFactorizationOperations.SolveUsingDoolittleLU(A, A), Is.EqualTo(Matrix<float>.Identity(A.RowCount))
            .Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));

        Assert.That(PluFactorizationOperations.SolveUsingPLU(A, A, Constants.DefaultFloatTolerance), Is.EqualTo(Matrix<float>.Identity(A.RowCount))
            .Using<Matrix<float>>((a, b) => a.ApproxEquals(b)));
    }

    [TestCase(5)]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    public void SolveMatrixVectorUsingPlu(int n)
    {
        Randomizer randomizer = new(1234567890);
        Matrix<float> A = GenerateMatrix(n, n, randomizer.NextFloat);
        ColumnVector<float> b = GenerateColumnVector(n, randomizer.NextFloat);

        ColumnVector<float> x = PluFactorizationOperations.SolveUsingPLU(A, b, Constants.DefaultFloatTolerance);

        Assert.That(x.RowCount, Is.EqualTo(n));
        Assert.That(A * x, Is.EqualTo(b).Using<Vector<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }

    [TestCase(5, 5)]
    [TestCase(10, 5)]
    [TestCase(50, 40)]
    [TestCase(100, 70)]
    public void SolveMatrixMatrixUsingPlu(int n, int m)
    {
        Randomizer randomizer = new(1234567890);

        Matrix<float> A = GenerateMatrix(n, n, randomizer.NextFloat);
        Matrix<float> B = GenerateMatrix(n, m, randomizer.NextFloat);

        Matrix<float> X = PluFactorizationOperations.SolveUsingPLU(A, B, Constants.DefaultFloatTolerance);

        Assert.That((X.RowCount, X.ColumnCount), Is.EqualTo((n, m)));
        Assert.That(A * X, Is.EqualTo(B).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-5f)));
    }


    [TestCase(5)]
    [TestCase(10)]
    [TestCase(50)]
    [TestCase(100)]
    public void SolveMatrixVectorUsingDoolittle(int n)
    {
        Randomizer randomizer = new(1234567890);
        Matrix<float> A = GenerateMatrix(n, n, randomizer.NextFloat);
        ColumnVector<float> b = GenerateColumnVector(n, randomizer.NextFloat);

        ColumnVector<float> x = PluFactorizationOperations.SolveUsingDoolittleLU(A, b);

        Assert.That(x.RowCount, Is.EqualTo(n));
        // Doolittle becomes less accurate due to many divisions, the larger the vector gets.
        Assert.That(A * x, Is.EqualTo(b).Using<Vector<float>>((a, b) => a.ApproxEquals(b, 1e-2f)));
    }

    [TestCase(5, 5)]
    [TestCase(10, 5)]
    [TestCase(50, 40)]
    [TestCase(100, 70)]
    public void SolveMatrixMatrixUsingDoolittle(int n, int m)
    {
        Randomizer randomizer = new(1234567890);

        Matrix<float> A = GenerateMatrix(n, n, randomizer.NextFloat);
        Matrix<float> B = GenerateMatrix(n, m, randomizer.NextFloat);

        Matrix<float> X = PluFactorizationOperations.SolveUsingDoolittleLU(A, B);

        Assert.That((X.RowCount, X.ColumnCount), Is.EqualTo((n, m)));
        // Doolittle becomes less accurate due to many divisions, the larger the vector gets.
        Assert.That(A * X, Is.EqualTo(B).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-2f)));
    }

    private static Matrix<T> GenerateMatrix<T>(int  n, int m, Func<T> randomGenerator) where T : System.Numerics.INumber<T>
    {
        Matrix<T> A = new(n, m);
        for (int i = 0; i < n; i++)
        {
            for (var j = 0; j < m; j++)
            {
                A[i, j] = randomGenerator();
            }
        }
        return A;
    }

    private static ColumnVector<T> GenerateColumnVector<T>(int n, Func<T> randomGenerator) where T : System.Numerics.INumber<T>
    {
        ColumnVector<T> A = new(n);
        for (int i = 0; i < n; i++)
        {
            A[i] = randomGenerator();
        }
        return A;
    }
}
