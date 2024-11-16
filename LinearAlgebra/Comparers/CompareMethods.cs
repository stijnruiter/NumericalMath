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

    public static bool ApproxEquals<T>(this AbstractVector<T> left, AbstractVector<T> right, T tolerance) where T : struct, INumber<T> => SequenceApproxEqual(left.Span, right.Span, tolerance);
    public static bool ApproxEquals(this AbstractVector<float> left, AbstractVector<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this AbstractVector<double> left, AbstractVector<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

    public static bool ApproxEquals<T>(this Matrix<T> left, Matrix<T> right, T tolerance) where T : struct, INumber<T> => SequenceApproxEqual<T>(left.Span, right.Span, tolerance);
    public static bool ApproxEquals(this Matrix<float> left, Matrix<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this Matrix<double> left, Matrix<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

}
