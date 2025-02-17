using System;

namespace NumericalMath.Geometry.Structures;

public struct Triangle(Vertex2 v1, Vertex2 v2, Vertex2 v3)
{
    public Vertex2 V1 = v1;
    public Vertex2 V2 = v2;
    public Vertex2 V3 = v3;

    public static bool Contains(Vertex2 p, Vertex2 v1, Vertex2 v2, Vertex2 v3)
    {
        var triangle = new Triangle(v1, v2, v3);
        return triangle.Contains(p);
    }

    public readonly bool Contains(Vertex2 vertex)
    {
        var d1 = HalfPlaneSide(vertex, V1, V2);
        var d2 = HalfPlaneSide(vertex, V2, V3);
        var d3 = HalfPlaneSide(vertex, V3, V1);

        var has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        var has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(has_neg && has_pos);
    }

    private static float HalfPlaneSide(Vertex2 p1, Vertex2 p2, Vertex2 p3)
    {
        return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
    }

    public static Triangle ContainingTriangle(ReadOnlySpan<Vertex2> vertices, float dilate = 0f)
    {
        var boundingBox = Rectangle.BoundingBox(vertices, dilate);
        var v1 = new Vertex2(boundingBox.Left, boundingBox.Bottom);
        var v2 = new Vertex2(boundingBox.Width + boundingBox.Height, boundingBox.Bottom);
        var v3 = new Vertex2(boundingBox.Left, boundingBox.Width + boundingBox.Height);
        return new Triangle(v1, v2, v3);
    }
}
