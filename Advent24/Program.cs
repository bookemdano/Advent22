using AoCLibrary;
using System.Diagnostics;
using System.Reflection;

namespace Advent24;
internal class Program
{
	static bool _crazyTimers = false;
	static async Task Main()
	{
		Utils.AppName = "RUN";

		//var runner = GetDayRunner("14");
		var runner = GetDayRunner(ElfHelper.DayString);
		if (runner == null)
			ElfHelper.DayLog("No runner found for Day" + ElfHelper.DayString);
		else
		{
			if (_crazyTimers)
			{
				var best = TimeSpan.MaxValue;
				for (int i = 0; i < 10; i++)
				{
					var res = await RunAsync(runner);
					if (res < best)
						best = res;
				}
				Utils.MonthLog($"Best run {ElfHelper.SmallString(best)}");
			}
			else
				await RunAsync(runner);
		}
	}
	static async Task<TimeSpan> RunAsync(IDayRunner runner)
	{
		ElfHelper.ResetDayLog();
		ElfHelper.MonthLogPlus($"Run() {runner.GetType().Name} r:{runner.IsReal}");
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
			ElfHelper.DayLog($"r1:{res.Star1} t1: {ElfHelper.SmallString(t1)}");
		res.Star2 = runner.Star2();
		var rv = sw.Elapsed;
		var t2 = rv - t1;
		if (!_crazyTimers)
		{
			ElfHelper.DayLog($"r2:{res.Star2} t2: {ElfHelper.SmallString(t2)}");
			ElfHelper.DayLog($"total: {ElfHelper.SmallString(rv)}");
		}
		else
			ElfHelper.MonthLogPlus($"{res} t1: {ElfHelper.SmallString(t1)} t2: {ElfHelper.SmallString(t2)} total: {ElfHelper.SmallString(rv)}");
		return rv;
	}
	static IDayRunner? GetDayRunner(string dayString)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		var advent = assemblies[1];
		if (advent?.FullName?.Contains($"Advent{ElfHelper.Year2}") != true)
		{
			ElfHelper.DayLog("No assembly[1] found!");
			return null;
		}
		var className = $"Advent{ElfHelper.Year2}.Day{dayString}";
		var dayClass = advent.GetType(className);
		if (dayClass == null)
		{
			ElfHelper.DayLog($"No class {className} found in {advent}");
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
		if (real)
			return Path.Combine("Assets", $"Day{ElfHelper.DayString}.txt");


		var partString = string.Empty;
		if (part != null)
			partString = $"Part{part + 1}";

		string filename = Path.Combine("Assets", $"Day{ElfHelper.DayString}Fake{partString}.txt");
		if (!IsFileThere(filename))
		{
			filename = Path.Combine("Assets", $"Day{ElfHelper.DayString}Fake{star}{partString}.txt");
			if (!IsFileThere(filename)) // try start 1
				filename = Path.Combine("Assets", $"Day{ElfHelper.DayString}Fake{StarEnum.Star1}{partString}.txt");
		}
		return filename;
	}
	static Dictionary<string, string[]> _dictLines = [];
	static internal string[] GetLines(StarCheckKey key, bool raw = false)
	{
		return GetLines(key.Star, key.IsReal, key.Part);
	}
	static internal string[] GetLines(StarEnum star, bool real, int? part = null, bool raw = false)
	{
		var filename = InputFile(real, star, part);
		if (!_dictLines.ContainsKey(filename))
		{
			ElfHelper.MonthLogPlus("ReadLines- " + filename);
			var lines = File.ReadAllLines(filename);
			if (!raw)
				_dictLines[filename] = lines.ToArray();
			else
				_dictLines[filename] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
		}
		return _dictLines[filename];
	}
	static Dictionary<string, string> _dictText = [];
	static internal string GetText(StarCheckKey key)
	{
		return GetText(key.Star, key.IsReal, key.Part);
	}
	static internal string GetText(StarEnum star, bool real, int? part)
	{
		var filename = InputFile(real, star, part);
		if (!_dictText.ContainsKey(filename))
		{
			ElfHelper.MonthLogPlus("ReadText- " + filename);
			_dictText[filename] = File.ReadAllText(filename);
		}
		return _dictText[filename];
	}
}
