using AoCLibrary;
using System.Diagnostics;
using System.Reflection;

namespace Advent23
{
	internal class Program
	{
		static bool _crazyTimers = false;
		static async Task Main()
		{
			Utils.AppName = "RUN";

			var runner = GetDayRunner();
			if (runner == null)
                Utils.Log("No runner found for Day" + ElfHelper.DayString());
			else
			{
				if (_crazyTimers)
				{
					var best = double.MaxValue;
					for(int i = 0; i < 10; i++)
					{
						var res = await RunAsync(runner);
						if (res.TotalMilliseconds < best)
							best = res.TotalMilliseconds;
					}
					Utils.Log($"Best run {best:0.0}ms");
				}
				else
					await RunAsync(runner);
			}
		}
		static async Task<TimeSpan> RunAsync(IDayRunner runner)
		{
			Utils.ResetTestLog();
			Utils.TestLog($"Run() {runner.GetType().Name} r:{runner.IsReal}");
			if (runner.IsReal && !IsFileThere(InputFile(runner.IsReal, StarEnum.NA)))
			{
				var str = await ElfHelper.WriteInputFileAsync(ElfHelper.Day); 
				var filename = InputFile(runner.IsReal, StarEnum.NA);
				File.WriteAllText(filename, str);
			}

			var res = new RunnerResult();

			var sw = Stopwatch.StartNew();
			res.Star1 = runner.Star1();
			var t1 = sw.Elapsed;
			res.Star2 = runner.Star2();
			var rv = sw.Elapsed;
			var t2 = rv - t1;
			Utils.Log(res + $" t1: {t1.TotalMilliseconds:0.0}ms t2: {t2.TotalMilliseconds:0.0}ms");

			return rv;
		}
		static IDayRunner? GetDayRunner()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var advent = assemblies[1];
			if (advent?.FullName?.Contains("Advent23") != true)
			{
                Utils.Log("No assembly[1] found!");
				return null;
			}
			var className = $"Advent23.Day{ElfHelper.DayString()}";
			var dayClass = advent.GetType(className);
			if (dayClass == null)
			{
                Utils.Log($"No class {className} found in {advent}");
				//await ElfHelper.WriteStubFiles(ElfHelper.Day, false);
				return null;
			}
			var o = Activator.CreateInstance(dayClass);
			if (o is IDayRunner rv)
				return rv;
			return null;
		}

		static bool IsFileThere(string file)
		{
			if (!File.Exists(file))
				return false;
			var info = new FileInfo(file);
			return (info.Length > 0);
		}
		static internal string InputFile(bool real, StarEnum star)
		{
			string filename;
			if (real)
			{
				filename = $"Day{ElfHelper.DayString()}.txt";
			}
			else
			{
				if (star == StarEnum.Star1)
					filename = $"Day{ElfHelper.DayString()}FakeStar1.txt";
				else
					filename = $"Day{ElfHelper.DayString()}FakeStar2.txt";
			}
			var rv = Path.Combine("Assets", filename);
			if (!IsFileThere(rv) && star == StarEnum.Star2 && real == false)
			{
				filename = $"Day{ElfHelper.DayString()}FakeStar1.txt";
				rv = Path.Combine("Assets", filename);
			}
			return rv;
		}
		static internal string[] GetLines(StarEnum star, bool real)
		{
			return ReadLines(star, real);
		}
		static internal string GetText(StarEnum star, bool real)
		{
			return ReadText(star, real);
		}

		static string[] ReadLines(StarEnum star, bool real)
		{
			var filename = InputFile(real, star);
			Utils.Log("ReadLines" + filename);
			return File.ReadAllLines(filename).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
		}
		static string ReadText(StarEnum star, bool real)
        {
			var filename = InputFile(real, star);
			Utils.Log("ReadText" + filename);
			return File.ReadAllText(filename);
		}
	}
	public enum StarEnum
	{
		NA,
		Star1,
		Star2
	}
}
