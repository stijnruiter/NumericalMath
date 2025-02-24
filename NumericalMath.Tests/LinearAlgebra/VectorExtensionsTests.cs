using System;
using System.Collections.Generic;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Tests.LinearAlgebra;

[TestFixture]
public class VectorExtensionsTests
{
    public static IEnumerable<TestCaseData> VectorNorms()
    {
        yield return new TestCaseData(Array.Empty<float>(), 0f, 0f, null);
        yield return new TestCaseData(new float[] { 1, 2, 3, 4, 5 }, 15f, 7.416198f, 5f);
        yield return new TestCaseData(new float[] { -5.2f, 3, 2.8f, 4, 2, 5 }, 22f, 9.427619f, 5.2f);
    }
    
    [TestCaseSource(nameof(VectorNorms))]
    public void VectorNorm(float[] vector, float sum, float mean, float? max)
    {
        var column = new ColumnVector<float>(vector);
        Assert.That(column.Norm1(), Is.EqualTo(sum).Within(1e-5));
        Assert.That(column.Norm2(), Is.EqualTo(mean).Within(1e-5));
        Assert.That(column.NormInf(), Is.EqualTo(max).Within(1e-5));
    }
}