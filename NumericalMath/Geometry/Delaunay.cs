using NumericalMath.Geometry.Structures;
using NumericalMath.LinearAlgebra.Structures;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Linq;

namespace NumericalMath.Geometry;

public class Delaunay
{
    public static (Vertex2[] Vertices, TriangleElement[] Interior, LineElement[] Boundary) CreateTriangulation(ReadOnlySpan<Vertex2> vertices)
    {
        if (vertices.Length < 3)
            throw new ArgumentException("Unable to create triangulation. At least 3 vertices are required.");

        // Create a convex hull (rectangle) of the vertices, with a 0.1 margin
        var bounds = Rect.BoundingBox(vertices, 0.1f);
        var convexHullVertices = new List<Vertex2>()
        {
            new(bounds.Left, bounds.Bottom),
            new(bounds.Right, bounds.Bottom),
            new(bounds.Right, bounds.Top),
            new(bounds.Left, bounds.Top),
        };
        var interiorElements = new List<TriangleElement> {
            new(0, 1, 2), new(0, 2, 3)
        };

        var boundaryElements = new List<LineElement> {
            new(0, 1),new(1, 2),
            new(2, 3),new(0, 3)
        };

        // Insert points using Delaunay
        for (int i = 0; i < vertices.Length; i++)
        {
            InsertPoint(vertices[i], convexHullVertices, interiorElements, boundaryElements);
        }

        // Remove the 4 convex hull points
        boundaryElements.Clear();
        for (var i = interiorElements.Count - 1; i >= 0; i--)
        {
            var element = interiorElements[i];
            if (element.I < 4)
            {
                if (element.J >= 4 && element.K >= 4)
                {
                    boundaryElements.Add(new LineElement(element.J - 4, element.K - 4));
                }
                interiorElements.RemoveAt(i);
            }
            else if (element.J < 4)
            {
                if (element.K >= 4)
                {
                    boundaryElements.Add(new LineElement(element.K - 4, element.I - 4));
                }
                interiorElements.RemoveAt(i);
            }
            else if (element.K < 4)
            {
                boundaryElements.Add(new LineElement(element.I - 4, element.J - 4));
                interiorElements.RemoveAt(i);
            }
            else
            {
                interiorElements[i] = new TriangleElement(element.I - 4, element.J - 4, element.K - 4);
            }
        }

        return (convexHullVertices.Skip(4).ToArray(), interiorElements.ToArray(), boundaryElements.ToArray());
    }

    private static void InsertPoint(Vertex2 p, List<Vertex2> vertices, List<TriangleElement> elements, List<LineElement> boundaryElements)
    {
        var element = FindElement(p, vertices, elements);
        var indexP = vertices.Count;

        // Replace ABC by ABP, ACP, BCP
        vertices.Add(p);
        elements.Remove(element);
        elements.AddRange(ReplaceElement(indexP, element));

        FlipTest(indexP, element.I, element.J, vertices, elements, boundaryElements);
        FlipTest(indexP, element.J, element.K, vertices, elements, boundaryElements);
        FlipTest(indexP, element.K, element.I, vertices, elements, boundaryElements);
    }

    private static void FlipTest(int indexP, int i, int j, List<Vertex2> vertices, List<TriangleElement> elements, List<LineElement> boundaryElements)
    {
        if (boundaryElements.Contains(new LineElement(i, j)) || boundaryElements.Contains(new LineElement(j, i)))
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
        FlipTest(indexP, i, otherP, vertices, elements, boundaryElements);
        FlipTest(indexP, otherP, j, vertices, elements, boundaryElements);
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
            var v1 = vertices[element.I];
            var v2 = vertices[element.J];
            var v3 = vertices[element.K];
            if (!PointInTriangle(point, v1, v2, v3))
                continue;

            return element;
        }
        throw new Exception($"Point {point} not in elements.");
    }

    private static bool InCircle(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        return InCircleDet(a, b, c, d) > 0;
    }

    private static float InCircleDet(Vertex2 a, Vertex2 b, Vertex2 c, Vertex2 d)
    {
        return new Matrix<float>(new float[,]{
            { a.X, a.Y, a.X * a.X + a.Y * a.Y, 1 },
            { b.X, b.Y, b.X * b.X + b.Y * b.Y, 1},
            { c.X, c.Y, c.X * c.X + c.Y * c.Y, 1},
            { d.X, d.Y, d.X * d.X + d.Y * d.Y, 1} }).Determinant();
    }

    private static bool PointInTriangle(Vertex2 p, Vertex2 v1, Vertex2 v2, Vertex2 v3)
    {
        var d1 = HalfPlaneSide(p, v1, v2);
        var d2 = HalfPlaneSide(p, v2, v3);
        var d3 = HalfPlaneSide(p, v3, v1);

        var has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        var has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);

        float HalfPlaneSide(Vertex2 p1, Vertex2 p2, Vertex2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}