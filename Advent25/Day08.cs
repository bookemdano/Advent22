using AoCLibrary;

namespace Advent25;

internal class Day08 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2025/day/8
	// Input https://adventofcode.com/2025/day/8/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        StarCheck check;
        if (!isReal)
			check = new StarCheck(key, 40L);
		else
			check = new StarCheck(key, 112230L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
        var pts = new List<Point3D>();
        foreach(var line in lines)
        {
            var pt = Point3D.FromXYZ(line);
            pts.Add(pt);
        }
        var dists = new List<DistanceSet>(); 
        for (int i = 0; i < pts.Count; i++)
        {
            for(int j = i + 1; j < pts.Count; j++)
            {
                dists.Add(new DistanceSet(pts[i], pts[j]));
            }
        }
        dists = dists.OrderBy(d => d.Distance).ToList();
        var circuits = new List<List<Point3D>>();
        var maxConnects = 10;
        if (isReal)
            maxConnects = 1000;
        for(int i = 0; i < maxConnects; i++)
        {
            var dist = dists[i];

            if (!circuits.Any(c => c.Contains(dist.P1) || c.Contains(dist.P2)))
            {
                var circuit = new List<Point3D>();
                circuit.Add(dists[i].P1);
                circuit.Add(dists[i].P2);
                circuits.Add(circuit);
            }
            if (circuits.Any(c => c.Contains(dist.P1) && c.Contains(dist.P2)))
                continue;
            var cir = circuits.FirstOrDefault(c => c.Contains(dist.P1));
            if (cir != null)
            {
                if (!cir.Contains(dist.P2))
                    cir?.Add(dist.P2);
            }
            cir = circuits.FirstOrDefault(c => c.Contains(dist.P2));
            if (cir != null)
            {
                if (!cir.Contains(dist.P1))
                    cir?.Add(dist.P1);
            }
        }
        var changed = true;
        while(changed)
        {
            changed = false;
            for(int i = 0; i < circuits.Count; i++)
            {
                for(int j = i + 1; j < circuits.Count; j++)
                {
                    var cir1 = circuits[i];
                    var cir2 = circuits[j];
                    if (cir1.Intersect(cir2).Any())
                    {
                        cir1.AddRange(cir2.Where(p => !cir1.Contains(p)));
                        circuits.RemoveAt(j);
                        changed = true;
                        break;
                    }
                }
                if (changed)
                    break;
            }
        }
        circuits = circuits.OrderByDescending(c => c.Count).Take(3).ToList();
        rv = circuits[0].Count * circuits[1].Count * circuits[2].Count;
        var res = new RunnerResult();
        //1440 too low
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
    }
    class DistanceSet
    {
        public DistanceSet(Point3D p1, Point3D p2)
        {
            P1 = p1;
            P2 = p2;
            Distance = p1.Distance(p2);
        }
        override public string ToString()
        {
            return $"{P1} <-> {P2} = {Distance}";
        }
        public Point3D P1;
        public Point3D P2;
        public double Distance;
    }
    class Circuit
    {
        public List<Point3D> Points = new List<Point3D>();

        internal void Add(Point3D pt)
        {
            Points.Add(pt);
        }
        internal bool Contains(Point3D pt)
        {
            return Points.Contains(pt);
        }
        internal IEnumerable<Point3D> Intersect(Circuit other)
        {
            return Points.Intersect(other.Points);
        }

        internal void AddIntersect(Circuit other)
        {
            Points.AddRange(other.Points.Where(p => !Points.Contains(p)));
        }
        public override string ToString()
        {
            return $"{Points.Count()}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        StarCheck check;
        if (!isReal)
            check = new StarCheck(key, 25272L);
        else
            check = new StarCheck(key, 0L);

        var lines = Program.GetLines(check.Key);
        //var text = Program.GetText(check.Key);
        var rv = 0L;
        // magic
        var pts = new List<Point3D>();
        foreach (var line in lines)
        {
            var pt = Point3D.FromXYZ(line);
            pts.Add(pt);
        }
        var dists = new List<DistanceSet>();
        var circuits = new List<Circuit>();
        for (int i = 0; i < pts.Count; i++)
        {
            var circuit = new Circuit();
            circuit.Add(pts[i]);
            circuits.Add(circuit);
            for (int j = i + 1; j < pts.Count; j++)
            {
                dists.Add(new DistanceSet(pts[i], pts[j]));
            }
        }
        dists = dists.OrderBy(d => d.Distance).ToList();
        var maxConnects = 10;
        if (isReal)
            maxConnects = 1000;
        for (int i = 0; i < maxConnects; i++)
        {
            var dist = dists[i];

            if (!circuits.Any(c => c.Contains(dist.P1) || c.Contains(dist.P2)))
            {
                var circuit = new Circuit();
                circuit.Add(dist.P1);
                circuit.Add(dist.P2);
                circuits.Add(circuit);
            }
            if (circuits.Any(c => c.Contains(dist.P1) && c.Contains(dist.P2)))
                continue;
            var cir = circuits.FirstOrDefault(c => c.Contains(dist.P1));
            if (cir != null)
            {
                if (!cir.Contains(dist.P2))
                    cir?.Add(dist.P2);
            }
            cir = circuits.FirstOrDefault(c => c.Contains(dist.P2));
            if (cir != null)
            {
                if (!cir.Contains(dist.P1))
                    cir?.Add(dist.P1);
            }
        }
        CombineCircuits(circuits);
        long iDist = 0;
        foreach (var dist in dists)
        {
            iDist++;
            var cir = circuits.FirstOrDefault(c => c.Contains(dist.P1) && c.Contains(dist.P2));
            if (cir == null)
            {
                //if (!circuits.All(c => c.Contains(dist.P1) || c.Contains(dist.P2)))
                //    continue;
                var copyCircuits = new List<Circuit>();
                foreach(var iCir in circuits)
                {
                    var newCir = new Circuit();
                    newCir.Points.AddRange(iCir.Points.ToList());
                    copyCircuits.Add(newCir);
                }

                var circuit = new Circuit();
                circuit.Add(dist.P1);
                circuit.Add(dist.P2);
                copyCircuits.Add(circuit);
                CombineCircuits(copyCircuits);
                if (copyCircuits.Count == 1)
                {
                    rv = dist.P1.X * dist.P2.X;
                    break;
                }
            }
        }

        var res = new RunnerResult();
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
    }
    
    private static void CombineCircuits(List<Circuit> circuits)
    {
        var changed = true;
        while (changed)
        {
            changed = false;
            for (int i = 0; i < circuits.Count; i++)
            {
                for (int j = i + 1; j < circuits.Count; j++)
                {
                    var cir1 = circuits[i];
                    var cir2 = circuits[j];
                    if (cir1.Intersect(cir2).Any())
                    {
                        cir1.AddIntersect(cir2);
                        circuits.RemoveAt(j);
                        changed = true;
                        break;
                    }
                }
                if (changed)
                    break;
            }
        }
    }
}

