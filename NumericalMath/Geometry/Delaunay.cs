using NumericalMath.Geometry.Structures;
using NumericalMath.LinearAlgebra.Structures;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;

namespace NumericalMath.Geometry;

public class Delaunay : DelaunayBase
{
    private List<Vertex2> _vertices = [];
    private List<TriangleElement> _interiorElements = [];
    private List<LineElement> _boundaryElements = [];

    public Delaunay() { }

    public override (List<TriangleElement> Interior, List<LineElement> Boundary) CreateTriangulation(ReadOnlySpan<Vertex2> vertices)
    {
        Initialize(vertices);

        // Insert points using Delaunay
        for (int i = 3; i < _vertices.Count; i++)
        {
            InsertPoint(i);
        }

        Cleanup(_interiorElements, _boundaryElements);
        return (_interiorElements, _boundaryElements);
    }

    private void Initialize(ReadOnlySpan<Vertex2> vertices)
    {
        // Create an "infinite" containing triangle
        var containingTriangle = Triangle.ContainingTriangle(vertices.ToArray(), 1e5f);
        _vertices =
        [
            containingTriangle.V1,
            containingTriangle.V2,
            containingTriangle.V3,
            .. vertices,
        ];
        _interiorElements = [new(0, 1, 2)];
        _boundaryElements = [];
    }


    private void InsertPoint(int indexP)
    {
        var p = _vertices[indexP];
        (var element, var index) = FindElement(indexP);

        // Replace ABC by ABP, ACP, BCP
        _interiorElements.RemoveAt(index);
        _interiorElements.Add(new(indexP, element.I, element.J));
        _interiorElements.Add(new(indexP, element.J, element.K));
        _interiorElements.Add(new(indexP, element.K, element.I));

        FlipTest(indexP, element.I, element.J);
        FlipTest(indexP, element.J, element.K);
        FlipTest(indexP, element.K, element.I);
    }

    private void FlipTest(int indexP, int i, int j)
    {
        if (IsBoundaryEdge(i, j))
            return;

        // TODO: store neighbours
        var elementsWithEdge = FindElementsWithEdge(i, j).ToArray();
        Debug.Assert(elementsWithEdge.Length == 2);
        
        (var element_pij, var other_ij) = Contains(elementsWithEdge[0], indexP) ? (elementsWithEdge[0], elementsWithEdge[1]) : (elementsWithEdge[1], elementsWithEdge[0]);

        Debug.Assert(!Contains(other_ij, indexP));
        Debug.Assert(Contains(element_pij, indexP));

        var otherP = GetThirdIndex(other_ij, i, j);
        if (!InCircle(j, indexP, i, otherP))
            return;

        // Flip edges
        _interiorElements.Remove(element_pij);
        _interiorElements.Remove(other_ij);
        _interiorElements.Add(new TriangleElement(indexP, i, otherP));
        _interiorElements.Add(new TriangleElement(indexP, otherP, j));

        // Test new edges 
        FlipTest(indexP, i, otherP);
        FlipTest(indexP, otherP, j);
    }

    private IEnumerable<TriangleElement> FindElementsWithEdge(int i, int j)
    {
        foreach (var element in _interiorElements)
        {
            if (Contains(element, i) && Contains(element, j))
                yield return element;
        }
    }

    private (TriangleElement Element, int Index) FindElement(int indexP)
    {
        Vertex2 point = _vertices[indexP];
        for (var i = 0; i < _interiorElements.Count; i++)
        {
            var element = _interiorElements[i];
            if (Triangle.Contains(point, _vertices[element.I], _vertices[element.J], _vertices[element.K]))
                return (element, i);
        }
        throw new Exception($"Point {point} not in elements.");
    }

    private bool InCircle(int ai, int bi, int ci, int di)
    {
        return InCircleDet(_vertices[ai], _vertices[bi], _vertices[ci], _vertices[di]) > 0;
    }

    internal override float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        return new Matrix<float>(new float[,]{
            { a.X, a.Y, a.X * a.X + a.Y * a.Y, 1 },
            { b.X, b.Y, b.X * b.X + b.Y * b.Y, 1},
            { c.X, c.Y, c.X * c.X + c.Y * c.Y, 1},
            { d.X, d.Y, d.X * d.X + d.Y * d.Y, 1} }).Determinant();
    }

    private static int GetThirdIndex(TriangleElement element, int i, int j)
    {
        if (element.I != i && element.I != j)
            return element.I;
        if (element.J != i && element.J != j)
            return element.J;
        return element.K;
    }

    private static bool Contains(TriangleElement element, int i)
    {
        return element.I == i || element.J == i || element.K == i;
    }

    private static bool IsBoundaryEdge(int i, int j)
    {
        // It is an edge if both indices are 0, 1, 2
        return i < 3 && j < 3;
    }
}

public class DelaunayOpt : DelaunayBase
{
    private Vertex2[] _vertices;
    private HalfEdgeTriangulation _triangulation;

    public DelaunayOpt() { }

    public override (List<TriangleElement> Interior, List<LineElement> Boundary) CreateTriangulation(ReadOnlySpan<Vertex2> vertices)
    {
        
        var containing = Triangle.ContainingTriangle(vertices, 1e5f);
        _vertices = new Vertex2[vertices.Length + 3];
        _vertices[0] = containing.V1;
        _vertices[1] = containing.V2;
        _vertices[2] = containing.V3;
        vertices.CopyTo(_vertices.AsSpan()[3..]);

        _triangulation = new HalfEdgeTriangulation(2 * _vertices.Length + 1);
        _triangulation.AddTriangle(0, 1, 2);
        
        for (int i = 3; i < _vertices.Length; i++)
        {
            InsertPoint(i);
        }

        var mesh = _triangulation.ToMesh();
        Cleanup(mesh.Item1, mesh.Item2);
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
        return InCircleDet(_vertices[ai], _vertices[bi], _vertices[ci], _vertices[di]) > 0;
    }

    internal override float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
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
}

public abstract class DelaunayBase
{
    public abstract (List<TriangleElement> Interior, List<LineElement> Boundary) CreateTriangulation(ReadOnlySpan<Vertex2> vertices);
    
    internal abstract float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d);

    protected static void Cleanup(List<TriangleElement> interiorElements, List<LineElement> boundaryElements)
    {
        boundaryElements.Clear();
        for (var i = interiorElements.Count - 1; i >= 0; i--)
        {
            var element = interiorElements[i];
            if (element.I < 3)
            {
                if (element.J >= 3 && element.K >= 3)
                {
                    boundaryElements.Add(new LineElement(element.J - 3, element.K - 3));
                }
                interiorElements.RemoveAt(i);
            }
            else if (element.J < 3)
            {
                if (element.K >= 3)
                {
                    boundaryElements.Add(new LineElement(element.K - 3, element.I - 3));
                }
                interiorElements.RemoveAt(i);
            }
            else if (element.K < 3)
            {
                boundaryElements.Add(new LineElement(element.I - 3, element.J - 3));
                interiorElements.RemoveAt(i);
            }
            else
            {
                interiorElements[i] = new TriangleElement(element.I - 3, element.J - 3, element.K - 3);
            }
        }
    }
}