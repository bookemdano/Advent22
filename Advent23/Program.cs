using System.Diagnostics;
using System.Reflection;

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
		static Dictionary<StarEnum, string[]> _starLines = [];

		static void Main(string[] args)
		{
			//for(int i = 1; i < 26; i++)
			//	WriteStubFiles(i);
			var sw = Stopwatch.StartNew();
			var runner = GetDayRunner();
			if (runner == null)
				Log("No runner found for Day" + DayString());
			else
			{
				Stopwatch.StartNew();
				Log(runner.Run(), sw);
			}
		}
		static IDayRunner? GetDayRunner()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var advent = assemblies[1];
			if (advent == null || !advent.FullName.Contains("Advent23"))
			{
				Log("No assembly[1] found!");
				return null;
			}
			var className = $"Advent23.Day{DayString()}";
			var dayClass = advent.GetType(className);
			if (dayClass == null)
			{
				Log($"No class {className} found in {advent}");
				WriteStubFiles(Day());
				return null;
			}
			return (IDayRunner) Activator.CreateInstance(dayClass);
		}

		private static void WriteStubFiles(int day)
		{
			var strDay = $"{day:00}";
			var dir = @"c:\temp\data";
			var cs = File.ReadAllText("assets\\DayCS.txt");
			cs = cs.Replace("Day : IDayRunner", $"Day{strDay} : IDayRunner");
			File.WriteAllText(Path.Combine(dir, $"Day{strDay}.cs"), cs);
			File.Copy("assets\\Day01.txt", Path.Combine(dir, $"Day{strDay}.txt"), true);
			File.Copy("assets\\Day01FakeStar1.txt", Path.Combine(dir, $"Day{strDay}FakeStar1.txt"), true);
			File.Copy("assets\\Day01FakeStar2.txt", Path.Combine(dir, $"Day{strDay}FakeStar2.txt"), true);
		}

		static string DayString()
		{
			return $"{Day():00}";
		}
		static int Day()
		{
			return 2;// (int) (DateTime.Today - new DateTime(2023, 11, 30)).TotalDays;
		}
		static void Read(bool real)
		{
			if (real)
			{
				var filename = $"Day{Day():00}.txt";
				Log("Read" + filename);
				_starLines.Add(StarEnum.Star1, Read(filename));
			}
			else
			{
				for(int i = 0; i < 2; i++)
				{
					var filename = $"Day{Day():00}FakeStar{i + 1}.txt";
					Log("Read" + filename);
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
		internal static void Log(object o, Stopwatch? sw = null)
		{
			var str = o?.ToString()??"";
			if (sw != null)
				str += $" {sw.ElapsedMilliseconds:0}ms";
			File.AppendAllText($"c:\\temp\\data\\endless{DateTime.Today.Year}.log", $"{DateTime.Now} {str}{Environment.NewLine}");
			Console.WriteLine(o);
		}
	}
	public enum StarEnum
	{
		NA,
		Star1,
		Star2
	}
}
