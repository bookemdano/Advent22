using AoCLibrary;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Advent21;
// #working
internal class Day19 : IRunner
{
	// Day https://adventofcode.com/2021/day/19
	// Input https://adventofcode.com/2021/day/19/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 79L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var beacons = new List<Point3D>();
		var scanners = new List<Region19>();
        int iBeacon = 0;
        foreach (var line in lines)
		{
			if (line.StartsWith("--") || string.IsNullOrWhiteSpace(line))
			{
				if (beacons.Count() > 0)
					scanners.Add(new Region19(beacons, iBeacon++));
				beacons = new List<Point3D>();			
			}
			else
				beacons.Add(Point3D.Parse(line));
		}
        if (beacons.Count() > 0)
            scanners.Add(new Region19(beacons, iBeacon++));

        var main = new MasterRegion19();
        main.Add(scanners[0]);
        for(int i = 1;  i < scanners.Count(); i++)
        {
            main.Merge(scanners[i]);
        }
        var allBeacons = main.AllBeacons();
        rv = allBeacons.Count();
        res.CheckGuess(rv);
        return res;
    }
    class Square19
    {
        public Point3D Min;
        public Point3D Max;
        public Square19(List<Point3D> points)
        {
            Min = new Point3D(points.Min(b => b.X), points.Min(b => b.Y), points.Min(b => b.Z));
            Max = new Point3D(points.Max(b => b.X), points.Max(b => b.Y), points.Max(b => b.Z));
        }
        public override string ToString()
        {
            return $"{Min} to {Max}";
        }

        public bool Contains(Point3D pt)
        {
            if (pt.X < Min.X || pt.X > Max.X)
                return false;
            if (pt.Y < Min.Y || pt.Y > Max.Y)
                return false;
            if (pt.Z < Min.Z || pt.Z > Max.Z)
                return false;
            return true;
        }
        internal void Include(Point3D pt)
        {
            if (Min.X > pt.X)
                Min.X = pt.X;
            if (Min.Y > pt.Y)
                Min.Y = pt.Y;
            if (Min.Z > pt.Z)
                Min.Z = pt.Z;

            if (Max.X < pt.X)
                Max.X = pt.X;
            if (Max.Y < pt.Y)
                Max.Y = pt.Y;
            if (Max.Z < pt.Z)
                Max.Z = pt.Z;
        }
    }
    class MasterRegion19
    {
        List<Region19> _regions = [];
        public MasterRegion19()
        {
            
        }
        public void Add(Region19 region)
        {
            _regions.Add(region);
        }
        public List<Point3D> AllBeacons() 
        {
            var rv = new List<Point3D>();
            foreach (var region in _regions)
            {
                foreach(var beacon in region.Beacons)
                {
                    if (!rv.Contains(beacon))
                        rv.Add(beacon);
                }
            }
            return rv;
        }

        internal void Merge(Region19 other)
        {
            var allBeacons = AllBeacons();
            var rotations = other.Rotations();

            for (var i = 0; i < allBeacons.Count(); i++)
            {
                for (var j = 0; j < other.Beacons.Count(); j++)
                {
                    var offset = other.Beacons[j].Offset(allBeacons[i]);
                    var offsetted = new Region19(other, offset);
                    var count = offsetted.CountMatch(allBeacons);
                    if (count >= 12)
                    {
                        Add(offsetted);
                        return;
                    }
                    if (count > 1)
                        continue;
                }
            }
        }
    }

    class Region19
    {
        public List<Point3D> Beacons { get; } = [];
        public Point3D Scanner { get; }
        public Square19 Square { get; }
        string _name;
        public Region19(List<Point3D> beacons, int iBeacon)
        {
            _name = "R:" + iBeacon;
            // normalize
            var square = new Square19(beacons);
            var zeroPt = new Point3D(0, 0, 0);
            square.Include(zeroPt);
            var newBeacons = new List<Point3D>();
            foreach (var beacon in beacons)
                newBeacons.Add(beacon.Subtract(square.Min));

            Beacons = newBeacons.OrderBy(b => b.Distance(new Point3D(0, 0, 0))).ToList();
            Scanner = zeroPt.Subtract(square.Min);
            Square = new Square19(beacons);
            Square.Include(Scanner);
        }
        public override string ToString()
        {
            return _name + " s:" + Scanner;
        }
        internal int CountMatch(List<Point3D> allBeacons)
        {
            var rv = 0;
            foreach (var beacon in Beacons)
                rv += allBeacons.Count(b => b.FuzzyMatch(beacon));
            return rv;
        }
        internal List<Region19> Rotations()
        {
            var rv = new List<Region19>();
            rv.Add(this);
            var beacons = Beacons.Select(b => b.Flip(AxisEnum.X)).ToList();
            var flipX = Beacons.Select(b => b.Flip(AxisEnum.X));
            var flipXY = flipX.Select(b => b.Flip(AxisEnum.Y));
            var flipXZ = flipX.Select(b => b.Flip(AxisEnum.Z));
            var flipXYZ = flipXY.Select(b => b.Flip(AxisEnum.Z));
            var flipY = Beacons.Select(b => b.Flip(AxisEnum.Y));
            var flipYZ = flipY.Select(b => b.Flip(AxisEnum.Z));
            var flipZ = Beacons.Select(b => b.Flip(AxisEnum.Z));
            
            rv.Add(new Region19(flipX.ToList(), Scanner.Flip(AxisEnum.X), _name + " FlipX"));
            rv.Add(new Region19(flipY.ToList(), Scanner.Flip(AxisEnum.Y), _name + " FlipY"));
            rv.Add(new Region19(flipZ.ToList(), Scanner.Flip(AxisEnum.Z), _name + " FlipZ"));
            rv.Add(new Region19(flipXY.ToList(), Scanner.Flip(AxisEnum.X), _name + " FlipXY"));
            rv.Add(new Region19(flipXZ.ToList(), Scanner.Flip(AxisEnum.Y), _name + " FlipXZ"));
            rv.Add(new Region19(flipYZ.ToList(), Scanner.Flip(AxisEnum.Z), _name + " FlipYZ"));
            rv.Add(new Region19(flipXYZ.ToList(), Scanner.Flip(AxisEnum.Z), _name + " FlipXYZ"));

            return rv;
        }

        private Region19(List<Point3D> beacons, Point3D scanner, string name)
        {
            Beacons = beacons;
            Scanner = scanner;
            _name = name;
        }

        public Region19(Region19 other, Point3D offset)
        {
            // normalize
            foreach (var beacon in other.Beacons)
                Beacons.Add(beacon.Add(offset));
            Scanner = other.Scanner.Add(offset);
            _name = other._name + " o:" + offset;
            
            Square = new Square19(Beacons);
            Square.Include(Scanner);
        }

    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

