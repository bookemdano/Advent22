using AoCLibrary;
using Microsoft.Win32;
using System.Reflection.Metadata.Ecma335;
using static Advent23.Day24;
using static System.Formats.Asn1.AsnWriter;
namespace Advent23
{
    internal class Day24 : IDayRunner
    {
        public bool IsReal => false;

        // Day https://adventofcode.com/2023/day/24
        // Input https://adventofcode.com/2023/day/24/input
        public object? Star1()
        {
            var key = new StarCheckKey(StarEnum.Star1, IsReal);
            StarCheck check;
            if (!IsReal)
                check = new StarCheck(key, 2L);
            else
                check = new StarCheck(key, 16727L);

            var lines = Program.GetLines(check.Key);
            var rv = 0L;
            // magic
            var minBounds = 7L;
            var maxBounds = 27L;
            var precision = 0.00001M;
            if (IsReal)
            {
                minBounds = 200000000000000L;
                maxBounds = 400000000000000L;
                //precision = 1;
            }

            var stones = new List<Stone23>();
            int i = 0;
            foreach (var line in lines)
            {
                stones.Add(new Stone23(Utils.CompactName(i++), line));
            }
            for (i = 0; i < stones.Count - 1; i++)
            {
                var stone1 = stones[i];
                var m1 = stone1.Slope;
                var b1 = stone1.Intercept;
                for (int j = i + 1; j < stones.Count; j++)
                {
                    var stone2 = stones[j];
                    var m2 = stone2.Slope;
                    var b2 = stone2.Intercept;
                    if (m1 - m2 == 0)
                    {
                        ElfHelper.DayLog($"1:{stone1}*2:{stone2} slope issue!");
                        continue;
                    }

                    var x = (b2 - b1) / (m1 - m2);
                    var y1 = m1 * x + b1;
                    var y2 = m2 * x + b2;
                    //ElfHelper.DayLog($"1:{stone1}*2:{stone2} at {x},y1:{y1}|y2:{y2}");
                    if (Math.Abs(y1 - y2) > precision)
                        continue;
                    if (stone1.IsFutureX(x) == false || stone2.IsFutureX(x) == false)
                        continue;
                    if (x >= minBounds && x <= maxBounds && y1 >= minBounds && y1 <= maxBounds)
                        rv++;
                }
            }

            check.Compare(rv);
            //16727
            return rv;
        }
        public object? Star2()
        {
            var key = new StarCheckKey(StarEnum.Star2, IsReal);
            StarCheck check;
            if (!IsReal)
                check = new StarCheck(key, 47L);
            else
                check = new StarCheck(key, 0L);

            var lines = Program.GetLines(check.Key);
            var rv = 0L;
            // magic
            var stones = new Stones23();
            int i = 0;
            foreach (var line in lines)
            {
                stones.Add(new Stone23(Utils.CompactName(i++), line));
            }
            int t = 1;
            for (i = 0; i < stones.Count; i++)  // first point
            {
                
                var usedStones = new Dictionary<int, Stone23>();
                usedStones.Add(1, stones[i]);
                var stonePt1 = stones[i].Move(t);
                t++;

                //Closest(stones.Where(s => usedStones.Contains(s)).ToList(), stones[i], 1);
                for (int j = 0; j < stones.Count; j++) // second point
                {
                    if (i == j)
                        continue;
                    usedStones.Add(stones[i]);
                    var stonePt2 = stones[j].Move(usedStones.Count());
                    var slope = stonePt1.Slope2D(stonePt2);
                    var intercept = (decimal)stonePt1.Y - (stonePt1.X * slope);
                    bool found = true;
                    while (found && usedStones.Count() < stones.Count())
                    {
                        found = false;
                        for (int k = 0; k < stones.Count; k++) // second point
                        {
                            if (usedStones.Any(n => n == stones[k].Name))
                                continue;
                            var stonePtK = stones[k].Move(usedStones.Count());
                            if (stonePtK.IsOnLine2D(slope, intercept))
                            {
                                usedStones.Add(stones[k].Name);
                                found = true;
                            }
                        }
                    }
                    if (usedStones.Count() == stones.Count())
                    {
                        ElfHelper.DayLog("Match!");
                        break;
                    }
                }
            }
            check.Compare(rv);
            return rv;
        }
        Stone23 Closest(List<Stone23> stones, Stone23 stone, int t)
        {
            var min = double.MaxValue;
            Stone23 rv = stone;
            for (int i = 0; i < stones.Count(); i++) // second point
            {
                if (stones[i].Name == stone.Name)
                    continue;
                var d = stones[i].Move(t).Distance(stone.Pos);
                if (d < min)
                {
                    rv = stones[i];
                    min = d;
                }
            }
            return rv;
        }
    }
    public class Stones23 : List<Stone23>
    {

    }
    public class Stone23
    {
        public Stone23(string name, string line)
        {
            //19, 13, 30 @ -2,  1, -2
            Name = name;
            var parts = Utils.Split('@', line);
            Pos = Point3D.FromXYZ(parts[0]);
            Speed = Point3D.FromXYZ(parts[1]);
        }
        public Point3D Pos { get; set; }
        public Point3D Speed { get; }
        public string Name { get; }
        public decimal Slope => (decimal)Speed.Y / Speed.X;
        public decimal Intercept => (decimal)Pos.Y - (Pos.X * Slope);

        public override string ToString()
        {
            return $"{Name} {Pos}@{Speed}";
        }

        internal bool? IsFutureX(decimal x)
        {
            if (Speed.X > 0 && x > Pos.X)
                return true;
            else if (Speed.X < 0 && x < Pos.X)
                return true;
            else if (Speed.X == 0)
                return null;
            return false;
        }

        internal Point3D Move(int t)
        {
            return new Point3D(Pos.X + Speed.X * t, Pos.Y + Speed.Y * t, Pos.Z + Speed.Z * t);
        }
    }
}
