using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LinearAlgebra.Structures;

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

    public static bool ApproxEquals<T>(this Structures.Vector<T> left, Structures.Vector<T> right, T tolerance) where T : INumber<T> => SequenceApproxEqual<T>((T[])left, (T[])right, tolerance);
    public static bool ApproxEquals(this Structures.Vector<float> left, Structures.Vector<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this Structures.Vector<double> left, Structures.Vector<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

    public static bool ApproxEquals<T>(this Matrix<T> left, Matrix<T> right, T tolerance) where T : INumber<T> => SequenceApproxEqual<T>(left.Elements, right.Elements, tolerance);
    public static bool ApproxEquals(this Matrix<float> left, Matrix<float> right, float tolerance = Constants.DefaultFloatTolerance) => ApproxEquals<float>(left, right, tolerance);
    public static bool ApproxEquals(this Matrix<double> left, Matrix<double> right, double tolerance = Constants.DefaultDoubleTolerance) => ApproxEquals<double>(left, right, tolerance);

}
