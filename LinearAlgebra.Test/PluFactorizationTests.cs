using LinearAlgebra.Structures;
using System.Collections.Generic;
using System.Linq;

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

        Assert.That(lower, Is.EqualTo(expectedL).Using<Matrix<float>>(ApproxEquals));
        Assert.That(upper, Is.EqualTo(expectedU).Using<Matrix<float>>(ApproxEquals));


        // PA = LU
        Matrix<float> PA = new Matrix<float>(A.RowCount, A.ColumnCount);
        for (var i = 0; i < A.RowCount; i++)
        {
            for (var j = 0; j < A.ColumnCount; j++)
            {
                PA[i, j] = A[pivots[i], j];
            }
        }
        Assert.That(lower * upper, Is.EqualTo(PA).Using<Matrix<float>>(ApproxEquals));
    }

    private bool ApproxEquals(Matrix<float> a, Matrix<float> b) 
    {
        if (a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
            return false;

        for (int i =  0; i < a.RowCount; i++)
        {
            for (int j = 0; j < b.ColumnCount; j++)
            {
                if (float.Abs(a[i, j] - b[i, j]) > 1e-5f)
                    return false;
            }
        }

        return true;
    }

}
