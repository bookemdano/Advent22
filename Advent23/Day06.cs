using AoCLibrary;

namespace Advent23
{
	internal class Day06 : IDayRunner
	{
		public bool IsReal => true;

		public object? Star1()
		{
			var rv = 1L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var times = Utils.SplitNums(' ', Utils.RemoveLabel(lines[0]));
            var dists = Utils.SplitNums(' ', Utils.RemoveLabel(lines[1]));
			for (int i = 0; i < times.Count(); i++)
				rv *= GetWinQuad(times[i], dists[i]);
				//rv *= GetWinBinarys(times[i], dists[i]);
            if (!IsReal)
                Utils.Assert(rv, 288);
            return rv;
		}
		public object? Star2()
		{
            var rv = 1L;
            var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var time = long.Parse(Utils.RemoveLabel(lines[0]).Replace(" ", ""));
            var dist = long.Parse(Utils.RemoveLabel(lines[1]).Replace(" ", ""));
			rv *= GetWinQuad(time, dist);
			//rv *= GetWinBinaries(time, dist);

			if (!IsReal)
                Utils.Assert(rv, 71503);
            return rv;
        }
		long GetWinQuad(long time, long dist)
		{
			var a = -1; // upside down parabala
			var b = time;
			var c = -(dist + .00000001); // because we want to win, not tie
			var plus = ((0 - b) + Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
			var minus = ((0 - b) - Math.Sqrt(b * b - 4 * a * c)) / (2 * a);
			var firstWin = (long)Math.Ceiling(plus);
			var lastWin = (long)Math.Floor(minus);

			var race = new Race(time, dist);
			Utils.Assert(race.Win(firstWin), "firstWin");
			Utils.Assert(!race.Win(firstWin - 1), "lastEarlyLoss");
			Utils.Assert(race.Win(lastWin), "lastWin");
			Utils.Assert(!race.Win(lastWin + 1), "firstLateLoss");
			var wins = lastWin - firstWin + 1;
			Utils.TestLog($"{race} w:{wins}");
			return wins;
		}

		long GetWinBinaries(long time, long dist)
		{
			var race = new Race(time, dist);
			var firstWin = IRace.BinSearch(1L, time / 2, race, true);
			var firstLateLoss = IRace.BinSearch(time / 2, time - 1, race, false);
			var lastWin = firstLateLoss - 1L;

			Utils.Assert(race.Win(firstWin), "firstWin");
			Utils.Assert(!race.Win(firstWin - 1), "lastEarlyLoss");
			Utils.Assert(race.Win(lastWin), "lastWin");
			Utils.Assert(!race.Win(lastWin + 1), "firstLateLoss");
			var wins = lastWin - firstWin + 1;
			Utils.TestLog($"{race} w:{wins}");
			return wins;
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

	public class Race : IRace
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
			return (t * (Time - t) > Distance);
		}
		public override string ToString()
		{
			return $"t:{Time} d:{Distance}";
		}
	}
}
