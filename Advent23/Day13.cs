using AoCLibrary;
namespace Advent23
{
	internal class Day13 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/13
		// Input https://adventofcode.com/2023/day/13/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (IsReal)
				check = new StarCheck(key, 27502L);
			else
				check = new StarCheck(key, 405L);

			var lines = Program.GetLines(check.Key, raw: true);
			var rv = 0L;
			// magic
			var grids = new List<Grid13>();
			var clump = new List<string>();
			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					grids.Add(Grid13.FromLines(clump));
					clump = new List<string>();
				}
				else
					clump.Add(line);
			}
			if (clump.Any())
				grids.Add(Grid13.FromLines(clump));

			foreach (var grid in grids)
				rv += grid.FindCompleteMirror1();

			// too low 20464
			//27502
			check.Compare(rv);
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (IsReal)
				check = new StarCheck(key, 31947L);
			else
				check = new StarCheck(key, 400L);

			var lines = Program.GetLines(check.Key, raw: true);
			var rv = 0L;
			// magic
			var grids = new List<Grid13>();
			var clump = new List<string>();
			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					grids.Add(Grid13.FromLines(clump));
					clump = new List<string>();
				}
				else
					clump.Add(line);
			}
			if (clump.Any())
				grids.Add(Grid13.FromLines(clump));

			foreach (var grid in grids)
				rv += grid.FindNearMirrors();

			//  too low 31600
			// 31947
			check.Compare(rv);
			return rv;
		}
	}
	public class Mirrors
	{
		public Mirrors(int min, List<int> rows, List<int> cols)
		{
			_tolerance = min;
			Rows = rows;
			Cols = cols;
		}
		public long GetScore()
		{
			var rv = 0;
			foreach (var col in Cols)
				rv += col;
			foreach (var row in Rows)
				rv += row * 100;
			ElfHelper.DayLog($"r:{string.Join(",", Rows)} c:{string.Join(",", Cols)} = {rv}");
			return rv;
		}
		int _tolerance;
		internal List<int> Rows { get;  }
		internal List<int> Cols { get; }
	}
	public class Grid13 : Grid<Node>
	{
		public Grid13(List<Node> nodes)
		{
			Init(nodes);
		}
		internal static Grid13 FromLines(IEnumerable<string> lines)
		{
			return new Grid13(GetNodes(lines));
		}
		internal long FindCompleteMirror1()
		{
			var symCols = new List<int>();
			for (var iCol = 1; iCol < _cols; iCol++)
				symCols.Add(iCol);

			for (var iRow = 0; iRow < _rows; iRow++)
			{
				var str = NodesToString(NodesInRow(iRow));
				symCols = FindMirrorsForLine(str, symCols);
				if (!symCols.Any())
					break;
			}

			var symRows = new List<int>();
			for (var iRow = 1; iRow < _rows; iRow++)
				symRows.Add(iRow);

			for (var iCol = 0; iCol < _cols; iCol++)
			{
				var str = NodesToString(NodesInCol(iCol));
				symRows = FindMirrorsForLine(str, symRows);
				if (!symRows.Any())
					break;
			}
			var rv = 0;
			foreach (var col in symCols)
				rv += col;
			foreach (var row in symRows)
				rv += row * 100;
			ElfHelper.DayLog($"r:{string.Join(",", symRows)} c:{string.Join(",", symCols)} = {rv}");
			return rv;
		}

		internal long FindNearMirrors()
		{
			var symCols = new List<Possible>();
			for (var iCol = 1; iCol < _cols; iCol++)
				symCols.Add(new Possible(iCol));

			for (var iRow = 0; iRow < _rows; iRow++)
			{
				var str = NodesToString(NodesInRow(iRow));
				symCols = FindNearMirrorsForLine(str, symCols);
				if (!symCols.Any())
					break;
			}


			var symRows = new List<Possible>();
			for (var iRow = 1; iRow < _rows; iRow++)
				symRows.Add(new Possible(iRow));

			for (var iCol = 0; iCol < _cols; iCol++)
			{
				var str = NodesToString(NodesInCol(iCol));
				symRows = FindNearMirrorsForLine(str, symRows);
				if (!symRows.Any())
					break;
			}
			var rv = 0;
			foreach (var col in symCols.Where(p => p.Smudges == 1))
				rv += col.Position;
			foreach (var row in symRows.Where(p => p.Smudges == 1))
				rv += row.Position * 100;
			return rv;
		}
		internal Mirrors FindMirrors()
		{
			var symCols = new List<int>();
			for (var iCol = 1; iCol < _cols; iCol++)
				symCols.Add(iCol);

			for (var iRow = 0; iRow < _rows; iRow++)
			{
				var str = NodesToString(NodesInRow(iRow));
				symCols = FindMirrorsForLine(str, symCols);
				if (!symCols.Any())
					break;
			}

			var symRows = new List<int>();
			for (var iRow = 1; iRow < _rows; iRow++)
				symRows.Add(iRow);

			for (var iCol = 0; iCol < _cols; iCol++)
			{
				var str = NodesToString(NodesInCol(iCol));
				symRows = FindMirrorsForLine(str, symRows);
				if (!symRows.Any())
					break;
			}
			return new Mirrors(0, symRows, symCols);
		}
		static int Diffs(string str, int pos)
		{
			return DiffList(str, pos).Count();
		}
		static List<int> DiffList(string str, int pos)
		{
			var len = str.Length;
			var diffs = new List<int>();
			for (int i = 1; i < len; i++)
			{
				var l = pos - i;
				var r = pos + i - 1;
				if (r >= len || l < 0)
					break;
				if (str[l] != str[r])
					diffs.Add(l);
			}
			return diffs;
		}
		static List<int> FindMirrorsForLine(string str, List<int> possibles)
		{
			// could merge later
			var rv = new List<int>();
			foreach (var pos in possibles)
			{
				if (Diffs(str, pos) == 0)
					rv.Add(pos);
			}
			return rv;
		}
		static List<Possible> FindNearMirrorsForLine(string str, List<Possible> possibles)	
		{
			var rv = new List<Possible>();
			foreach(var pos in possibles)
			{
				var list = DiffList(str, pos.Position);
				if (list.Count() + pos.Smudges <= 1)
					rv.Add(new Possible(pos, list));
			}
			return rv;
		}

		internal long FindNearMirrors(Mirrors mirrors)
		{
			/*
			var symCols = mirrors.Cols;
			var rvCols = new List<int>();
			foreach(var col in symCols)
			{
				var str = NodesToString(NodesInCol(col));
				for (int i = 0; i < str.Length; i++)
				{
					var chars = str.ToCharArray();
					if (chars[i] == '.')
						chars[i] = '#';
					else
						chars[i] = '.';
					var newStr = string.Join("", chars);
					for (int pos = 1; pos < str.Length; pos++)
					{
						if (Diffs(newStr, pos) == 0)
						{
							rvCols += 
						}
					}
				}

			}

			for (var iRow = 0; iRow < _rows; iRow++)
			{
				var str = NodesToString(NodesInRow(iRow));
				for (int i = 0; i < str.Length; i++)
				{
					symCols = FindMirrors(str, symCols, min);
					if (!symCols.Any())
						break;
					for (int pos = 0; pos < str.Length; pos++)


				}
			}
			*/
			return 0L;
		}
	}
	public class Possible
	{
		public Possible(Possible other, List<int> indexes)
		{
			Position = other.Position;
			CharIndex = other.CharIndex;
			if (indexes.Count() > 0)
				CharIndex = indexes.First();
		}
		public Possible(int pos)
		{
			Position = pos;
			CharIndex = null;
		}
		public int Position { get; }

		public int Smudges
		{
			get
			{
				return CharIndex == null ? 0 : 1;
			}
		}
		public int? CharIndex { get; }

		public override string ToString()
		{
			return $"p:{Position} c:{CharIndex}";
		}
	}
}
