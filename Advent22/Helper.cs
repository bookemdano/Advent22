using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent22
{
    internal class Helper
    {
        static public void Log(object o)
        {
            Console.WriteLine(o?.ToString());
            File.AppendAllText($"endless{DateTime.Today.ToString("yyyyMMdd")}.log", $"{DateTime.Now} {o}\n");
        }
    }
    internal class BasePoint
    {
        public BasePoint(BasePoint other)
        {
            X = other.X; Y = other.Y;
        }
        public BasePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        internal static BasePoint Parse(string coord)
        {
            var parts = coord.Split(',');
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            return new BasePoint(x, y);
        }


        public bool Same(int x, int y)
        {
            return X == x && Y == y;
        }
        public bool Same(BasePoint other)
        {
            return Same(other.X, other.Y);
        }
        public override string ToString()
        {
            return X + "," + Y;
        }
    }
}
