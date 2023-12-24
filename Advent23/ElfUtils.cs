using AoCLibrary;
using System.Net.Security;

namespace Advent23
{
	public class Node
	{
		static public Node Create(Point pt, char c)
		{
			return new Node(pt, c);
		}
		public Node(Point pt, char c)
		{
			Pt = pt;
			Char = c;
		}
		public Node(Node other)
		{
			Pt = other.Pt;
			Char = other.Char;
		}
		public override string ToString()
		{ 
			return $"{Pt} '{Char}'";
		}

		internal void SetChar(char c)
		{
			Char = c;
		}

		public Point Pt { get; }
		public char Char { get; internal set; }

		internal List<Point> Neighbors()
		{
			return Pt.Neighbors();
		}

	}
	public class Grid<T> : Dictionary<Point, T> where T : Node
	{
		internal int Rows { get; private set; }
		internal int Cols { get; private set;}
		protected void Init(List<T> nodes)
		{
			foreach (var node in nodes)
				this.Add(node.Pt, node);
			Rows = nodes.Max(n => n.Pt.Row) + 1;
			Cols = nodes.Max(n => n.Pt.Col) + 1;
		}
		static protected List<T> GetNodes(IEnumerable<string> lines)
		{
			int iRow = 0;
			var nodes = new List<T>();
			var constructors = typeof(T).GetConstructors();
			var con = constructors[0];
			foreach (var line in lines)
			{
				int iCol = 0;
				foreach (var c in line)
				{
					var list = new object[2] { new Point(iRow, iCol++), c};
					var node = con.Invoke(list) as T;
					if (node != null)
						nodes.Add(node);
				}
				iRow++;
			}
			return nodes;
		}
		internal T? Find(Point pt)
		{
			if (!this.TryGetValue(pt, out T? value))
				return null;
			return value;
		}
		internal T? Find(char c)
		{
			return Values.FirstOrDefault(n => n.Char == c);
		}
		internal bool IsValid(Point pt)
		{
            if (pt.Col < 0 || pt.Col >= Cols || pt.Row < 0 || pt.Row >= Rows)
                return false;
            return true;
        }
        public void WriteBase(string tag)
		{
			var lines = new List<string>();
			for (int row = 0; row < Rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < Cols; col++)
				{
					var v = Find(new Point(row, col))!;
					parts.Add(v.Char.ToString());
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Base", tag, lines);
		}
		protected List<List<T>> AllCols()
		{
			var rv = new List<List<T>>();
			for (var iCol = 0; iCol < Cols; iCol++)
				rv.Add(NodesInCol(iCol));
			return rv;
		}
		protected List<T> NodesInCol(int iCol)
		{
			return this.Values.Where(n => n.Pt.Col == iCol).ToList();
		}
		internal List<T> NodesInRow(int iRow)
		{
			return this.Values.Where(n => n.Pt.Row == iRow).ToList();
		}

		internal string NodesToString(IList<T> nodes)
		{
			return string.Join("", nodes.Select(n => n.Char));
		}
        internal List<T> Neighbors(T node)
        {
			var pts = node.Neighbors();
			var rv = new List<T>();
			foreach (var pt in pts)
			{
				var near = Find(pt);
				if (near != null)
					rv.Add(near);
			}
			return rv;
        }

    }
    public class GridPlain : Grid<Node>
	{
		protected GridPlain()
		{
			// in case child wants to init
		}
        public GridPlain(string[] lines)
        {
            Init(GetNodes(lines));
        }
        public GridPlain(List<Node> nodes)
		{
			Init(nodes);
		}
		public override string ToString()
		{
			return $"({Rows}, {Cols})";
		}
		internal static GridPlain FromLines(string[] lines)
		{
			return new GridPlain(GetNodes(lines));
		}

    }
	public class Vector
	{
		public Vector(Point pt, DirEnum dir)
		{
			Pt = pt;
			Dir = dir;
		}
		public Point Pt { get; }
		public DirEnum Dir { get; }

		public override bool Equals(object? obj)
		{
			if (obj is Vector17 other)
			{
				return other.Pt.Equals(Pt) && other.Dir == Dir;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return Pt.GetHashCode() * ((int)Dir + 1);
		}
		public override string ToString()
		{
			return $"{Pt} {Dir}";
		}
	}
	public class Point3D : IEquatable<Point3D>
	{
		public Point3D(long x, long y, long z)
		{
			X = x;
			Y = y;
			Z = z;
		}

        static public Point3D FromXYZ(string str)
		{
			var parts = Utils.SplitLongs(',', str);
			return new Point3D(parts[0], parts[1], parts[2]);
		}

		public long X { get; set; }
		public long Y { get; set; }
		public long Z { get; set; }
		public bool Equals(Point3D? other)
		{
			return other?.X == X && other?.Y == Y && other?.Z == Z;
		}
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
			//return (int)((X * 1E6) + (Y * 1E3) + Z);
		}
		public override bool Equals(object? obj)
		{
			if (obj is not Point3D other)
				return false;

		return Equals(other);
		}
		public override string ToString()
		{
			return $"({X}, {Y}, {Z})";
		}

