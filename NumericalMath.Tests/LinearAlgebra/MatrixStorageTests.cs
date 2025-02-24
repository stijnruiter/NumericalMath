using System;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.LinearAlgebra.Structures;
using NumericalMath.LinearAlgebra.Structures.MatrixStorage;
using NumericalMath.Tests.Comparers;

namespace NumericalMath.Tests.LinearAlgebra;

[TestFixture]
public class MatrixStorageTests
{

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
    
    public static IEnumerable<TestCaseData> StoragesEnumerable()
    {
        RowVector<int>[] expectedRows = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
        var values = new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        yield return new TestCaseData(new ColumnMajorMatrixStorage<int>(values)).Returns(expectedRows);
        yield return new TestCaseData(new RowMajorMatrixStorage<int>(values)).Returns(expectedRows);
    }
    
    [TestCaseSource(nameof(StoragesEnumerable))]
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

    public static IEnumerable<TestCaseData> Storages()
    {
        yield return new TestCaseData(new RowMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } })).SetArgDisplayNames("RowMajor");
        yield return new TestCaseData(new ColumnMajorMatrixStorage<int>(new[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } })).SetArgDisplayNames("ColumnMajor");
    }
    
    [TestCaseSource(nameof(Storages))]
    public void SwapRows(IMatrixStorage<int> storage)
    {
        Assume.That(storage.GetRow(0).ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        Assume.That(storage.GetRow(1).ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
        Assume.That(storage.GetRow(2).ToArray(), Is.EqualTo(new[] { 7, 8, 9 }));
        
        storage.SwapRows(0, 1);
        
        Assert.That(storage.GetRow(0).ToArray(), Is.EqualTo(new[] { 4, 5, 6 }));
        Assert.That(storage.GetRow(1).ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(storage.GetRow(2).ToArray(), Is.EqualTo(new[] { 7, 8, 9 }));
    }

    [TestCaseSource(nameof(Storages))]
    public void StorageColumnSlicesStartIndex(IMatrixStorage<int> storage)
    {
        Assert.That(storage.GetColumnSlice(0, 0).ToArray(), Is.EqualTo(new int[] { 1, 4, 7 }));
        Assert.That(storage.GetColumnSlice(1, 0).ToArray(), Is.EqualTo(new int[] { 2, 5, 8 }));
        Assert.That(storage.GetColumnSlice(2, 0).ToArray(), Is.EqualTo(new int[] { 3, 6, 9 }));

        Assert.That(storage.GetColumnSlice(0, 1).ToArray(), Is.EqualTo(new int[] { 4, 7 }));
        Assert.That(storage.GetColumnSlice(1, 1).ToArray(), Is.EqualTo(new int[] { 5, 8 }));
        Assert.That(storage.GetColumnSlice(2, 1).ToArray(), Is.EqualTo(new int[] { 6, 9 }));

        Assert.That(storage.GetColumnSlice(0, 2).ToArray(), Is.EqualTo(new int[] { 7 }));
        Assert.That(storage.GetColumnSlice(1, 2).ToArray(), Is.EqualTo(new int[] { 8 }));
        Assert.That(storage.GetColumnSlice(2, 2).ToArray(), Is.EqualTo(new int[] { 9 }));

        Assert.That(storage.GetColumnSlice(0, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(storage.GetColumnSlice(1, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(storage.GetColumnSlice(2, 3).ToArray(), Is.EqualTo(new int[] { }));
    }
    
    [TestCaseSource(nameof(Storages))]
    public void StorageColumnSlicesStartWithLength(IMatrixStorage<int> storage)
    {
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
    public void StorageRowSlicesStartIndex(IMatrixStorage<int> storage)
    {
        Assert.That(storage.GetRowSlice(0, 0).ToArray(), Is.EqualTo(new int[] { 1, 2, 3 }));
        Assert.That(storage.GetRowSlice(1, 0).ToArray(), Is.EqualTo(new int[] { 4, 5, 6 }));
        Assert.That(storage.GetRowSlice(2, 0).ToArray(), Is.EqualTo(new int[] { 7, 8, 9 }));

        Assert.That(storage.GetRowSlice(0, 1).ToArray(), Is.EqualTo(new int[] { 2, 3 }));
        Assert.That(storage.GetRowSlice(1, 1).ToArray(), Is.EqualTo(new int[] { 5, 6 }));
        Assert.That(storage.GetRowSlice(2, 1).ToArray(), Is.EqualTo(new int[] { 8, 9 }));

        Assert.That(storage.GetRowSlice(0, 2).ToArray(), Is.EqualTo(new int[] { 3 }));
        Assert.That(storage.GetRowSlice(1, 2).ToArray(), Is.EqualTo(new int[] { 6 }));
        Assert.That(storage.GetRowSlice(2, 2).ToArray(), Is.EqualTo(new int[] { 9 }));

        Assert.That(storage.GetRowSlice(0, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(storage.GetRowSlice(1, 3).ToArray(), Is.EqualTo(new int[] { }));
        Assert.That(storage.GetRowSlice(2, 3).ToArray(), Is.EqualTo(new int[] { }));
    }
    
    [TestCaseSource(nameof(Storages))]
    public void StorageRowSlicesStartWithLength(IMatrixStorage<int> storage)
    {
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
    public void ColumnCopy()
    {
        var column = new ColumnMajorMatrixStorage<int>(3, 4, TestContext.CurrentContext.Random.RandomNumbers<int>(12));
        var columCopy = column.Copy();
        Assert.That(column.Span.ToArray(), Is.EqualTo(columCopy.Span.ToArray()));
        column.SetElement(1,2, column.GetElement(1,2)+ 1);
        Assert.That(column.Span.ToArray(), Is.Not.EqualTo(columCopy.Span.ToArray()));
    }

}