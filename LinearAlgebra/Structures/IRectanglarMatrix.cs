﻿using System.Numerics;

namespace LinearAlgebra.Structures;

public interface IRectanglarMatrix<T> where T : INumber<T>
{
    public int RowCount { get; }
    public int ColumnCount { get; }
    public T this[int index] { get; set; }
    public T this[int rowIndex, int columnIndex] { get; set; }
}
