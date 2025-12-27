using AoCLibrary;

namespace Advent20;

internal class Day17 : IRunner
{
	// Day https://adventofcode.com/2020/day/17
	// Input https://adventofcode.com/2020/day/17/input
	public RunnerResult Star1(bool isReal)
	{
		var key = new StarCheckKey(StarEnum.Star1, isReal, null);
		var res = new RunnerResult();
		if (!isReal)
			res.Check = new StarCheck(key, 112L);
		else
			res.Check = new StarCheck(key, 426L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		//var cubes = lines.Select(l => Point3D.Parse(l));
		var actives = new Sky17Star1(lines);

		//var others = new Point3D(0, 0, 0).Get26Neighbors();
		for (int i = 0; i < 6; i++)
		{
			var newActives = new List<Point3D>();
            var min = actives.Min();
            var max = actives.Max();
			for (var x = min.X - 1; x <= max.X + 1; x++)
			{
				for (var y = min.Y - 1; y <= max.Y + 1; y++)
				{
					for (var z = min.Z - 1; z <= max.Z + 1; z++)
					{
                        var pt = new Point3D(x, y, z);
                        var count = actives.CountActiveNeighbors(pt);
                        var active = actives.IsActive(pt);
                        if (active && count != 2 && count != 3)
                            active = false;
                        if (!active && count == 3)
                            active = true;

                        if (active)
                            newActives.Add(pt);
                    }
                }
			}
            actives = new Sky17Star1(newActives);

		}
        rv = actives.CountActives();

		res.CheckGuess(rv);
		return res;
	}
	class Sky17Star1
	{
		List<Point3D> _actives = [];

        public Sky17Star1(IEnumerable<string> lines)
        {
			int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    if (c == '#')
                        _actives.Add(new(x, y, 1));
                    x++;
                }
                y++;
            }
        }

        public Sky17Star1(List<Point3D> newActives)
        {
            _actives = newActives;
        }
        public override string ToString()
        {
            return $"{Min}-{Max}";
        }
        public Point3D Min()
        {
            return new Point3D(_actives.Min(p => p.X), _actives.Min(p => p.Y), _actives.Min(p => p.Z));
        }
        public Point3D Max()
        {
            return new Point3D(_actives.Max(p => p.X), _actives.Max(p => p.Y), _actives.Max(p => p.Z));
        }

        internal int CountActiveNeighbors(Point3D active)
        {
            var ns = active.GetNeighbors();
			return ns.Count(a => IsActive(a));
        }

        internal bool IsActive(Point3D pt)
        {
            return _actives.Contains(pt);
        }

        internal int CountActives()
        {
            return _actives.Count();
        }
    }
    class Sky17Star2
    {
        List<Point4D> _actives = [];

        public Sky17Star2(IEnumerable<string> lines)
        {
            int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    if (c == '#')
                        _actives.Add(new(x, y, 1, 1));
                    x++;
                }
                y++;
            }
        }

        public Sky17Star2(List<Point4D> newActives)
        {
            _actives = newActives;
        }
        public override string ToString()
        {
            return $"{Min}-{Max}";
        }
        public Point4D Min()
        {
            return new Point4D(_actives.Min(p => p.X), _actives.Min(p => p.Y), _actives.Min(p => p.Z), _actives.Min(p => p.W));
        }
        public Point4D Max()
        {
            return new Point4D(_actives.Max(p => p.X), _actives.Max(p => p.Y), _actives.Max(p => p.Z), _actives.Max(p => p.W));
        }

        internal int CountActiveNeighbors(Point4D active)
        {
            var ns = active.GetNeighbors();
            return ns.Count(a => IsActive(a));
        }

        internal bool IsActive(Point4D pt)
        {
            return _actives.Contains(pt);
        }

        internal int CountActives()
        {
            return _actives.Count();
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 848L);
		else
			res.Check = new StarCheck(key, 1892L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var actives = new Sky17Star2(lines);

        for (int i = 0; i < 6; i++)
        {
            var newActives = new List<Point4D>();
            var min = actives.Min();
            var max = actives.Max();
            for (var x = min.X - 1; x <= max.X + 1; x++)
            {
                for (var y = min.Y - 1; y <= max.Y + 1; y++)
                {
                    for (var z = min.Z - 1; z <= max.Z + 1; z++)
                    {
                        for (var w = min.W - 1; w <= max.W + 1; w++)
                        {
                            var pt = new Point4D(x, y, z, w);
                            var count = actives.CountActiveNeighbors(pt);
                            var active = actives.IsActive(pt);
                            if (active && count != 2 && count != 3)
                                active = false;
                            if (!active && count == 3)
                                active = true;

                            if (active)
                                newActives.Add(pt);
                        }
                    }
                }
            }
            actives = new Sky17Star2(newActives);

        }
        rv = actives.CountActives();

        res.CheckGuess(rv);
        return res;
	}
}

