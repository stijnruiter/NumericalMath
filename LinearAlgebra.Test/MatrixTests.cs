using LinearAlgebra.Comparers;
using LinearAlgebra.Exceptions;
using LinearAlgebra.Structures;
using System;
using System.Collections.Generic;

namespace LinearAlgebra.Test;

[TestFixture]
internal class MatrixTests
{
    [Test]
    public void EmptyMatrix()
    {
        Matrix<int> empty = Matrix<int>.Zero(3);
        Assert.That(empty, Is.EqualTo(new Matrix<int>(new int[,]
        {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 },
        })));
        Assert.That(empty.Equals(null), Is.False);
    }

    [Test]
    public void Indexing()
    {
        Matrix<int> matrix = new Matrix<int>(5, 3);

        Assert.That(matrix.RowCount, Is.EqualTo(5));
        Assert.That(matrix.ColumnCount, Is.EqualTo(3));

        Assert.That(matrix[0, 0], Is.EqualTo(0));

        Assert.That(matrix[4, 2], Is.EqualTo(0));
        Assert.Throws<IndexOutOfRangeException>(delegate { var x = matrix[2, 4]; });
    }


    [Test]
    public void AssigningMultiDimensionalArray()
    {
        int[,] values = { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix<int> matrix = new Matrix<int>(values);

        Assert.That(matrix.RowCount, Is.EqualTo(2));
        Assert.That(matrix.ColumnCount, Is.EqualTo(3));

        Assert.That(matrix[0, 0], Is.EqualTo(1));
        Assert.That(matrix[0, 1], Is.EqualTo(2));
        Assert.That(matrix[0, 2], Is.EqualTo(3));

        Assert.That(matrix[1, 0], Is.EqualTo(4));
        Assert.That(matrix[1, 1], Is.EqualTo(5));
        Assert.That(matrix[1, 2], Is.EqualTo(6));
    }

    [Test]
    public void ColumnRowPropertyJagged()
    {
        Matrix<int> matrix = new Matrix<int>(new int[,] { { 1, 2, 3 }, { 4, 5, 6 } });

        Assert.That(matrix.ColumnArray(0), Is.EqualTo(new int[] { 1, 4 }));
        Assert.That(matrix.ColumnArray(1), Is.EqualTo(new int[] { 2, 5 }));
        Assert.That(matrix.ColumnArray(2), Is.EqualTo(new int[] { 3, 6 }));

        Assert.That(matrix.RowArray(0).ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(matrix.RowArray(1).ToArray(), Is.EqualTo(new int[] { 4, 5, 6 }));

        Assert.That(matrix.Column(0), Is.EqualTo(new ColumnVector<int>([1, 4])));
        Assert.That(matrix.Column(1), Is.EqualTo(new ColumnVector<int>([2, 5])));
        Assert.That(matrix.Column(2), Is.EqualTo(new ColumnVector<int>([3, 6])));

        Assert.That(matrix.Row(0), Is.EqualTo(new RowVector<int>([1, 2, 3])));
        Assert.That(matrix.Row(1), Is.EqualTo(new RowVector<int>([4, 5, 6])));
    }


    [Test]
    public void ColumnRowProperty()
    {
        int[,] values = { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix<int> matrix = new Matrix<int>(values);

        Assert.That(matrix.ColumnArray(0), Is.EqualTo(new int[] { 1, 4 }));
        Assert.That(matrix.ColumnArray(1), Is.EqualTo(new int[] { 2, 5 }));
        Assert.That(matrix.ColumnArray(2), Is.EqualTo(new int[] { 3, 6 }));

        Assert.That(matrix.RowArray(0).ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(matrix.RowArray(1).ToArray(), Is.EqualTo(new int[] { 4, 5, 6 }));

        Assert.That(matrix.Column(0), Is.EqualTo(new ColumnVector<int>([1, 4])));
        Assert.That(matrix.Column(1), Is.EqualTo(new ColumnVector<int>([2, 5])));
        Assert.That(matrix.Column(2), Is.EqualTo(new ColumnVector<int>([3, 6])));

        Assert.That(matrix.Row(0), Is.EqualTo(new RowVector<int>([1, 2, 3])));
        Assert.That(matrix.Row(1), Is.EqualTo(new RowVector<int>([4, 5, 6])));
    }

    [Test]
    public void AdditionSubtractionMatrix()
    {
        Matrix<int> M1 = new Matrix<int>(new int[,]
        {
            { 7, 3, 2 },
            { 9, 8, 6 },
        });
        Matrix<int> M2 = new Matrix<int>(new int[,]
        {
            { 1, 2, 3 },
            { 4, 5, 6 },
        });

        Matrix<int> M1PlusM2Result = new Matrix<int>(new int[,]
        {
            {  8,  5,  5 },
            { 13, 13, 12 },
        });

        Matrix<int> M1MinusM2Result = new Matrix<int>(new int[,]
        {
            {  6,  1, -1 },
            { 5, 3, 0 },
        });

        Assert.That(M1 + M2, Is.EqualTo(M1PlusM2Result));
        Assert.That(M1 - M2, Is.EqualTo(M1MinusM2Result));

        Assert.Throws<DimensionMismatchException>(() => { var _ = M1.Transpose() + M2; });
        Assert.Throws<DimensionMismatchException>(() => { var _ = M1.Transpose() - M2; });
    }

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
        Assert.That(A.ToDoubles().Inverse().ToFloats(), Is.EqualTo(invA).Using<Matrix<float>>((a, b) => a.ApproxEquals(b, 5e-6f)));
    }

    [TestCaseSource(typeof(PluFactorizationTests), nameof(PluFactorizationTests.InverseMatricesSets))]
    public void InverseMatrixExtensionDouble(Matrix<float> A, Matrix<float> invA)
    {
    }

    public static IEnumerable<TestCaseData> MatrixPropertySets
    {
        get
        {
            Matrix<float> A = new Matrix<float>(new float[,]{{ 6, -7},
                                                             { 0,  3}});
            float trace = 9f;
            float determinant = 18f;
            yield return new TestCaseData(A, trace, determinant, determinant);

            A = new Matrix<float>(new float[,] {{ 1, 2, 3},
                                                { 3, 2, 1},
                                                { 2, 1, 3}});
            trace = 6;
            determinant = -12;
            yield return new TestCaseData(A, trace, determinant, determinant);


            A = new Matrix<float>(new float[,]{ { 1.0f,   5.0f,  3.2f,  4.3f, 8.2f,  6.0f },
                                                { 1.0f,   4.3f,  5.6f,  2.4f, 5.1f, -5.0f },
                                                { 0.0f,   0.0f,  3.1f,  4.0f, 5.0f,  6.0f },
                                                { 0.0f,  18.0f,  0.1f,  2.0f, 5.0f,  8.0f },
                                                { 1.0f, -50.0f, -3.21f, 3.0f, 1.0f,  0.0f },
                                                { 0.0f, - 2.0f,  3.1f,  4.0f, 0.0f,  0.0f }});
            trace = 11.4f;
            determinant = -12805.829f;
            double determinant2 = -12805.830597d;
            yield return new TestCaseData(A, trace, determinant, determinant2);
        }
    }

    [TestCaseSource(nameof(MatrixPropertySets))]
    public void MatrixProperties(Matrix<float> A, float trace, float determinant, double determinant2)
    {
        Assert.That(A.Trace(), Is.EqualTo(trace).Within(1e-5f));
        Assert.That(A.ToDoubles().Trace(), Is.EqualTo(trace).Within(1e-6d));
        Assert.That(A.Determinant(), Is.EqualTo(determinant).Within(1e-5f));
        Assert.That(A.ToDoubles().Determinant(), Is.EqualTo(determinant2).Within(1e-6d));
    }

    [Test]
    public void MatrixMatrixMultiplication()
    {
        Matrix<int> identity = Matrix<int>.Identity(3);
        Assert.That(identity * identity, Is.EqualTo(identity));

        Matrix<int> nonSquare = new Matrix<int>(new int[,]
        {
            {1, 2, 3, 4},
            {3, 2, 1, 5},
            {4, 5, 1, 5},
        });

        Assert.Throws<DimensionMismatchException>(() => { var _ = nonSquare * Matrix<int>.Identity(3); });
        Assert.Throws<DimensionMismatchException>(() => { var _ = Matrix<int>.Identity(4) * nonSquare; });
        Assert.That(nonSquare * Matrix<int>.Identity(4), Is.EqualTo(nonSquare));
        Assert.That(Matrix<int>.Identity(3) * nonSquare, Is.EqualTo(nonSquare));


        Matrix<int> otherNonSquare = new Matrix<int>(new int[,]
        {
            {1, 2, 3, 4, 3},
            {3, 2, 1, 5, 2},
            {4, 5, 1, 5, 1},
            {4, 5, 1, 5, 1},
        });

        Matrix<int> productResult = new Matrix<int>(new int[,]
        {
            {35, 41, 12, 49, 14}, 
            {33, 40, 17, 52, 19}, 
            {43, 48, 23, 71, 28}
        });

        Assert.Throws<DimensionMismatchException>(() => { var _ = otherNonSquare * nonSquare; });
        Assert.That(nonSquare * otherNonSquare, Is.EqualTo(productResult));
    }

    [Test]
    public void RowMatrixMultiplication()
    {
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3, 4, 5]);
        Assert.That(rowVector * Matrix<int>.Identity(5), Is.EqualTo(rowVector));
        Assert.Throws<DimensionMismatchException>(() => { var _ = rowVector * new Matrix<int>(3, 5); });
        Assert.Throws<DimensionMismatchException>(() => { var _ = rowVector * Matrix<int>.Identity(4); });

        Matrix<int> nonSquare = new Matrix<int>(new int[, ]
        {
            { 1,  2,  3 },
            { 4,  5,  6 },
            { 7,  8,  9 },
            {10, 11, 12 },
            {13, 14, 15 }
        });
        RowVector<int> expected = new RowVector<int>([135, 150, 165]);
        Assert.That(rowVector * nonSquare, Is.EqualTo(expected));
    }

    [Test]
    public void MatrixColumnMultiplication()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4, 5]);
        Assert.That(Matrix<int>.Identity(5) * columnVector, Is.EqualTo(columnVector));
        Assert.Throws<DimensionMismatchException>(() => { var _ = new Matrix<int>(5, 3) * columnVector; });
        Assert.Throws<DimensionMismatchException>(() => { var _ = Matrix<int>.Identity(4) * columnVector; });

        Matrix<int> nonSquare = new Matrix<int>(new int[,]
        {
            {  1,  2,  3,  4,  5 },
            {  6,  7,  8,  9, 10 },
            { 11, 12, 13, 14, 15 }
        });
        ColumnVector<int> expected = new ColumnVector<int>([55, 130, 205]);
        Assert.That(nonSquare * columnVector, Is.EqualTo(expected));
    }

    [Test]
    public void Transpose()
    {
        Matrix<int> identity = Matrix<int>.Identity(5);
        Assert.That(identity.Transpose(), Is.EqualTo(identity));

        Matrix<int> nonSquare = new Matrix<int>(new int[,]
        {
            {  1,  2,  3,  4,  5 },
            {  6,  7,  8,  9, 10 },
            { 11, 12, 13, 14, 15 }
        });
        Matrix<int> nonSquareTransposed = new Matrix<int>(new int[,]
        {
            { 1,  6, 11 },
            { 2,  7, 12 },
            { 3,  8, 13 },
            { 4,  9, 14 },
            { 5, 10, 15 }
        });

        Assert.That(nonSquare.Transpose(), Is.EqualTo(nonSquareTransposed));
        Assert.That(nonSquare.Transpose().Transpose(), Is.EqualTo(nonSquare));
    }
}
