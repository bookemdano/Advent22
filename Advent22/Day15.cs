using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static Advent22.Days.Day14;
using static System.Net.Mime.MediaTypeNames;

namespace Advent22
{
    internal class Day15
    {
        public class Pair
        {
            public Pair(string line)
            {
                var parts = line.Split("=, :".ToCharArray());
                Sensor = new BasePoint(int.Parse(parts[3]), int.Parse(parts[6]));
                Beacon = new BasePoint(int.Parse(parts[13]), int.Parse(parts[16]));
            }

            public BasePoint Sensor { get; }
            public BasePoint Beacon { get; }
            public override string ToString()
            {
                return $"S:{Sensor} B:{Beacon}";
            }

            public int MinX()
            {
                return Math.Min(Sensor.X, Beacon.X);
            }
            public int MinY()
            {
                return Math.Min(Sensor.Y, Beacon.Y);
            }
            public int MaxX()
            {
                return Math.Max(Sensor.X, Beacon.X);
            }
            public int MaxY()
            {
                return Math.Max(Sensor.Y, Beacon.Y);
            }
            public static int Distance(int x1, int y1, int x2, int y2)
            {
                return Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
            }
            public int Distance()
            {
                return Math.Abs(Sensor.X - Beacon.X) + Math.Abs(Sensor.Y - Beacon.Y);
            }
        }
        public class Surface
        {
            private int _targetY;

            public Surface(string[] lines, int targetY)
            {
                Pairs = new List<Pair>();
                foreach (var line in lines)
                    Pairs.Add(new Pair(line));
                _targetY = targetY;
            }

            public void Draw()
            {
                Helper.Log($"Pairs: {Pairs.Count()}");
                var minX = Pairs.Min(p => p.MinX());
                var minY = Pairs.Min(p => p.MinY());
                var maxX = Pairs.Max(p => p.MaxX());
                var maxY = Pairs.Max(p => p.MaxY());
                if ((maxX - minX) + (maxY - minY) > 1000)
                    return;
                Helper.Log("   " + minX);
                for (int y = minY; y <= maxY; y++)
                {
                    var line = y.ToString("000");
                    for (int x = minX; x <= maxX; x++)
                    {
                        var pt = new BasePoint(x, y);
                        if (SensorAt(pt))
                            line += "S";
                        else if (BeaconAt(pt))
                            line += "B";
                        else if (Impossibles.Any(p => p.X == x && p.Y == y))
                            line += "#";
                        else if (RangesContains(pt))
                            line += "#";
                        else
                            line += ".";
                    }
                    Helper.Log(line);
                }
            }
            bool RangesContains(BasePoint pt)
            {
                if (ImpossibleRanges?.ContainsKey(pt.Y) != true)
                    return false;

                return ImpossibleRanges[pt.Y].Any(r => r.Contains(pt.X));
            }
            public BasePoint GetEmptySlow()
            {
                int min = 0;
                int max = _targetY;
                for (int y = min; y <= max; y++)
                {
                    for (int x = min; x <= max; x++)
                    {
                        var pt = new BasePoint(x, y);
                        if (SensorAt(pt))
                            continue;
                        else if (BeaconAt(pt))
                            continue;
                        else if (Impossibles.Any(p => p.X == x && p.Y == y))
                            continue;
                        else
                            return pt;
                    }
                }
                return null;
            }
            public BasePoint GetEmpty()
            {
                foreach (var kvp in ImpossibleRanges)
                {
                    var ranges = kvp.Value;
                    var orderedRanges = ranges.OrderBy(r => r.Start).ToArray();
                    var firstFree = 0;
                    foreach (var r in orderedRanges)
                    {

                        if (r.Start > firstFree)
                            return new BasePoint(firstFree, kvp.Key);
                        firstFree = Math.Max(r.End + 1, firstFree); // in case a bubble is inside a larger bubble
                        if (firstFree >= _targetY)
                            break;
                    }
                }
                return null;
            }
            public List<Pair> Pairs { get; set; }
            public List<BasePoint> Impossibles { get; set; } = new List<BasePoint>();
            public Dictionary<int, List<Range>> ImpossibleRanges { get; private set; }

