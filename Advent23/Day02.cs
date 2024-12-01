using AoCLibrary;

namespace Advent23
{
	internal class Day02 : IDayRunner
	{
		public object? Star1()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var dictMax = new Dictionary<string, int>() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };
			foreach (var line in lines)
			{
				var game = line.Split(":");
				var gameId = int.Parse(game[0].Split(" ")[1]);
				var pulls = game[1].Split(";");
				var bad = false;
				foreach (var pull in pulls)
				{
					var colors = pull.Split(",");
					var dict = new Dictionary<string, int>();
					foreach (var color in colors)
					{
						var parts = color.Split(" ", StringSplitOptions.RemoveEmptyEntries);
						var val = int.Parse(parts[0]);
						var type = parts[1];
						if (dict.ContainsKey(type))
							dict[type] += val;
						else
							dict[type] = val;
					}
					foreach (var kvp in dict)
					{
						if (kvp.Value > dictMax[kvp.Key])
						{
							bad = true;
							break;
						}
					}
				}
				if (!bad)
					rv += gameId;
			}
			return rv;
		}
		public object? Star2()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var dictMin = new Dictionary<string, int>() { { "red", 0 }, { "green", 0 }, { "blue", 0 } };
			foreach (var line in lines)
			{
				var game = line.Split(":");
				var gameId = int.Parse(game[0].Split(" ")[1]);
				dictMin = new Dictionary<string, int>() { { "red", 0 }, { "green", 0 }, { "blue", 0 } };
				var pulls = game[1].Split(";");
				foreach (var pull in pulls)
				{
					var colors = pull.Split(",");
					var dict = new Dictionary<string, int>();
					foreach (var color in colors)
					{
						var parts = color.Split(" ", StringSplitOptions.RemoveEmptyEntries);
						var val = int.Parse(parts[0]);
						var type = parts[1];
						if (dict.ContainsKey(type))
							dict[type] += val;
						else
							dict[type] = val;
					}
					foreach (var kvp in dict)
					{
						if (kvp.Value > dictMin[kvp.Key])
							dictMin[kvp.Key] = kvp.Value;
					}
				}
				rv += dictMin["green"] * dictMin["red"] * dictMin["blue"];
			}
			return rv;
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
