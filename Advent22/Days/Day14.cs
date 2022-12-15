using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static Advent22.Days.Day14;
using static System.Net.Mime.MediaTypeNames;

namespace Advent22.Days
{
    internal class Day14
    {
        internal class Point : BasePoint
        {
            public Point(int x, int y) : base(x, y)
            {

            }
            public Point(Point pt) : base(pt)
            {

            }
            public Point Fall()
            {
                return new Point(X, Y + 1);
            }
            public Point FallLeft()
            {
                return new Point(X - 1, Y + 1);
            }
            public Point FallRight()
            {
                return new Point(X + 1, Y + 1);
            }
        }
        public class Cave
        {
            int? _floor;
            public Cave(string[] lines, bool useFloor)
            {
                foreach (var line in lines)
                {
                    var coords = line.Split(" -> ");
                    Point lastPt = null;
                    foreach (var coord in coords)
                    {
                        var pt = Point.Parse(coord);
                        if (lastPt != null)
                        {
                            var minX = Math.Min(pt.X, lastPt.X);
                            var maxX = Math.Max(pt.X, lastPt.X);
                            var minY = Math.Min(pt.Y, lastPt.Y);
                            var maxY = Math.Max(pt.Y, lastPt.Y);
                            for (int x = minX; x <= maxX; x++)
                            {
                                for (int y = minY; y <= maxY; y++)
                                {
                                    Rocks.Add(new Point(x, y));
                                }
                            }
                        }
                        lastPt = (Point) pt;
                    }
                }
                if (useFloor)
                    _floor = Rocks.Max(r => r.Y) + 2;
                _rightEdge = Rocks.Max(p => p.X);
                _leftEdge = Rocks.Min(p => p.X);

            }
            //List<Point> rock, List<Point> sands, Point source, Point sand
            public List<Point> Rocks { get; } = new List<Point>();
            public List<Point> Sands { get; } = new List<Point>();
            public Point Source { get; } = new Point(500, 0);
            public Point FallingSand { get; set; }
            int _rightEdge;
            int _leftEdge;
            internal void Draw()
            {
                Helper.Log($"Sands: {Sands.Count()}");
                var minX = Rocks.Min(p => p.X) - 2;
                var maxX = Rocks.Max(p => p.X) + 2;
                var minY = Rocks.Min(p => p.Y) - 1;
                var maxY = Rocks.Max(p => p.Y) + 1;
                minY = Math.Min(minY, 0);
                for (int y = minY; y <= maxY; y++)
                {
                    var line = "";
                    for (int x = minX; x <= maxX; x++)
                    {
                        var foundRock = Rocks.Any(f => f.X == x && f.Y == y);
                        var foundSand = Sands.Any(f => f.X == x && f.Y == y);
                        if (foundRock)
                            line += "#";
                        else if (foundSand)
                            line += "o";
                        else if (Source.Same(x, y))
                            line += "+";
                        else if (FallingSand?.Same(x, y) == true)
                            line += "0";
                        else
                            line += ".";
                    }
                    Helper.Log(line);
                }
                if (_floor != null)
                {
                    var line = "";
                    for (int x = minX; x <= maxX; x++)
                        line += "█";
                    Helper.Log(line);
                }
            }
            static bool AnyAt(List<Point> points, Point pt)
            {
                return points.Any(p => p.Same(pt));
            }
            static int NextY(List<Point> points, Point pt)
            {
                var next = points.Where(r => r.X == pt.X);
                if (!next.Any())
                    return int.MaxValue;
                return next.Min(r => r.Y);
            }
            // return true for end game
            internal bool Tick()
            {
                if (FallingSand == null)
                {
                    FallingSand = new Point(Source);
                    return false;
                }
                /*var nextRock = NextY(Rocks, sand);
                var nextSands = NextY(Sands, sand);
                var next = Math.Min(nextSands, nextRock) - 1;
                if (_floor != null)
                    next = Math.Min(next, _floor.Value) - 1;
                if (next > sand.Y)
                    sand.Y = next;
                */

                if (_floor.HasValue && _floor == FallingSand.Y + 1)
                {
                    Sands.Add(new Point(FallingSand));
                    FallingSand = null;
                    return false;
                }

                var sand = new Point(FallingSand);
                var options = Rocks.Where(r => r.Y == sand.Y + 1 && Math.Abs(r.X - sand.X) <= 1).ToList();
                options.AddRange(Sands.Where(r => r.Y == sand.Y + 1 && Math.Abs(r.X - sand.X) <= 1));
                var nextSand = sand.Fall();
                if (options.Any(o => o.Same(nextSand))) // collision                
                {
                    nextSand = sand.FallLeft();
                    if (options.Any(o => o.Same(nextSand)) || FallingSand.X < _leftEdge)  // collision                
                    {
                        nextSand = sand.FallRight();
                        if (options.Any(o => o.Same(nextSand)) || FallingSand.X > _rightEdge)  // collision                
                        {
                            Sands.Add(new Point(sand));
                            //Draw();
                            nextSand = null;
                            if (sand.Same(Source))
                                return true;
                        }
                    }
                }
                if (nextSand != null && Abyss(nextSand))
                    return true;
                FallingSand = nextSand;
                return false;
            }
            internal bool Collision(Point pt)
            {
                if (_floor != null && pt.Y >= _floor)
                    return true;
                return Sands.Any(s => s.Same(pt)) || Rocks.Any(s => s.Same(pt));
            }

            internal bool Abyss(Point pt)
            {
                if (_floor != null)
                    return false;
                var rv = Rocks.Where(r => r.X == pt.X && r.Y > pt.Y);
                return !Rocks.Any(r => r.X == pt.X && r.Y > pt.Y);
            }


            internal int LeftTopSand()
            {
                var leftSand = Sands.Min(s => s.X);
                for (int y = _floor.Value - 1; y >= 0; y--)
                {
                    if (!AnyAt(Sands, new Point(leftSand, y)))
                        return _floor.Value - y - 1;
                }
                return -1;
            }
            internal int RightTopSand()
            {
                var rightSand = Sands.Max(s => s.X);
                for (int y = _floor.Value - 1; y >= 0; y--)
                {
                    if (!AnyAt(Sands, new Point(rightSand, y)))
                        return _floor.Value - y - 1;
                }
                return -1;
            }
        }

        static public void Run()
        {
            var input = File.ReadAllLines("Day14.txt");
            var cave = new Cave(input, true);
            cave.Draw();
            var stopwatch = Stopwatch.StartNew();
            for (var t = 0; t < int.MaxValue; t++)
            {
                if (stopwatch.Elapsed.TotalSeconds > 20)
                {
                    cave.Draw();
                    Helper.Log("Draw " + t);
                    stopwatch.Restart();
                }
                if (cave.Tick())
                    break;
            }
            cave.Draw();
            int extraSand = 0;
            var leftTopSand = cave.LeftTopSand() - 1;
            for (int i = 0; i < leftTopSand; i++)
                extraSand += i + 1;
            var rightTopSand = cave.RightTopSand() - 1;
            for (int i = 0; i < rightTopSand; i++)
                extraSand += i + 1;
            var n = cave.Sands.Count() + extraSand;
            Helper.Log("Star1 Score: " + cave.Sands.Count()); // 5910 is too high, 0 is different than []
        }
    }
}