            bool SensorAt(BasePoint pt)
            {
                return Pairs.Any(p => p.Sensor.X == pt.X && p.Sensor.Y == pt.Y);
            }
            bool BeaconAt(BasePoint pt)
            {
                return Pairs.Any(p => p.Beacon.X == pt.X && p.Beacon.Y == pt.Y);
            }
            public void MarkImpossibles1()
            {
                var impossibles = new List<BasePoint>();
                foreach(var pair in Pairs)
                {
                    var dist = pair.Distance();
                    for (int y = 0 - dist; y <= dist; y++)
                    {
                        // we only care about impossibles on this row
                        if (pair.Sensor.Y + y != _targetY)
                            continue;
                        for (int x = 0 - dist; x <= dist; x++)
                        {
                            if (Math.Abs(x) + Math.Abs(y) <= dist)
                            {
                                var pt = new BasePoint(pair.Sensor.X + x, pair.Sensor.Y + y);
                                impossibles.Add(pt);
                            }
                        }
                    }
                }    
                Impossibles = impossibles.DistinctBy(i => i.ToString()).ToList();
            }
            public void MarkImpossibles2()
            {
                ImpossibleRanges = new Dictionary<int, List<Range>>();
                foreach (var pair in Pairs)
                {
                    var dist = pair.Distance();
                    for (int y = 0 - dist; y <= dist; y++)
                    {
                        
                        var absY = pair.Sensor.Y + y;
                        if (absY > _targetY || absY < 0)
                            continue;
                        if (!ImpossibleRanges.ContainsKey(absY))
                            ImpossibleRanges[absY] = new List<Range>();
                        // for the impossible, just save the center and radius for that row
                        var offset = dist - Math.Abs(y);
                        var range = new Range(pair.Sensor.X, offset + 1);
                        ImpossibleRanges[absY].Add(range);
                    }
                    //Draw();
                }
                //Draw();
                //Impossibles = impossibles.DistinctBy(i => i.ToString()).ToList();
            }
        }
        internal struct Range
        {
            public Range(int x, int radius)
            {
                X = x;  // Y is stored in key of dictionary
                Radius = radius;
            }
            public bool Contains(int x)
            {
                return (Math.Abs(X - x) <= Radius - 1);
            }
            public int X { get; set; }
            public int Radius { get; set; }
            public override string ToString()
            {
                return $"X:{X} R:{Radius}";
            }
            public int Start
            {
                get
                {
                    return X - (Radius - 1);
                }
            }
            public int End
            {
                get
                {
                    return X + (Radius - 1);
                }
            }

        }
        static public void Run()
        {
            //Day1();
            Day2();
        }
        static void Day1()
        {
            var input = File.ReadAllLines("Day15.txt");
            var target = 2000000;
            var surface = new Surface(input, target);
            surface.Draw();
            surface.MarkImpossibles1();
            surface.Draw();
            var ones = surface.Impossibles.Where(p => p.Y == target).OrderBy(p => p.X).ToArray();
            var beaconsInRow = surface.Pairs.Where(p => p.Beacon.Y == target).DistinctBy(p => p.Beacon.ToString()).Count();// beacons can overlap, P1 failure
            var sensorsInRow = surface.Pairs.Count(p => p.Sensor.Y == target);
            var score = surface.Impossibles.Count(p => p.Y == target) - beaconsInRow - sensorsInRow;
            Helper.Log("Star1 Score: " + score); // not 5508231 too low
        }
        static void Day2()
        {
            var input = File.ReadAllLines("Day15.txt");
            var max = 20;
            if (input.Length > 15)
                max = 4000000;
            var surface = new Surface(input, max);
            surface.Draw();
            surface.MarkImpossibles2();
            surface.Draw();
            var pt = surface.GetEmpty();
            var score = pt.X * 4000000L + pt.Y;

            Helper.Log("Star2 Score: " + score); 
        }
    }
}

