using System;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;
using System.Collections.Generic;

namespace NumericalMath.Tests.Geometry;

[TestFixture(typeof(Delaunay))]
[TestFixture(typeof(DelaunayNaive))]
public class DelaunayTests<T> where T : IDelaunay
{
    [TestCase(0f, 0f, 1f, 0f, 0f, 1f, 3f, 3f, ExpectedResult = -12f, TestName = "Outside circle")] // Outside circle
    [TestCase(0f, 0f, 1f, 0f, 0f, 1f, 1f, 0.5f, ExpectedResult = 0.25f, TestName = "Inside cricle")] // Inside circle
    public float InCircleDeterminant(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy)
    {
        var a = new Vertex2(ax, ay);
        var b = new Vertex2(bx, by);
        var c = new Vertex2(cx, cy);
        var d = new Vertex2(dx, dy);

        return T.InCircleDeterminant(a, b, c, d);
    }

    private static IEnumerable<TestCaseData> DelaunayData()
    {
        var input = new Vertex2[] {
            new(0.13375837f, -0.20463276f),
            new(-0.0393703f, 0.3368733f),
            new(-0.37653553f, 0.30180907f),
            new(0.17102325f, -0.39327747f),
            new(-0.1741004f, 0.5621339f)
        };
        var expectedInterior = new TriangleElement[]
        {
             new(0, 2, 3),
             new(1, 4, 2),
             new(0, 1, 2),
        };
        var expectedBoundary = new LineElement[]
        {
            new(0, 1),
            new(0, 3),
            new(2, 3),
            new(2, 4),
            new(4, 1),
        };
        yield return new TestCaseData(input, expectedInterior, expectedBoundary).SetName("Triangulation with 5 vertices");

        input = [
            new (0.13375837f, -0.20463276f),
            new (-0.0393703f, 0.3368733f),
            new (-0.37653553f, 0.30180907f),
            new (0.17102325f, -0.39327747f),
            new (-0.1741004f, 0.5621339f),
            new (0.7005063f, -0.49821767f),
            new (0.6051475f, 0.061599255f),
            new (0.50220823f, -0.20197105f),
            new (0.17317522f, 0.010558367f),
            new (0.58225656f, 0.36910498f),
            new (-0.3951408f, 0.41237962f),
            new (0.041640043f, -0.664723f),
            new (0.26772082f, -0.35440302f),
            new (0.13839245f, -0.19876397f),
            new (0.25869608f, -0.67407477f),
            new (-0.6667158f, -0.039070487f),
            new (0.6294924f, 0.015692353f),
            new (0.15297616f, 0.4711753f),
            new (0.49639237f, -0.5018836f),
            new (0.17881477f, -0.66860473f),
            new (-0.031089962f, 0.3436073f),
            new (0.51314855f, -0.15569323f),
            new (-0.506284f, -0.48687214f),
            new (0.31463242f, 0.5525198f),
            new (-0.3399611f, 0.7265549f),
        ];
        expectedInterior =
        [
            new( 9, 16,  5),
            new( 6,  9,  8),
            new( 6, 16,  9),
            new(10, 24, 15),
            new( 2, 10, 15),
            new(21,  6,  8),
            new( 6, 21, 16),
            new(16, 21,  5),
            new(22,  0, 15),
            new( 0,  2, 15),
            new( 2,  0,  8),
            new( 1,  2,  8),
            new(20,  1,  8),
            new(17, 20,  8),
            new( 9, 17,  8),
            new(23, 17,  9),
            new(18, 14,  5),
            new(14, 18, 12),
            new(14, 19, 11),
            new(13, 21,  8),
            new( 0, 13,  8),
            new(13,  0, 12),
            new( 0,  3, 12),
            new(19,  3, 11),
            new( 3, 22, 11),
            new( 3,  0, 22),
            new( 3, 14, 12),
            new( 3, 19, 14),
            new(17,  4, 20),
            new(10,  4, 24),
            new( 4, 23, 24),
            new( 4, 17, 23),
            new( 4,  1, 20),
            new( 4, 10,  2),
            new( 1,  4,  2),
            new( 7, 13, 12),
            new(13,  7, 21),
            new(18,  7, 12),
            new(21,  7,  5),
            new( 7, 18,  5),
        ];
        expectedBoundary = [
            new (15, 24),
            new (24, 23),
            new (23, 9),
            new (9, 5),
            new (5, 14),
            new (14, 11),
            new (11, 22),
            new (22, 15),
        ];

        yield return new TestCaseData(input, expectedInterior, expectedBoundary).SetName("Triangulation with 25 vertices");


    }

    [TestCaseSource(nameof(DelaunayData))]
    public void DelaunayTest(Vertex2[] input, TriangleElement[] expectedInterior, LineElement[] expectedBoundary)
    {
        var output = T.CreateTriangulation(input);
        Assert.That(output.Interior, Is.EquivalentTo(expectedInterior)
            .Using<TriangleElement>(CyclicalIdentical));

        Assert.That(output.Boundary, Is.EquivalentTo(expectedBoundary)
            .Using<LineElement>(CyclicalIdentical));
    }

    private static bool CyclicalIdentical(TriangleElement element1, TriangleElement element2)
    {
        return (element1.I == element2.I && element1.J == element2.J && element1.K == element2.K) ||
               (element1.I == element2.J && element1.J == element2.K && element1.K == element2.I) ||
               (element1.I == element2.K && element1.J == element2.I && element1.K == element2.J);
    }

    private static bool CyclicalIdentical(LineElement element1, LineElement element2)
    {
        if (element1.I == element2.I && element1.J == element2.J)
            return true;
        if (element1.I == element2.J && element1.J == element2.I)
            return true;
        return false;
    }
}