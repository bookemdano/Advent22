using AoCLibrary;

namespace Advent21;

internal class Day17 : IRunner
{
	// Day https://adventofcode.com/2021/day/17
	// Input https://adventofcode.com/2021/day/17/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 45L);
		else
			res.Check = new StarCheck(key, 10296L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        //target area: x=20..30, y=-10..-5
        var range = new Range17(lines[0]);
        var start = new Loc(0, 0);
        var vel = new Loc(0, 0);
        var maxY = 0;

        for (int vx = 2; vx < range.MaxX; vx++)
        {
            int vy = 2;
            while(vy < 1000)
            {
                var pt = new Point(0, 0);
                var vector = new Vector17(new Point17(0,0), new Point17(vx, vy));

                while(true)
                {
                    vector.Step();
                    if (range.Contains(vector.Pos))
                    {
                        if (vector.MaxY > maxY)
                            maxY = vector.MaxY;
                        break;
                    }
                    if (range.Missed(vector))
                        break;
                }
                vy++;
            }

        }

        rv = maxY;
        // too low 4950
        res.CheckGuess(rv);
        return res;
    }
    class Vector17
    {
        public Point17 Pos { get; }
        public Point17 Vel { get; }
        public int MaxY { get; private set; }
        public Vector17(Point17 pos, Point17 vel)
        {
            Pos = pos;
            Vel = vel;  
        }

        internal void Step()
        {
            Pos.X = Pos.X + Vel.X;
            Pos.Y = Pos.Y + Vel.Y;
            if (Pos.Y > MaxY)
                MaxY = Pos.Y;
            if (Vel.X > 0)
                Vel.X--;
            else if (Vel.X < 0)
                Vel.X++;
            Vel.Y--;
        }
        public override string ToString()
        {
            return $"p:{Pos} v:{Vel}";
        }
    }
    class Point17
    {
        public Point17(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X { get; set;  }
        public int Y { get; set; }
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }

    class Range17
    {
        private List<int> _xrange;
        private List<int> _yrange;

        public Range17(string line)
        {
            var parts = line.Split(":, =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            _xrange = parts[3].Split("..").Select(p => int.Parse(p)).ToList();
            _yrange = parts[5].Split("..").Select(p => int.Parse(p)).ToList();
        }
        public int MaxX => _xrange[1];
        public bool Contains(Point17 pt)
        {
            return (pt.X >= _xrange[0] && pt.X <= _xrange[1] && pt.Y >= _yrange[0] && pt.Y <= _yrange[1]);
        }
        public bool Missed(Vector17 vector)
        {
            if (_yrange[0] > vector.Pos.Y)
                return true;
            if (vector.Vel.X == 0 && (vector.Pos.X < _xrange[0] || vector.Pos.X > _xrange[1]))
                return true;
            return false;
        }
        public override string ToString()
        {
            return $"({_xrange[0]}..{_xrange[1]},{_yrange[0]}..{_yrange[1]}";
        }

    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 112L);
		else
			res.Check = new StarCheck(key, 2371L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var range = new Range17(lines[0]);
        var start = new Loc(0, 0);
        var vel = new Loc(0, 0);

        var vs = new List<Point17>();
        for (int vx = 2; vx <= range.MaxX; vx++)
        {
            int vy = -1000;
            while (vy < 1000)
            {
                var pt = new Point(0, 0);
                var vector = new Vector17(new Point17(0, 0), new Point17(vx, vy));

                while (true)
                {
                    vector.Step();
                    if (range.Contains(vector.Pos))
                    {
                        vs.Add(new Point17(vx, vy));
                        rv++;
                        break;
                    }
                    if (range.Missed(vector))
                        break;
                }
                vy++;
            }

        }
        File.WriteAllLines("c:\\temp\\vs.csv", vs.Select(v => v.ToString()));
        // 1051 too low
        res.CheckGuess(rv);
        return res;
	}
}

