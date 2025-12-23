using AoCLibrary;

namespace Advent21;

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
			res.Check = new StarCheck(key, 313L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        var pt = new Point3D(5, 6, -4);
        var rots = Rotation3D.AllRotations();
        foreach(var rot in rots)
        {
            ElfHelper.DayLogPlus(rot.Apply(pt));
        }


        var regions = Region19.ReadAll(lines);

        var main = new MasterRegion19();
        main.Add(regions[0]);
        regions.RemoveAt(0);

        main.MergeAll(regions, key.IsReal);
        var allBeacons = main.AllBeacons();
        rv = allBeacons.Count();

        res.CheckGuess(rv);
        return res;
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
        internal List<Point3D> AllScanners()
        {
            return _regions.Select(r => r.Scanner).ToList();
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

        bool Merge(Region19 other, bool isReal)
        {
            var allBeacons = AllBeacons();
            Point3D? savedOffset = null;
            var str = Utils.ReadConfig($"Y21D19-{other.BeaconNumber}-{isReal}", "none");
            if (str != "none")
                savedOffset = Point3D.Parse(str);

            foreach (var rot in Rotation3D.AllRotations())
            {
                var otherBeacons = other.Beacons.Select(s => rot.Apply(s)).ToList();
                var rotated = new Region19(otherBeacons, 0);
                var max = 0;
                for (var i = 0; i < allBeacons.Count(); i++)
                {
                    for (var j = 0; j < rotated.Beacons.Count(); j++)
                    {
                        var offset = rotated.Beacons[j].Offset(allBeacons[i]);
                        if (savedOffset != null)
                            offset = savedOffset;
                        var offsetted = new Region19(rotated, offset);
                        var overlap = offsetted.Beacons.Where(b => allBeacons.Contains(b)).ToList();
                        var count = offsetted.Beacons.Count(b => allBeacons.Contains(b));
                        if (count >= 12)
                        {
                            Utils.WriteConfig($"Y21D19-{other.BeaconNumber}-{isReal}", offset.ToString());
                            ElfHelper.DayLogPlus($"Merged {other} o:{offset}");
                            Add(offsetted);
                            return true;
                        }
                        if (count > max)
                            max = count;
                        if (savedOffset != null)
                            break;
                    }
                    if (savedOffset != null)
                        break;
                }

            }
            return false;
        }

        internal void MergeAll(List<Region19> regions, bool isReal)
        {
            while (regions.Any())
            {
                ElfHelper.DayLogPlus("Checking " + regions.Count() + " more scanners");
                var removes = new List<Region19>();
                for (int i = 0; i < regions.Count(); i++)
                {
                    if (Merge(regions[i], isReal))
                        removes.Add(regions[i]);
                }
                if (!removes.Any())
                    break;
                foreach (var remove in removes)
                    regions.Remove(remove);
            }
        }
    }

    class Region19
    {
        public List<Point3D> Beacons { get; } = [];
        public Point3D Scanner { get; }
        public int BeaconNumber { get; }
        public Region19(List<Point3D> beacons, int iBeacon)
        {
            BeaconNumber = iBeacon;
            // normalize
            Beacons = beacons;
            Scanner = new Point3D(0, 0, 0);
        }
        public override string ToString()
        {
            return BeaconNumber.ToString();
        }

        public Region19(Region19 other, Point3D offset)
        {
            // normalize
            foreach (var beacon in other.Beacons)
                Beacons.Add(beacon.Add(offset));
            Scanner = other.Scanner.Add(offset);
            BeaconNumber = other.BeaconNumber;
        }

        static public List<Region19> ReadAll(IEnumerable<string> lines)
        {
            var beacons = new List<Point3D>();
            var regions = new List<Region19>();
            int iBeacon = 0;
            foreach (var line in lines)
            {
                if (line.StartsWith("--"))
                    iBeacon = int.Parse(line.Substring(12, 2));

                if (line.StartsWith("--") || string.IsNullOrWhiteSpace(line))
                {
                    if (beacons.Count() > 0)
                        regions.Add(new Region19(beacons, iBeacon));
                    beacons = new List<Point3D>();
                }
                else
                    beacons.Add(Point3D.Parse(line));
            }
            if (beacons.Count() > 0)
                regions.Add(new Region19(beacons, iBeacon++));
            return regions;
        }
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 3621L);
		else
			res.Check = new StarCheck(key, 10656L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var regions = Region19.ReadAll(lines);

        var main = new MasterRegion19();
        main.Add(regions[0]);
        regions.RemoveAt(0);

        main.MergeAll(regions, key.IsReal);
            

        var allScanners = main.AllScanners();
        var max = 0L;
        for(int i = 0; i < allScanners.Count-1; i++)
            for(int j = i; j < allScanners.Count; j++)
            {
                var dist = allScanners[i].ManhattanDistance(allScanners[j]);
                if (dist > max)
                {
                    ElfHelper.DayLogPlus("New Max " + dist);
                    max = dist;
                }
            }    

        rv = max;

        res.CheckGuess(rv);
        return res;
	}
}

