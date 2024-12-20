namespace AoCLibrary;
public enum DirEnum
{
	N, E, S, W
}
public class LocDir : Loc
{
	public DirEnum Dir { get; }

	public LocDir(int row, int col, DirEnum dir) : base(row, col)
	{
		Dir = dir;
	}
	public override string ToString()
	{
		return $"{base.ToString()}{Dir}";
	}

	internal static DirEnum ParseDir(char c)
	{
		if (c == '^')
			return DirEnum.N;
		else if (c == '>')
			return DirEnum.E;
		else if (c == 'v')
			return DirEnum.S;
		else 
			return DirEnum.W;
	}
}
public class Region
{
	public Point Ul { get; set; }
	public Point Br { get; set; }

	public Region(Point ul, Point br)
	{
		Ul = ul;
		Br = br;
	}
	public override string ToString()
	{
		return $"{Ul} to {Br}";
	}
	public bool Contains(Point pt)
	{
		return (pt.X >= Ul.X && pt.Y >= Ul.Y && pt.X <= Br.X && pt.Y <= Br.Y);
	}
}
public class Point
{
	public long X { get; set; }
	public long Y { get; set; }
	public Point(long x, long y)
	{
		Y = y; X = x;
	}
	public override string ToString()
	{
		return $"({X},{Y})";
	}

	public Point Diff(Point other)
	{
		return new Point(other.X - X, other.Y - Y);
	}

	public Point Minus(Point diff)
	{
		return new Point(X - diff.X, Y - diff.Y);
	}

	public Point Plus(Point diff)
	{
		return new Point(X + diff.X, Y + diff.Y);
	}
	public List<Point> AllMoves()
	{
		var rv = new List<Point>();
		rv.Add(new Point(X, Y - 1));
		rv.Add(new Point(X + 1, Y));
		rv.Add(new Point(X, Y + 1));
		rv.Add(new Point(X - 1, Y));
		return rv;
	}
	public Point Move(DirEnum dir)
	{
		if (dir == DirEnum.N)
			return new Point(X, Y - 1);
		else if (dir == DirEnum.E)
			return new Point(X + 1, Y);
		else if (dir == DirEnum.S)
			return new Point(X, Y + 1);
		else //if (dir == DirEnum.W)
			return new Point(X - 1, Y);
	}
	public bool Same(Point other)
	{
		return (Y == other.Y && X == other.X);
	}

	internal Point Copy()
	{
		return new Point(X, Y);
	}
}



public class Loc
{
	public int Row { get; set; }
	public int Col { get; set; }
	public Loc(int row, int col)
	{
		Row = row; Col = col;
	}
	public override string ToString()
	{
		return $"({Row},{Col})";
	}

	public override int GetHashCode()
	{
		return Row * 1000 + Col;
	}
	public Loc Diff(Loc other)
	{
		return new Loc(other.Row - Row, other.Col - Col);
	}

	public Loc Minus(Loc diff)
	{
		return new Loc(Row - diff.Row, Col - diff.Col);
	}

	public Loc Plus(int row, int col)
	{
		return new Loc(Row + row, Col + col);
	}
	public Loc Plus(Loc diff)
	{
		return Plus(diff.Row, diff.Col);
	}
	public List<Loc> AllMoves()
	{
		var rv = new List<Loc>();
		rv.Add(new Loc(Row - 1, Col));
		rv.Add(new Loc(Row, Col + 1));
		rv.Add(new Loc(Row + 1, Col));
		rv.Add(new Loc(Row, Col - 1));
		return rv;
	}
	public Loc Move(DirEnum dir)
	{
		if (dir == DirEnum.N)
			return new Loc(Row - 1, Col);
		else if (dir == DirEnum.E)
			return new Loc(Row, Col + 1);
		else if (dir == DirEnum.S)
			return new Loc(Row + 1, Col);
		else //if (dir == DirEnum.W)
			return new Loc(Row, Col - 1);
	}
	public bool Same(Loc other)
	{
		return (Row == other.Row && Col == other.Col);
	}


}
public enum MoveEnum
{
	Good,
	OffMap,
	Loop
}
public class GridMap
{
	List<char[]> _map = [];
	public GridMap(IEnumerable<string>? lines)
	{
		if (lines == null)
			return;
		foreach (var line in lines)
		{
			_map.Add(line.ToCharArray());
		}
	}
	public int Rows => _map.Count();
	public int Cols => _map[0].Count();
	internal Loc? Find(char target)
	{
		int iRow = 0;
		foreach (var row in _map)
		{
			int iCol = 0;
			foreach (var c in row)
			{
				if (c == target)
					return new Loc(iRow, iCol);
				iCol++;
			}

			iRow++;
		}
		return null;
	}

	public char? Get(Loc loc)
	{
		if (!IsValid(loc))
			return null;
		return _map[loc.Row][loc.Col];
	}
	public int? GetInt(Loc loc)
	{
		var c = Get(loc);
		if (c == null)
			return null;
		return int.Parse(c.ToString());
	}
	public int Count(char target)
	{
		var rv = 0;
		foreach (var row in _map)
		{
			foreach (var c in row)
			{
				if (c == target)
					rv++;
			}
		}
		return rv;
	}

	public void Set(int row, int col, char c)
	{
		_map[row][col] = c;
	}
	public void Set(Loc loc, char c)
	{
		Set(loc.Row, loc.Col, c);
	}

	public override string ToString()
	{
		var outs = new List<string>();
		outs.Add("GridMap");
		foreach (var row in _map)
		{
			outs.Add(string.Join("", row));
		}
		return string.Join(Environment.NewLine, outs);
	}

	public List<Loc> FindAll(char target)
	{
		var rv = new List<Loc>();
		int iRow = 0;
		foreach (var row in _map)
		{
			int iCol = 0;
			foreach (var c in row)
			{
				if (c == target)
					rv.Add(new Loc(iRow, iCol));
				iCol++;
			}

			iRow++;
		}
		return rv;
	}

	public bool IsValid(Loc loc)
	{
		if (loc.Row < 0 || loc.Col < 0)
			return false;
		if (loc.Row >= _map.Count() || loc.Col >= _map[0].Count())
			return false;
		return true;
	}

	public string Text()
	{
		var rv = "";
		foreach (var row in _map)
			rv += string.Join("", row);
		return rv;
	}

}

public class FLoc
{
	public double Row { get; }
	public double Col { get; }
	public FLoc(double row, double col)
	{
		Row = row; Col = col;
	}
	public override string ToString()
	{
		return $"({Row},{Col})";
	}
}
