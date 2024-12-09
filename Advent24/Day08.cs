using AoCLibrary;
namespace Advent24;

internal class Day08 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/8
	// Input https://adventofcode.com/2024/day/8/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 14L);
		else
			check = new StarCheck(key, 276L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
		var uniques = map.Text().Distinct().ToList();
		uniques.Remove('.');
		var ans = new List<Loc>();
		foreach (var unique in uniques)
		{
			var locs = map.FindAll(unique);
			for (int i = 0; i < locs.Count - 1; i++)
			{
				for (int j = i + 1; j < locs.Count; j++)
				{
					var loc1 = locs[i];
					var loc2 = locs[j];
					var diff = loc1.Diff(loc2);
					var an1 = loc1.Minus(diff);
					if (map.IsValid(an1))
					{
						if (!ans.Any(l => l.Same(an1)))
							ans.Add(an1);
						rv++;
					}
					var an2 = loc2.Plus(diff);
					if (map.IsValid(an2))
					{
						if (!ans.Any(l => l.Same(an2)))
							ans.Add(an2);
						//map.Set(an2, '#');
						rv++;
					}
				}
			}
		}
		rv = ans.Count();
		ElfHelper.DayLog(map);

		check.Compare(rv);
		return rv;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 34L);
		else
			check = new StarCheck(key, 991L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
		var uniques = map.Text().Distinct().ToList();
		uniques.Remove('.');
		var ans = new List<Loc>();
		foreach (var unique in uniques)
		{
			var locs = map.FindAll(unique);
			for (int i = 0; i < locs.Count - 1; i++)
			{
				for (int j = i + 1; j < locs.Count; j++)
				{
					var loc1 = locs[i];
					AddIfNew(ans, loc1);
					var loc2 = locs[j];
					AddIfNew(ans, loc2);
					var diff = loc1.Diff(loc2);
					while (map.IsValid(loc1) || map.IsValid(loc2))
					{
						var an1 = loc1.Minus(diff);
						if (map.IsValid(an1))
							AddIfNew(ans, an1);
						loc1 = an1;
						var an2 = loc2.Plus(diff);
						if (map.IsValid(an2))
							AddIfNew(ans, an2);
						loc2 = an2;
					}
				}
			}
		}
		ans = ans.OrderBy(l => l.Row).ToList();

		rv = ans.Count();

		check.Compare(rv);
		return rv;
	}
	void AddIfNew(List<Loc> locs, Loc loc)
	{
		if (!locs.Any(l => l.Same(loc)))
			locs.Add(loc);
	}
}

