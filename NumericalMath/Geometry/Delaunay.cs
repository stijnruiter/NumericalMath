using System;
using System.Collections.Generic;
using System.Diagnostics;
using NumericalMath.Geometry.Structures;
using NumericalMath.LinearAlgebra.Structures;

namespace NumericalMath.Geometry;

public class Delaunay : IDelaunay
{
    private readonly Vertex2[] _vertices;
    private readonly HalfEdgeTriangulation _triangulation;

    private Delaunay(ReadOnlySpan<Vertex2> vertices)
    {
        var containing = Triangle.ContainingTriangle(vertices, 1e5f);
        _vertices = new Vertex2[vertices.Length + 3];
        _vertices[0] = containing.V1;
        _vertices[1] = containing.V2;
        _vertices[2] = containing.V3;
        vertices.CopyTo(_vertices.AsSpan()[3..]);

        _triangulation = new HalfEdgeTriangulation(2 * _vertices.Length + 1);
        _triangulation.AddTriangle(0, 1, 2);
        
        for (var i = 3; i < _vertices.Length; i++)
        {
            InsertPoint(i);
        }
    }

    public static (List<TriangleElement> Interior, List<LineElement> Boundary) CreateTriangulation(ReadOnlySpan<Vertex2> vertices)
    {
        var delaunay = new Delaunay(vertices);
        var mesh = delaunay._triangulation.ToMesh();
        Cleanup(mesh.Interior, mesh.Boundary);
        return mesh;
    }

    private void InsertPoint(int indexP)
    {
        var elementIndex = FindElement(indexP);
        var edges = _triangulation.RefineTriangle(elementIndex, indexP);
        Debug.Assert(edges.Length == 3);
        FlipTest(edges[0]);
        FlipTest(edges[1]);
        FlipTest(edges[2]);
    }

    private int FindElement(int indexP)
    {
        var point = _vertices[indexP];
        for (var i = 0; i < _triangulation.ElementCount; i++)
        {
            var indices = _triangulation.GetTriangleVertices(i);
            if (Triangle.Contains(point, _vertices[indices.I], _vertices[indices.J], _vertices[indices.K]))
                return i;
        }
        
        throw new Exception($"Point {point} at index {indexP} not found");
    }

    private void FlipTest(int edgeIndex)
    {
        var edge = _triangulation.GetEdge(edgeIndex);
        if (edge.TwinEdge < 0)
            return;

        var twinEdge = _triangulation.GetEdge(edge.TwinEdge);

        var triangleThirdVertex = _triangulation.GetEdge(edge.NextEdge).V2;
        var twinTriangleThirdVertex = _triangulation.GetEdge(twinEdge.NextEdge).V2;

        if (!InCircle(edge.V2, triangleThirdVertex, edge.V1, twinTriangleThirdVertex))
            return;

        _triangulation.FlipEdge(edgeIndex);
        FlipTest(twinEdge.NextEdge);
        FlipTest(twinEdge.PrevEdge);
    }

    private bool InCircle(int ai, int bi, int ci, int di)
    {
        return InCircleDeterminant(_vertices[ai], _vertices[bi], _vertices[ci], _vertices[di]) > 0;
    }

    public static float InCircleDeterminant(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        var aLengthSquared = a.X * a.X + a.Y * a.Y;
        var bLengthSquared = b.X * b.X + b.Y * b.Y;
        var cLengthSquared = c.X * c.X + c.Y * c.Y;
        var dLengthSquared = d.X * d.X + d.Y * d.Y;

        return a.X * Matrix<float>.Determinant(b.Y, bLengthSquared, 1, c.Y, cLengthSquared, 1, d.Y, dLengthSquared, 1)
            - a.Y * Matrix<float>.Determinant(b.X, bLengthSquared, 1, c.X, cLengthSquared, 1, d.X, dLengthSquared, 1)
            + aLengthSquared * Matrix<float>.Determinant(b.X, b.Y, 1, c.X, c.Y, 1, d.X, d.Y, 1)
            - Matrix<float>.Determinant(b.X, b.Y, bLengthSquared, c.X, c.Y, cLengthSquared, d.X, d.Y, dLengthSquared);
    }
    
    private static void Cleanup(List<TriangleElement> interior, List<LineElement> boundary)
    {
        boundary.Clear();
        for (var i = interior.Count - 1; i >= 0; i--)
        {
            var element = interior[i];
            if (element.I < 3)
            {
                if (element is { J: >= 3, K: >= 3 })
                {
                    boundary.Add(new LineElement(element.J - 3, element.K - 3));
                }
                interior.RemoveAt(i);
            }
            else if (element.J < 3)
            {
                if (element.K >= 3)
                {
                    boundary.Add(new LineElement(element.K - 3, element.I - 3));
                }
                interior.RemoveAt(i);
            }
            else if (element.K < 3)
            {
                boundary.Add(new LineElement(element.I - 3, element.J - 3));
                interior.RemoveAt(i);
            }
            else
            {
                interior[i] = new TriangleElement(element.I - 3, element.J - 3, element.K - 3);
            }
        }
    }
}