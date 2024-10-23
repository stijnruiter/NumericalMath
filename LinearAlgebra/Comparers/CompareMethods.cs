using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace LinearAlgebra.Comparers;

public static class CompareMethods
{

    public static bool ApproxEquals<T>(this T left, T right, T tolerance) where T : INumber<T> => T.Abs(left - right) < tolerance;
    public static bool ApproxEquals(this float left, float right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this double left, double right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

    public static bool SequenceApproxEqual<T>(this IEnumerable<T> left, IEnumerable<T> right, T tolerance) where T : INumber<T>
    {
        SequenceToleranceEqualityComparer<T> comparer = new(tolerance);
        return comparer.Equals(left, right);
    }
    public static bool SequenceApproxEqual(this IEnumerable<float> left, IEnumerable<float> right, float tolerance = Constants.DefaultFloatTolerance) => SequenceApproxEqual<float>(left, right, tolerance);
    public static bool SequenceApproxEqual(this IEnumerable<double> left, IEnumerable<double> right, double tolerance = Constants.DefaultDoubleTolerance) => SequenceApproxEqual<double>(left, right, tolerance);

    public static bool SequenceApproxEquals<T>(this T[][] left, T[][] right, T tolerance) where T : INumber<T>
    {
        SequenceToleranceEqualityComparer<T> comparer = new SequenceToleranceEqualityComparer<T>(tolerance);
        return left.SequenceEqual(right, comparer);
    }
    public static bool SequenceApproxEquals(this float[][] left, float[][] right, float tolerance = Constants.DefaultFloatTolerance) => SequenceApproxEquals<float>(left, right, tolerance);
    public static bool SequenceApproxEquals(this double[][] left, double[][] right, double tolerance = Constants.DefaultFloatTolerance) => SequenceApproxEquals<double>(left, right, tolerance);

    public static bool ApproxEquals<T>(this Structures.Vector<T> left, Structures.Vector<T> right, T tolerance) where T : INumber<T> => SequenceApproxEqual<T>((T[])left, (T[])right, tolerance);
    public static bool ApproxEquals(this Structures.Vector<float> left, Structures.Vector<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this Structures.Vector<double> left, Structures.Vector<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

    public static bool ApproxEquals<T>(this Structures.Matrix<T> left, Structures.Matrix<T> right, T tolerance) where T : INumber<T> => SequenceApproxEquals<T>(left.Elements, right.Elements, tolerance);
    public static bool ApproxEquals(this Structures.Matrix<float> left, Structures.Matrix<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this Structures.Matrix<double> left, Structures.Matrix<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

    public static bool JaggedSequenceEqual<T>(this IEnumerable<IEnumerable<T>> left, IEnumerable<IEnumerable<T>> right) => left.SequenceEqual(right, new SequenceEqualityComparer<T>());

}
