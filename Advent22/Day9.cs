namespace Advent22
{
    internal class Day9
    {
        static public void Run()
        {
            var input = File.ReadAllLines("Day9-input.txt");
            input = new string[] {
                "R 4",
                "U 4",
                "L 3",
                "D 1",
                "R 4",
                "D 1",
                "L 5",
                "R 2",
            };

            var h = new Point();
            var ts = new List<Point>();
            int nTails = 9;
            for (int j = 0; j < nTails; j++)
                ts.Add(new Point());
            var tailVisited = new List<string>() { "0,0" };
            var tail10Visited = new List<string>() { "0,0" };
            foreach (var line in input)
            {
                var parts = line.Split(' ');
                var dir = parts[0];
                var steps = int.Parse(parts[1]);
                for (int i = 0; i < steps; i++)
                {
                    h.Move(dir);
                    ts[0].MoveTo(h);
                    for (int j = 1; j < nTails; j++)
                        ts[j].MoveTo(ts[j - 1]);
                    Draw(dir, h, ts);
                    //var oldT = new Point(ts[0]);
                    // Console.WriteLine(dir + " h:" + h + " oldt:" + oldT + " to t:" + t);
                    if (!tailVisited.Contains(ts[0].ToString()))
                        tailVisited.Add(ts[0].ToString());
                    if (!tail10Visited.Contains(ts[nTails - 1].ToString()))
                        tail10Visited.Add(ts[nTails - 1].ToString());
                }
            }
            File.WriteAllLines("visited1.log", tailVisited);
            Console.WriteLine("Score1 = " + tailVisited.Count());   // not 3295
        }
        static public void Draw(string dir, Point h, List<Point> ts)
        {
            var minX = h.X;
            var minY = h.Y;
            var maxX = h.X;
            var maxY = h.Y;
            foreach (var t in ts)
            {
                minX = Math.Min(t.X, minX);
                minY = Math.Min(t.Y, minY);
                maxX = Math.Max(t.X, maxX);
                maxY = Math.Max(t.Y, maxY);
            }
            minX--;
            minY--;
            if (maxX - minX < 5)
                maxX += 5;
            if (maxY - minY < 5)
                maxY += 5;
            maxX++;
            maxY++;
            var lines = new List<string>();
            lines.Add(dir);
            for (int y = minY; y < maxY; y++)
            {
                var line = "";
                for (int x = minX; x < maxX; x++)
                {
                    var c = '.';

                    var iT = 1;
                    foreach (var t in ts)
                    {
                        if (t.X - minX == x && t.Y - minY == y)
                        {
                            c = (char)(iT + '0');
                            break;
                        }
                        iT++;
                    }
                    if (h.X - minX == x && h.Y - minY == y)
                        c = 'H';
                    line += c;
                }
                lines.Add(line);
            }
            lines.Reverse();
            foreach (var line in lines)
                Console.WriteLine(line);
        }
    }
    class Point
    {
        public Point()
        {
        }
        public Point(Point other)
        {
            X = other.X;
            Y = other.Y;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public bool Touching(Point other)
        {
            if (Math.Abs(X - other.X) > 1)
                return false;
            if (Math.Abs(Y - other.Y) > 1)
                return false;
            return true;
        }
        public bool MoveTo(Point head)
        {
            var deltaX = Math.Abs(X - head.X);
            var deltaY = Math.Abs(Y - head.Y);
            var both = (deltaX + deltaY > 2);
            var rv = false;

            if (deltaX > 1 || both)
            {
                if (head.X > X)
                    X++;
                if (head.X < X)
                    X--;
                rv = true;
            }    
            if (deltaY > 1 || both)
            {
                if (head.Y > Y)
                    Y++;
                if (head.Y < Y)
                    Y--;
                rv = true;
            }
            return rv;
        }
        public void Move(string dir)
        {
            if (dir == "R")
                X++;
            else if (dir == "L")
                X--;
            else if (dir == "U")
                Y++;
            else if (dir == "D")
                Y--;
        }
        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}

