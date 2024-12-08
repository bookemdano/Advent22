using AoCLibrary;
namespace Advent24;

internal class Day07 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/7
	// Input https://adventofcode.com/2024/day/7/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 3749);
		else
			check = new StarCheck(key, 1153997401072L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
			var bridges = new List<Bridge>() { new Bridge(line) };
			while (bridges.Count > 0)
			{
				var bridge = bridges.First();
				var acts = "+x";
				var found = false;
				foreach(var act in acts)
				{
					var b1 = bridge.Act(act, out var d1);
					if (b1 != null)
					{
						if (d1 == DoneEnum.DoneGood)
						{
							rv += b1.Target;
							found = true;
							break;
						}
						if (d1 == DoneEnum.NotDone)
							bridges.Add(b1);
					}
				}
				if (found)
					break;
				bridges.Remove(bridge);
			}
		}

		check.Compare(rv);
		return rv;
	}
	public enum DoneEnum
	{
		NotDone,
		DoneGood,
		DoneBad
	}
	public class Bridge
	{
		public override string ToString()
		{
			return $"{Target}:{string.Join(',', Links)}({Total})";
		}
		public long? Total { get; set; }
		public long Target { get; set; }
		public List<long> Links { get; set; } = [];
		public Bridge(string line)
		{
			var parts = line.Split(':');
			Target = long.Parse(parts[0]);
			var links = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
			foreach(var link in links)
				Links.Add(long.Parse(link));
		}

		public Bridge(long target, long total, List<long> links)
		{
			Target = target;
			Total = total;
			Links = links;
		}

		internal Bridge? Act(char c, out DoneEnum done)
		{
			long total = 0;
			int linksUsed = 0;
			done = DoneEnum.NotDone;
			if (Total == null)
			{
				if (Links.Count >= 2)
				{
					if (c == '+')
						total = Links[0] + Links[1];
					else if (c == 'x')
						total = Links[0] * Links[1];
					else if (c == '|')
						total = long.Parse($"{Links[0]}{Links[1]}");
					linksUsed = 2;
				}
			}
			else
			{
				if (Links.Count >= 1)
				{
					if (c == '+')
						total = (Total ?? 0) + Links[0];
					else if (c == 'x')
						total = (Total ?? 1) * Links[0];
					else if (c == '|')
						total = long.Parse($"{Total}{Links[0]}");
					linksUsed = 1;
				}
			}

			if (linksUsed == 0)
			{
				return null;
			}
			//List<int> newLinks;
			//if (linksUsed == Links.Count)
			var rv = new Bridge(Target, total, Links[linksUsed..]);
			if (rv.Done())
				done = DoneEnum.DoneGood;
			else if (rv.Links.Count == 0 || Total > Target)
				done = DoneEnum.DoneBad;

			return rv;
		}

		internal bool Done()
		{
			return (Total == Target && Links.Count == 0);
		}
	}

	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 11387);
		else
			check = new StarCheck(key, 97902809384118L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
			var bridges = new List<Bridge>() { new Bridge(line) };
			while (bridges.Count > 0)
			{
				var bridge = bridges.First();
				var acts = "+x|";
				var found = false;
				foreach (var act in acts)
				{
					var b1 = bridge.Act(act, out var d1);
					if (b1 != null)
					{
						if (d1 == DoneEnum.DoneGood)
						{
							rv += b1.Target;
							found = true;
							break;
						}
						if (d1 == DoneEnum.NotDone)
							bridges.Add(b1);
					}
				}
				if (found)
					break;
				bridges.Remove(bridge);
			}
		}

		check.Compare(rv);
		return rv;
	}
}

