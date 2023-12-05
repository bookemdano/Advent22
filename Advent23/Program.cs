using AoCLibrary;
using System.Diagnostics;
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

			var sw = Stopwatch.StartNew();
			var runner = GetDayRunner();
			if (runner == null)
                Utils.Log("No runner found for Day" + ElfHelper.DayString());
			else
			{
				Stopwatch.StartNew();
                Utils.ResetTestLog();
                Utils.Log(runner.Run(), sw);
			}
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

		static internal string InputFile(bool real, StarEnum star)
		{
			if (real)
			{
				return $"Day{ElfHelper.DayString()}.txt";
			}
			else
			{
				if (star == StarEnum.Star1)
					return $"Day{ElfHelper.DayString()}FakeStar1.txt";
				else
					return $"Day{ElfHelper.DayString()}FakeStar2.txt";
			}
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
			return File.ReadAllLines(Path.Combine("Assets", filename)).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
		}
		static string ReadText(bool real, StarEnum star)
		{
			var filename = InputFile(real, star);
			Utils.Log("ReadText" + filename);
			return File.ReadAllText(Path.Combine("Assets", filename));
		}
	}
	public enum StarEnum
	{
		NA,
		Star1,
		Star2
	}
}
