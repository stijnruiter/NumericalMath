using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry;

[TestFixture]
public class HalfEdgeTriangulationTests
{
    [Test]
    public void HalfEdgeStructureDefault()
    {
        var halfEdge = new HalfEdge();
        var halfEdgeDefault = new HalfEdge(-1, -1, -1, -1, -1, -1);
        Assert.That(halfEdge, Is.EqualTo(halfEdgeDefault));
    }
    
    [Test]
    public void HalfEdgeStructure()
    {
        var halfEdge = new HalfEdge(4, 5, 6, 7, 8, 9);
        
        Assert.That(halfEdge.V1, Is.EqualTo(4));
        Assert.That(halfEdge.V2, Is.EqualTo(5));
        Assert.That(halfEdge.PrevEdge, Is.EqualTo(6));
        Assert.That(halfEdge.NextEdge, Is.EqualTo(7));
        Assert.That(halfEdge.TwinEdge, Is.EqualTo(8));
        Assert.That(halfEdge.ElementIndex, Is.EqualTo(9));
        
        Assert.That(halfEdge.ToString(), Does.Contain("(4, 5)"));
    }
    
    [Test]
    public void HalfEdgeDataStructureConstructoir()
    {
        var halfEdges = new HalfEdgeTriangulation(5);
        Assert.That(halfEdges.ElementCount, Is.EqualTo(0));
        Assert.That(halfEdges.EdgeCount, Is.EqualTo(0));
        
        Assert.That(halfEdges.ElementCapacity, Is.EqualTo(5));
        Assert.That(halfEdges.EdgeCapacity, Is.EqualTo(15));
    }

    [Test]
    public void ExceedCapacity()
    {
        var halfEdges = new HalfEdgeTriangulation(5);
        halfEdges.AddTriangle(0, 1, 2);
        halfEdges.AddTriangle(1, 3, 2);
        halfEdges.AddTriangle(3, 4, 2);
        halfEdges.AddTriangle(1, 5, 3);
        halfEdges.AddTriangle(5, 6, 3);
        Assert.That(() => halfEdges.AddTriangle(3, 6, 7), Throws.Exception);
    }
    
    [Test]
    public void AddTriangle()
    {
        HalfEdgeTriangulation halfEdges = new HalfEdgeTriangulation(2);
        halfEdges.AddTriangle(0, 1, 2);
        halfEdges.AddTriangle(1, 3, 2);
        Assert.That(halfEdges.ElementCount, Is.EqualTo(2));
        Assert.That(halfEdges.EdgeCount, Is.EqualTo(6));
    
        var expectedEdges = new[]
        {
            new HalfEdge(0, 1, 2, 1, -1, 0),
            new HalfEdge(1, 2, 0, 2, 5, 0),
            new HalfEdge(2, 0, 1, 0, -1, 0),
            
            new HalfEdge(1, 3, 5, 4, -1, 1),
            new HalfEdge(3, 2, 3, 5, -1, 1),
            new HalfEdge(2, 1, 4, 3, 1, 1),
        };
        Assert.That(halfEdges.Edges.ToArray(), Is.EquivalentTo(expectedEdges));
    }
    
    [Test]
    public void EnumerateElement()
    {
        // Create "hexagon" shaped triangulation
        var halfEdges = new HalfEdgeTriangulation(6);
        halfEdges.AddTriangle(0, 1, 2);
        halfEdges.AddTriangle(3, 4, 2);
        halfEdges.AddTriangle(2, 1, 3);
        halfEdges.AddTriangle(6, 2, 5);
        halfEdges.AddTriangle(2, 6, 0);
        halfEdges.AddTriangle(5, 2, 4);
        Assume.That(halfEdges.ElementCount, Is.EqualTo(6));
        Assume.That(halfEdges.EdgeCount, Is.EqualTo(18));
        var edges = halfEdges.Edges.ToArray();
        Assert.That(edges.Length, Is.EqualTo(18));

        TriangleElement[] expectedTriangles =
        [
            new(0, 1, 2),
            new(3, 4, 2),
            new(2, 1, 3),
            new(6, 2, 5),
            new(2, 6, 0),
            new(5, 2, 4)
        ];
        Assert.That(halfEdges.GetTriangleElements(), Is.EqualTo(expectedTriangles));

        LineElement[] expectedBoundaries =
        [
            new(0, 1), new(1, 3), new(3, 4), new(4, 5), new(5, 6), new(6, 0)
        ];
        Assert.That(edges.Where(IsBoundary).Select(ToLine), Is.EquivalentTo(expectedBoundaries));

        LineElement[] neighbourPairs =
        [
            new(0,2), new(2,0),
            new(1,2), new(2,1),
            new(3,2), new(2,3),
            new(4,2), new(2,4),
            new(5,2), new(2,5),
            new(6,2), new(2,6),
        ];
        Assert.That(edges.Where(IsNoBoundary).Select(ToLine), Is.EquivalentTo(neighbourPairs));
        for (var i = 0; i < edges.Length; i++)
        {
            if (IsBoundary(edges[i]))
                continue;
            // Check that twin edges point to eachother
            Assert.That(edges[edges[i].TwinEdge].TwinEdge, Is.EqualTo(i));
        }
    }
    
