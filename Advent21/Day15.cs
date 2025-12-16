using AoCLibrary;
using System.Diagnostics.Tracing;
namespace Advent21;
// #working
internal class Day15 : IRunner
{
	// Day https://adventofcode.com/2021/day/15
	// Input https://adventofcode.com/2021/day/15/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 40L);
		else
			res.Check = new StarCheck(key, 472L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
		rv = Solve(map);
	
        res.CheckGuess(rv);
        return res;
    }
	long Solve(GridMap map)
	{
        var start = new Loc(0, 0);

        var dictionary = new Dictionary<Loc, int>();

        var paths = new Stack<Path15>();
        paths.Push(new Path15(new Loc(0, 0), 0));
        var end = new Loc(map.Rows - 1, map.Cols - 1);
        var bests = new Dictionary<Loc, long>();
        var bestPath = int.MaxValue;
        while (paths.Any())
        {
            var path = paths.Pop();
            if (path.Head.Same(end))
            {
                if (path.Score < bestPath)
                {
                    var pct = (double) bests.Count() / (map.Rows * map.Cols);
                    ElfHelper.DayLogPlus($"New Best Score {path.Score} still checking {paths.Count()} covered {pct:P}");
                    bestPath = path.Score;
                }
                continue;
            }
            if (path.Score >= bestPath)
                continue;

            var moves = path.Head.AllMoves();
            var moveCounts = new Dictionary<Loc, int>();
            foreach (var move in moves)
            {
                var v = map.GetInt(move);
                if (v != null)
                {
                    moveCounts.Add(move, (int)v);
                }
            }

            foreach (var move in moveCounts)
            {
                var newValue = path.Score + (int)move.Value;

                if (!bests.ContainsKey(move.Key))
                    bests.Add(move.Key, newValue);
                else
                {
                    if (bests[move.Key] <= newValue)
                        continue;
                    else
                        bests[move.Key] = newValue;
                }
                paths.Push(new Path15(move.Key, newValue));
            }
        }
        return bestPath;
    }
    class Path15
	{
        public Path15(Loc head, int score)
        {
			Head = head;
			Score = score;
        }

        public Loc Head { get; }
        public int Score { get; }
        public override string ToString()
        {
			return $"{Head} {Score}";
        }
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 315L);
        else
            res.Check = new StarCheck(key, 2851L);

        var lines = Program.GetLines(key);
        //var text = Program.GetText(key);

        var rv = 0L;
        // magic
        var map = new GridMap(lines);
        var bigMap = new GridMap(map.Rows * 5, map.Cols * 5);
        int colOffset = 0;
        int rowOffset = 0;
        for (int gridRow = 0; gridRow < 5; gridRow++)
        {
            for (int gridCol = 0; gridCol < 5; gridCol++)
            {
                if (gridCol > 0)
                {
                    colOffset = map.Cols;
                    rowOffset = 0;
                }
                else
                {
                    rowOffset = map.Rows;
                    colOffset = 0;
                }

                for (int row = 0; row < map.Rows; row++)
                {
                    for (int col = 0; col < map.Cols; col++)
                    {
                        var bigRow = gridRow * map.Rows + row;
                        var bigCol = gridCol * map.Cols + col;
                        var loc = new Loc(bigRow, bigCol);
                        if (gridRow == 0 && gridCol == 0)
                            bigMap.SetInt(loc, (int) map.GetInt(loc));
                        else
                        {
                            var offsetLoc = new Loc(bigRow - rowOffset, bigCol - colOffset);
                            var val = (int)bigMap.GetInt(offsetLoc) + 1;
                            if (val > 9)
                                val = 1;
                            bigMap.SetInt(loc, val);
                        }
                    }
                }
            }
        }
        //ElfHelper.DayLogPlus(bigMap);
        rv = Solve(bigMap);

        res.CheckGuess(rv);
        return res;
	}
}

