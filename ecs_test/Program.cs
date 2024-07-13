using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;
using ecs_test.Tests;

[MemoryDiagnoser]
[EtwProfiler]
[HardwareCounters(
	HardwareCounter.CacheMisses,
	HardwareCounter.BranchMispredictions,
	HardwareCounter.BranchInstructions
)]
public class EcsBenchmark
{
	private const int EntityCount = 1000000;

	[ParamsAllValues]
	public EcsType Type { get; set; }

	private IEcsTestable Ecs;

	public enum EcsType
	{
		SoA,
		AoS,
		SIMD_SoA
	}

	[GlobalSetup]
	public void Setup()
	{
		Ecs = Type switch
		{
			EcsType.SoA => new SoA(),
			EcsType.AoS => new AoS(),
			EcsType.SIMD_SoA => new SIMD_SoA(),
			_ => throw new ArgumentOutOfRangeException()
		};

		var rand = new Random(42);
		for (int i = 0; i < EntityCount; i++)
		{
			Ecs.AddEntity(
				rand.Next(-100, 100), rand.Next(-100, 100), rand.Next(-100, 100),
				rand.Next(-10, 10), rand.Next(-10, 10), rand.Next(-10, 10),
				rand.Next(50, 100), rand.Next(1, 10), rand.Next(1, 3)
			);
		}
	}

	[Benchmark]
	public void UpdatePositions() => Ecs.UpdatePositions(0.016f);

	[Benchmark]
	public void ApplyGlobalDamage() => Ecs.ApplyGlobalDamage(5);

	[Benchmark]
	public void ScaleEntities() => Ecs.ScaleEntities(1.1f);

	[Benchmark]
	public int CountAliveEntities() => Ecs.CountAliveEntities();

	[Benchmark]
	public void ResetDamage() => Ecs.ResetDamage();
}

public class Program
{
	public static void Main(string[] args)
	{
		var summary = BenchmarkRunner.Run<EcsBenchmark>();
		Console.WriteLine("Benchmarks completed. Press any key to exit.");
		Console.ReadKey();
	}
}