using AoCLibrary;
namespace Advent21;

internal class Day09 : IRunner
{
	// Day https://adventofcode.com/2021/day/9
	// Input https://adventofcode.com/2021/day/9/input
	public RunnerResult Star1(bool isReal)
	{
		var key = new StarCheckKey(StarEnum.Star1, isReal, null);
		var res = new RunnerResult();
		if (!isReal)
			res.Check = new StarCheck(key, 15L);
		else
			res.Check = new StarCheck(key, 524L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = -1L;
		// magic
		var map = new GridMap(lines);
		var lows = new List<Loc>();
        for (int iRow = 0; iRow < map.Rows; iRow++)
        {
            for (int iCol = 0; iCol < map.Cols; iCol++)
			{
				var loc = new Loc(iRow, iCol);
                if (Lowest(map, loc))
					lows.Add(loc);
			}

        }
		var ints = lows.Select(l => map.GetInt(l)).ToList();
		rv = lows.Sum(l => (map.GetInt(l)??0) + 1);

        res.CheckGuess(rv);
        return res;
    }
	bool Lowest(GridMap map, Loc loc)
	{
        var moves = loc.AllMoves();
        var alt = map.GetInt(loc);

		foreach (var move in moves)
			if (map.GetInt(move) <= alt)
				return false;
		return true;
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 1134L);
		else
			res.Check = new StarCheck(key, 1235430L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var map = new GridMap(lines);
        var lows = new List<Loc>();
        for (int iRow = 0; iRow < map.Rows; iRow++)
        {
            for (int iCol = 0; iCol < map.Cols; iCol++)
            {
                var loc = new Loc(iRow, iCol);
                if (Lowest(map, loc))
                    lows.Add(loc);
            }

        }
		rv = 1L;
		var sizes = new List<int>();
		foreach(var low in lows)
		{
			sizes.Add(Size(map, low));
		}
		var top3 = sizes.OrderByDescending(x => x).Take(3).ToList();
		rv = top3[0] * top3[1] * top3[2];
        res.CheckGuess(rv);
        return res;
	}
	int Size(GridMap map, Loc low)
	{
		var locs = new Stack<Loc>();
		locs.Push(low);
		var founds = new List<Loc>() { low };

        while (locs.Any())
		{
			var first = locs.Pop();
            var newLocs = first.AllMoves();
			foreach(var newLoc in newLocs)
			{
				if (map.GetInt(newLoc) < 9)
				{
					if (!founds.Contains(newLoc))
					{
						founds.Add(newLoc);
						locs.Push(newLoc);
					}
				}
			}
        }
		return founds.Count();
    }

}

