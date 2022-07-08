using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Password;

BenchmarkRunner.Run<TestPerf>();

//new TestPerf().OptimizedPrint();
//new TestPerf().NotOptimizedPrint();

[MemoryDiagnoser]
public class TestPerf
{
	private readonly string password;
	private readonly byte[] salt;

	public TestPerf()
	{
		salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
		password = "Password to test perf!.";
	}

	[Benchmark]
	public void Optimized()
	{
		for (int i = 0; i < 100; i++)
			Utils.GeneratePasswordHashUsingSaltMemoryOptimized(password, salt);
	}

	[Benchmark]
	public void NotOptimized()
	{
		for (int i = 0; i < 100; i++)
			Utils.GeneratePasswordHashUsingSaltNotOptimized(password, salt);
	}

	public void OptimizedPrint()
	{
		for (int i = 0; i < 10; i++)
		{
			var result = Utils.GeneratePasswordHashUsingSaltMemoryOptimized(password, salt);
			Console.WriteLine(result);
		}
	}

	public void NotOptimizedPrint()
	{
		for (int i = 0; i < 10; i++)
		{
			var result = Utils.GeneratePasswordHashUsingSaltNotOptimized(password, salt);
			Console.WriteLine(result);
		}
	}
}