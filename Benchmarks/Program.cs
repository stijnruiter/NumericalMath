using BenchmarkDotNet.Running;
using Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<MathFuncBenchmark<float>>();
    }
}