        internal double Distance(Point3D other)
        {
			return Math.Sqrt((other.X - X) ^ 2 + (other.Y - Y) ^ 2 + (other.Z - Z) ^ 2);
        }
        internal decimal Slope2D(Point3D pt2)
        {
            if ((pt2.X - X) == 0)
                return decimal.MaxValue;
            return (decimal)(pt2.Y - Y) / (pt2.X - X);
        }

        internal bool IsOnLine2D(decimal slope, decimal intercept)
        {
			return (Y == slope * X + intercept);
        }
    }

	public class Point : IEquatable<Point>
	{
		public Point(int row, int col)
		{
			Row = row;
			Col = col;
		}
		public Point(Point other)
		{
			Row = other.Row;
			Col = other.Col;
		}
		public override int GetHashCode()
		{
			return Row * 100000 + Col;
		}
		public override bool Equals(object? obj)
		{
			if (obj is not Point other)
				return false;
			return (other.Row == Row && other.Col == Col);
		}
		public override string ToString()
		{
			return $"({Row}, {Col})";
		}

		public bool Equals(Point? other)
		{
			return (other?.Row == Row && other?.Col == Col);
		}

        internal DirEnum GetDir(Point to)
        {
            if (Col < to.Col)
                return DirEnum.East;
            else if (Col > to.Col)
                return DirEnum.West;
            else if (Row > to.Row)
                return DirEnum.North;
            else if (Row < to.Row)
                return DirEnum.South;
			else
				return DirEnum.NA;
        }
        public Point Translate(DirEnum dir, int len)
        {
            Point rv;
            if (dir == DirEnum.North)
                rv = new Point(Row - len, Col);
            else if (dir == DirEnum.South)
                rv = new Point(Row + len, Col);
            else if (dir == DirEnum.East)
                rv = new Point(Row, Col + len);
            else //if (dir == DirEnum.West)
                rv = new Point(Row, Col - len);
            return rv;
        }

		internal static DirEnum OtherDir(DirEnum dir)
		{
			if (dir == DirEnum.North)
				return DirEnum.South;
			else if (dir == DirEnum.South)
				return DirEnum.North;
			else if (dir == DirEnum.East)
				return DirEnum.West;
			else //if (dir == DirEnum.West)
				return DirEnum.East;
		}

		public int Row { get; }
		public int Col { get; }

		internal List<Point> Neighbors()
		{
			var rv = new List<Point>();
			rv.Add(new(Row + 1, Col));
			rv.Add(new(Row - 1, Col));
			rv.Add(new(Row, Col + 1));
			rv.Add(new(Row, Col - 1));

			return rv;
		}

	}
	internal class ElfUtils
	{
		internal static void WriteLines(string stub, string tag, List<string> lines)
		{
			File.WriteAllLines(Path.Combine(ElfHelper.DayDir, $"{stub}Day{ElfHelper.DayString}-{tag}.csv"), lines);
		}
	}

	public class StarCheckKey
	{
		public StarCheckKey(StarEnum star, bool isReal, int? part = null)
		{
			Star = star;
			IsReal = isReal;
			Part = part;
		}
		public override string ToString()
		{
			return $"{Star} r:{IsReal} p:{Part}";
		}
		public StarEnum Star { get; }
		public bool IsReal { get; }
		public int? Part { get; }
	}
	public class StarCheck
	{
		public StarCheck(StarCheckKey key, long expected)
		{
			Key = key;
			Expected = expected;
		}

		public StarCheckKey Key { get; }
		public long Expected { get; }
		public override string ToString()
		{
			return $"{Key} e:{Expected}";
		}
		public void Compare(long answer)
		{
			ElfHelper.MonthLogPlus($"Compare {this} ?= a:{answer}");
			Utils.CaptainsLog($"Compare {this} ?= a:{answer}");
			Utils.Assert(answer, Expected);
		}
	}
	static public class Misc
	{
		// https://www.c-sharpcorner.com/UploadFile/0c1bb2/program-to-find-lcm-lowest-common-multiples-of-two-numbers/
		public static long FindLCM(long a, long b)
		{
			if (a < b)
				(b, a) = (a, b);

			for (long i = 1; i <= b; i++)
			{
				if ((a * i) % b == 0)
					return i * a;
			}
			return b;
		}

	}
	public interface IRace
	{
		// winSearch- searching for first win(true) or first loss(false)
		static public long BinSearch(long min, long max, IRace race, bool winSearch)
		{
			while (max != min + 1)
			{
				var t = (long)(min + (max - min) / 2);
				var b = race.Compare(t, winSearch);
				if (b)
					max = t;
				else
					min = t;
			}
			return max;
		}
		public bool Compare(long t, bool winSearch)
		{
			ElfHelper.DayLog($"Compare({t}) for {this} w:{winSearch}");
			if (winSearch)
				return Win(t);
			else
				return !Win(t);
		}
		bool Win(long t);
	}
}
