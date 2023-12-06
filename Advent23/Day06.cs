using AoCLibrary;
using System.Collections.Generic;

namespace Advent23
{
	internal class Day06 : IDayRunner
	{
		public bool IsReal => true;

		private object? Star1()
		{
			var rv = 1L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var times = Utils.SplitNums(' ', Utils.RemoveLabel(lines[0]));
            var dists = Utils.SplitNums(' ', Utils.RemoveLabel(lines[1]));
      	    for(int i = 0; i < times.Count(); i++)
                rv *= GetWinBinarys(times[i], dists[i]);
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
                
            rv *= GetWinBinarys(time, dist);

            if (!IsReal)
                Utils.Assert(rv, 71503);
            return rv;
        }
		long GetWinBinarys(long time, long dist)
		{
			var race = new Race(time, dist);
			var firstWin = BinSearch(1L, time - 1, race, true);
			var firstLateLoss = BinSearch(firstWin, time - 1, race, false);
			var lastWin = firstLateLoss - 1L;

			Utils.Assert(race.Win(firstWin), "firstWin");
			Utils.Assert(!race.Win(firstWin - 1), "lastEarlyLoss");
			Utils.Assert(race.Win(lastWin), "lastWin");
			Utils.Assert(!race.Win(lastWin + 1), "firstLateLoss");
			return lastWin - firstWin + 1;
		}

		private long BinSearch(long min, long max, Race race, bool winSearch)
		{
			while (max != min + 1)
			{
				var t = (long)(min + (max - min) / 2);
				var b = race.Win(t);
				if (!winSearch)
					b = !b;

				if (b)
					max = t;
				else
					min = t;
			}
			return max;
		}

		int GetWins(long time, long dist)
		{
            var wins = 0;
            for (int t = 1; t < time - 1; t++)
            {
                var d = t * (time - t);
				Utils.TestLog($"{t}, {d}, {time}");
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
	public class Race
	{
		public Race(long time, long dist)
		{
			Time = time;
			Distance = dist;
		}
		public long Time { get; set; }
		public long Distance { get; set; }
		public bool Win(long t)
		{
			var d = t * (Time - t);
			return (d > Distance);

		}
	}
}
