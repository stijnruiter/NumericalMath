﻿using LinearAlgebra.Exceptions;
using LinearAlgebra.Structures;

namespace LinearAlgebra.Test;

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
    }

    [Test]
    public void ColumnVectorIndexing()
    {
        ColumnVector<int> columnVector = new ColumnVector<int>([ 1, 2, 3, 4 ]);
        Assert.That(columnVector.Length, Is.EqualTo(4));
        Assert.That(columnVector.RowCount, Is.EqualTo(4));
        Assert.That(columnVector.ColumnCount, Is.EqualTo(1));

        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[-1]; });
        Assert.That(columnVector[0], Is.EqualTo(1));
        Assert.That(columnVector[1], Is.EqualTo(2));
        Assert.That(columnVector[2], Is.EqualTo(3));
        Assert.That(columnVector[3], Is.EqualTo(4));
        Assert.Throws<IndexOutOfRangeException>(() => { int result = columnVector[4]; });
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

        Assert.That(columnVector * rowVector1, Is.EqualTo(new Matrix<int>([[4, 8, 12], [5, 10, 15], [6, 12, 18]])));
        Assert.That(columnVector * rowVector2, Is.EqualTo(new Matrix<int>([[16, 20, -24], [20, 25, -30], [24, 30, -36]])));
    }

}