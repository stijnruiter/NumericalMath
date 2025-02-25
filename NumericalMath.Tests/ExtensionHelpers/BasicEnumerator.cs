using System.Collections;
using System.Collections.Generic;

namespace NumericalMath.Tests.ExtensionHelpers;

public static class EnumeratorExtensions
{
    public static IEnumerable<object?> ToEnumerable(this IEnumerator enumerator)
    {
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
}