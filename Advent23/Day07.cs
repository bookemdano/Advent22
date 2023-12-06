using AoCLibrary;
namespace Advent23
{
	internal class Day07 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/7
		// Input https://adventofcode.com/2023/day/7/input
		private object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
            if (!IsReal)
                Utils.Assert(rv, 0L);
			return rv;
		}
		private object? Star2()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
            if (!IsReal)
                Utils.Assert(rv, 0L);
			return rv;
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
