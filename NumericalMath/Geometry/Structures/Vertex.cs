using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NumericalMath.Geometry.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[DebuggerDisplay("({X}, {Y})")]
public struct Vertex2(float x, float y)
{
    public float X = x;
    public float Y = y;
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
[DebuggerDisplay("({X}, {Y}, {Z})")]
public struct Vertex3(float x, float y, float z)
{
    public float X = x;
    public float Y = y;
    public float Z = z;
}
