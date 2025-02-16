using System.Collections.Generic;

namespace NumericalMath.Geometry.Structures;

public struct Rect(float left, float right, float bottom, float top)
{
    public float Left = left;
    public float Right = right;
    public float Top = top;
    public float Bottom = bottom;

    public readonly float Width => Right - Left;
    public readonly float Height => Top - Bottom;

    /// <summary>
    /// Return the four corners of the rectangle in counter-clockwise order
    /// </summary>
    public IEnumerable<Vertex2> ToVertices()
    {
        yield return new Vertex2(Left, Bottom);
        yield return new Vertex2(Right, Bottom);
        yield return new Vertex2(Right, Top);
        yield return new Vertex2(Left, Top);
    }
}
