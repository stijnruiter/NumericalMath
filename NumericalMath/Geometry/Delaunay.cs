using NumericalMath.Geometry.Structures;
using NumericalMath.LinearAlgebra.Structures;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;

namespace NumericalMath.Geometry;

public class Delaunay : DelaunayBase
{
    public Delaunay() { }

    internal override float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        return new Matrix<float>(new float[,]{
            { a.X, a.Y, a.X * a.X + a.Y * a.Y, 1 },
            { b.X, b.Y, b.X * b.X + b.Y * b.Y, 1},
            { c.X, c.Y, c.X * c.X + c.Y * c.Y, 1},
            { d.X, d.Y, d.X * d.X + d.Y * d.Y, 1} }).Determinant();
    }
}

public class DelaunayOpt : DelaunayBase
{
    public DelaunayOpt() { }

    internal override float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        float aLengthSquared = a.X * a.X + a.Y * a.Y;
        float bLengthSquared = b.X * b.X + b.Y * b.Y;
        float cLengthSquared = c.X * c.X + c.Y * c.Y;
        float dLengthSquared = d.X * d.X + d.Y * d.Y;

        return a.X * Matrix<float>.Determinant(b.Y, bLengthSquared, 1, c.Y, cLengthSquared, 1, d.Y, dLengthSquared, 1)
            - a.Y * Matrix<float>.Determinant(b.X, bLengthSquared, 1, c.X, cLengthSquared, 1, d.X, dLengthSquared, 1)
            + aLengthSquared * Matrix<float>.Determinant(b.X, b.Y, 1, c.X, c.Y, 1, d.X, d.Y, 1)
            - Matrix<float>.Determinant(b.X, b.Y, bLengthSquared, c.X, c.Y, cLengthSquared, d.X, d.Y, dLengthSquared);
    }
}

public abstract class DelaunayBase
{

    public (Vertex2[] Vertices, TriangleElement[] Interior, LineElement[] Boundary) CreateTriangulation(ReadOnlySpan<Vertex2> vertices)
    {
        if (vertices.Length < 3)
            throw new ArgumentException("Unable to create triangulation. At least 3 vertices are required.");

        // Create an "infinite" containing triangle
        var containingTriangle = Triangle.ContainingTriangle(vertices, 1e5f);
        var convexHullVertices = new List<Vertex2>()
        {
            containingTriangle.V1,
            containingTriangle.V2,
            containingTriangle.V3,
        };
        var interiorElements = new List<TriangleElement> { new(0, 1, 2) };

        // Insert points using Delaunay
        for (int i = 0; i < vertices.Length; i++)
        {
            InsertPoint(vertices[i], convexHullVertices, interiorElements);
        }

        var boundaryElements = RemoveContainingTriangle(interiorElements);
        return (convexHullVertices.Skip(3).ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }

    private static List<LineElement> RemoveContainingTriangle(List<TriangleElement> interiorElements)
    {
        var boundaryElements = new List<LineElement>();
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
        return boundaryElements;
    }

    private void InsertPoint(Vertex2 p, List<Vertex2> vertices, List<TriangleElement> elements)
    {
        var element = FindElement(p, vertices, elements);
        var indexP = vertices.Count;

        // Replace ABC by ABP, ACP, BCP
        vertices.Add(p);
        elements.Remove(element);
        elements.AddRange(ReplaceElement(indexP, element));

        FlipTest(indexP, element.I, element.J, vertices, elements);
        FlipTest(indexP, element.J, element.K, vertices, elements);
        FlipTest(indexP, element.K, element.I, vertices, elements);
    }

    private static bool IsBoundaryEdge(int i, int j)
    {
        // It is an edge if both indices are 0, 1, 2
        return i < 3 && j < 3;
    }

    private void FlipTest(int indexP, int i, int j, List<Vertex2> vertices, List<TriangleElement> elements)
    {
        if (IsBoundaryEdge(i,j))
            return;

        // TODO: store neighbours
        var elementsWithEdge = FindElementsWithEdge(i, j, elements).ToArray();
        Debug.Assert(elementsWithEdge.Length == 2);
        (var element_pij, var other_ij) = Contains(elementsWithEdge[0], indexP) ?
            (elementsWithEdge[0], elementsWithEdge[1]) : (elementsWithEdge[1], elementsWithEdge[0]);

        Debug.Assert(!Contains(other_ij, indexP));
        Debug.Assert(Contains(element_pij, indexP));

        var otherP = MirroredVertex(other_ij, i, j);

        var vi = vertices[i];
        var vj = vertices[j];
        var vp = vertices[indexP];
        var vOther = vertices[otherP];
        if (!InCircle(vj, vp, vi, vOther))
            return;

        // Flip edges
        var newE1 = new TriangleElement(indexP, i, otherP);
        var newE2 = new TriangleElement(indexP, otherP, j);
        elements.Remove(element_pij);
        elements.Remove(other_ij);
        elements.Add(newE1);
        elements.Add(newE2);

        // Test new edges 
        FlipTest(indexP, i, otherP, vertices, elements);
        FlipTest(indexP, otherP, j, vertices, elements);
    }

    private static int MirroredVertex(TriangleElement element, int i, int j)
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

    private static IEnumerable<TriangleElement> FindElementsWithEdge(int i, int j, List<TriangleElement> elements)
    {
        foreach (var element in elements)
        {
            if (Contains(element, i) && Contains(element, j))
                yield return element;
        }
    }

    private static IEnumerable<TriangleElement> ReplaceElement(int indexP, TriangleElement element)
    {
        yield return new TriangleElement(indexP, element.I, element.J);
        yield return new TriangleElement(indexP, element.J, element.K);
        yield return new TriangleElement(indexP, element.K, element.I);
    }

    private static TriangleElement FindElement(Vertex2 point, List<Vertex2> vertices, List<TriangleElement> elements)
    {
        foreach (var element in elements)
        {
            if (Triangle.Contains(point, vertices[element.I], vertices[element.J], vertices[element.K]))
                return element;
        }
        throw new Exception($"Point {point} not in elements.");
    }

    private bool InCircle(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        return InCircleDet(a, b, c, d) > 0;
    }

    internal abstract float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d);
}