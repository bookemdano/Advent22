using System.Threading;

namespace AoCLibrary;
public enum DirEnum
{
	N, E, S, W,
	NE,NW,SE,SW
}
public class LocDir(int row, int col, DirEnum dir) : Loc(row, col)
{
    public DirEnum Dir { get; } = dir;

    public LocDir(Loc loc, DirEnum dir) : this(loc.Row, loc.Col, dir)
	{
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
        var rv = new List<LocDir>
        {
            new(Row - 1, Col, DirEnum.N),
            new(Row, Col + 1, DirEnum.E),
            new(Row + 1, Col, DirEnum.S),
            new(Row, Col - 1, DirEnum.W)
        };
        return rv;
    }
    public List<LocDir> All8DirMoves()
    {
        var rv = new List<LocDir>
        {
            new(Row - 1, Col, DirEnum.N),
            new(Row - 1, Col + 1, DirEnum.NE),
            new(Row, Col + 1, DirEnum.E),
            new(Row + 1, Col + 1, DirEnum.SE),
            new(Row + 1, Col, DirEnum.S),
            new(Row + 1, Col - 1, DirEnum.SW),
            new(Row, Col - 1, DirEnum.W),
            new(Row - 1, Col - 1, DirEnum.NW)
        };
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
public class Region(Point ul, Point br)
{
    public Point Ul { get; set; } = ul;
    public Point Br { get; set; } = br;

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
    internal static readonly Point Zero = new Point(0,0);

    public long X { get; }
	public long Y { get; }
	public Point(long x, long y)
	{
		Y = y; X = x;
	}

    public Point(string str)
    {
        var parts = str.Split(",");
        X = int.Parse(parts[0]);
        Y = int.Parse(parts[1]);
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
        var rv = new List<Point>
        {
            new(X, Y - 1),
            new(X + 1, Y),
            new(X, Y + 1),
            new(X - 1, Y)
        };
		return rv;
	}
	public List<PointDir> AllDirMoves()
	{
		var rv = new List<PointDir>
        {
            new(X, Y - 1, DirEnum.N),
            new(X + 1, Y, DirEnum.E),
            new(X, Y + 1, DirEnum.S),
            new(X - 1, Y, DirEnum.W)
        };
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

    internal static List<Point> ReadAll(string[] lines)
    {
        var rv = new List<Point>();
        foreach (var line in lines)
        {
			rv.Add(Parse(line));
        }
		return rv;
    }

    internal double Distance(Point other)
    {
		return Math.Sqrt((other.X - X) * (other.X - X) + (other.Y - Y) * (other.Y - Y));
    }
}



public class Loc
{
    public Loc(string str)
    {
        var parts = str.Split(",");
		Row = int.Parse(parts[0]);
		Col = int.Parse(parts[1]);
    }
    public Loc(int row, int col)
    {
        Row = row; 
		Col = col;
    }
    public Loc(Loc pos) : this(pos.Row, pos.Col)
    {
    }

    public int Row { get; set; } 
	public int Col { get; set; }

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
        var rv = new List<Loc>
        {
            new(Row - 1, Col),
            new(Row, Col + 1),
            new(Row + 1, Col),
            new(Row, Col - 1)
        };
        return rv;
    }
    public List<Loc> All8Moves()
    {
        var rv = new List<Loc>
        {
            new(Row - 1, Col),
            new(Row - 1, Col + 1),
            new(Row, Col + 1),
            new(Row + 1, Col + 1),
            new(Row + 1, Col),
            new(Row + 1, Col - 1),
            new(Row, Col - 1),
            new(Row - 1, Col - 1)
        };
        return rv;
    }
    public Loc Move(DirEnum dir, int amount = 1)
	{
        if (dir == DirEnum.N)
            return new Loc(Row - amount, Col);
        else if (dir == DirEnum.NE)
            return new Loc(Row - amount, Col + amount);
        else if (dir == DirEnum.E)
            return new Loc(Row, Col + amount);
        else if (dir == DirEnum.SE)
            return new Loc(Row + amount, Col + amount);
        else if (dir == DirEnum.S)
            return new Loc(Row + amount, Col);
        else if (dir == DirEnum.SW)
            return new Loc(Row + amount, Col - amount);
        else if (dir == DirEnum.W)
            return new Loc(Row, Col - amount);
        else //if (dir == DirEnum.NW)
            return new Loc(Row - amount, Col - amount);
    }
    public bool Same(Loc other)
	{
		return (Row == other.Row && Col == other.Col);
	}

	public override bool Equals(object? obj)
	{
		if (obj is Loc loc)
			return Same(loc);
		else
			return false;
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
    public int Cols => _map[0].Length;
    public GridMapBase(IEnumerable<string>? lines)
	{
		if (lines == null)
			return;
		foreach (var line in lines)
		{
			_map.Add(line.ToCharArray());
		}
	}
    public GridMapBase(int x, int y)
    {
        for (int row = 0; row < y; row++)
            _map.Add(new string('.', x).ToCharArray());
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
    public override bool Equals(object? obj)
    {
		if (obj is GridMapBase other)
			return other.Text() == Text();
		return false;
    }
    public override int GetHashCode()
    {
		return Text().GetHashCode();
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
    public string GetCol(int col)
    {
		var rv = string.Empty;
		foreach (var row in _map)
			rv += row[col];
		return rv;
    }
    public char[] GetRow(int iRow)
    {
        return _map[iRow];
    }

    public IEnumerable<string> GetRows()
    {
        var rv = new List<string>();
        for (int r = 0; r < Rows; r++)
        {
            rv.Add(new string(GetRow(r)));
        }
        return rv;
    }

}


public class GridMapXY : GridMapBase
{
	public GridMapXY(IEnumerable<string>? lines) : base(lines)
	{

	}

    public GridMapXY(GridMapXY other)
    {
        _map = other._map.Select(row => row.ToArray()).ToList();
    }

    public GridMapXY(int x, int y) : base(x, y)
    {
    }

    public void Set(int x, int y, char c)
    {
        _map[y][x] = c;
    }
    public void Set(Point pt, char c) 
    {
		Set((int) pt.X, (int)pt.Y, c);
    }
    public char? Get(Point pt)
	{
		if (!IsValid(pt))
			return null;
		return DirectGet((int)pt.X, (int)pt.Y);
    }
 
    public char DirectGet(int x, int y)
    {
        return _map[y][x];
    }
    public bool IsValid(Point pt)
	{
		if (pt.Y < 0 || pt.X < 0)
			return false;
		if (pt.Y >= _map.Count || pt.X >= _map[0].Length)
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
	// only works for square
    internal GridMapXY Rotate()
    {
        var rv = new GridMapXY(Cols, Rows);
		for (int r = 0; r < Rows; r++)
			for (int c = 0; c < Cols; c++)
				rv.Set(r, c, DirectGet(c, r));
        //ElfHelper.DayLogPlus(this);
        //ElfHelper.DayLogPlus(rv);
        return rv;
    }
	internal void Union(GridMapXY other, int targetX, int targetY)
	{
        for (int x = 0; x < other.Cols; x++)
        {
            for (int y = 0; y < other.Rows; y++)
            {
				Set(x + targetX, y + targetY, other.DirectGet(x, y));
            }
        }
    }

    internal GridMapXY FlipVert()
    {
        var rv = new GridMapXY(Cols, Rows);
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                rv.Set(r, Cols - c - 1, DirectGet(r, c));
        //ElfHelper.DayLogPlus(this);
        //ElfHelper.DayLogPlus(rv);
        return rv;
    }

    internal GridMapXY FlipHorz()
    {
        var rv = new GridMapXY(Cols, Rows);
        for (int r = 0; r < Rows; r++)
            for (int c = 0; c < Cols; c++)
                rv.Set(Rows - r - 1, c, DirectGet(r, c));
        //ElfHelper.DayLogPlus(this);
        //ElfHelper.DayLogPlus(rv);
        return rv;
    }
}
public class GridMap : GridMapBase
{
    public GridMap(IEnumerable<string>? lines) : base(lines)
    {

    }
    public GridMap(int rows, int cols) : base(cols, rows)
    {
    }

    public GridMap()
    {

    }

    public GridMap(GridMap other)
    {
		_map = other._map.Select(r => r.ToArray()).ToList();
    }

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
		return c - '0';
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
		if (loc.Row >= _map.Count || loc.Col >= _map[loc.Row].Length)
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

    public void SetInt(Loc loc, int val)
    {
        Set(loc, (char)(val + (int) '0'));
    }
    public void IncInt(Loc loc)
    {
		var v = GetInt(loc);
		if (v != null)
			SetInt(loc, (int) v + 1);
    }
}

public class FLoc(double row, double col)
{
    public double Row { get; } = row; public double Col { get; } = col;

    public override string ToString()
	{
		return $"({Row},{Col})";
	}
}
public class Point3D(long x, long y, long z) : IEquatable<Point3D>
{
    static public Point3D FromXYZ(string str)
    {
        var parts = Utils.SplitLongs(',', str);
        return new Point3D(parts[0], parts[1], parts[2]);
    }

    public long X { get; set; } = x;
    public long Y { get; set; } = y;
    public long Z { get; set; } = z;
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
		var dx = other.X - X;
		var dy = other.Y - Y;
		var dz = other.Z - Z;
		return Math.Sqrt(dx * dx + dy * dy + dz * dz);
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
