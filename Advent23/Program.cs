using AoCLibrary;
using System.Diagnostics;
using System.Reflection;

namespace Advent23
{
	internal class Program
	{
		static bool _crazyTimers = true;
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
					var best = TimeSpan.MaxValue;
					for(int i = 0; i < 10; i++)
					{
						var res = await RunAsync(runner);
						if (res < best)
							best = res;
					}
					Utils.Log($"Best run {ElfHelper.SmallString(best)}");
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
			if (!_crazyTimers)
				Utils.Log($"r1:{res.Star1} t1: {ElfHelper.SmallString(t1)}");
			res.Star2 = runner.Star2();
			var rv = sw.Elapsed;
			var t2 = rv - t1;
			if (!_crazyTimers)
			{
				Utils.Log($"r2:{res.Star2} t2: {ElfHelper.SmallString(t2)}");
				Utils.Log($"total: {ElfHelper.SmallString(rv)}");
			}
			else
				Utils.Log($"{res} t1: {ElfHelper.SmallString(t1)} t2: {ElfHelper.SmallString(t2)} total: {ElfHelper.SmallString(rv)}");
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
		static internal string InputFile(bool real, StarEnum star, int? part = null)
		{
			string filename;
			if (real)
			{
				filename = $"Day{ElfHelper.DayString()}.txt";
			}
			else
			{
				if (part == null)
					filename = $"Day{ElfHelper.DayString()}Fake{star}.txt";
				else
					filename = $"Day{ElfHelper.DayString()}Fake{star}Part{part + 1}.txt";
			}
			var rv = Path.Combine("Assets", filename);
			if (!IsFileThere(rv) && star == StarEnum.Star2 && real == false)
			{
				filename = $"Day{ElfHelper.DayString()}FakeStar1.txt";
				rv = Path.Combine("Assets", filename);
			}
			return rv;
		}
		static Dictionary<string, string[]> _dictLines = [];
		static internal string[] GetLines(StarCheckKey key)
		{
			return GetLines(key.Star, key.IsReal, key.Part);
		}
		static internal string[] GetLines(StarEnum star, bool real, int? part = null)
		{
			var filename = InputFile(real, star, part);
			if (!_dictLines.ContainsKey(filename))
			{
				Utils.Log("ReadLines- " + filename);
				_dictLines[filename] = File.ReadAllLines(filename).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
			}
			return _dictLines[filename];
		}
		static Dictionary<string, string> _dictText = [];
		static internal string GetText(StarEnum star, bool real)
		{
			var filename = InputFile(real, star);
			if (!_dictText.ContainsKey(filename))
			{
				Utils.Log("ReadText- " + filename);
				_dictText[filename] = File.ReadAllText(filename);
			}
			return _dictText[filename];
		}
	}
	public enum StarEnum
	{
		NA,
		Star1,
		Star2
	}
}
