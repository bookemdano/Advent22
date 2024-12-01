using AoCLibrary;
namespace Advent24;

internal class Day01 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/1
	// Input https://adventofcode.com/2024/day/1/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 11L);
		else
			check = new StarCheck(key, 1603498L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		var lhs = new List<int>();
		var rhs = new List<int>();
		foreach (var line in lines)
		{
			var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			lhs.Add(int.Parse(parts[0]));
			rhs.Add(int.Parse(parts[1]));
		}
		lhs = lhs.OrderBy(i => i).ToList();
		rhs = rhs.OrderBy(i => i).ToList();
		var dist = 0;
		for(int i = 0; i<lhs.Count(); i++)
		{
			dist += Math.Abs(lhs[i] - rhs[i]);
		}
		rv = dist;
		// magic

		check.Compare(rv);
		return rv;
	}
	
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 31L);
		else
			check = new StarCheck(key, 25574739L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var lhs = new List<int>();
		var rhs = new List<int>();
		foreach (var line in lines)
		{
			var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			lhs.Add(int.Parse(parts[0]));
			rhs.Add(int.Parse(parts[1]));
		}
		for (int i = 0; i < lhs.Count(); i++)
		{
			var c = rhs.Count(v => v == lhs[i]);
			rv += c * lhs[i];
		}

		check.Compare(rv);
		return rv;
	}
}
