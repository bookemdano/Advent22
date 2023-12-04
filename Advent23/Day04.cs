using AoCLibrary;
using static System.Formats.Asn1.AsnWriter;

namespace Advent23
{
	internal class Day04 : IDayRunner
	{
		private object? Star1()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			foreach(var line in lines)
			{
				var parts = line.Split("|:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				var winning = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var mine = parts[2].Split(' ', StringSplitOptions.RemoveEmptyEntries);
				int i = 0;
				var cardVal = 0;
				foreach (var my in mine)
				{
					if (winning.Contains(my))
						i++;
				}
				if (i > 0)
					rv += (int)Math.Pow(2, i - 1);
			}
			return rv;
		}
		private object? Star2()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var dict = new Dictionary<int, int>();  // cardIndex, count
			for (int i = 0; i < lines.Count(); i++)
			{
				dict[i] = 1;
			}
			for (int i = 0; i < lines.Count(); i++)
			{
				var line = lines[i];
				var wins = Wins(line);
				if (wins > 0)
				{
					ElfHelper.TestLog($"Card: {i + 1} Wins: {wins}");
					//rv += Score(winsOn);
					for (var j = i + 1; j < i + wins + 1; j++)
					{
						dict[j] += dict[i];
					}
				}
			}
			foreach(var kvp in dict)
			{
				var score = Score(lines[kvp.Key]);
				ElfHelper.TestLog($"Card: {kvp.Key + 1} Score: {score} Count: {kvp.Value}");
			}
			rv = dict.Values.Sum();
			return rv;
		}
		Dictionary<string, int> _wins = [];
		int Score(string line)
		{
			var wins = Wins(line);
			return (int)Math.Pow(2, wins - 1);
		}

		private int Wins(string line)
		{
			if (_wins.ContainsKey(line))
				return _wins[line];
			var parts = line.Split("|:".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var winning = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var mine = parts[2].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			int i = 0;
			foreach (var my in mine)
			{
				if (winning.Contains(my))
					i++;
			}
			_wins[line] = i;
			return i;
		}

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
	}
}
