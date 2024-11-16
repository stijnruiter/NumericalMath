using BenchmarkDotNet.Attributes;
using LinearAlgebra;
using LinearAlgebra.Structures;
using System.Numerics;

namespace Benchmarks;

/**
 * Results 11/11/2024
 * BenchmarkDotNet v0.14.0, Windows 11
 * AMD Ryzen 5 5500H with Radeon Graphics, 1 CPU, 8 logical and 4 physical cores
 * .NET SDK 8.0.303
 *  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
 *  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
 * 
| Method             | Size | Mean                | Error             | StdDev            | Median              | Ratio    | RatioSD |
|------------------- |----- |--------------------:|------------------:|------------------:|--------------------:|---------:|--------:|
| Addition           | 10   |            33.82 ns |          0.319 ns |          0.298 ns |            33.77 ns |     1.00 |    0.01 |
| MathNetAddition    | 10   |            77.03 ns |          1.207 ns |          1.070 ns |            76.44 ns |     2.28 |    0.04 |
| SolveMatrix        | 10   |         4,980.54 ns |         22.746 ns |         20.164 ns |         4,981.98 ns |   147.26 |    1.38 |
| MathNetSolveMatrix | 10   |         1,464.73 ns |         11.514 ns |         10.206 ns |         1,461.40 ns |    43.31 |    0.47 |
| SolveVector        | 10   |         1,771.61 ns |         17.392 ns |         14.523 ns |         1,766.45 ns |    52.38 |    0.61 |
| MathNetSolveVector | 10   |           711.93 ns |          3.732 ns |          3.491 ns |           711.60 ns |    21.05 |    0.21 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 100  |         2,038.89 ns |         22.479 ns |         19.927 ns |         2,040.97 ns |     1.00 |    0.01 |
| MathNetAddition    | 100  |         4,158.44 ns |         35.177 ns |         32.904 ns |         4,163.16 ns |     2.04 |    0.02 |
| SolveMatrix        | 100  |     1,072,977.34 ns |     16,649.164 ns |     12,998.572 ns |     1,070,017.58 ns |   526.30 |    7.90 |
| MathNetSolveMatrix | 100  |     1,004,488.27 ns |      6,377.225 ns |      5,965.260 ns |     1,006,257.03 ns |   492.71 |    5.47 |
| SolveVector        | 100  |       148,380.21 ns |        676.545 ns |        564.946 ns |       148,445.81 ns |    72.78 |    0.74 |
| MathNetSolveVector | 100  |       234,148.15 ns |      2,714.746 ns |      2,406.552 ns |       233,826.54 ns |   114.85 |    1.58 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 500  |       227,133.03 ns |     16,781.883 ns |     48,419.573 ns |       217,643.93 ns |     1.05 |    0.32 |
| MathNetAddition    | 500  |       189,059.09 ns |      3,732.838 ns |      6,133.160 ns |       188,484.33 ns |     0.87 |    0.19 |
| SolveMatrix        | 500  |   114,389,228.57 ns |  2,017,839.340 ns |  1,788,762.017 ns |   114,755,120.00 ns |   526.80 |  113.51 |
| MathNetSolveMatrix | 500  |   159,672,882.09 ns |  2,034,345.158 ns |  4,834,836.863 ns |   158,090,366.67 ns |   735.35 |  159.60 |
| SolveVector        | 500  |     7,209,045.31 ns |     20,649.281 ns |     18,305.049 ns |     7,208,526.95 ns |    33.20 |    7.14 |
| MathNetSolveVector | 500  |    30,436,600.72 ns |    296,092.197 ns |    247,250.395 ns |    30,357,990.62 ns |   140.17 |   30.15 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 1000 |       914,673.94 ns |     17,841.675 ns |     27,777.341 ns |       907,698.83 ns |     1.00 |    0.04 |
| MathNetAddition    | 1000 |       892,355.51 ns |     20,792.824 ns |     60,981.757 ns |       923,125.59 ns |     0.98 |    0.07 |
| SolveMatrix        | 1000 |   841,347,580.00 ns |  6,370,411.044 ns |  5,958,886.446 ns |   839,792,100.00 ns |   920.64 |   27.89 |
| MathNetSolveMatrix | 1000 | 1,463,665,333.33 ns | 16,664,660.218 ns | 15,588,133.515 ns | 1,462,550,000.00 ns | 1,601.61 |   50.06 |
| SolveVector        | 1000 |    48,670,396.10 ns |    739,081.836 ns |    655,176.797 ns |    48,490,086.36 ns |    53.26 |    1.72 |
| MathNetSolveVector | 1000 |   248,910,355.56 ns |  1,646,410.828 ns |  1,540,053.710 ns |   248,734,433.33 ns |   272.37 |    8.20 |
*/

public class MatrixOptimization<T> where T : struct, INumber<T>
{
    [Params(10, 100, 500, 1000)]
    public int Size { get; set; }

    private Matrix<T> left;
    private Matrix<T> right;
    private ColumnVector<T> rightVec;

    private MathNet.Numerics.LinearAlgebra.Matrix<float> mLeft;
    private MathNet.Numerics.LinearAlgebra.Matrix<float> mRight;
    private MathNet.Numerics.LinearAlgebra.Vector<float> mRightVec;


    [GlobalSetup]
    public void Setup()
    {
        left = Matrix<T>.Random(Size, Size);
        right = Matrix<T>.Random(Size, Size);
        rightVec = ColumnVector<T>.Random(Size);

        mLeft = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Random(Size, Size, 12345);
        mRight = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Random(Size, Size, 45678);
        mRightVec = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Random(Size, 234567);
    }

    [Benchmark(Baseline = true)]
    public Matrix<T> Addition() => left + right;

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Matrix<float> MathNetAddition() => mLeft + mRight;

    //[Benchmark]
    //public Matrix<T> Product() => left * right;

    [Benchmark]
    public Matrix<T> SolveMatrix() => PluFactorizationOperations.SolveUsingPLU(left, right, T.Zero);

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Matrix<float> MathNetSolveMatrix() => mLeft.Solve(mRight);

    [Benchmark]
    public ColumnVector<T> SolveVector() => PluFactorizationOperations.SolveUsingPLU(left, rightVec, T.Zero);

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Vector<float> MathNetSolveVector() => mLeft.Solve(mRightVec);
}
