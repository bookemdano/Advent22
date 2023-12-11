using AoCLibrary;
using System.Xml.Linq;

namespace Advent23
{
	internal class Day11 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/11
		// Input https://adventofcode.com/2023/day/11/input
		public object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var grid = Grid11.FromLines(lines);
			grid.WriteCounts(true, "og");
			grid.Expand(2);
			grid.WriteCounts(true, "ex");
			var stars = grid.GetStars();
			foreach(var from in stars)
			{
				foreach (var to in stars)
				{
					if (to.StarNum <= from.StarNum)
						continue;
					if (from.StarNum == 1 && to.StarNum == 7)
						rv += 0;
					if (from.StarNum == 5 && to.StarNum == 9)
						rv += 0;
					var d = grid.Distance(from.Pt, to.Pt);
					Utils.TestLog($"{from.StarNum} {to.StarNum} d:{d}");
					rv += d;
				}

			}
			grid.WriteCounts(false, "ex");
			if (!IsReal)
				Utils.Assert(rv, 374L);
			else
				Utils.Assert(rv, 9623138L);
			// 		rv	9623138	long

			return rv;
		}
		public object? Star2()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
			var grid = Grid11.FromLines(lines);
			grid.WriteCounts(true, "og");
			if (IsReal)
				grid.Expand(1000000);
			else
				grid.Expand(100);
			grid.WriteCounts(true, "ex");
			var stars = grid.GetStars();
			foreach (var from in stars)
			{
				foreach (var to in stars)
				{
					if (to.StarNum <= from.StarNum)
						continue;
					var d = grid.Distance(from.Pt, to.Pt);
					Utils.TestLog($"{from.StarNum} {to.StarNum} d:{d}");
					rv += d;
				}

			}
			grid.WriteCounts(false, "ex");
			if (!IsReal)
				Utils.Assert(rv, 8410L);
			else
				Utils.Assert(rv, 726820169514L);
			//726820169514
			return rv;
		}

	}
	public class Node11
	{
		public Node11(Point pt, char c)
		{
			Pt = pt;
			Char = c;
		}

		public Node11(int row, int col, char c)
		{
			Pt = new Point(row, col);
			Char = c;
		}
		public override string ToString()
		{
			return $"{Pt} '{Char}' s:{StarNum}";
		}

		public Point Pt { get; }
		public char Char { get; private set; }
		public int? StarNum { get; internal set; }
	}
	public class Grid11
	{
		Dictionary<Point, Node11> _dict = [];
		private int _rows;
		private int _cols;
		Dictionary<int, int> _rowSizes = [];
		Dictionary<int, int> _colSizes = [];

		public Grid11(List<Node11> nodes)
		{
			foreach (var node in nodes)
				_dict.Add(node.Pt, node);
			_rows = nodes.Max(n => n.Pt.Row) + 1;
			_cols = nodes.Max(n => n.Pt.Col) + 1;
			for (int iRow = 0; iRow < _rows; iRow++)
				_rowSizes[iRow] = 1;
			for (int iCol = 0; iCol < _cols; iCol++)
				_colSizes[iCol] = 1;
		}

		static public Grid11 FromLines(string[] lines)
		{
			int iRow = 0;
			var nodes = new List<Node11>();
			foreach (var line in lines)
			{
				int iCol = 0;
				foreach (var c in line)
					nodes.Add(new Node11(iRow, iCol++, c));
				iRow++;
			}
			return new Grid11(nodes);
		}
		public List<Node11> GetStars()
		{
			var stars = _dict.Values.Where(v => v.Char == '#').OrderBy(v => v.Pt.GetHashCode()).ToList();
			int i = 1;
			foreach (var star in stars)
				star.StarNum = i++;
			return stars;
		}
		internal void Expand(int expandTo)
		{
			for (int iRow = 0; iRow < _rows; iRow++)
			{
				var nodesInRow = _dict.Values.Where(n => n.Pt.Row == iRow).ToList();
				if (nodesInRow.All(c => c.Char == '.'))
					_rowSizes[iRow] = expandTo;
			}
			for (int iCol = 0; iCol < _cols; iCol++)
			{
				var nodesInCol = _dict.Values.Where(n => n.Pt.Col == iCol).ToList();
				if (nodesInCol.All(c => c.Char == '.'))
					_colSizes[iCol] = expandTo;
			}
		}
		internal Grid11 Expand()
		{
			var rowNodes = ExpandRows();
			var colNodes = ExpandCols(rowNodes);
			return new Grid11(colNodes);
		}
		static List<Node11> ExpandCols(List<Node11> nodes)
		{
			List<Node11> rv = new List<Node11>();
			int newCols = 0;
			int nRows = nodes.Max(n => n.Pt.Row) + 1;
			int nCols = nodes.Max(n => n.Pt.Col) + 1;
			for (int iCol = 0; iCol < nCols; iCol++)
			{
				var rows = nodes.Where(n => n.Pt.Col == iCol).ToList();
				for (int iRow = 0; iRow < nRows; iRow++)
				{
					var pt = new Point(iRow, iCol + newCols);
					rv.Add(new Node11(pt, rows[iRow].Char));
				}
				if (rows.All(c => c.Char == '.'))
				{
					newCols++;
					for (int iRow = 0; iRow < nRows; iRow++)
					{
						var pt = new Point(iRow, iCol + newCols);
						rv.Add(new Node11(pt, '.'));
					}
				}
			}
			return rv;
		}

		private List<Node11> ExpandRows()
		{
			List<Node11> rv = new List<Node11>();
			int newRows = 0;
			for (int iRow = 0; iRow < _rows; iRow++)
			{
				var cols = _dict.Values.Where(n => n.Pt.Row == iRow).ToList();
				for (int iCol = 0; iCol < _cols; iCol++)
				{
					var pt = new Point(iRow + newRows, iCol);
					rv.Add(new Node11(pt, cols[iCol].Char));
				}
				if (cols.All(c => c.Char == '.'))
				{
					newRows++;
					for (int iCol = 0; iCol < _cols; iCol++)
					{
						var pt = new Point(iRow + newRows, iCol);
						rv.Add(new Node11(pt, '.'));
					}
				}
			}

			return rv;
		}

		Node11? Find(Point pt)
		{
			if (!_dict.TryGetValue(pt, out Node11? value))
				return null;
			return value;
		}
		public void WriteCounts(bool raw, string tag)
		{
			var lines = new List<string>();
			for (int row = 0; row < _rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < _cols; col++)
				{
					var v = Find(new Point(row, col))!;
					if (raw)
						parts.Add(v.Char.ToString());
					else
						parts.Add(v.StarNum?.ToString()??" ");
				}
				lines.Add(string.Join(",", parts));
			}
			File.WriteAllLines(Path.Combine(Utils.Dir, $"counts{ElfHelper.DayString()}{raw}{tag}.csv"), lines);
		}

		internal long Distance(Point from, Point to)
		{
			if (from == to)
				return 0;
			var rv = 0;
			var lowCol = Math.Min(from.Col, to.Col);
			var highCol = Math.Max(from.Col, to.Col);
			for (int iCol = lowCol; iCol < highCol; iCol++)
				rv += _colSizes[iCol];

			var lowRow = Math.Min(from.Row, to.Row);
			var highRow = Math.Max(from.Row, to.Row);
			for (int iRow = lowRow; iRow < highRow; iRow++)
				rv += _rowSizes[iRow];
			return rv;
		}
	}
}
