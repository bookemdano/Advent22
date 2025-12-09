using AoCLibrary;

namespace Advent25;

internal class Day08 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/8
	// Input https://adventofcode.com/2025/day/8/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();

        if (!isReal)
            res.Check = new StarCheck(key, 40L);
		else
            res.Check = new StarCheck(key, 112230L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        var maxConnects = 10;
        if (isReal)
            maxConnects = 1000;

        var pts = lines.Select(l => Point3D.FromXYZ(l)).ToList();
        var pairs = new List<Pair3D>();
        var circuits = new CircuitList();
        for (int i = 0; i < pts.Count; i++)
        {
            circuits.Add(pts[i]);
            for (int j = i + 1; j < pts.Count; j++)
                pairs.Add(new Pair3D(pts[i], pts[j]));
        }

        pairs = pairs.OrderBy(d => d.Distance).Take(maxConnects).ToList();
        foreach(var pair in pairs)
            circuits.Link(pair);

        //CombineCircuits(circuits);
        rv = circuits.TopThreeSum();

        //1440 too low
        res.CheckGuess(rv);
        return res;
    }
    struct Pair3D
    {
        public Pair3D(Point3D p1, Point3D p2)
        {
            P1 = p1;
            P2 = p2;
            Distance = p1.Distance(p2);
        }
        public override readonly string ToString()
        {
            return $"{P1} <-> {P2} = {Distance}";
        }
        public Point3D P1;
        public Point3D P2;
        public double Distance;
    }
    class CircuitList
    {
        readonly List<Circuit> _circuits = [];

        public int Count()
        {
            return _circuits.Count;
        }

        public CircuitList()
        {
        }

        internal void Add(Point3D pt)
        {
            _circuits.Add(new Circuit(pt));            
        }

        internal void Link(Pair3D pair)
        {
            if (_circuits.Any(c => c.Contains(pair.P1) && c.Contains(pair.P2)))
                return; // already linked
            var cir1 = _circuits.SingleOrDefault(c => c.Contains(pair.P1));
            var cir2 = _circuits.SingleOrDefault(c => c.Contains(pair.P2));
            Utils.Assert(cir1 != null && cir2 != null, "circuits");
            cir1!.Combine(cir2!);
            _circuits.Remove(cir2!);
        }

        internal long TopThreeSum()
        {
            var tops = _circuits.OrderByDescending(c => c.Points.Count).Take(3).ToList();
            return tops[0].Points.Count * tops[1].Points.Count * tops[2].Points.Count;
        }
    }

    class Circuit
    {
        public List<Point3D> Points = [];

        public Circuit(Point3D point3D)
        {
            Points.Add(point3D);
        }

        internal bool Contains(Point3D pt)
        {
            return Points.Contains(pt);
        }

        internal void Combine(Circuit other)
        {
            Points.AddRange(other.Points.Where(p => !Points.Contains(p)));
        }
        public override string ToString()
        {
            return $"{Points.Count}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 25272L);
        else
            res.Check = new StarCheck(key, 2573952864L);

        var lines = Program.GetLines(key);
        //var text = Program.GetText(key);
        var rv = 0L;
        // magic
        var pts = lines.Select(l => Point3D.FromXYZ(l)).ToList();
        var pairs = new List<Pair3D>();
        var circuits = new CircuitList();
        for (int i = 0; i < pts.Count; i++)
        {
            circuits.Add(pts[i]);
            for (int j = i + 1; j < pts.Count; j++)
                pairs.Add(new Pair3D(pts[i], pts[j]));
        }

        pairs = pairs.OrderBy(d => d.Distance).ToList();
        foreach(var pair in pairs)
        {
            circuits.Link(pair);
            if (circuits.Count() == 1)
            {
                rv = pair.P1.X * pair.P2.X;
                break;
            }
        }

        res.CheckGuess(rv);
        return res;
    }
}

