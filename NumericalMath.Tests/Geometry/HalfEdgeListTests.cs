using System.Linq;
using NumericalMath.Geometry.Structures;

namespace NumericalMath.Tests.Geometry;

[TestFixture]
public class HalfEdgeTriangulationTests
{
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
    
    private static LineElement ToLine(HalfEdge halfEdge) => new LineElement(halfEdge.V1, halfEdge.V2);
    private static bool IsBoundary(HalfEdge halfEdge) => halfEdge.TwinEdge == -1;
    private static bool IsNoBoundary(HalfEdge halfEdge) => !IsBoundary(halfEdge);
}