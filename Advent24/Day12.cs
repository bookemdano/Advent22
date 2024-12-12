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
			foreach (var inside in this)
			{
				var moves = inside.AllMoves();

				var outsides = moves.Where(m => !ContainsSame(m));
				foreach(var outside in outsides)
					sides.Add(Side.Between(inside, outside));
			}

			var verts = sides.Where(s => s.SideDir == SideDirEnum.Vert);
			foreach (var sideVerts in verts.GroupBy(v => v.Col))
			{
				var sets = sideVerts.OrderBy(v => v.Row).ToList();
				double? last = null;
				foreach(var set in sets)
				{
					if (last == null || last + 1 != set.Row)
						rv++;
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
					if (last == null || last + 1 != set.Col)
						rv++;
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
	public class Side : FLoc
	{
		public SideDirEnum SideDir => ((int)Row == Row)?SideDirEnum.Vert:SideDirEnum.Horz;

		public Side(double row, double col) : base(row, col)
		{
		}
		static public Side Between(Loc inside, Loc outside)
		{
			if (inside.Row == outside.Row)
			{
				var min = Math.Min(inside.Col, outside.Col);
				var max = Math.Max(inside.Col, outside.Col);
				if (inside.Col < outside.Col)
					return new Side(inside.Row, min + (max - min) / 4.0);
				else
					return new Side(inside.Row, min + (max - min) * 3 / 4.0);
			}
			if (inside.Col == outside.Col)
			{
				var min = Math.Min(inside.Row, outside.Row);
				var max = Math.Max(inside.Row, outside.Row);
				if (inside.Row < outside.Row)
					return new Side(min + (max - min) / 4.0, inside.Col);
				else
					return new Side(min + (max - min) * 3 / 4.0, inside.Col);
			}
			var deltaR = inside.Row - outside.Row;
			var deltaC = inside.Col - outside.Col;

			return new Side(inside.Row - deltaR / 2.0, outside.Col - deltaC / 2.0);
		}
		public override string ToString()
		{
			return $"{base.ToString()} {SideDir}";
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

