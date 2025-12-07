
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace AoCLibrary;
public enum DirEnum
{
	N, E, S, W,
	NE,NW,SE,SW
}
public class LocDir : Loc
{
	public DirEnum Dir { get; }

	public LocDir(Loc loc, DirEnum dir) : this(loc.Row, loc.Col, dir)
	{
	}
	public LocDir(int row, int col, DirEnum dir) : base(row, col)
	{
		Dir = dir;
	}
	public override bool Equals(object? obj)
	{
		if (obj as LocDir == null)
			return false;
		return SameDir((obj as LocDir)!);
	}
	public override int GetHashCode()
	{
		return ((int)Dir)*1000000 + Row*1000 + Col;
	}
	public override string ToString()
	{
		return $"{base.ToString()}{Dir}";
	}
	public bool SameDir(LocDir other)
	{
		return (Row == other.Row && Col == other.Col && Dir == other.Dir);
	}
    public List<LocDir> AllDirMoves()
    {
        var rv = new List<LocDir>();
        rv.Add(new LocDir(Row - 1, Col, DirEnum.N));
        rv.Add(new LocDir(Row, Col + 1, DirEnum.E));
        rv.Add(new LocDir(Row + 1, Col, DirEnum.S));
        rv.Add(new LocDir(Row, Col - 1, DirEnum.W));
        return rv;
    }
    public List<LocDir> All8DirMoves()
    {
        var rv = new List<LocDir>();
        rv.Add(new LocDir(Row - 1, Col, DirEnum.N));
        rv.Add(new LocDir(Row - 1, Col + 1, DirEnum.NE));
        rv.Add(new LocDir(Row, Col + 1, DirEnum.E));
        rv.Add(new LocDir(Row + 1, Col + 1, DirEnum.SE));
        rv.Add(new LocDir(Row + 1, Col, DirEnum.S));
        rv.Add(new LocDir(Row + 1, Col - 1, DirEnum.SW));
        rv.Add(new LocDir(Row, Col - 1, DirEnum.W));
        rv.Add(new LocDir(Row - 1, Col - 1, DirEnum.NW));
        return rv;
    }
    public LocDir DirMove()
	{
		return DirMove(Dir);
	}

	public LocDir DirMove(DirEnum dir)
	{
		return new LocDir(Move(dir), dir);
	}

	internal static DirEnum? TryParseDir(char? c)
	{
		if (c == '^')
			return DirEnum.N;
		else if (c == '>')
			return DirEnum.E;
		else if (c == 'v')
			return DirEnum.S;
		else if (c == '<')
			return DirEnum.W;
		else
			return null;
	}

	internal static DirEnum ParseDir(char c)
	{
		if (c == '^')
			return DirEnum.N;
		else if (c == '>')
			return DirEnum.E;
		else if (c == 'v')
			return DirEnum.S;
		else //if (c == '<')
			return DirEnum.W;
	}

	internal static char DirChar(DirEnum dir)
	{
		if (dir == DirEnum.N)
			return '^';
		if (dir == DirEnum.E)
			return '>';
		if (dir == DirEnum.S)
			return 'v';
		else
			return '<';
	}

	internal static DirEnum Opposite(DirEnum dir)
	{
		if (dir == DirEnum.N)
			return DirEnum.S;
		else if (dir == DirEnum.S)
			return DirEnum.N;
		else if (dir == DirEnum.E)
			return DirEnum.W;
		else //if (dir == DirEnum.S)
			return DirEnum.E;
	}
}
public class DirDist
{
	public DirDist(DirEnum dir, long dist)
	{
		Dir = dir;
		Dist = dist;
	}

	static internal List<DirDist> FindDirs(Point from, Point target)
	{
		var rv = new List<DirDist>();
		var current = from;
		while (!current.Same(target))
		{
			var dirDist = FindDir(current, target);
			current = current.Move(dirDist);
			rv.Add(dirDist);
		}
		return rv;
	}

	static internal DirDist FindDir(Point from, Point target)
	{
		if (target.X > from.X)
			return new DirDist(DirEnum.E, target.X - from.X);
		else if (from.X > target.X)
			return new DirDist(DirEnum.W, from.X - target.X);
		else if (target.Y > from.Y)
			return new DirDist(DirEnum.S, target.Y - from.Y);
		else if (from.Y > target.Y)
			return new DirDist(DirEnum.N, from.Y - target.Y);
		else
			return new DirDist(DirEnum.E, 0);
	}

	public DirEnum Dir { get; set; }
	public long Dist { get; set; }
	public string Chars()
	{
		return new string(LocDir.DirChar(Dir), (int) Dist);
	}

	public override string ToString()
	{
		return $"{Dir}{Dist}";
	}

