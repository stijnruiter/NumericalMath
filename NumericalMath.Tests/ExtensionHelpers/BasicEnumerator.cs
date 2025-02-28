using System.Collections;
using System.Collections.Generic;

namespace NumericalMath.Tests.ExtensionHelpers;

public static class EnumeratorExtensions
{
    public static IEnumerable<object?> ToWeakTypedEnumerable(this IEnumerator enumerator)
    {
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    public static IEnumerable<T> ToStrongTypedEnumerable<T>(this IEnumerator<T> enumerator)
    {
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}