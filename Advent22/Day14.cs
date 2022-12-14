using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Advent22
{
    internal class Day14
    {
        internal class Point
        {
            public Point(Point other)
            {
                X = other.X; Y = other.Y;  
            }
            public Point(int x, int y)
            {
                X = x; 
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }

            internal static Point Parse(string coord)
            {
                var parts = coord.Split(',');
                var x = int.Parse(parts[0]);
                var y = int.Parse(parts[1]);
                return new Point(x, y);
            }
            public bool Same(int x, int y)
            {
                return X == x && Y == y;
            }
            public bool Same(Point other)
            {
                return Same(other.X, other.Y);
            }
            public override string ToString()
            {
                return X + "," + Y;
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
            public Cave(string[] lines)
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
                        lastPt = pt;
                    }
                }

            }
            //List<Point> rock, List<Point> sands, Point source, Point sand
            public List<Point> Rocks { get; } = new List<Point>();
            public List<Point> Sands { get; } = new List<Point>();
            public Point Source { get; } = new Point(500, 0);
            public Point FallingSand { get; set; }
            internal void Draw()
            {
                Helper.Log($"Sands: {Sands.Count()}");
                var minX = Rocks.Min(p => p.X) - 1;
                var maxX = Rocks.Max(p => p.X) + 1;
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
            }

            // return true for abyss
            internal bool Tick()
            {
                if (FallingSand == null)
                {
                    FallingSand = new Point(Source);
                    return false;
                }

                var nextSand = FallingSand.Fall();
                if (Collision(nextSand))
                {
                    nextSand = FallingSand.FallLeft();
                    if (Collision(nextSand))
                    {
                        nextSand = FallingSand.FallRight();
                        if (Collision(nextSand))
                        {
                            Sands.Add(new Point(FallingSand));
                            //Draw();
                            nextSand = null;
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
                return Rocks.Any(s => s.Same(pt)) || Sands.Any(s => s.Same(pt));
            }

            internal bool Abyss(Point pt)
            {
                var rv = Rocks.Where(r => r.X == pt.X && r.Y > pt.Y);
                return !Rocks.Any(r => r.X == pt.X && r.Y > pt.Y);
            }
        }

        static public void Run()
        {
            var input = File.ReadAllLines("Day14.txt");
            var cave = new Cave(input);
            cave.Draw();
            for(var t = 0; t < int.MaxValue; t++)
            {
                if (t % 10000 == 0)
                    cave.Draw();
                if (cave.Tick())
                    break;
            }
            cave.Draw();
            Helper.Log("Star1 Score: " + cave.Sands.Count()); // 5910 is too high, 0 is different than []
        }
    }
}

