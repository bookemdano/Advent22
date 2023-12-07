using AoCLibrary;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Advent23
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//for(int i = 1; i < 26; i++)
			//	WriteStubFiles(i);
			Utils.AppName = "RUN";

			var runner = GetDayRunner();
			if (runner == null)
                Utils.Log("No runner found for Day" + ElfHelper.DayString());
			else
			{
				var res = Run(runner);
			}
		}
		static RunnerResult Run(IDayRunner runner)
		{
			Utils.ResetTestLog();
			Utils.TestLog($"Run() {runner.GetType().Name} r:{runner.IsReal}");
			if (runner.IsReal && !IsFileThere(InputFile(runner.IsReal, StarEnum.NA)))
				ElfHelper.WriteInputFile(ElfHelper.Day);

			var sw = Stopwatch.StartNew();

			var rv = new RunnerResult();
			rv.Star1 = runner.Star1();
			rv.Star2 = runner.Star2();

			Utils.Log(rv, sw);

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
			return ReadLines(real, star);
		}
		static internal string GetText(StarEnum star, bool real)
		{
			return ReadText(real, star);
		}

		static string[] ReadLines(bool real, StarEnum star)
		{
			var filename = InputFile(real, star);
			Utils.Log("ReadLines" + filename);
			return File.ReadAllLines(filename).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
		}
		static string ReadText(bool real, StarEnum star)
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
