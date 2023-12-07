using System.Diagnostics;

namespace Advent23
{
	internal class Day01 : IDayRunner
	{
		public bool IsReal
		{
			get
			{
				return true;
			}
		}

		public RunnerResult Run()
		{
			var rv = new RunnerResult();
			rv.Star1 = Star1();
			rv.Star2 = Star2();
			return rv;
		}
		public object? Star1()
		{
			var total = 0;
			foreach (var line in Program.GetLines(StarEnum.Star1, IsReal))
			{
				var f = line.First(c => char.IsDigit(c)).ToString();
				var l = line.Last(c => char.IsDigit(c)).ToString();
				total += int.Parse(f + l);
			}
			return total;
		}
		public object? Star2()
		{
			var total = 0;
			var dict = new Dictionary<string, int>();
			dict.Add("one", 1);
			dict.Add("two", 2);
			dict.Add("three", 3);
			dict.Add("four", 4);
			dict.Add("five", 5);
			dict.Add("six", 6);
			dict.Add("seven", 7);
			dict.Add("eight", 8);
			dict.Add("nine", 9);
			dict.Add("1", 1);
			dict.Add("2", 2);
			dict.Add("3", 3);
			dict.Add("4", 4);
			dict.Add("5", 5);
			dict.Add("6", 6);
			dict.Add("7", 7);
			dict.Add("8", 8);
			dict.Add("9", 9);
			foreach (var line in Program.GetLines(StarEnum.Star2, IsReal))
			{
				var f = First(line, dict).ToString();
				var l = Last(line, dict).ToString();
				var nStr = f + l;
				total += int.Parse(nStr);
				//Program.Log(line + " => " + nStr + " " + total);
			}
			return total;
		}



		private int First(string line, Dictionary<string, int> dict)
		{
			var first = line.Length;
			var val = -1;
			foreach (var kvp in dict)
			{
				var found = line.IndexOf(kvp.Key);
				if (found == -1)
					continue;
				if (found < first)
				{
					first = found;
					val = kvp.Value;
				}
			}
			Debug.Assert(val != -1);

			return val;					
		}
		private int Last(string line, Dictionary<string, int> dict)
		{
			var last = -1;
			var val = -1;
			foreach (var kvp in dict)
			{
				var found = line.LastIndexOf(kvp.Key);
				if (found == -1)
					continue;
				if (found > last)
				{
					last = found;
					val = kvp.Value;
				}
			}
			Debug.Assert(val != -1);
			return val;
		}
	}
}
