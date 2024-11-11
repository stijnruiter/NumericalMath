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
| Addition           | 10   |            28.59 ns |          0.508 ns |          0.475 ns |            28.33 ns |     1.00 |    0.02 |
| MathNetAddition    | 10   |            78.35 ns |          0.758 ns |          0.672 ns |            78.26 ns |     2.74 |    0.05 |
| SolveMatrix        | 10   |         5,470.25 ns |         30.713 ns |         28.729 ns |         5,474.30 ns |   191.40 |    3.20 |
| MathNetSolveMatrix | 10   |         1,471.34 ns |          8.297 ns |          7.761 ns |         1,469.30 ns |    51.48 |    0.86 |
| SolveVector        | 10   |           895.72 ns |          4.413 ns |          4.128 ns |           895.49 ns |    31.34 |    0.52 |
| MathNetSolveVector | 10   |           726.77 ns |          3.851 ns |          3.602 ns |           725.71 ns |    25.43 |    0.42 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 100  |         2,017.02 ns |         39.868 ns |         59.672 ns |         2,011.01 ns |     1.00 |    0.04 |
| MathNetAddition    | 100  |         4,265.74 ns |         42.926 ns |         38.052 ns |         4,250.22 ns |     2.12 |    0.06 |
| SolveMatrix        | 100  |     3,083,615.49 ns |     36,188.989 ns |     28,253.982 ns |     3,075,712.70 ns | 1,530.07 |   45.65 |
| MathNetSolveMatrix | 100  |     1,012,586.68 ns |      4,739.311 ns |      4,201.276 ns |     1,012,822.07 ns |   502.44 |   14.46 |
| SolveVector        | 100  |        92,950.47 ns |        496.018 ns |        439.707 ns |        92,799.93 ns |    46.12 |    1.33 |
| MathNetSolveVector | 100  |       235,179.01 ns |      3,106.164 ns |      2,905.507 ns |       233,977.08 ns |   116.69 |    3.61 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 500  |       157,522.61 ns |      2,062.230 ns |      1,722.055 ns |       158,237.90 ns |     1.00 |    0.01 |
| MathNetAddition    | 500  |       193,658.63 ns |      3,843.693 ns |      5,984.167 ns |       191,241.88 ns |     1.23 |    0.04 |
| SolveMatrix        | 500  |   360,893,573.33 ns |  2,709,461.842 ns |  2,534,432.290 ns |   360,550,300.00 ns | 2,291.31 |   28.93 |
| MathNetSolveMatrix | 500  |   156,140,100.00 ns |    977,960.359 ns |  1,402,561.640 ns |   155,638,183.33 ns |   991.33 |   13.70 |
| SolveVector        | 500  |     5,727,793.60 ns |     16,251.086 ns |     13,570.393 ns |     5,731,665.23 ns |    36.37 |    0.40 |
| MathNetSolveVector | 500  |    30,376,998.32 ns |    132,748.284 ns |    110,850.830 ns |    30,362,275.00 ns |   192.86 |    2.16 |
|                    |      |                     |                   |                   |                     |          |         |
| Addition           | 1000 |       950,256.29 ns |     18,044.141 ns |     19,307.029 ns |       951,131.45 ns |     1.00 |    0.03 |
| MathNetAddition    | 1000 |       894,631.61 ns |     20,213.565 ns |     59,600.156 ns |       920,017.92 ns |     0.94 |    0.07 |
| SolveMatrix        | 1000 | 3,107,099,328.57 ns | 23,985,715.760 ns | 21,262,712.273 ns | 3,106,345,400.00 ns | 3,271.02 |   68.25 |
| MathNetSolveMatrix | 1000 | 1,470,581,453.33 ns | 16,888,658.049 ns | 15,797,661.224 ns | 1,467,532,600.00 ns | 1,548.17 |   34.61 |
| SolveVector        | 1000 |    43,002,117.19 ns |    800,437.463 ns |    786,136.617 ns |    42,757,275.00 ns |    45.27 |    1.20 |
| MathNetSolveVector | 1000 |   249,113,647.22 ns |  2,242,858.367 ns |  1,751,076.285 ns |   248,777,766.67 ns |   262.26 |    5.49 |
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
        left = new Matrix<T>(Size, Size, T.One + T.One + T.One);
        right = Matrix<T>.Diagonal(Size, T.One + T.One);
        rightVec = new ColumnVector<T>(Size, T.One);

        mLeft = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Random(Size, Size, 12345);
        mRight = MathNet.Numerics.LinearAlgebra.Matrix<float>.Build.Random(Size, Size, 45678);
        mRightVec = MathNet.Numerics.LinearAlgebra.Vector<float>.Build.Random(Size, 234567);
    }

    [Benchmark(Baseline = true)]
    public Matrix<T> Addition() => left + right;

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Matrix<float> MathNetAddition() => mLeft + mRight;

    //[Benchmark]
    //public Matrix<T> Subtraction() => left - right;

    //[Benchmark]
    //public Matrix<T> Product() => left * right;

    [Benchmark]
    public Matrix<T> SolveMatrix() => PluFactorizationOperations.SolveUsingPLU(left, right, -T.One);

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Matrix<float> MathNetSolveMatrix() => mLeft.Solve(mRight);

    [Benchmark]
    public ColumnVector<T> SolveVector() => PluFactorizationOperations.SolveUsingPLU(left, rightVec, -T.One);

    [Benchmark]
    public MathNet.Numerics.LinearAlgebra.Vector<float> MathNetSolveVector() => mLeft.Solve(mRightVec);
}
