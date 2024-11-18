using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LinearAlgebra.Structures;

namespace LinearAlgebra.Comparers;

public static class CompareMethods
{
    public static bool SequenceApproxEqual<T>(this IEnumerable<T> left, IEnumerable<T> right, T tolerance) where T : struct, INumber<T> => left.SequenceEqual(right, new ToleranceEqualityComparer<T>(tolerance));
    public static bool SequenceApproxEqual<T>(this Span<T> left, Span<T> right, T tolerance) where T : struct, INumber<T> => left.SequenceEqual(right, new ToleranceEqualityComparer<T>(tolerance));

    public static bool SequenceApproxEqual(this IEnumerable<float> left, IEnumerable<float> right, float tolerance = Constants.DefaultFloatTolerance) => SequenceApproxEqual<float>(left, right, tolerance);
    public static bool SequenceApproxEqual(this IEnumerable<double> left, IEnumerable<double> right, double tolerance = Constants.DefaultDoubleTolerance) => SequenceApproxEqual<double>(left, right, tolerance);

    public static bool ApproxEquals<T>(this IRectanglarMatrix<T> left, IRectanglarMatrix<T> right, T tolerance) where T : struct, INumber<T>
    {
        if (left.RowCount != right.RowCount)
            return false;
        if (left.ColumnCount != right.ColumnCount)
            return false;

        for (int i = 0; i < left.RowCount; i++)
            for (int j = 0; j < left.ColumnCount; j++)
                if (T.Abs(left[i, j] - right[i, j]) > tolerance)
                    return false;
        return true;
        //return SequenceApproxEqual(left.Span, right.Span, tolerance);
    }

    public static bool ApproxEquals(this IRectanglarMatrix<float> left, IRectanglarMatrix<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this IRectanglarMatrix<double> left, IRectanglarMatrix<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);
}
