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
        foreach (var line in lines)
		{
			if (line.StartsWith("--") || string.IsNullOrWhiteSpace(line))
			{
				if (beacons.Count() > 0)
					scanners.Add(new Region19(beacons));
				beacons = new List<Point3D>();			
			}
			else
				beacons.Add(Point3D.Parse(line));
		}
        if (beacons.Count() > 0)
            scanners.Add(new Region19(beacons));

        var main = new MasterRegion19();
        main.Add(scanners[0]);
        for(int i = 1;  i < scanners.Count(); i++)
        {
            main = main.Add(scanners[i]);
        }


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
        List<Point3D> _beacons = [];
        List<Point3D> _scannerPts = [];
        List<Square19> _squares = [];
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
                rv.AddRange(region.Beacons);
            return rv;
        }
        internal Region19? Merge(Region19 other)
        {
            var allBeacons = AllBeacons();
            for (var i = 0; i < allBeacons.Count(); i++)
            {
                for (var j = 0; j < other.Beacons.Count(); j++)
                {
                    var offset = other.Beacons[j].Offset(allBeacons[i]);
                    var offsetted = other.Offset(offset);
                    if (Matches(offsetted))
                        return Add(offsetted);
                }
            }
            return null;
        }
        public MasterRegion19(List<Point3D> beacons)
        {
            // normalize
            var square = new Square19(beacons);

            var zeroPt = new Point3D(0, 0, 0);
            square.Include(zeroPt);
            var newBeacons = new List<Point3D>();
            foreach (var beacon in beacons)
                newBeacons.Add(beacon.Subtract(square.Min));

            _beacons = newBeacons.OrderBy(b => b.Distance(new Point3D(0, 0, 0))).ToList();
            var scannerPt = zeroPt.Subtract(square.Min);
            _scannerPts.Add(scannerPt);
            var nSquare = new Square19(beacons);
            nSquare.Include(scannerPt);
            _maps.Add(nSquare);
        }
        /*
        public MasterRegion19(Region19 other)
        {
            _beacons = other._beacons.ToList();
            _scannerPts = other._scannerPts.ToList();
        }
        */
    }
    class Region19s
    {
        List<Region19> _regions = [];
    }
    class Region19
    {
        public List<Point3D> Beacons { get; } = [];
        public Point3D Scanner { get; }
        public Square19 Square { get; }
        public Region19(List<Point3D> beacons)
        {
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




        internal Region19? Merge(Region19 other)
        {
            for (var i = 0; i < Beacons.Count(); i++)
            {
                for (var j = 0; j < other.Beacons.Count(); j++)
                {
                    var offset = other.Beacons[j].Offset(Beacons[i]);
                    var offsetted = other.Offset(offset);
                    if (Matches(offsetted))
                        return Add(offsetted);
                }
            }
            return null;

        }
        bool WithinMapped(Point3D pt)
        {
            foreach(var map in _maps)
            {
                if (map.Contains(pt))
                    return true;
            }
            return false;
        }
        private bool Matches(Region19 other)
        {
            foreach (var beacon in other.Beacons)
            {
                if (WithinMapped(beacon) && !Beacons.Contains(beacon))
                    return false;
            }
            return true;
        }

        private Region19? Add(Region19 other)
        {
            foreach (var beacon in other.Beacons)
            {
                Beacons.Add(beacon);
            }
            throw new NotImplementedException();
        }

        private Region19 Offset(Point3D offset)
        {
            var beacons = new List<Point3D>();
            foreach (var beacon in Beacons)
                beacons.Add(beacon.Add(offset));
            var rv  = new Region19(beacons);
            
            var scanners = new List<Point3D>();
            foreach (var scanner in _scannerPts)
                scanners.Add(scanner.Add(offset));
            rv._scannerPts.AddRange(scanners);

            return rv;
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

