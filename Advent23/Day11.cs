using AoCLibrary;
using System.Xml.Linq;

namespace Advent23
{
	internal class Day11 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/11
		// Input https://adventofcode.com/2023/day/11/input
		public object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var grid = Grid11.FromLines(lines);
			rv = Star(grid, 2);
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
			if (IsReal)
				rv = Star(grid, 1000000);
			else
				rv = Star(grid, 100);

			if (!IsReal)
				Utils.Assert(rv, 8410L);
			else
				Utils.Assert(rv, 726820169514L);
			//726820169514
			return rv;
		}
		long Star(Grid11 grid, int expandTo)
		{
			grid.WriteCounts(true, "og");
			grid.Expand(expandTo);
			grid.WriteCounts(true, "ex");
			long rv = 0L;
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
			return rv;

		}
	}
	public class Node11 : Node
	{
		public Node11(Point pt, char c) : base(pt, c)
		{
		}

		public override string ToString()
		{
			return $"{base.ToString()} s:{StarNum}";
		}

		public int? StarNum { get; internal set; }
	}
	public class Grid11 : Grid<Node11>
	{
		Dictionary<int, int> _rowSizes = [];
		Dictionary<int, int> _colSizes = [];

		public Grid11(List<Node11> nodes)
		{
			Init(nodes);
			for (int iRow = 0; iRow < _rows; iRow++)
				_rowSizes[iRow] = 1;
			for (int iCol = 0; iCol < _cols; iCol++)
				_colSizes[iCol] = 1;
		}
		internal static Grid11 FromLines(string[] lines)
		{
			return new Grid11(GetNodes(lines));
		}

		public List<Node11> GetStars()
		{
			var stars = this.Values.Where(v => v.Char == '#').OrderBy(v => v.Pt.GetHashCode()).ToList();
			int i = 1;
			foreach (var star in stars)
				star.StarNum = i++;
			return stars;
		}
		internal void Expand(int expandTo)
		{
			for (int iRow = 0; iRow < _rows; iRow++)
			{
				var nodesInRow = this.Values.Where(n => n.Pt.Row == iRow).ToList();
				if (nodesInRow.All(c => c.Char == '.'))
					_rowSizes[iRow] = expandTo;
			}
			for (int iCol = 0; iCol < _cols; iCol++)
			{
				var nodesInCol = this.Values.Where(n => n.Pt.Col == iCol).ToList();
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
				var cols = this.Values.Where(n => n.Pt.Row == iRow).ToList();
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

		public void WriteCounts(bool raw, string tag)
		{
			if (raw)
			{
				WriteCounts(tag);
				return;

			}	
			var lines = new List<string>();
			for (int row = 0; row < _rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < _cols; col++)
				{
					var v = Find(new Point(row, col))!;
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
