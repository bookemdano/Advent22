using System.Diagnostics;
using System.Reflection;

namespace Advent23
{
	internal class Program
	{

		static internal bool Real = false;
		static void Main(string[] args)
		{
			var sw = Stopwatch.StartNew();
			var runner = GetDayRunner();
			if (runner == null)
				Log("No runner found for Day" + DayString());
			else
				Log(runner.Run(Read()), sw);
		}
		static IDayRunner? GetDayRunner()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var advent = assemblies[1];
			if (advent == null)
			{
				Log("No assembly[1] found!");
				return null;
			}
			var className = $"Advent23.Day{DayString()}";
			var dayClass = advent.GetType(className);
			if (dayClass == null)
			{
				Log($"No class {className} found in {advent}");
				return null;
			}
			return (IDayRunner) Activator.CreateInstance(dayClass);
		}
		static string DayString()
		{
			return $"{Day():00}";
		}
		static int Day()
		{
			return 2;// (int) (DateTime.Today - new DateTime(2023, 11, 30)).TotalDays;
		}
		static string[] Read()
		{
			var stub = "Day";
			if (!Real)
				stub += "Fake";
			var filename = $"{stub}{Day():00}.txt";
			Log("Read" + filename);
			return File.ReadAllLines(filename);
			
		}
		internal static void Log(object o, Stopwatch? sw = null)
		{
			var reality = "REAL";
			if (!Real)
				reality = "FAKE";
			var str = o?.ToString()??"";
			if (sw != null)
				str += $" {sw.ElapsedMilliseconds:0}ms";
			File.AppendAllText($"c:\\temp\\data\\endless{DateTime.Today.Year}.log", $"{DateTime.Now} {reality} {str}{Environment.NewLine}");
			Console.WriteLine(reality + " " + o);
		}
	}
}
