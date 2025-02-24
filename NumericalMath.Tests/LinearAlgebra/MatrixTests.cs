using NumericalMath.Exceptions;
using NumericalMath.LinearAlgebra.Structures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.Comparers;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;
using NumericalMath.Tests.Comparers;

namespace NumericalMath.Tests.LinearAlgebra;

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

        empty = [];
        Assert.That(empty, Is.EqualTo(Matrix<int>.Empty));
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
    public void ColumnRowProperty()
    {
        int[,] values = { { 1, 2, 3 }, { 4, 5, 6 } };
        Matrix<int> matrix = new Matrix<int>(values);

        Assert.That(matrix.Row(0).ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(matrix.Row(1).ToArray(), Is.EqualTo(new int[] { 4, 5, 6 }));

        Assert.That(matrix.Column(0).ToArray(), Is.EqualTo(new int[] { 1, 4 }));
        Assert.That(matrix.Column(1).ToArray(), Is.EqualTo(new int[] { 2, 5 }));
        Assert.That(matrix.Column(2).ToArray(), Is.EqualTo(new int[] { 3, 6 }));
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
    public void MatrixTrace(Matrix<float> A, float trace, float determinant, double determinant2)
    {
        Assert.That(A.Trace(), Is.EqualTo(trace).Within(1e-5f));
        Assert.That(A.ToDoubles().Trace(), Is.EqualTo(trace).Within(1e-6d));
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
        RowVector<int> rowVector = new RowVector<int>(new int[] { 1, 2, 3, 4, 5 });
        Assert.That(rowVector * Matrix<int>.Identity(5), Is.EqualTo(rowVector));
        Assert.Throws<DimensionMismatchException>(() => { var _ = rowVector * new Matrix<int>(3, 5); });
        Assert.Throws<DimensionMismatchException>(() => { var _ = rowVector * Matrix<int>.Identity(4); });

        Matrix<int> nonSquare = new Matrix<int>(new int[,]
        {
            { 1,  2,  3 },
            { 4,  5,  6 },
            { 7,  8,  9 },
            {10, 11, 12 },
            {13, 14, 15 }
        });
        RowVector<int> expected = new RowVector<int>(new int[] { 135, 150, 165 });
        Assert.That(rowVector * nonSquare, Is.EqualTo(expected));
    }

    [Test]
    public void MatrixColumnMultiplication()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>(new int[] { 1, 2, 3, 4, 5 });
        Assert.That(Matrix<int>.Identity(5) * columnVector, Is.EqualTo(columnVector));
        Assert.Throws<DimensionMismatchException>(() => { var _ = new Matrix<int>(5, 3) * columnVector; });
        Assert.Throws<DimensionMismatchException>(() => { var _ = Matrix<int>.Identity(4) * columnVector; });

        Matrix<int> nonSquare = new Matrix<int>(new int[,]
        {
            {  1,  2,  3,  4,  5 },
            {  6,  7,  8,  9, 10 },
            { 11, 12, 13, 14, 15 }
        });
        ColumnVector<int> expected = new ColumnVector<int>(new int[] { 55, 130, 205 });
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

    [Test]
    public void RandomMatrix()
    {
        var mat1 = Matrix<float>.Random(3, 4);
        var mat2 = Matrix<float>.Random(3, 4);
        Assert.That(mat1, Is.Not.EqualTo(mat2));
    }

    [Test]
    public void MatrixEquals()
    {
        Matrix<int> mat1 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        Matrix<int> mat2 = [[1, 2, 3], [4, 5, 6]];
        Matrix<int> mat3 = [[1, 2], [4, 5], [7, 8]];
        Matrix<int> mat4 = [[2, 2, 3], [4, 5, 6], [7, 8, 9]];
        
        Assert.That(mat2, Is.Not.EqualTo(mat1));
        Assert.That(mat3, Is.Not.EqualTo(mat1));
        Assert.That(mat4, Is.Not.EqualTo(mat1));
    }

    [Test]
    public void Diagonal()
    {
        Matrix<int> mat1 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        Matrix<int> mat2 = [[1, 2, 3], [4, 5, 6]];
        Matrix<int> mat3 = [[1, 2], [4, 5], [7, 8]];
        
        Assert.That(mat1.Diagonal(), Is.EqualTo(new ColumnVector<int>([1, 5, 9])));
        Assert.That(mat2.Diagonal(), Is.EqualTo(new ColumnVector<int>([1, 5])));
        Assert.That(mat3.Diagonal(), Is.EqualTo(new ColumnVector<int>([1, 5])));
    }

    [Test]
    public void Copy()
    {
        Matrix<int> mat1 = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        Matrix<int> mat2 = mat1.Copy();
        Assert.That(mat1, Is.EqualTo(mat2));
        mat1[1, 1] = 3;
        Assert.That(mat1, Is.Not.EqualTo(mat2));
    }


    public static IEnumerable<TestCaseData> RowColumnStorageEnumerable()
    {
        RowVector<int>[] rows =
        [
            [1, 2, 3],
            [4, 5, 6],
            [7, 8, 9]
        ];
        var values = new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        yield return new TestCaseData(new ColumnMajorMatrixStorage<int>(values)).Returns(rows);
        yield return new TestCaseData(new RowMajorMatrixStorage<int>(values)).Returns(rows);
    }
    
    [TestCaseSource(nameof(RowColumnStorageEnumerable))]
    public IEnumerable<RowVector<int>> Enumerator(IMatrixStorage<int> matrixStorage)
    {
        var matrix = new Matrix<int>(matrixStorage);

        var enum1 = new List<RowVector<int>>();
        
        using var enumerator = matrix.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enum1.Add(enumerator.Current);
        }

        var enum2 = enumerator.ToEnumerable().ToArray();
        
        Assert.That(enum2, Is.EqualTo(enum1));
        return enum1;
    }

    [Test]
    public void EmptyColumnStorage()
    {
        Assert.That(() => new ColumnMajorMatrixStorage<int>(-1, 5), Throws.ArgumentException);
        Assert.That(() => new ColumnMajorMatrixStorage<int>(5, -1), Throws.ArgumentException);
        Assert.That(() => new ColumnMajorMatrixStorage<int>(-5, 1, null), Throws.ArgumentException);
        Assert.That(() => new ColumnMajorMatrixStorage<int>(5, -1, null), Throws.ArgumentException);
    }
    
    [Test]
    public void EmptyRowStorage()
    {
        Assert.That(() => new RowMajorMatrixStorage<int>(-1, 5), Throws.ArgumentException);
        Assert.That(() => new RowMajorMatrixStorage<int>(5, -1), Throws.ArgumentException);
        Assert.That(() => new RowMajorMatrixStorage<int>(-5, 1, null), Throws.ArgumentException);
        Assert.That(() => new RowMajorMatrixStorage<int>(5, -1, null), Throws.ArgumentException);
        var storage = new RowMajorMatrixStorage<int>(Array.Empty<RowVector<int>>());
        Assert.That(storage.ColumnCount, Is.EqualTo(0));
        Assert.That(storage.RowCount, Is.EqualTo(0));
    }

    public static IEnumerable<TestCaseData> Storages()
    {
        yield return new TestCaseData(new RowMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } })).SetArgDisplayNames("RowMajor");
        yield return new TestCaseData(new ColumnMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } })).SetArgDisplayNames("ColumnMajor");
    }
    
    [TestCaseSource(nameof(Storages))]
    public void SwapRows(IMatrixStorage<int> storage)
    {
        Assume.That(storage.GetRow(0).ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assume.That(storage.GetRow(1).ToArray(), Is.EqualTo(new int[] { 4, 5, 6 }));
        Assume.That(storage.GetRow(2).ToArray(), Is.EqualTo(new int[] { 7, 8, 9 }));
        storage.SwapRows(0, 1);
        Assert.That(storage.GetRow(0).ToArray(), Is.EqualTo(new int[] { 4, 5, 6 }));
        Assert.That(storage.GetRow(1).ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(storage.GetRow(2).ToArray(), Is.EqualTo(new int[] { 7, 8, 9 }));
    }
    
    [TestCaseSource(nameof(Storages))]
    public void StorageColumnSlices(IMatrixStorage<int> storage)
    {
        Assert.That(storage.GetColumnSlice(0, 0).ToArray(), Is.EqualTo(new int[] {1, 4, 7}));
        Assert.That(storage.GetColumnSlice(1, 0).ToArray(), Is.EqualTo(new int[] {2, 5, 8}));
        Assert.That(storage.GetColumnSlice(2, 0).ToArray(), Is.EqualTo(new int[] {3, 6, 9}));
        
        Assert.That(storage.GetColumnSlice(0, 1).ToArray(), Is.EqualTo(new int[] {4, 7}));
        Assert.That(storage.GetColumnSlice(1, 1).ToArray(), Is.EqualTo(new int[] {5, 8}));
        Assert.That(storage.GetColumnSlice(2, 1).ToArray(), Is.EqualTo(new int[] {6, 9}));
        
        Assert.That(storage.GetColumnSlice(0, 2).ToArray(), Is.EqualTo(new int[] {7}));
        Assert.That(storage.GetColumnSlice(1, 2).ToArray(), Is.EqualTo(new int[] {8}));
        Assert.That(storage.GetColumnSlice(2, 2).ToArray(), Is.EqualTo(new int[] {9}));
        
        Assert.That(storage.GetColumnSlice(0, 3).ToArray(), Is.EqualTo(new int[] {}));
        Assert.That(storage.GetColumnSlice(1, 3).ToArray(), Is.EqualTo(new int[] {}));
        Assert.That(storage.GetColumnSlice(2, 3).ToArray(), Is.EqualTo(new int[] {}));
        
        
        Assert.That(storage.GetColumnSlice(0, 0, 3).ToArray(), Is.EqualTo(new int[] {1, 4, 7}));
        Assert.That(storage.GetColumnSlice(1, 0, 2).ToArray(), Is.EqualTo(new int[] {2, 5}));
        Assert.That(storage.GetColumnSlice(2, 0, 1).ToArray(), Is.EqualTo(new int[] {3}));
        
        Assert.That(() => storage.GetColumnSlice(0, 1, 3), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(storage.GetColumnSlice(1, 1, 2).ToArray(), Is.EqualTo(new int[] {5, 8}));
        Assert.That(storage.GetColumnSlice(2, 1, 1).ToArray(), Is.EqualTo(new int[] {6}));
        
        Assert.That(() => storage.GetColumnSlice(0, 2, 2), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(storage.GetColumnSlice(1, 2, 1).ToArray(), Is.EqualTo(new int[] {8}));
        Assert.That(storage.GetColumnSlice(2, 2, 0).ToArray(), Is.EqualTo(new int[] {}));
        
        Assert.That(storage.GetColumnSlice(0, 3, 1).ToArray(),  Is.EqualTo(new int[] {}));
        Assert.That(storage.GetColumnSlice(1, 3, 0).ToArray(), Is.EqualTo(new int[] {}));
    }
    
    [TestCaseSource(nameof(Storages))]
    public void StorageRowSlices(IMatrixStorage<int> storage)
    {
        Assert.That(storage.GetRowSlice(0, 0).ToArray(), Is.EqualTo(new int[] {1, 2, 3}));
        Assert.That(storage.GetRowSlice(1, 0).ToArray(), Is.EqualTo(new int[] {4, 5, 6}));
        Assert.That(storage.GetRowSlice(2, 0).ToArray(), Is.EqualTo(new int[] {7, 8, 9}));
        
        Assert.That(storage.GetRowSlice(0, 1).ToArray(), Is.EqualTo(new int[] {2, 3}));
        Assert.That(storage.GetRowSlice(1, 1).ToArray(), Is.EqualTo(new int[] {5, 6}));
        Assert.That(storage.GetRowSlice(2, 1).ToArray(), Is.EqualTo(new int[] {8, 9}));
        
        Assert.That(storage.GetRowSlice(0, 2).ToArray(), Is.EqualTo(new int[] {3}));
        Assert.That(storage.GetRowSlice(1, 2).ToArray(), Is.EqualTo(new int[] {6}));
        Assert.That(storage.GetRowSlice(2, 2).ToArray(), Is.EqualTo(new int[] {9}));
        
        Assert.That(storage.GetRowSlice(0, 3).ToArray(), Is.EqualTo(new int[] {}));
        Assert.That(storage.GetRowSlice(1, 3).ToArray(), Is.EqualTo(new int[] {}));
        Assert.That(storage.GetRowSlice(2, 3).ToArray(), Is.EqualTo(new int[] {}));
        
        Assert.That(storage.GetRowSlice(0, 0, 3).ToArray(), Is.EqualTo(new int[] {1, 2, 3}));
        Assert.That(storage.GetRowSlice(1, 0, 2).ToArray(), Is.EqualTo(new int[] {4, 5}));
        Assert.That(storage.GetRowSlice(2, 0, 1).ToArray(), Is.EqualTo(new int[] {7}));
        
        Assert.That(() => storage.GetRowSlice(0, 1, 3), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(storage.GetRowSlice(1, 1, 2).ToArray(), Is.EqualTo(new int[] {5, 6}));
        Assert.That(storage.GetRowSlice(2, 1, 1).ToArray(), Is.EqualTo(new int[] {8}));
        
        Assert.That(() => storage.GetRowSlice(0, 2, 2), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        Assert.That(storage.GetRowSlice(1, 2, 1).ToArray(), Is.EqualTo(new int[] {6}));
        Assert.That(storage.GetRowSlice(2, 2, 0).ToArray(), Is.EqualTo(new int[] {}));
        
        Assert.That(storage.GetRowSlice(0, 3, 1).ToArray(),  Is.EqualTo(new int[] {}));
        Assert.That(storage.GetRowSlice(1, 3, 0).ToArray(), Is.EqualTo(new int[] {}));
    }

    [Test]
    public void ColumnStorageConstructor()
    {
        var values = new int[12];
        values.AsSpan().Fill(3);
        var fill = new ColumnMajorMatrixStorage<int>(3, 4, values);
        var constant = new ColumnMajorMatrixStorage<int>(3, 4, 3);
        
        Assert.That(fill.Span.ToArray(), Is.EqualTo(values));
        Assert.That(constant.Span.ToArray(), Is.EqualTo(values));
    }

    [Test]
    public void ColumnCopy()
    {
        var column = new ColumnMajorMatrixStorage<int>(3, 4, TestContext.CurrentContext.Random.RandomNumbers<int>(12));
        var columCopy = column.Copy();
        Assert.That(column.Span.ToArray(), Is.EqualTo(columCopy.Span.ToArray()));
        column.SetElement(1,2, column.GetElement(1,2)+ 1);
        Assert.That(column.Span.ToArray(), Is.Not.EqualTo(columCopy.Span.ToArray()));
    }

    [Test]
    public void NotSupportedRandomNumberGenerator()
    {
        Assert.That(() => TestContext.CurrentContext.Random.RandomNumbers<byte>(12), Throws.Exception.TypeOf<NotSupportedException>());
    }
}
