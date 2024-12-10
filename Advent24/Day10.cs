using AoCLibrary;

namespace Advent24;

internal class Day10 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/10
	// Input https://adventofcode.com/2024/day/10/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 32L);
		else
			check = new StarCheck(key, 778L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var map = new MyMap(lines);
		var heads = map.FindAll('0');
		foreach(var head in heads)
		{
			var trails = new List<Loc>();
			trails.Add(head);
			for(int i = 0; i < 9; i++)
			{
				var adds = new List<Loc>();
				foreach (var trail in trails)
					adds.AddRange(map.Next(trail));
				trails = adds.DistinctBy(l => l.GetHashCode()).ToList();
				if (trails.Count == 0)
					break;
			}
			foreach (var trail in trails)
				if (map.GetInt(trail) == 9)
					rv++;
		}

		/*while(heads.Count > 0)
		{
			var adds = new List<Loc>();
			foreach (var head in heads)
			{
				adds.AddRange(map.Next(head));
			}
			heads = adds;
			var removes = new List<Loc>();
			foreach (var head in heads)
			{
				if (map.GetInt(head) == 9)
				{
					removes.Add(head);
					rv++;
				}
			}
			foreach (var remove in removes)
				heads.Remove(remove);
		}*/

		check.Compare(rv);
		return rv;
	}

	public class MyMap : GridMap
	{
		public MyMap(string[]? lines) : base(lines)
		{
		}

		internal List<Loc> Next(Loc head)
		{
			var rv = new List<Loc>();
			var n = GetInt(head);
			if (n == 9)
				return rv;
			n++;
			foreach (var dir in Enum.GetValues<DirEnum>())
			{
				var next = head.Move(dir);
				if (GetInt(next) == n)
					rv.Add(next);
			}
			return rv;
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 81L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var map = new MyMap(lines);
		var heads = map.FindAll('0');
		foreach (var head in heads)
		{
			var trails = new List<Loc>();
			trails.Add(head);
			for (int i = 0; i < 9; i++)
			{
				var adds = new List<Loc>();
				foreach (var trail in trails)
					adds.AddRange(map.Next(trail));
				trails = adds;
				//trails = adds.DistinctBy(l => l.GetHashCode()).ToList();
				if (trails.Count == 0)
					break;
			}
			foreach (var trail in trails)
				if (map.GetInt(trail) == 9)
					rv++;
		}
		check.Compare(rv);
		return rv;
	}
}

