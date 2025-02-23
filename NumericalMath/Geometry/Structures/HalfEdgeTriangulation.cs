using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NumericalMath.Geometry.Structures;

public class HalfEdgeTriangulation(int triangleCount)
{
    private readonly int[] _elements = new int[triangleCount];
    private readonly HalfEdge[] _edges = new HalfEdge[triangleCount * 3];

    public int ElementCount { get; private set; } = 0;
    public int EdgeCount { get; private set; } = 0;

    public int ElementCapacity => _elements.Length;
    public int EdgeCapacity => _edges.Length;
    
    public ReadOnlySpan<HalfEdge> Edges => _edges.AsSpan()[..EdgeCount];

    public void AddTriangle(int vertexIndex1, int vertexIndex2, int vertexIndex3)
    {
        AssertNewTrianglesFits(1);
        
        var edgeIndex1 = EdgeCount;
        var edgeIndex2 = edgeIndex1 + 1;
        var edgeIndex3 = edgeIndex1 + 2;

        _elements[ElementCount] = edgeIndex1;
        InsertEdge(vertexIndex1, vertexIndex2, edgeIndex1, edgeIndex3, edgeIndex2, ElementCount);
        InsertEdge(vertexIndex2, vertexIndex3, edgeIndex2, edgeIndex1, edgeIndex3, ElementCount);
        InsertEdge(vertexIndex3, vertexIndex1, edgeIndex3, edgeIndex2, edgeIndex1, ElementCount);

        ElementCount += 1;
        EdgeCount += 3;
    }

    public int[] RefineTriangle(int triangleIndex, int newVertexIndex)
    {
        AssertNewTrianglesFits(2);

        var oldEdgeIndex1 = _elements[triangleIndex];
        var oldEdge1 = _edges[oldEdgeIndex1];
        
        var oldEdgeIndex2 = oldEdge1.NextEdge;
        var oldEdge2 = _edges[oldEdgeIndex2];
        
        var oldEdgeIndex3 = oldEdge2.NextEdge;
        var oldEdge3 = _edges[oldEdgeIndex3];
        
        Debug.Assert(oldEdge1.PrevEdge == oldEdgeIndex3);
        Debug.Assert(oldEdge3.NextEdge == oldEdgeIndex1);

        var newEdgeIndex1 = EdgeCount;
        var newEdgeIndex2 = EdgeCount + 1;
        var newEdgeIndex3 = EdgeCount + 2;
        var newEdgeIndex4 = EdgeCount + 3;
        var newEdgeIndex5 = EdgeCount + 4;
        var newEdgeIndex6 = EdgeCount + 5;

        // Replace "old" triangle with Triangle 1
        _edges[oldEdgeIndex1] = new HalfEdge(oldEdge1.V1, oldEdge1.V2, newEdgeIndex2, newEdgeIndex1, oldEdge1.TwinEdge, oldEdge1.ElementIndex);
        _edges[newEdgeIndex1] = new HalfEdge(oldEdge1.V2, newVertexIndex, oldEdgeIndex1, newEdgeIndex2, newEdgeIndex4, oldEdge1.ElementIndex);
        _edges[newEdgeIndex2] = new HalfEdge(newVertexIndex, oldEdge1.V1, newEdgeIndex1, oldEdgeIndex1, newEdgeIndex5, oldEdge1.ElementIndex);
        _elements[oldEdge1.ElementIndex] = oldEdgeIndex1;
        
        // Triangle 2
        _edges[oldEdgeIndex2] = new HalfEdge(oldEdge2.V1, oldEdge2.V2, newEdgeIndex4, newEdgeIndex3, oldEdge2.TwinEdge, ElementCount);
        _edges[newEdgeIndex3] = new HalfEdge(oldEdge2.V2, newVertexIndex, oldEdgeIndex2, newEdgeIndex4, newEdgeIndex6, ElementCount);
        _edges[newEdgeIndex4] = new HalfEdge(newVertexIndex, oldEdge2.V1, newEdgeIndex3, oldEdgeIndex2, newEdgeIndex1, ElementCount);
        _elements[ElementCount] = oldEdgeIndex2;
        
        // Triangle 3
        _edges[oldEdgeIndex3] = new HalfEdge(oldEdge3.V1, oldEdge3.V2, newEdgeIndex6, newEdgeIndex5, oldEdge3.TwinEdge, ElementCount + 1);
        _edges[newEdgeIndex5] = new HalfEdge(oldEdge3.V2, newVertexIndex, oldEdgeIndex3, newEdgeIndex6, newEdgeIndex2, ElementCount + 1);
        _edges[newEdgeIndex6] = new HalfEdge(newVertexIndex, oldEdge3.V1, newEdgeIndex5, oldEdgeIndex3, newEdgeIndex3, ElementCount + 1);
        _elements[ElementCount + 1] = oldEdgeIndex3;

        ElementCount += 2;
        EdgeCount += 6;

        return [oldEdgeIndex1, oldEdgeIndex2, oldEdgeIndex3];
    }

