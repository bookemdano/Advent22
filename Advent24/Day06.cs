using AoCLibrary;

namespace Advent24;

internal class Day06 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/6
	// Input https://adventofcode.com/2024/day/6/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 41L);
		else
			check = new StarCheck(key, 5145L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
		map.Solve();
		rv = map.Count('X');

		check.Compare(rv);
		return rv;
	}

	public enum DirEnum
	{
		N,E,S,W
	}
	public class Loc
	{
		public int Row { get; set; }
		public int Col { get; set; }
		public DirEnum Dir { get; }

		public Loc(int row, int col, DirEnum dir)
		{
			Row = row; Col = col; Dir = dir;
		}
		public override string ToString()
		{
			return $"({Row},{Col}){Dir}";
		}
		public Loc Move()
		{
			if (Dir == DirEnum.N)
				return new Loc(Row - 1, Col, Dir);
			else if (Dir == DirEnum.E)
				return new Loc(Row, Col + 1, Dir);
			else if (Dir == DirEnum.S)
				return new Loc(Row + 1, Col, Dir);
			else// if (Dir == DirEnum.W)
				return new Loc(Row, Col - 1, Dir);
		}

		internal Loc Turn()
		{
			if (Dir == DirEnum.N)
				return new Loc(Row, Col, DirEnum.E);
			else if (Dir == DirEnum.E)
				return new Loc(Row, Col, DirEnum.S);
			else if (Dir == DirEnum.S)
				return new Loc(Row, Col, DirEnum.W);
			else// if (Dir == DirEnum.W)
				return new Loc(Row, Col, DirEnum.N);
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
		Loc _guard;
		public GridMap(string[]? lines)
		{
			foreach(var line in lines)
			{
				_map.Add(line.ToCharArray());
			}
			_guard = FindGuard();
			Set(_guard.Row, _guard.Col, 'X');
		}
		internal Loc FindGuard()
		{
			int iRow = 0;
			foreach(var row in _map)
			{
				int iCol = 0;
				foreach (var c in row)
				{
					if (c == '^')
						return new Loc(iRow, iCol, DirEnum.N);
					iCol++;
				}

				iRow++;
			}
			return new Loc(0, 0, DirEnum.N);
		}
		
		internal MoveEnum Move()
		{
			_guardHistory.Add(_guard);
			var pot = _guard.Move();
			if (_guardHistory.Any(p => p.Row == pot.Row && p.Col == pot.Col && p.Dir == pot.Dir))
				return MoveEnum.Loop;
			var c = Get(pot);
			if (c == null)
				return MoveEnum.OffMap;
			else if (c == 'X' || c == '.')
			{
				_guard = pot;
				Set(_guard.Row, _guard.Col, 'X');
			}
			else if (c == '#' || c == '0')
				_guard = _guard.Turn();
			return MoveEnum.Good;
		}
		List<Loc> _guardHistory = [];
		char? Get(Loc loc)
		{
			if (loc.Row < 0 || loc.Col < 0)
				return null;
			if (loc.Row >= _map.Count() || loc.Col >= _map[0].Count())
				return null;
			return _map[loc.Row][loc.Col];
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

		internal void Set(int row, int col, char c)
		{
			_map[row][col] = c;
		}
		public MoveEnum Solve()
		{
			while (true)
			{
				var res = Move();
				if (res != MoveEnum.Good)
					return res;
			}
		}

		internal List<Loc> Candidates()
		{
			int iRow = 0;
			var rv = new List<Loc>();
			foreach (var row in _map)
			{
				int iCol = 0;
				foreach (var c in row)
				{
					if (c == 'X')
						rv.Add(new Loc(iRow, iCol, DirEnum.N));
					iCol++;
				}
				iRow++;
			}
			return rv;
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 6L);
		else
			check = new StarCheck(key, 1523L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		var mapPrime = new GridMap(lines);
		mapPrime.Solve();
		var candidates = mapPrime.Candidates();
		// magic
		foreach(var candidate in candidates)
		{
			var map = new GridMap(lines);
			map.Set(candidate.Row, candidate.Col, '0');
			var res = map.Solve();
			if (res == MoveEnum.Loop)
				rv++;
		}

		check.Compare(rv);
		return rv;
	}
}

