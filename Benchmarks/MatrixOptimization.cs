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

    [GlobalSetup]
    public void Setup()
    {
        left = new Matrix<T>(Size, Size, T.One + T.One + T.One);
        right = new Matrix<T>(Size, Size, T.One + T.One);
    }

    [Benchmark(Baseline = true)]
    public Matrix<T> Addition() => left + right;

    [Benchmark]
    public Matrix<T> Subtraction() => left - right;

    [Benchmark]
    public Matrix<T> Product() => left * right;

    [Benchmark]
    public Matrix<T> Solve() => PluFactorizationOperations.SolveUsingPLU(left, right, T.Zero);
}
