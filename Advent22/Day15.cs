using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
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
                        else
                            line += ".";
                    }
                    Helper.Log(line);
                }
            }
            public List<Pair> Pairs { get; set; }
            public List<BasePoint> Impossibles { get; set; } = new List<BasePoint>();
            bool SensorAt(BasePoint pt)
            {
                return Pairs.Any(p => p.Sensor.X == pt.X && p.Sensor.Y == pt.Y);
            }
            bool BeaconAt(BasePoint pt)
            {
                return Pairs.Any(p => p.Beacon.X == pt.X && p.Beacon.Y == pt.Y);
            }
            public void MarkImpossibles()
            {
                var impossibles = new List<BasePoint>();
                foreach(var pair in Pairs)
                {
                    var dist = pair.Distance();
                    for (int y = 0 - dist; y <= dist; y++)
                    {
                        if (pair.Sensor.Y + y != _targetY)
                            continue;
                        for (int x = 0 - dist; x <= dist; x++)
                        {
                            if (Math.Abs(x) + Math.Abs(y) <= dist)
                            {
                                var pt = new BasePoint(pair.Sensor.X + x, pair.Sensor.Y + y);
                                //if (!Impossibles.Any(p => p.Same(pt)) && !SensorAt(pt) && !BeaconAt(pt))
                                {
                                    impossibles.Add(pt);
                                }

                            }
                        }
                    }
                }    
                Impossibles = impossibles.DistinctBy(i => i.ToString()).ToList();

            }

        }
        static public void Run()
        {
            var input = File.ReadAllLines("Day15.txt");
            var target = 2000000;
            var surface = new Surface(input, target);
            surface.Draw();
            surface.MarkImpossibles();
            surface.Draw();
            var ones = surface.Impossibles.Where(p => p.Y == target).OrderBy(p => p.X).ToArray();
            var score = surface.Impossibles.Count(p => p.Y == target) - surface.Pairs.Count(p => p.Beacon.Y == target) - surface.Pairs.Count(p => p.Sensor.Y == target);
            Helper.Log("Star1 Score: " + score); 
        }
    }
}

