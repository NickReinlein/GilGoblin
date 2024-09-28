using BenchmarkDotNet.Running;
using PriceBenchmarks;

namespace PriceBenchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<PriceSavingBenchmarks>();
    }
}