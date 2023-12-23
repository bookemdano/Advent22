using AoCLibrary;

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
			StarCheck check;
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			if (IsReal)
				check = new StarCheck(key, 9623138L);
			else
				check = new StarCheck(key, 374L);

			var lines = Program.GetLines(check.Key);
			var grid = Grid11.FromLines(lines);
			rv = Star(grid, expandTo: 2);

			check.Compare(rv);
			// 		rv	9623138	long

			return rv;
		}
		public object? Star2()
		{
			var rv = 0L;
			StarCheck check;
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			if (IsReal)
				check = new StarCheck(key, 726820169514L);
			else
				check = new StarCheck(key, 8410L);

			var lines = Program.GetLines(key);
			var grid = Grid11.FromLines(lines);
			if (IsReal)
				rv = Star(grid, expandTo: 1000000);
			else
				rv = Star(grid, expandTo: 100);

			check.Compare(rv);
			//726820169514
			return rv;
		}
		long Star(Grid11 grid, int expandTo)
		{
			grid.WriteBase("og");
			grid.Expand(expandTo);
			grid.WriteBase("ex");
			long rv = 0L;
			var stars = grid.GetStars();
			foreach (var from in stars)
			{
				foreach (var to in stars)
				{
					if (to.StarNum <= from.StarNum)
						continue;	// only do one way
					var d = grid.Distance(from.Pt, to.Pt);
					if (!IsReal)
						ElfHelper.DayLog($"{from.StarNum} {to.StarNum} d:{d}");
					rv += d;
				}
			}
			grid.Write("ex");
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
			for (int iRow = 0; iRow < Rows; iRow++)
				_rowSizes[iRow] = 1;
			for (int iCol = 0; iCol < Cols; iCol++)
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
			for (int iRow = 0; iRow < Rows; iRow++)
			{
				var nodesInRow = this.Values.Where(n => n.Pt.Row == iRow).ToList();
				if (nodesInRow.All(c => c.Char == '.'))
					_rowSizes[iRow] = expandTo;
			}
			for (int iCol = 0; iCol < Cols; iCol++)
			{
				var nodesInCol = this.Values.Where(n => n.Pt.Col == iCol).ToList();
				if (nodesInCol.All(c => c.Char == '.'))
					_colSizes[iCol] = expandTo;
			}
		}

		public void Write(string tag)
		{
			var lines = new List<string>();
			for (int row = 0; row < Rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < Cols; col++)
				{
					var v = Find(new Point(row, col))!;
					parts.Add(v.StarNum?.ToString()??" ");
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Grid11", tag, lines);
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
