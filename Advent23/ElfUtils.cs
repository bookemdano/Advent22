
using AoCLibrary;

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
		public override string ToString()
		{ 
			return $"{Pt} '{Char}'";
		}
		internal List<Point> Neighbors()
		{
			var rv = new List<Point>();
			rv.Add(new(Pt.Row + 1, Pt.Col));
			rv.Add(new(Pt.Row - 1, Pt.Col));
			rv.Add(new(Pt.Row, Pt.Col + 1));
			rv.Add(new(Pt.Row, Pt.Col - 1));

			return rv;
		}

		public Point Pt { get; }
		public char Char { get; protected set; }
	}
	public class Grid<T> : Dictionary<Point, T> where T : Node
	{
		protected int _rows;
		protected int _cols;
		protected void Init(List<T> nodes)
		{
			foreach (var node in nodes)
				this.Add(node.Pt, node);
			_rows = nodes.Max(n => n.Pt.Row) + 1;
			_cols = nodes.Max(n => n.Pt.Col) + 1;
		}
		static protected List<T> GetNodes(string[] lines)
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
		protected T? Find(Point pt)
		{
			if (!this.TryGetValue(pt, out T? value))
				return null;
			return value;
		}
		public void WriteBase(string tag)
		{
			var lines = new List<string>();
			for (int row = 0; row < _rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < _cols; col++)
				{
					var v = Find(new Point(row, col))!;
					parts.Add(v.Char.ToString());
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Base", tag, lines);
		}

	}
	public class Point
	{
		public Point(int row, int col)
		{
			Row = row;
			Col = col;
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
		public int Row { get; }
		public int Col { get; }
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
			Utils.Log($"Compare {this} ?= a:{answer}");
			Utils.Assert(answer, Expected);
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
