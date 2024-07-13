using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Diagnosers;

[MemoryDiagnoser]
[EtwProfiler]
[HardwareCounters(
	HardwareCounter.CacheMisses,
	HardwareCounter.BranchMispredictions,
	HardwareCounter.BranchInstructions
)]
public class EcsBenchmark
{
	private const int EntityCount = 100000;
	private AoSProgram m_AosProgram;
	private SoAProgram m_SoaProgram;
	private SoA2Program m_Soa2Program;

	[GlobalSetup]
	public void Setup()
	{
		m_AosProgram = new AoSProgram();
		m_SoaProgram = new SoAProgram();
		m_Soa2Program = new SoA2Program();

		for (int i = 0; i < EntityCount; i++)
		{
			m_AosProgram.AddEntity(100, i * 0.1f, i * 0.2f);
			m_SoaProgram.AddEntity(100, i * 0.1f, i * 0.2f);
			m_Soa2Program.AddEntity(100, i * 0.1f, i * 0.2f);
		}
	}

	[Benchmark]
	public void AoSUpdatePositions()
	{
		m_AosProgram.UpdatePositions();
	}

	[Benchmark]
	public void SoAUpdatePositions()
	{
		m_SoaProgram.UpdatePositions();
	}
    
	[Benchmark]
	public void SoA2UpdatePositions()
	{
		m_Soa2Program.UpdatePositions();
	}

	[Benchmark]
	public void AoSDamageEntities()
	{
		m_AosProgram.DamageEntities(1000);
	}

	[Benchmark]
	public void SoADamageEntities()
	{
		m_SoaProgram.DamageEntities(1000);
	}
    
	[Benchmark]
	public void SoA2DamageEntities()
	{
		m_Soa2Program.DamageEntities(1000);
	}
}

public class Program
{
	public static void Main(string[] args)
	{
		Console.WriteLine("Running ECS Benchmarks...");
		var summary = BenchmarkRunner.Run<EcsBenchmark>();
		Console.WriteLine("Benchmarks completed. Press any key to exit.");
		Console.ReadKey();
	}
}