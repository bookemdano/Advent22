using AoCLibrary;
namespace Advent24;

internal class Day04 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/4
	// Input https://adventofcode.com/2024/day/4/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 18L);
		else
			check = new StarCheck(key, 2454L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var ws = new WS(lines);
		for (var row = 0; row < ws.Rows; row++)
		{
			for (var col = 0; col < ws.Cols; col++)
			{
				if (ws.Get(new RowCol(row, col)) != "X")
					continue;

				var rcds = ws.FindNext(new RowCol(row, col), "M");
				foreach (var rcd in rcds)
				{
					var next = rcd.Move();
					if (ws.Get(next) == "A")
					{
						next = next.Move();
						if (ws.Get(next) == "S")
							rv++;
					}
				}
			}
		}
		check.Compare(rv);
		return rv;
	}
	public enum DirEnum
	{
		N,
		NE,
		E,
		SE,
		S,
		SW,
		W,
		NW
	}
	public class RCDir : RowCol
	{
		public DirEnum Dir { get; set; }
		public RCDir(int row, int col, DirEnum dir) : base(row, col)
		{
			Dir = dir;
		}
		public RCDir Move()
		{
			return base.Move(Dir);
		}
		public override string ToString()
		{
			return $"{base.ToString()}{Dir}";
		}

	}
	public class RowCol
	{
		public int Row { get; set; }
		public int Col { get; set; }
		public RowCol(int row, int col)
		{
			Row = row; Col = col;
		}
		public RCDir Move(DirEnum dir)
		{
			if (dir == DirEnum.N)
				return new RCDir(Row - 1, Col, dir);
			else if (dir == DirEnum.NE)
				return new RCDir(Row - 1, Col + 1, dir);
			else if (dir == DirEnum.E)
				return new RCDir(Row, Col + 1, dir);
			else if (dir == DirEnum.SE)
				return new RCDir(Row+1, Col + 1, dir);
			else if (dir == DirEnum.S)
				return new RCDir(Row+1, Col, dir);
			else if (dir == DirEnum.SW)
				return new RCDir(Row+1, Col -1, dir);
			else if (dir == DirEnum.W)
				return new RCDir(Row, Col - 1, dir);
			else if (dir == DirEnum.NW)
				return new RCDir(Row - 1, Col - 1, dir);
			else
				return new RCDir(Row, Col, dir);
		}
		public override string ToString()
		{
			return $"({Row},{Col})";
		}
	}
	public class WS
	{
		public List<RCDir> FindNext(RowCol rc, string target)
		{
			var rv = new List<RCDir>();
			var rcds = GetSet(rc);
			foreach (var rcd in rcds)
			{
				if (target == Get(rcd))
					rv.Add(rcd);
			}
			return rv;
		}
		List<RCDir> GetSet(RowCol rc)
		{
			var rv = new List<RCDir>();
			foreach (DirEnum dir in Enum.GetValues(typeof(DirEnum)))
			{
				var rcd = rc.Move(dir);
				if (IsValid(rcd))
					rv.Add(rcd);
			}
			return rv;
		}
		public bool IsValid(RowCol rc)
		{
			if (rc.Row < 0 || rc.Row >= Rows)
				return false;
			if (rc.Col < 0 || rc.Col >= Cols)
				return false;
			return true;
		}
		public string? Get(RowCol rc)
		{
			if (!IsValid(rc))
				return null;
			return _grid[rc.Row][rc.Col];
		}
		List<List<string>> _grid = [];
		public int Rows => _grid.Count;
		public int Cols => _grid[0].Count;
		public WS(string[] lines)
		{
			foreach (var line in lines)
			{
				var chars = new List<string>();
				foreach (var c in line)
					chars.Add(c.ToString());
				_grid.Add(chars);
			}
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 9L);
		else
			check = new StarCheck(key, 1858L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var ws = new WS(lines);
		for (var row = 0; row < ws.Rows; row++)
		{
			for (var col = 0; col < ws.Cols; col++)
			{
				var rc = new RowCol(row, col);
				if (ws.Get(rc) != "A")
					continue;
				var mas = 0;
				if (ws.Get(rc.Move(DirEnum.NE)) == "M" && ws.Get(rc.Move(DirEnum.SW)) == "S")
					mas++;
				if (ws.Get(rc.Move(DirEnum.NE)) == "S" && ws.Get(rc.Move(DirEnum.SW)) == "M")
					mas++;
				if (ws.Get(rc.Move(DirEnum.NW)) == "M" && ws.Get(rc.Move(DirEnum.SE)) == "S")
					mas++;
				if (ws.Get(rc.Move(DirEnum.NW)) == "S" && ws.Get(rc.Move(DirEnum.SE)) == "M")
					mas++;
				if (mas == 2)
					rv++;
			}
		}

		check.Compare(rv);
		return rv;
	}
}

