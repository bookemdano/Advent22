namespace Advent22.Days
{
    internal class Day18
    {
        static public void Run()
        {
            //Day1();
            Day2();
        }
        public class Cube
        {
            public Cube(string line)
            {
                var parts = line.Split(',');
                X = int.Parse(parts[0]);
                Y = int.Parse(parts[1]);
                Z = int.Parse(parts[2]);
            }
            public Cube(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
            public int Distance(Cube other)
            {
                return Math.Abs(other.X - X) + Math.Abs(other.Y - Y) + Math.Abs(other.Z - Z);
            }
            public bool Bump(Cube other)
            {
                if (Distance(other) == 1)
                {
                    if (other.X - X == 1)
                    {
                        Sides[0] = true;
                        other.Sides[1] = true;
                    }
                    else if (X - other.X == 1)
                    {
                        Sides[1] = true;
                        other.Sides[0] = true;
                    }
                    else if (other.Y - Y == 1)
                    {
                        Sides[2] = true;
                        other.Sides[3] = true;
                    }
                    else if (Y - other.Y == 1)
                    {
                        Sides[3] = true;
                        other.Sides[2] = true;
                    }
                    else if (other.Z - Z == 1)
                    {
                        Sides[4] = true;
                        other.Sides[5] = true;
                    }
                    else if (Z - other.Z == 1)
                    {
                        Sides[5] = true;
                        other.Sides[4] = true;
                    }

                    return true;
                }
                return false;
            }
            public int Exposed { get; set; } = 6;

            // true if covered
            // +x, -x, +y, -y, +z, -z
            public bool[] Sides { get; set; } = new bool[6];
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public bool BubbleChecked { get; internal set; }

            internal void SetCheck()
            {
                BubbleChecked = true;
            }
            public override string ToString()
            {
                return $"{X},{Y},{Z} {BubbleChecked}";
            }

            internal bool Same(Cube other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }
        }
        static void Day1()
        {
            var input = File.ReadAllLines("Day18.txt");
            //input = new string[] { "1,1,1", "1,1,2" };
            var cubes = new List<Cube>();
            foreach (var line in input)
                cubes.Add(new Cube(line));

            Helper.Log("Star start: " + cubes.Sum(c => c.Exposed));
            for (int i = 0; i < cubes.Count - 1; i++)
            {
                var iCube = cubes[i];
                for (int j = i + 1; j < cubes.Count; j++)
                {
                    var jCube = cubes[j];
                    if (iCube.Distance(jCube) == 1)
                    {
                        iCube.Exposed--;
                        jCube.Exposed--;
                    }
                }
            }
            Helper.Log("Star Score: " + cubes.Sum(c => c.Exposed));
        }
        public class CubeSpace
        {
            public List<Cube> Cubes = new List<Cube>();
            public CubeSpace(string[] lines)
            {
                foreach (var line in lines)
                    Cubes.Add(new Cube(line));
                MinX = Cubes.Min(c => c.X);
                MaxX = Cubes.Max(c => c.X);
                MinY = Cubes.Min(c => c.Y);
                MaxY = Cubes.Max(c => c.Y);
                MinZ = Cubes.Min(c => c.Z);
                MaxZ = Cubes.Max(c => c.Z);
            }

            internal void BumpAll()
            {
                for (int i = 0; i < Cubes.Count - 1; i++)
                {
                    var iCube = Cubes[i];
                    for (int j = i + 1; j < Cubes.Count; j++)
                    {
                        var jCube = Cubes[j];
                        iCube.Bump(jCube);
                    }
                }
            }

            public int MinX { get; }
            public int MaxX { get; }
            public int MinY { get; }
            public int MaxY { get; }
            public int MinZ { get; }
            public int MaxZ { get; }

            internal void CheckPockets()
            {
                // good(closed) and bad(open) bubbles
                var bubbles = new List<Tuple<bool, List<Cube>>>();
                for (int x = MinX; x <= MaxX; x++)
                {
                    for (int y = MinY; y <= MaxY; y++)
                    {
                        for (int z = MinZ; z <= MaxZ; z++)
                        {
                            if (Occupied(x, y, z))
                                continue;
                            Cube current = new Cube(x, y, z);
                            if (bubbles.Any(b => b.Item2.Any(c => c.Same(current))))
                                continue;
                            var bubble = new List<Cube>();
                            bubble.Add(current);
                            var open = false;
                            while (true)
                            {
                                if (!Check(current.X + 1, current.Y, current.Z, bubble))
                                    open = true;
                                if (!Check(current.X - 1, current.Y, current.Z, bubble))
                                    open = true;
                                if (!Check(current.X, current.Y + 1, current.Z, bubble))
                                    open = true;
                                if (!Check(current.X, current.Y - 1, current.Z, bubble))
                                    open = true;
                                if (!Check(current.X, current.Y, current.Z + 1, bubble))
                                    open = true;
                                if (!Check(current.X, current.Y, current.Z - 1, bubble))
                                    open = true;

                                current.BubbleChecked = true;
                                if (!bubble.Any(b => !b.BubbleChecked))
                                    break;
                                current = bubble.First(b => !b.BubbleChecked);
                            }
                            bubbles.Add(new Tuple<bool, List<Cube>>(open, bubble));

                        }
                    }
                }
                foreach (var t in bubbles.Where(b => b.Item1 == false))
                {
                    foreach (var b in t.Item2)
                    {
                        foreach (var c in Cubes)
                            c.Bump(b);
                    }
                }
            }

            private bool Check(int x, int y, int z, List<Cube> bubble)
            {
                var next = new Cube(x, y, z);
                if (bubble.Any(b => b.Same(next)))
                    return true;
                if (!Inbounds(next))
                    return false; // steam escaped
                if (!Occupied(next))
                    bubble.Add(next);
                return true;
            }

            private bool Inbounds(Cube cube)
            {
                return MinX <= cube.X && MaxX >= cube.X && MinY <= cube.Y && MaxY >= cube.Y && MinZ <= cube.Z && MaxZ >= cube.Z;
            }
            private bool Occupied(Cube cube)
            {
                return Occupied(cube.X, cube.Y, cube.Z);
            }
            private bool Occupied(int x, int y, int z)
            {
                return Cubes.Any(c => c.X == x && c.Y == y && c.Z == z);
            }
        }
        static void Day2()
        {
            var input = File.ReadAllLines("Day18.txt");
            var cubeSpace = new CubeSpace(input);

            Helper.Log("Star start: " + cubeSpace.Cubes.Sum(c => c.Exposed));

            cubeSpace.BumpAll();

            var exposedSome = cubeSpace.Cubes.Sum(c => c.Sides.Count(s => s == false));

            cubeSpace.CheckPockets();

            var exposed = cubeSpace.Cubes.Sum(c => c.Sides.Count(s => s == false));

            Helper.Log("Star Score: " + exposed);
        }
    }
}

