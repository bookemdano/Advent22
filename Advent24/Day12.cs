using AoCLibrary;

namespace Advent24;

internal class Day12 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/12
	// Input https://adventofcode.com/2024/day/12/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 1930L);
		else
			check = new StarCheck(key, 1359028L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
		GetAreas(map);
		foreach(var area in _areas)
			rv += area.Perimeter() * area.Count();

		check.Compare(rv);
		return rv;
	}
	List<Area> _areas;
	public void GetAreas(GridMap map)
	{
		_areas = new List<Area>();
		var used = new Area();
		for (int iRow = 0; iRow < map.Rows; iRow++)
		{
			for (int iCol = 0; iCol < map.Cols; iCol++)
			{
				var rc = new Loc(iRow, iCol);
				if (used.Any(l => l.Same(rc)))
					continue;

				var c = map.Get(rc);
				var heads = new List<Loc>() { rc };
				var area = new Area() { rc };
				area.C = c;
				used.Add(rc);
				while (heads.Count() > 0)
				{
					var news = new List<Loc>();
					foreach (var head in heads)
					{
						var moves = head.AllMoves();
						foreach (var move in moves)
						{
							if (used.ContainsSame(move))
								continue;
							if (c != map.Get(move))
								continue;
							area.Add(move);
							used.Add(move);
							news.Add(move);
						}
					}
					heads = news;
				}
				_areas.Add(area);
			}
		}
	}
	public class Area : List<Loc>
	{
		public char? C { get; set; }
		internal bool ContainsSame(Loc loc)
		{
			return this.Any(l => l.Same(loc));
		}
		internal int Perimeter()
		{
			var rv = 0;
			foreach (var loc in this)
			{
				var moves = loc.AllMoves();
				rv += moves.Count(m => !ContainsSame(m));
			}
			return rv;
		}
		internal int Sides()
		{
			var rv = 0;
			var sides = new List<Side>();
			foreach (var loc in this)
			{
				var moves = loc.AllMoves();

				var outsides = moves.Where(m => !ContainsSame(m));
				foreach(var outside in outsides)
				{
					sides.Add(Side.Average(loc, outside));
				}
			}
			var verts = sides.Where(s => s.SideDir == SideDirEnum.Vert);
			foreach (var sideVerts in verts.GroupBy(v => v.Col))
			{
				var sets = sideVerts.OrderBy(v => v.Row).ToList();
				double? last = null;
				foreach(var set in sets)
				{
					if (last == null)
					{
						rv++;
					}
					else if (last + 1 != set.Row)
					{
						rv++;
					}
					last = set.Row;
				}
			}
			var horzs = sides.Where(s => s.SideDir == SideDirEnum.Horz);
			foreach (var sideHorzs in horzs.GroupBy(v => v.Row))
			{
				var sets = sideHorzs.OrderBy(v => v.Col).ToList();
				double? last = null;
				foreach (var set in sets)
				{
					if (last == null)
					{
						rv++;
					}
					else if (last + 1 != set.Col)
					{
						rv++;
					}
					last = set.Col;
				}
			}

			return rv;
		}
	}

	/*
AAAAAA	fake with hole 368 phase 2
AAABBA
AAABBA
ABBAAA
ABBAAA
AAAAAA
*/
	public enum SideDirEnum
	{
		Vert,
		Horz
	}
	public class Side
	{
		public double Row { get; }
		public double Col { get; }
		public SideDirEnum SideDir { get; }

		public Side(double row, double col, SideDirEnum sideDir)
		{
			Row = row; Col = col; SideDir = sideDir;
		}
		static public Side Average(Loc lh, Loc rh)
		{
			if (lh.Row == rh.Row)
			{
				var min = Math.Min(lh.Col, rh.Col);
				var max = Math.Max(lh.Col, rh.Col);
				if (lh.Col < rh.Col)
					return new Side(lh.Row, min + (max - min) / 4.0, SideDirEnum.Vert);
				else
					return new Side(lh.Row, min + (max - min) * 3 / 4.0, SideDirEnum.Vert);
			}
			if (lh.Col == rh.Col)
			{
				var min = Math.Min(lh.Row, rh.Row);
				var max = Math.Max(lh.Row, rh.Row);
				if (lh.Row < rh.Row)
					return new Side(min + (max - min) / 4.0, lh.Col, SideDirEnum.Horz);
				else
					return new Side(min + (max - min) * 3 / 4.0, lh.Col, SideDirEnum.Horz);
			}
			var deltaR = lh.Row - rh.Row;
			var deltaC = lh.Col - rh.Col;

			return new Side(lh.Row - deltaR / 2.0, rh.Col - deltaC / 2.0, SideDirEnum.Horz);
		}
		public override string ToString()
		{
			return $"({Row},{Col} {SideDir})";
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 1206L);
		else
			check = new StarCheck(key, 839780L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
		//GetAreas(map);
		foreach (var area in _areas)
			rv += area.Sides() * area.Count();
		// wrong 834546
		check.Compare(rv);
		return rv;
	}
}

