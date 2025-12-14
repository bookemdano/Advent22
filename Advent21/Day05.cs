using AoCLibrary;

namespace Advent21;

internal class Day05 : IRunner
{
	// Day https://adventofcode.com/2021/day/5
	// Input https://adventofcode.com/2021/day/5/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 5L);
		else
			res.Check = new StarCheck(key, 6005L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var pairs = new List<Pair5>();
		foreach (var line in lines)
		{
            var pair = new Pair5(line);
            if (pair.Straight())
				pairs.Add(pair);
        }
        var grid = new GridMapXY((int)pairs.Max(p => Math.Max(p.From.X, p.To.X)) + 1, (int)pairs.Max(p => Math.Max(p.From.Y, p.To.Y)) + 1);
        for (var i = 0; i < pairs.Count(); i++)
            pairs[i].Mark(grid);

        if (key.IsReal == false)
            ElfHelper.DayLogPlus(grid);
        rv = grid.Count('*');

        res.CheckGuess(rv);
        return res;
    }
	class Pair5
	{
        public Pair5(string line)
        {
            var parts = line.Split(" -> ");
            From = new Point(parts[0]);
            To = new Point(parts[1]);
        }
        public Point From { get; set; }
		public Point To { get; set; }
        public override string ToString()
        {
			return $"{From}-{To}";
        }
        Point[]? _points = null;
        public Point[] GetPoints()
        {
            if (_points != null)
                return _points;
            var minX = Math.Min(From.X, To.X);
            var maxX = Math.Max(From.X, To.X);
            var minY = Math.Min(From.Y, To.Y);
            var maxY = Math.Max(From.Y, To.Y);
            var rangeX = maxX - minX + 1;
            var rangeY = maxY - minY + 1;
            var range = Math.Max(rangeX, rangeY);
            var iX = 0;
            if (rangeX == range)
            {
                if (From.X > To.X)
                    iX = -1;
                else
                    iX = 1;
            }
            var iY = 0;
            if (rangeY == range)
            {
                if (From.Y > To.Y)
                    iY = -1;
                else
                    iY = 1;
            }
            _points = new Point[range];
            for (var i = 0; i < range; i++)
                _points[i] = new Point(From.X + i * iX, From.Y + i * iY);
            return _points;
        }
        public int Intercepts(Pair5 other)
		{
            return GetPoints().Intersect(other.GetPoints()).Count();
		}

        internal bool Straight()
        {
            return (From.Y == To.Y || From.X == To.X);
        }

        internal void Mark(GridMapXY grid)
        {
            foreach (var pt in GetPoints())
            {
                var c = grid.Get(pt);
                if (c == '.')
                    grid.Set(pt, '#');
                else if (c == '#')
                    grid.Set(pt, '*');
            }
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 12L);
		else
			res.Check = new StarCheck(key, 23864L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var pairs = new List<Pair5>();
        foreach (var line in lines)
        {
            var pair = new Pair5(line);
            pairs.Add(pair);
        }
        var grid = new GridMapXY((int)pairs.Max(p => Math.Max(p.From.X, p.To.X)) + 1, (int)pairs.Max(p => Math.Max(p.From.Y, p.To.Y)) + 1);
        for (var i = 0; i < pairs.Count(); i++)
            pairs[i].Mark(grid);

        if (key.IsReal == false)
            ElfHelper.DayLogPlus(grid);
        rv = grid.Count('*');

        res.CheckGuess(rv);
        return res;
	}
}