    public void FlipEdge(int edgeIndex)
    {
        var edge = _edges[edgeIndex];
        var twinIndex = edge.TwinEdge;
        var twin = _edges[twinIndex];
        
        var previousEdgeIndex = edge.PrevEdge;
        var previousTwinIndex = twin.PrevEdge;
        var nextEdgeIndex = edge.NextEdge;
        var nextTwinIndex = twin.NextEdge;

        var edgeVertex = _edges[nextEdgeIndex].V2;
        var twinVertex = _edges[nextTwinIndex].V2;

        _edges[previousEdgeIndex].PrevEdge = edgeIndex;
        _edges[previousEdgeIndex].NextEdge = nextTwinIndex;
        _edges[previousEdgeIndex].ElementIndex = edge.ElementIndex;
        _edges[nextTwinIndex].PrevEdge = previousEdgeIndex;
        _edges[nextTwinIndex].NextEdge = edgeIndex;
        _edges[nextTwinIndex].ElementIndex = edge.ElementIndex;
        _edges[edgeIndex] = new HalfEdge(twinVertex, edgeVertex, nextTwinIndex, previousEdgeIndex, twinIndex, edge.ElementIndex);
        _elements[edge.ElementIndex] = edgeIndex;

        _edges[previousTwinIndex].PrevEdge = twinIndex;
        _edges[previousTwinIndex].NextEdge = nextEdgeIndex;
        _edges[previousTwinIndex].ElementIndex = twin.ElementIndex;
        _edges[nextEdgeIndex].PrevEdge = previousTwinIndex;
        _edges[nextEdgeIndex].NextEdge = twinIndex;
        _edges[nextEdgeIndex].ElementIndex = twin.ElementIndex;
        _edges[twinIndex] = new HalfEdge(edgeVertex, twinVertex, nextEdgeIndex, previousTwinIndex, edgeIndex, twin.ElementIndex);
        _elements[twin.ElementIndex] = twinIndex;
    }
    
    public TriangleElement GetTriangleVertices(int elementIndex)
    {
        return GetTriangleVerticesFromEdge(_elements[elementIndex]);
    }

    public TriangleElement GetTriangleVerticesFromEdge(int edgeIndex)
    {
        var edgeIndex2 = _edges[edgeIndex].NextEdge;
        return new TriangleElement(_edges[edgeIndex].V1, _edges[edgeIndex].V2, _edges[edgeIndex2].V2);
    }
    
    public HalfEdge GetEdge(int edgeIndex) => _edges[edgeIndex];
    
    public HalfEdgeElement GetElement(int elementIndex) => new(_edges, _elements[elementIndex]);

    public IEnumerable<HalfEdgeElement> GetElements()
    {
        for (int i = 0; i < ElementCount; i++)
        {
            yield return GetElement(i);
        }
    }

    public IEnumerable<TriangleElement> GetTriangleElements()
    {
        for (int i = 0; i < ElementCount; i++)
        {
            yield return GetTriangleVertices(i);
        }
    }
    
    public (List<TriangleElement>, List<LineElement>) ToMesh()
    {
        var elements = new List<TriangleElement>();
        var boundary = new List<LineElement>();

        // foreach (var element in GetElements())
        for(var i = 0; i < ElementCount; i++)
        {
            var edges = GetElement(i).ToArray();
            Debug.Assert(edges.Length == 3);
            
            elements.Add(new TriangleElement(edges[0].V1, edges[1].V1, edges[2].V1));
            boundary.AddRange(from edge in edges where edge.TwinEdge == -1 select new LineElement(edge.V1, edge.V2));
        }
        return (elements, boundary);
    }

    private void InsertEdge(int vertexIndexStart, int vertexIndexEnd, int edgeIndex, int edgeIndexPrevious, int edgeIndexNext, int elementIndex)
    {
        Debug.Assert(GetEdgeIndex(vertexIndexStart, vertexIndexEnd) == -1); // Edge already exists
        Debug.Assert(_edges[edgeIndex] == default);
        
        var twin = GetEdgeIndex(vertexIndexEnd, vertexIndexStart);
        if (twin >= 0)
        {
            Debug.Assert(_edges[twin].TwinEdge == -1); // Twin already has a neighbour
            _edges[twin].TwinEdge = edgeIndex;
        }
        _edges[edgeIndex] = new HalfEdge(vertexIndexStart, vertexIndexEnd, edgeIndexPrevious, edgeIndexNext, twin, elementIndex);
    }
    
    private void AssertNewTrianglesFits(int newTriangles = 1)
    {
        if (ElementCount + newTriangles > ElementCapacity)
            throw new Exception($"Element capacity exceeded");
        if (EdgeCount + newTriangles * 3 > EdgeCapacity)
            throw new Exception($"Edge capacity exceeded");
    }

    private int GetEdgeIndex(int vStart, int vEnd)
    {
        for (int i = 0; i < EdgeCount; i++)
        {
            if (_edges[i].V1 == vStart && _edges[i].V2 == vEnd)
                return i;
        }

        return -1;
    }
}
