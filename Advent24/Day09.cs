using AoCLibrary;
namespace Advent24;

internal class Day09 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2024/day/9
	// Input https://adventofcode.com/2024/day/9/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic

		check.Compare(rv);
		return rv;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic

		check.Compare(rv);
		return rv;
	}
}

