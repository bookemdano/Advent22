using AoCLibrary;
using System.Diagnostics;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Advent23
{
	internal class Program
	{
		static internal string[] GetLines(StarEnum star, bool real)
		{
			if (!_starLines.Any())
				Read(real);

			if (real)
				return _starLines[StarEnum.Star1];
			else
				return _starLines[star];
		}
		static readonly Dictionary<StarEnum, string[]> _starLines = [];

		static void Main(string[] args)
		{
			//for(int i = 1; i < 26; i++)
			//	WriteStubFiles(i);
			var sw = Stopwatch.StartNew();
			var runner = GetDayRunner();
			if (runner == null)
				ElfHelper.Log("No runner found for Day" + ElfHelper.DayString());
			else
			{
				Stopwatch.StartNew();
				ElfHelper.Log(runner.Run(), sw);
			}
		}
		static IDayRunner? GetDayRunner()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var advent = assemblies[1];
			if (advent?.FullName?.Contains("Advent23") != true)
			{
				ElfHelper.Log("No assembly[1] found!");
				return null;
			}
			var className = $"Advent23.Day{ElfHelper.DayString()}";
			var dayClass = advent.GetType(className);
			if (dayClass == null)
			{
				ElfHelper.Log($"No class {className} found in {advent}");
				ElfHelper.WriteStubFiles(ElfHelper.Day(), false);
				return null;
			}
			var o = Activator.CreateInstance(dayClass);
			if (o is IDayRunner rv)
				return rv;
			return null;
		}



		static void Read(bool real)
		{
			if (real)
			{
				var filename = $"Day{ElfHelper.Day():00}.txt";
				ElfHelper.Log("Read" + filename);
				_starLines.Add(StarEnum.Star1, Read(filename));
			}
			else
			{
				for(int i = 0; i < 2; i++)
				{
					var filename = $"Day{ElfHelper.Day():00}FakeStar{i + 1}.txt";
					ElfHelper.Log("Read" + filename);
					var star = StarEnum.Star1;
					if (i == 1)
						star = StarEnum.Star2;
					_starLines.Add(star, Read(filename));
				}
			}
		}
		static string[] Read(string filename)
		{
			return File.ReadAllLines(Path.Combine("Assets", filename)).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
		}
	}
	public enum StarEnum
	{
		NA,
		Star1,
		Star2
	}
}
