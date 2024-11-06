using LinearAlgebra.Structures;

namespace LinearAlgebra.Test;

[TestFixture]
public class ArithmeticsMiscTests
{
    [Test]
    public void TensorRow()
    {
        RowVector<int> left = new RowVector<int>([ 1, 2 ]);
        RowVector<int> right = new RowVector<int>([ 4, 5, 6 ]);

        Assert.That((RowVector<int>)Arithmetics.TensorProduct(left, right),
            Is.EqualTo(new RowVector<int>([4, 5, 6, 8, 10, 12])));

        Assert.That((RowVector<int>)Arithmetics.TensorProduct(right, left),
            Is.EqualTo(new RowVector<int>([4, 8, 5, 10, 6, 12])));
    }

    [Test]
    public void TensorColumn()
    {
        ColumnVector<int> left = new ColumnVector<int>([1, 2]);
        ColumnVector<int> right = new ColumnVector<int>([4, 5, 6]);

        Assert.That((ColumnVector<int>)Arithmetics.TensorProduct(left, right),
            Is.EqualTo(new ColumnVector<int>([4, 5, 6, 8, 10, 12])));

        Assert.That((ColumnVector<int>)Arithmetics.TensorProduct(right, left),
            Is.EqualTo(new ColumnVector<int>([4, 8, 5, 10, 6, 12])));
    }

    [Test]
    public void TensorMatrix()
    {
        Matrix<int> left = new Matrix<int>(new int[,]
        {
            {1, 2, 3 },
            {4, 5, 6 }
        });

        Matrix<int> right = new Matrix<int>(new int[,]
        {
            { 1,  2,  3,  4 },
            { 5,  6,  7,  8 },
            { 9, 10, 11, 12 }
        });

        Matrix<int> expectedResultLeftRight = new Matrix<int>(new int[,]
        {
            { 1,  2,  3,  4,  2,  4,  6,  8,  3,  6,  9, 12},
            { 5,  6,  7,  8, 10, 12, 14, 16, 15, 18, 21, 24},
            { 9, 10, 11, 12, 18, 20, 22, 24, 27, 30, 33, 36},
            { 4,  8, 12, 16,  5, 10, 15, 20,  6, 12, 18, 24},
            {20, 24, 28, 32, 25, 30, 35, 40, 30, 36, 42, 48},
            {36, 40, 44, 48, 45, 50, 55, 60, 54, 60, 66, 72}     
        });

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(left, right), Is.EqualTo(expectedResultLeftRight));
    }

    [Test]
    public void TensorVectorVector()
    {
        ColumnVector<int> left = new ColumnVector<int>([1, 2]);
        RowVector<int> right = new RowVector<int>([4, 5, 6]);

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(left, right), Is.EqualTo(new Matrix<int>(new int[,] {
            { 4,  5,  6 },
            { 8, 10, 12 }
        })));

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(right, left), Is.EqualTo(new Matrix<int>(new int[,] {
            { 4,  5,  6 },
            { 8, 10, 12 }
        })));
    }

    [Test]
    public void TensorColumnVectorMatrix()
    {
        ColumnVector<int> left = new ColumnVector<int>([1, 2, 3]);
        Matrix<int> right = new Matrix<int>(new int[,]{
            { 4, 5, 6 },
            { 7, 8, 9 }
        });

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(left, right), Is.EqualTo(new Matrix<int>(new int[,]
        {
            { 4, 5, 6},
            { 7, 8, 9},
            { 8, 10, 12},
            {14, 16, 18},
            {12, 15, 18},
            {21, 24, 27}
        })));

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(right, left), Is.EqualTo(new Matrix<int>(new int[,]
        {
            {  4,  5,  6 },
            {  8, 10, 12 },
            { 12, 15, 18 },
            {  7,  8,  9 },
            { 14, 16, 18 },
            { 21, 24, 27 }
        })));
    }
    [Test]
    public void TensorRowVectorMatrix()
    {
        RowVector<int> left = new RowVector<int>([1, 2, 3]);
        Matrix<int> right = new Matrix<int>(new int[,]{
            { 4, 5, 6 },
            { 7, 8, 9 }
        });

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(right, left),
            Is.EqualTo(new Matrix<int>(new int[,]
            {
                { 4,  8, 12, 5, 10, 15, 6, 12, 18 },
                { 7, 14, 21, 8, 16, 24, 9, 18, 27 }
            })));

        Assert.That((Matrix<int>)Arithmetics.TensorProduct(left, right),
            Is.EqualTo(new Matrix<int>(new int[,]
            {
                { 4, 5, 6,  8, 10, 12, 12, 15, 18 },
                { 7, 8, 9, 14, 16, 18, 21, 24, 27 }
            })));
    }


}
