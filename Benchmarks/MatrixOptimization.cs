using BenchmarkDotNet.Attributes;
using LinearAlgebra;
using LinearAlgebra.Structures;
using System.Numerics;

namespace Benchmarks;

public class MatrixOptimization<T> where T : struct, INumber<T>
{
    [Params(10, 100, 500)]
    public int Size { get; set; }

    private Matrix<T> left;
    private Matrix<T> right;
    private ColumnVector<T> rightVec;

    [GlobalSetup]
    public void Setup()
    {
        left = new Matrix<T>(Size, Size, T.One + T.One + T.One);
        right = Matrix<T>.Diagonal(Size, T.One + T.One);
        rightVec = new ColumnVector<T>(Size, T.One);

    }

    [Benchmark(Baseline = true)]
    public Matrix<T> Addition() => left + right;

    //[Benchmark]
    //public Matrix<T> Subtraction() => left - right;

    //[Benchmark]
    //public Matrix<T> Product() => left * right;

    [Benchmark]
    public Matrix<T> SolveMatrix() => PluFactorizationOperations.SolveUsingPLU(left, right, -T.One);


    [Benchmark]
    public ColumnVector<T> SolveVector() => PluFactorizationOperations.SolveUsingPLU(left, rightVec, -T.One);
}
