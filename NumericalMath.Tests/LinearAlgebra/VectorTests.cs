using NumericalMath.Exceptions;
using NumericalMath.LinearAlgebra.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;
using NumericalMath.Tests.ExtensionHelpers;

namespace NumericalMath.Tests.LinearAlgebra;

[TestFixture]
internal class VectorTests
{

    [Test]
    public void EmptyColumnVector()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>(4);
        Assert.That(columnVector.Length, Is.EqualTo(4));
        Assert.That(columnVector.RowCount, Is.EqualTo(4));
        Assert.That(columnVector.ColumnCount, Is.EqualTo(1));

        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[-1]; });
        Assert.That(columnVector[0], Is.EqualTo(0));
        Assert.That(columnVector[1], Is.EqualTo(0));
        Assert.That(columnVector[2], Is.EqualTo(0));
        Assert.That(columnVector[3], Is.EqualTo(0));
        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[4]; });

        Assert.That(columnVector.Equals(null), Is.False);
    }

    [Test]
    public void ColumnVectorIndexing()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4]);
        Assert.That(columnVector.Length, Is.EqualTo(4));
        Assert.That(columnVector.RowCount, Is.EqualTo(4));
        Assert.That(columnVector.ColumnCount, Is.EqualTo(1));

        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[-1]; });
        Assert.That(columnVector[0], Is.EqualTo(1));
        Assert.That(columnVector[1], Is.EqualTo(2));
        Assert.That(columnVector[2], Is.EqualTo(3));
        Assert.That(columnVector[3], Is.EqualTo(4));
        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[4]; });

        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[0, 1]; });
        Assert.That(columnVector[0, 0], Is.EqualTo(1));
        Assert.That(columnVector[1, 0], Is.EqualTo(2));
        Assert.That(columnVector[2, 0], Is.EqualTo(3));
        Assert.That(columnVector[3, 0], Is.EqualTo(4));
        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[4, 0]; });
    }

    [Test]
    public void ColumnVectorMatrixIndexing()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3, 4]);

        Assert.Throws<IndexOutOfRangeException>(() => { columnVector[0, 1] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[0, 1]; });
        Assert.Throws<IndexOutOfRangeException>(() => { columnVector[4, 0] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[4, 0]; });

        Assert.That(columnVector[0, 0], Is.EqualTo(1));
        Assert.That(columnVector[1, 0], Is.EqualTo(2));
        Assert.That(columnVector[2, 0], Is.EqualTo(3));
        Assert.That(columnVector[3, 0], Is.EqualTo(4));

        columnVector[0, 0] = 5;
        columnVector[1, 0] = 6;
        columnVector[2, 0] = 7;
        columnVector[3, 0] = 8;

        Assert.That(columnVector[0], Is.EqualTo(5));
        Assert.That(columnVector[1], Is.EqualTo(6));
        Assert.That(columnVector[2], Is.EqualTo(7));
        Assert.That(columnVector[3], Is.EqualTo(8));
    }

    [Test]
    public void EmptyRowVector()
    {
        RowVector<int> rowVector = new RowVector<int>(4);
        Assert.That(rowVector.Length, Is.EqualTo(4));
        Assert.That(rowVector.RowCount, Is.EqualTo(1));
        Assert.That(rowVector.ColumnCount, Is.EqualTo(4));

        Assert.Throws<IndexOutOfRangeException>(() => { int result = rowVector[-1]; });
        Assert.That(rowVector[0], Is.EqualTo(0));
        Assert.That(rowVector[1], Is.EqualTo(0));
        Assert.That(rowVector[2], Is.EqualTo(0));
        Assert.That(rowVector[3], Is.EqualTo(0));
        Assert.Throws<IndexOutOfRangeException>(() => { int result = rowVector[4]; });

        Assert.That(rowVector.Equals(null), Is.False);
    }

    [Test]
    public void RowVectorIndexing()
    {
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3, 4]);
        Assert.That(rowVector.Length, Is.EqualTo(4));
        Assert.That(rowVector.RowCount, Is.EqualTo(1));
        Assert.That(rowVector.ColumnCount, Is.EqualTo(4));

        Assert.Throws<IndexOutOfRangeException>(() => { int result = rowVector[-1]; });
        Assert.That(rowVector[0], Is.EqualTo(1));
        Assert.That(rowVector[1], Is.EqualTo(2));
        Assert.That(rowVector[2], Is.EqualTo(3));
        Assert.That(rowVector[3], Is.EqualTo(4));
        Assert.Throws<IndexOutOfRangeException>(() => { int result = rowVector[4]; });
    }

    [Test]
    public void RowVectorMatrixIndexing()
    {
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3, 4]);

        Assert.Throws<IndexOutOfRangeException>(() => { rowVector[1, 0] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { int result = rowVector[1, 0]; });
        Assert.Throws<IndexOutOfRangeException>(() => { rowVector[0, 4] = 1; });
        Assert.Throws<IndexOutOfRangeException>(() => { int result = rowVector[0, 4]; });

        Assert.That(rowVector[0, 0], Is.EqualTo(1));
        Assert.That(rowVector[0, 1], Is.EqualTo(2));
        Assert.That(rowVector[0, 2], Is.EqualTo(3));
        Assert.That(rowVector[0, 3], Is.EqualTo(4));

        rowVector[0, 0] = 5;
        rowVector[0, 1] = 6;
        rowVector[0, 2] = 7;
        rowVector[0, 3] = 8;

        Assert.That(rowVector[0], Is.EqualTo(5));
        Assert.That(rowVector[1], Is.EqualTo(6));
        Assert.That(rowVector[2], Is.EqualTo(7));
        Assert.That(rowVector[3], Is.EqualTo(8));
    }

    [Test]
    public void ScalarProducts()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3]);
        RowVector<int> rowVector = new RowVector<int>([4, 5, 6]);

        Assert.That(columnVector * 2, Is.EqualTo(new ColumnVector<int>([2, 4, 6])));
        Assert.That(2 * columnVector, Is.EqualTo(new ColumnVector<int>([2, 4, 6])));
        Assert.That(2 * columnVector * 3, Is.EqualTo(new ColumnVector<int>([6, 12, 18])));

        Assert.That(rowVector * 2, Is.EqualTo(new RowVector<int>([8, 10, 12])));
        Assert.That(2 * rowVector, Is.EqualTo(new RowVector<int>([8, 10, 12])));
        Assert.That(2 * rowVector * 3, Is.EqualTo(new RowVector<int>([24, 30, 36])));
    }

    [Test]
    public void Transpose()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([1, 2, 3]);
        RowVector<int> rowVector = new RowVector<int>([1, 2, 3]);

        Assert.That(columnVector.Transpose(), Is.EqualTo(rowVector));
        Assert.That(columnVector.Transpose().Transpose(), Is.EqualTo(columnVector));

        Assert.That(rowVector.Transpose(), Is.EqualTo(columnVector));
        Assert.That(rowVector.Transpose().Transpose(), Is.EqualTo(rowVector));
    }

    [Test]
    public void AdditionAndSubtraction()
    {
        ColumnVector<int> columnVector1 = new ColumnVector<int>([1, 2, 3]);
        ColumnVector<int> columnVector2 = new ColumnVector<int>([4, 5, 6]);

        RowVector<int> rowVector1 = new RowVector<int>([1, 2, 3]);
        RowVector<int> rowVector2 = new RowVector<int>([4, 5, 6]);

        Assert.That(columnVector1 + columnVector2, Is.EqualTo(new ColumnVector<int>([5, 7, 9])));
        Assert.That(columnVector1 - columnVector2, Is.EqualTo(new ColumnVector<int>([-3, -3, -3])));

        Assert.That(rowVector1 + rowVector2, Is.EqualTo(new RowVector<int>([5, 7, 9])));
        Assert.That(rowVector1 - rowVector2, Is.EqualTo(new RowVector<int>([-3, -3, -3])));

        Assert.Throws<DimensionMismatchException>(() => { var x = new ColumnVector<int>([1, 2, 3]) + new ColumnVector<int>([1, 2]); });
        Assert.Throws<DimensionMismatchException>(() => { var x = new RowVector<int>([1, 2, 3]) + new RowVector<int>([1, 2]); });
    }
    
    [Test]
    public void VectorizedVectorOperations()
    {
        int[] largeArray = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];
        int[] expectedSum = [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40];
        int[] expectedSub = new int[20];
        ColumnVector<int> columnVector1 = new ColumnVector<int>(largeArray);
        ColumnVector<int> columnVector2 = new ColumnVector<int>(largeArray);

        RowVector<int> rowVector1 = new RowVector<int>(largeArray);
        RowVector<int> rowVector2 = new RowVector<int>(largeArray);

        Assert.That(columnVector1 + columnVector2, Is.EqualTo(new ColumnVector<int>(expectedSum)));
        Assert.That(2 * columnVector2, Is.EqualTo(new ColumnVector<int>(expectedSum)));
        Assert.That(columnVector1 - columnVector2, Is.EqualTo(new ColumnVector<int>(expectedSub)));

        Assert.That(rowVector1 + rowVector2, Is.EqualTo(new RowVector<int>(expectedSum)));
        Assert.That(2 * rowVector2, Is.EqualTo(new RowVector<int>(expectedSum)));
        Assert.That(rowVector1 - rowVector2, Is.EqualTo(new RowVector<int>(expectedSub)));
    }

    [Test]
    public void InnerProduct()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([4, 5, 6]);
        RowVector<int> rowVector1 = new RowVector<int>([1, 2, 3]);
        RowVector<int> rowVector2 = new RowVector<int>([4, 5, -6]);

        Assert.That(rowVector1 * columnVector, Is.EqualTo(32));
        Assert.That(rowVector2 * columnVector, Is.EqualTo(5));
    }

    [Test]
    public void OuterProduct()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([4, 5, 6]);
        RowVector<int> rowVector1 = new RowVector<int>([1, 2, 3]);
        RowVector<int> rowVector2 = new RowVector<int>([4, 5, -6]);
        var outer1 = columnVector * rowVector1;
        var outer2 = columnVector * rowVector2;
        Assert.That(outer1, Is.EqualTo(new Matrix<int>(new int[,] { { 4, 8, 12 }, { 5, 10, 15 }, { 6, 12, 18 } })));
        Assert.That(outer2, Is.EqualTo(new Matrix<int>(new int[,] { { 16, 20, -24 }, { 20, 25, -30 }, { 24, 30, -36 } })));
    }

    [Test]
    public void DifferentStrideVectorOperations()
    {
        var columnVector1 = new ColumnVector<int>(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3);
        var columnVector2 = new ColumnVector<int>(new[] { 1, 2, 3, 4, 5, 6 }, 2);
        
        Assert.That(columnVector1, Has.Length.EqualTo(3));
        Assert.That(columnVector2, Has.Length.EqualTo(3));

        Assert.That((columnVector1 + columnVector2).ToArray(), Is.EqualTo(new[] { 2, 7, 12 }));
        Assert.That((columnVector1 - columnVector2).ToArray(), Is.EqualTo(new[] { 0, 1, 2 }));
        Assert.That((5 * columnVector1).ToArray(), Is.EqualTo(new[] { 5, 20, 35 }));
        Assert.That((4 * columnVector2).ToArray(), Is.EqualTo(new[] { 4, 12, 20 }));
    }

    [Test]
    public void DifferentStorageMatrixOperations()
    {
        var columnStorage = new ColumnMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        var rowStorage  = new RowMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } });
        var columnMatrix = new Matrix<int>(columnStorage);
        var rowMatrix = new Matrix<int>(rowStorage);

        
        Matrix<int> resultSum =
        [
            [2, 4, 6],
            [8, 10, 12],
            [14, 16, 18]
        ];
        
        Matrix<int> resultSub = Matrix<int>.Zero(3, 3);

        Matrix<int> scalarProduct =
        [
            [5, 10, 15],
            [20, 25, 30],
            [35, 40, 45]
        ];

        Assert.That(columnMatrix + rowMatrix, Is.EqualTo(resultSum));
        Assert.That(columnMatrix - rowMatrix, Is.EqualTo(resultSub));
        Assert.That(5 * columnMatrix, Is.EqualTo(scalarProduct));
        Assert.That(5 * rowMatrix, Is.EqualTo(scalarProduct));
        Assert.That(columnMatrix * 5, Is.EqualTo(scalarProduct));
        Assert.That(rowMatrix * 5, Is.EqualTo(scalarProduct));
    }

    [TestCase(new[]{1,2,3,4}, new[]{1,2,3,5})]
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3})]
    public void NotEqualRows(int[] array1, int[] array2)
    {
        var rowVector1 = new RowVector<int>(array1);
        var rowVector2 = new RowVector<int>(array2);
        Assert.That(rowVector1, Is.Not.EqualTo(rowVector2));
    }
    
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3,5})]
    [TestCase(new[]{1,2,3,4}, new[]{1,2,3})]
    public void NotEqualColumns(int[] array1, int[] array2)
    {
        var rowVector1 = new ColumnVector<int>(array1);
        var rowVector2 = new ColumnVector<int>(array2);
        Assert.That(rowVector1, Is.Not.EqualTo(rowVector2));
    }

    public static IEnumerable<TestCaseData> Vectors()
    {
        var elements = new[] { 1, 2, 3, 4, 5 };
        yield return new TestCaseData(new RowVector<int>(elements)).Returns(elements).SetArgDisplayNames("RowVector");
        yield return new TestCaseData(new ColumnVector<int>(elements)).Returns(elements).SetArgDisplayNames("ColumnVector");
    }
    
    [TestCaseSource(nameof(Vectors))]
    public IEnumerable<int> EnumeratorVector(AbstractVector<int> vector)
    {
        using var enumerator = vector.GetEnumerator();
        List<int> values = new();
        while (enumerator.MoveNext())
        {
            values.Add(enumerator.Current);
        }

        var basicValues = enumerator.ToEnumerable().ToArray();
        Assert.That(basicValues, Is.EqualTo(values));
        return values;
    }

    [Test]
    public void ConstantColumnVector()
    {
        var columnVector = new ColumnVector<int>(5, 0);
        Assert.That(columnVector, Is.EqualTo(ColumnVector<int>.Zero(5)));

        columnVector = new ColumnVector<int>(5, 3);
        Assert.That(columnVector.ToArray(), Is.EqualTo(new[] { 3, 3, 3, 3, 3 }));
    }

    [Test]
    public void ConstantRowVector()
    {
        var rowVector = new RowVector<int>(5, 0);
        Assert.That(rowVector, Is.EqualTo(RowVector<int>.Zero(5)));

        rowVector = new RowVector<int>(5, 4);
        Assert.That(rowVector.ToArray(), Is.EqualTo(new[] { 4, 4, 4, 4, 4 }));
    }

    [Test]
    public void CopyColumn()
    {
        var columnVector = new ColumnVector<int>([1, 2, 3, 4]);
        var columnVector2 = columnVector.Copy();
        
        Assert.That(columnVector, Is.EqualTo(columnVector2));
        columnVector[0] = 5;
        Assert.That(columnVector, Is.Not.EqualTo(columnVector2));
    }
    
    [Test]
    public void CopyRow()
    {
        var rowVector = new RowVector<int>([1, 2, 3, 4]);
        var rowVector2 = rowVector.Copy();
        
        Assert.That(rowVector, Is.EqualTo(rowVector2));
        rowVector[0] = 5;
        Assert.That(rowVector, Is.Not.EqualTo(rowVector2));
    }

    [Test]
    public void ExplicitCastVector()
    {
        int[] values = [1, 2, 3, 4, 5];
        var column = (ColumnVector<int>)values;
        var row = (RowVector<int>)values;

        Assert.That(column.ToArray(), Is.EqualTo(values));
        Assert.That(row.ToArray(), Is.EqualTo(values));
    }

    [Test]
    public void RandomColumnVector()
    {
        var randomColumn1 = ColumnVector<int>.Random(5);
        var randomColumn2 = ColumnVector<int>.Random(5);
        Assert.That(randomColumn1, Is.Not.EqualTo(randomColumn2));
    }

    [Test]
    public void RandomRowVector()
    {
        var randomRow1 = RowVector<double>.Random(5);
        var randomRow2 = RowVector<double>.Random(5);
        Assert.That(randomRow1, Is.Not.EqualTo(randomRow2));
    }

}