using AoCLibrary;
namespace Advent23
{
	internal class Day06 : IDayRunner
	{
		public bool IsReal => false;

		private object? Star1()
		{
			var rv = 1;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var times = Utils.SplitNums(' ', Utils.RemoveLabel(lines[0]));
            var dists = Utils.SplitNums(' ', Utils.RemoveLabel(lines[1]));
      	    for(int i = 0; i < times.Count(); i++)
                rv *= GetWins(times[i], dists[i]);
            if (!IsReal)
                Utils.Assert(rv, 288);
            return rv;
		}
		private object? Star2()
		{
            var rv = 1L;
            var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var time = long.Parse(Utils.RemoveLabel(lines[0]).Replace(" ", ""));
            var dist = long.Parse(Utils.RemoveLabel(lines[1]).Replace(" ", ""));
                
            rv *= GetWins(time, dist);

            if (!IsReal)
                Utils.Assert(rv, 71503);
            return rv;
        }
		int GetWins(long time, long dist)
		{
            var wins = 0;
            for (int t = 1; t < time - 1; t++)
            {
                var d = t * (time - t);
                if (d > dist)
                    wins++;
            }
			return wins;
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