    [TestCaseSource(nameof(TwoElementTriangulation))]
    public void HalfEdgeElement(HalfEdgeTriangulation triangulation, HalfEdge[] expectedEdges1, HalfEdge[] expectedEdges2)
    {
        var edges = triangulation.Edges.ToArray();
        Assume.That(edges, Has.Length.EqualTo(6));
        Assume.That(triangulation.ElementCount, Is.EqualTo(2));
        Assume.That(edges, Is.EquivalentTo(expectedEdges1.Concat(expectedEdges2)));
        
        Assert.That(() => triangulation.GetElement(-1), Throws.Exception.TypeOf<IndexOutOfRangeException>());
        Assert.That(() => triangulation.GetElement(2), Throws.Exception.TypeOf<IndexOutOfRangeException>());
        Assert.That(() => new HalfEdgeElement(edges, 6), Throws.Exception.TypeOf<IndexOutOfRangeException>());
        Assert.That(() => new HalfEdgeElement(edges, -1), Throws.Exception.TypeOf<IndexOutOfRangeException>());
        
        Assert.That( new HalfEdgeElement(edges, 0).ToArray(), Is.EquivalentTo(expectedEdges1));
        Assert.That( new HalfEdgeElement(edges, 1).ToArray(), Is.EquivalentTo(expectedEdges1));
        Assert.That( new HalfEdgeElement(edges, 2).ToArray(), Is.EquivalentTo(expectedEdges1));
        Assert.That( new HalfEdgeElement(edges, 3).ToArray(), Is.EquivalentTo(expectedEdges2));
        Assert.That( new HalfEdgeElement(edges, 4).ToArray(), Is.EquivalentTo(expectedEdges2));
        Assert.That( new HalfEdgeElement(edges, 5).ToArray(), Is.EquivalentTo(expectedEdges2));

        var enumerator = new HalfEdgeElement(edges, 0);
        var edgesFromEnumerable = AsWeakEnumerable(enumerator);
        Assert.That(edgesFromEnumerable, Is.EquivalentTo(expectedEdges1));
    }

    [TestCaseSource(nameof(TwoElementTriangulation))]
    public void HalfEdgeEnumerator(HalfEdgeTriangulation triangulation, HalfEdge[] expectedEdges1, HalfEdge[] expectedEdges2)
    {
        var edges = triangulation.Edges.ToArray();
        Assume.That(edges, Has.Length.EqualTo(6));
        Assume.That(triangulation.ElementCount, Is.EqualTo(2));
        Assume.That(edges, Is.EquivalentTo(expectedEdges1.Concat(expectedEdges2)));
        var haflEdgeElement = new HalfEdgeElement(edges, 0);
        using var enumerator = haflEdgeElement.GetEnumerator();
        List<HalfEdge> edgesFromEnum = new();
        while (enumerator.MoveNext())
        {
            edgesFromEnum.Add(enumerator.Current);
        }
        Assert.That(edgesFromEnum, Is.EquivalentTo(expectedEdges1));
        enumerator.Reset();
        edgesFromEnum.Clear();
        
        while (enumerator.MoveNext())
        {
            edgesFromEnum.Add(enumerator.Current);
        }
        Assert.That(edgesFromEnum, Is.EquivalentTo(expectedEdges1));
        



    }

    private static IEnumerable<TestCaseData> TwoElementTriangulation()
    {
        var halfEdges = new HalfEdgeTriangulation(2);
        halfEdges.AddTriangle(0, 1, 2);
        halfEdges.AddTriangle(1, 3, 2);
        var expectedEdges1 = new[]
        {
            new HalfEdge(0, 1, 2, 1, -1, 0),
            new HalfEdge(1, 2, 0, 2, 5, 0),
            new HalfEdge(2, 0, 1, 0, -1, 0),
        };
        var expectedEdges2 = new[]
        {
            new HalfEdge(1, 3, 5, 4, -1, 1),
            new HalfEdge(3, 2, 3, 5, -1, 1),
            new HalfEdge(2, 1, 4, 3, 1, 1),
        };
        return [new TestCaseData(halfEdges, expectedEdges1, expectedEdges2).SetArgDisplayNames("Two Triangles")];
    }
    
    private static LineElement ToLine(HalfEdge halfEdge) => new(halfEdge.V1, halfEdge.V2);
    private static bool IsBoundary(HalfEdge halfEdge) => halfEdge.TwinEdge == -1;
    private static bool IsNoBoundary(HalfEdge halfEdge) => !IsBoundary(halfEdge);

    private static List<object?> AsWeakEnumerable(IEnumerable source) => source.Cast<object?>().ToList();
}