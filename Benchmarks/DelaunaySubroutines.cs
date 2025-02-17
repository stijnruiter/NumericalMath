using BenchmarkDotNet.Attributes;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;
using System;

namespace Benchmarks;

/**
  *BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
  *AMD Ryzen 5 5500H with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
  *.NET SDK 9.0.102
  *  [Host]     : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  *  DefaultJob : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  *
  *
  *| Method            | Size | Mean      | Error    | StdDev    | Ratio | RatioSD | Gen0     | Allocated  | Alloc Ratio |
  *|------------------ |----- |----------:|---------:|----------:|------:|--------:|---------:|-----------:|------------:|
  *| DelaunayDefault   | 10   |  50.78 us | 0.851 us |  1.324 us |  1.00 |    0.04 |  26.1841 |  107.06 KB |        1.00 |
  *| DelaunayOptimized | 10   |  16.26 us | 0.317 us |  0.444 us |  0.32 |    0.01 |  10.2844 |   42.07 KB |        0.39 |
  *|                   |      |           |          |           |       |         |          |            |             |
  *| DelaunayDefault   | 25   | 169.70 us | 3.353 us |  6.045 us |  1.00 |    0.05 |  95.4590 |  390.66 KB |        1.00 |
  *| DelaunayOptimized | 25   |  73.86 us | 1.469 us |  2.454 us |  0.44 |    0.02 |  48.8281 |  199.51 KB |        0.51 |
  *|                   |      |           |          |           |       |         |          |            |             |
  *| DelaunayDefault   | 50   | 493.36 us | 9.815 us | 23.705 us |  1.00 |    0.07 | 290.0391 | 1185.88 KB |        1.00 |
  *| DelaunayOptimized | 50   | 259.79 us | 5.135 us |  6.306 us |  0.53 |    0.03 | 188.4766 |  770.83 KB |        0.65 |
  *
  *
  *| Method                 | Mean        | Error     | StdDev    | Gen0   | Allocated |
  *|----------------------- |------------:|----------:|----------:|-------:|----------:|
  *| InCircleDeterminant    | 1,100.07 ns | 21.989 ns | 38.512 ns | 0.5474 |    2296 B |
  *| InCircleDeterminantOpt |    40.49 ns |  0.444 ns |  0.415 ns |      - |         - |
  **/

[MemoryDiagnoser]
public class DelaunaySubroutines
{
    private readonly DelaunayBase _delaunayDefault = new Delaunay();
    private readonly DelaunayBase _delaunayOpt = new DelaunayOpt();

    //[Benchmark]
    //public void InCircleDeterminant()
    //{
    //    var v1 = new Vertex2(0, 0);
    //    var v2 = new Vertex2(1, 0);
    //    var v3 = new Vertex2(0, 1);
    //    var v4 = new Vertex2(1, 2);
    //    var v5 = new Vertex2(0.5f, 0.5f);
    //    float v = _delaunayDefault.InCircleDet(v1, v2, v3, v4);
    //    v = _delaunayDefault.InCircleDet(v1, v2, v3, v5);
    //}

    //[Benchmark]
    //public void InCircleDeterminantOpt()
    //{
    //    var v1 = new Vertex2(0, 0);
    //    var v2 = new Vertex2(1, 0);
    //    var v3 = new Vertex2(0, 1);
    //    var v4 = new Vertex2(1, 2);
    //    var v5 = new Vertex2(0.5f, 0.5f);
    //    float v = _delaunayOpt.InCircleDet(v1, v2, v3, v4);
    //    v = _delaunayOpt.InCircleDet(v1, v2, v3, v5);
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
        return _delaunayDefault.CreateTriangulation(_vertices);
    }

    [Benchmark]
    public (Vertex2[] Vertices, TriangleElement[] Interior, LineElement[] Boundary) DelaunayOptimized()
    {
        return _delaunayOpt.CreateTriangulation(_vertices);
    }
}
