using AoCLibrary;
namespace Advent23
{
	internal class Day08 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/8
		// Input https://adventofcode.com/2023/day/8/input
		public object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
            if (!IsReal)
                Utils.Assert(rv, 0L);
			return rv;
		}
		public object? Star2()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
            if (!IsReal)
                Utils.Assert(rv, 0L);
			return rv;
		}

	}
}