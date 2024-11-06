using LinearAlgebra.Structures;
using System;
using System.Linq;
using System.Xml.Linq;

namespace LinearAlgebra.Test;

[TestFixture]
internal class DenseMatrixTests
{

    [Test]
    public void EmptyMatrix()
    {
        DenseMatrix<int> matrix = new DenseMatrix<int>(3, 2, 0);
        Assert.That(matrix.RowCount, Is.EqualTo(3));
        Assert.That(matrix.ColumnCount, Is.EqualTo(2));

        Assert.That(matrix.At(0,0), Is.EqualTo(0));
        Assert.That(matrix.At(1,0), Is.EqualTo(0));
        Assert.That(matrix.At(2,0), Is.EqualTo(0));

        Assert.That(matrix.At(0,1), Is.EqualTo(0));
        Assert.That(matrix.At(1,1), Is.EqualTo(0));
        Assert.That(matrix.At(2,1), Is.EqualTo(0));

        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.At(3, 0); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.At(3, 0); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.At(-1, 1); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.At(2, -1); });
    }

    [Test]
    public void MatrixIndexing()
    {
        DenseMatrix<int> matrix = new DenseMatrix<int>(new int[,] { { 0, 1, 2 }, { 3, 4, 5 } });
        Assert.That(matrix.RowCount, Is.EqualTo(2));
        Assert.That(matrix.ColumnCount, Is.EqualTo(3));

        Assert.That(matrix.At(0, 0), Is.EqualTo(0));
        Assert.That(matrix.At(0, 1), Is.EqualTo(1));
        Assert.That(matrix.At(0, 2), Is.EqualTo(2));

        Assert.That(matrix.At(1, 0), Is.EqualTo(3));
        Assert.That(matrix.At(1, 1), Is.EqualTo(4));
        Assert.That(matrix.At(1, 2), Is.EqualTo(5));

        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.At(0, 3); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.At(2, 2); });

        Assert.That(matrix.GetColumn(0), Is.EqualTo(new int[2] { 0, 3 }));
        Assert.That(matrix.GetColumn(1), Is.EqualTo(new int[2] { 1, 4 }));
        Assert.That(matrix.GetColumn(2), Is.EqualTo(new int[2] { 2, 5 }));

        Assert.That(matrix.GetRow(0).ToArray(), Is.EqualTo(new int[3] { 0, 1, 2 }));
        Assert.That(matrix.GetRow(1).ToArray(), Is.EqualTo(new int[3] { 3, 4, 5 }));

        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.GetColumn(3); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.GetRow(2); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.GetColumn(-1); });
        Assert.Throws<IndexOutOfRangeException>(() => { var _ = matrix.GetRow(-1); });
    }

    [Test]
    public void SetMatrix()
    {
        DenseMatrix<int> matrix = new DenseMatrix<int>(new int[,] { { 0, 1, 2 }, { 3, 4, 5 } });

        Assert.That(matrix.At(1, 0), Is.EqualTo(3));
        matrix.At(1, 0, 5);
        Assert.That(matrix.GetRow(0).ToArray(), Is.EqualTo(new int[] { 0, 1, 2 }));
        Assert.That(matrix.GetRow(1).ToArray(), Is.EqualTo(new int[] { 5, 4, 5 }));

        matrix.SetRow(0, [6, 7, 8]);
        Assert.That(matrix.GetRow(0).ToArray(), Is.EqualTo(new int[] { 6, 7, 8 }));
        Assert.That(matrix.GetRow(1).ToArray(), Is.EqualTo(new int[] { 5, 4, 5 }));

        matrix.SetColumn(1, [12, 13]);
        Assert.That(matrix.GetRow(0).ToArray(), Is.EqualTo(new int[] { 6, 12, 8 }));
        Assert.That(matrix.GetRow(1).ToArray(), Is.EqualTo(new int[] { 5, 13, 5 }));

        int[] destination = new int[3];
        matrix.CopyRow(0, destination);
        Assert.That(destination, Is.EqualTo(new int[] { 6, 12, 8 }));
    }

}
