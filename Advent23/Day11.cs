using AoCLibrary;

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
			StarCheck check;
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			if (IsReal)
				check = new StarCheck(key, 9623138L);
			else
				check = new StarCheck(key, 374L);

			var lines = Program.GetLines(StarEnum.Star1, IsReal);
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
						continue;	// only do one way
					var d = grid.Distance(from.Pt, to.Pt);
					if (!IsReal)
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
