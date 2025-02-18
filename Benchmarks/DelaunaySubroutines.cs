using BenchmarkDotNet.Attributes;
using NumericalMath.Geometry;
using NumericalMath.Geometry.Structures;
using System;
using System.Collections.Generic;

namespace Benchmarks;

/**
  *BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4890/23H2/2023Update/SunValley3)
  *AMD Ryzen 5 5500H with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
  *.NET SDK 9.0.102
  *  [Host]     : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  *  DefaultJob : .NET 8.0.12 (8.0.1224.60305), X64 RyuJIT AVX2
  *
  *
  *| Method            | Size | Mean      | Error    | StdDev   | Ratio | RatioSD | Gen0     | Allocated  | Alloc Ratio |
  *|------------------ |----- |----------:|---------:|---------:|------:|--------:|---------:|-----------:|------------:|
  *| DelaunayDefault   | 10   |  42.78 us | 0.782 us | 0.731 us |  1.00 |    0.02 |  25.2075 |  103.07 KB |        1.00 |
  *| DelaunayOptimized | 10   |  12.91 us | 0.248 us | 0.296 us |  0.30 |    0.01 |   9.3231 |   38.08 KB |        0.37 |
  *|                   |      |           |          |          |       |         |          |            |             |
  *| DelaunayDefault   | 25   | 139.54 us | 1.838 us | 1.719 us |  1.00 |    0.02 |  90.3320 |  369.82 KB |        1.00 |
  *| DelaunayOptimized | 25   |  56.87 us | 0.905 us | 0.889 us |  0.41 |    0.01 |  43.7012 |  178.66 KB |        0.48 |
  *|                   |      |           |          |          |       |         |          |            |             |
  *| DelaunayDefault   | 50   | 392.30 us | 7.442 us | 7.309 us |  1.00 |    0.03 | 268.5547 | 1098.42 KB |        1.00 |
  *| DelaunayOptimized | 50   | 205.22 us | 2.219 us | 1.853 us |  0.52 |    0.01 | 167.2363 |  683.38 KB |        0.62 |
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

    private readonly DelaunayBase _delaunayDefault = new Delaunay();
    private readonly DelaunayBase _delaunayOpt = new DelaunayOpt();

    [Benchmark(Baseline = true)]
    public (List<TriangleElement> Interior, List<LineElement> Boundary) DelaunayDefault()
    {
        return _delaunayDefault.CreateTriangulation(_vertices);
    }

    [Benchmark]
    public (List<TriangleElement> Interior, List<LineElement> Boundary) DelaunayOptimized()
    {
        return _delaunayOpt.CreateTriangulation(_vertices);
    }
}