	internal bool Same(DirDist other)
	{
		return Dir == other.Dir && Dist == other.Dist;
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
public class PointDir : Point
{
	public PointDir(long x, long y, DirEnum dir) : base(x, y)
	{
		Dir = dir;
	}

	public DirEnum Dir { get; }
	
}
public class Point
{
	public long X { get; }
	public long Y { get; }
	public Point(long x, long y)
	{
		Y = y; X = x;
	}

	public Point(Loc loc)
	{
		Y = loc.Row;
		X = loc.Col;
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
	public List<PointDir> AllDirMoves()
	{
		var rv = new List<PointDir>();
		rv.Add(new PointDir(X, Y - 1, DirEnum.N));
		rv.Add(new PointDir(X + 1, Y, DirEnum.E));
		rv.Add(new PointDir(X, Y + 1, DirEnum.S));
		rv.Add(new PointDir(X - 1, Y, DirEnum.W));
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
	internal Point Move(List<DirDist> dirDists)
	{
		var pt = this; 
		foreach(var dirDist in dirDists)
			pt = pt.Move(dirDist);
		return pt;
	}

	public Point Move(DirDist dirDist)
	{
		if (dirDist.Dir == DirEnum.N)
			return new Point(X, Y - dirDist.Dist);
		else if (dirDist.Dir == DirEnum.E)
			return new Point(X + dirDist.Dist, Y);
		else if (dirDist.Dir == DirEnum.S)
			return new Point(X, Y + dirDist.Dist);
		else //if (dir == DirEnum.W)
			return new Point(X - dirDist.Dist, Y);
	}
	public bool Same(Point other)
	{
		return (Y == other.Y && X == other.X);
	}

	internal Point Copy()
	{
		return new Point(X, Y);
	}

	internal static Point Parse(string line)
	{
		var parts = line.Split(',');
		return new Point(long.Parse(parts[0]), long.Parse(parts[1]));
	}
	public override bool Equals(object? obj)
	{
		if (obj is Point)
			return Same((obj as Point)!);
		return false;
	}
	public override int GetHashCode()
	{
		return (int) X * 1000 + (int) Y;
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
    public List<Loc> All8Moves()
    {
        var rv = new List<Loc>();
        rv.Add(new Loc(Row - 1, Col));
        rv.Add(new Loc(Row - 1, Col + 1));
        rv.Add(new Loc(Row, Col + 1));
        rv.Add(new Loc(Row + 1, Col + 1));
        rv.Add(new Loc(Row + 1, Col));
        rv.Add(new Loc(Row + 1, Col - 1));
        rv.Add(new Loc(Row, Col - 1));
        rv.Add(new Loc(Row - 1, Col - 1));
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

	public override bool Equals(object? obj)
	{
		if (obj as Loc == null)
			return false;
		return Same(obj as Loc);
	}
}
public enum MoveEnum
{
	Good,
	OffMap,
	Loop
}
public class GridMapBase
{
	protected List<char[]> _map = [];
	protected GridMapBase()
	{

	}
	public int Rows => _map.Count;
	public GridMapBase(IEnumerable<string>? lines)
	{
		if (lines == null)
			return;
		foreach (var line in lines)
		{
			_map.Add(line.ToCharArray());
		}
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

	public override string ToString()
	{
		var outs = new List<string>();
		//outs.Add("GridMap");
		foreach (var row in _map)
		{
			outs.Add(string.Join("", row));
		}
		return string.Join(Environment.NewLine, outs);
	}
	public string Text()
	{
		var rv = "";
		foreach (var row in _map)
			rv += string.Join("", row);
		return rv;
	}

	protected Loc? FindRC(char target)
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
    internal string GetCol(int col)
    {
		var rv = string.Empty;
		foreach (var row in _map)
			rv += row[col];
		return rv;
    }
    internal char[] GetRow(int iRow)
    {
        return _map[iRow];
    }

}


public class GridMapXY : GridMapBase
{
	public GridMapXY(IEnumerable<string>? lines) : base(lines)
	{

	}
	public GridMapXY(int x, int y)
	{
		for (int row = 0; row < y; row++)
			_map.Add(new string('.', x).ToCharArray());
	}
	public void Set(Point pt, char c)
	{
		_map[(int) pt.Y][pt.X] = c;
	}
	public char? Get(Point pt)
	{
		if (!IsValid(pt))
			return null;
		return _map[(int) pt.Y][pt.X];
	}
	public bool IsValid(Point pt)
	{
		if (pt.Y < 0 || pt.X < 0)
			return false;
		if (pt.Y >= _map.Count() || pt.X >= _map[0].Count())
			return false;
		return true;
	}

	protected static void DrawOnText(string[] lines, Point pt, char c)
	{
		var row = lines[pt.Y].ToCharArray();
		row[pt.X] = c;
		lines[pt.Y] = new string(row);
	}
	internal Point? Find(char target)
	{
		var rv = FindRC(target);
		if (rv == null)
			return null;
		return new Point(rv);
	}

}
public class GridMap : GridMapBase
{
    public GridMap(IEnumerable<string>? lines) : base(lines)
    {

    }
    public GridMap()
	{

	}
    public GridMap(GridMap other)
    {
		_map = other._map.Select(r => r.ToArray()).ToList();
    }
    public int Rows => _map.Count();
	public int Cols => _map[0].Count();

	public char? Get(int iRow, int iCol)
	{
		return Get(new Loc(iRow, iCol));
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

	public void Set(int row, int col, char c)
	{
		_map[row][col] = c;
	}
	public void Set(Loc loc, char c)
	{
		Set(loc.Row, loc.Col, c);
	}
	internal Loc? Find(char target)
	{
		return FindRC(target);
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
		if (loc.Row >= _map.Count() || loc.Col >= _map[loc.Row].Count())
			return false;
		return true;
	}

    internal string FindSurroundingChars(Loc loc)
    {
		var moves = loc.All8Moves();
		var rv = "";
        foreach (var move in moves)
		{
			if (IsValid(move))
				rv += Get(move);
		}
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
