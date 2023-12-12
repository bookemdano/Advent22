using AoCLibrary;
namespace Advent23
{
	internal class Day13 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/13
		// Input https://adventofcode.com/2023/day/13/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (IsReal)
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
			if (IsReal)
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
}
