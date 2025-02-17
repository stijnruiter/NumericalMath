using BenchmarkDotNet.Attributes;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;
using System;

namespace Benchmarks;

[MemoryDiagnoser]
public class DelaunaySubroutines
{
    //[Benchmark]
    //public void InCircleDeterminant()
    //{
    //    var v1 = new Vertex2(0, 0);
    //    var v2 = new Vertex2(1, 0);
    //    var v3 = new Vertex2(0, 1);
    //    var v4 = new Vertex2(1, 2);
    //    var v5 = new Vertex2(0.5f, 0.5f);
    //    float v = new Delaunay().InCircleDet(v1, v2, v3, v4);
    //    v = new Delaunay().InCircleDet(v1, v2, v3, v5);
    //}

    //[Benchmark]
    //public void InCircleDeterminantOpt()
    //{
    //    var v1 = new Vertex2(0, 0);
    //    var v2 = new Vertex2(1, 0);
    //    var v3 = new Vertex2(0, 1);
    //    var v4 = new Vertex2(1, 2);
    //    var v5 = new Vertex2(0.5f, 0.5f);
    //    float v = new DelaunayOpt().InCircleDet(v1, v2, v3, v4);
    //    v = new DelaunayOpt().InCircleDet(v1, v2, v3, v5);
    //}

    [Params(10, 25, 50)]
    public int Size { get; set; }

    private Vertex2[] _vertices;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(123);
        _vertices = new Vertex2[Size];
        for (int i = 0; i < Size; i++)
        {
            _vertices[i] = new Vertex2(rnd.NextSingle(), rnd.NextSingle());
        }
    }

    [Benchmark(Baseline = true)]
    public (Vertex2[] Vertices, TriangleElement[] Interior, LineElement[] Boundary) DelaunayDefault()
    {
        return new Delaunay().CreateTriangulation(_vertices);
    }

    [Benchmark]
    public (Vertex2[] Vertices, TriangleElement[] Interior, LineElement[] Boundary) DelaunayOptimized()
    {
        return new DelaunayOpt().CreateTriangulation(_vertices);
    }
}
