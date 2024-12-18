using AoCLibrary;

namespace Advent24;

internal class Day18 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/18
	// Input https://adventofcode.com/2024/day/18/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 22L);
		else
			check = new StarCheck(key, 286L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var take = 1024;
		var size = 71;
		if (!IsReal)
		{
			take = 12;
			size = 7;
		}
		var map = new Map18(size, size);
		var end = new Point(size - 1, size - 1);
		map.Add(lines.Take(take));
		Console.WriteLine(map.ToString());
		var trails = new List<Trail18>() { new Trail18(new Point(0, 0)) };
		var used = new List<Point>() { new Point(0, 0) };
		var steps = 0;
		while(trails.Any() && rv == 0)
		{
			var newTrails = new List<Trail18>();
			foreach(var trail in trails)
				newTrails.AddRange(map.Step(trail));
			trails = [];
			Console.WriteLine($"NewSet s:{steps++} t:{newTrails.Count}");
			foreach(var newTrail in newTrails)
			{
				//map.Draw(newTrail);
				if (newTrail.Tail.Same(end))
				{
					map.Draw(newTrail);
					rv = newTrail.Points.Count() - 1;
					break;
				}
				if (!used.Contains(newTrail.Tail))
				{
					trails.Add(newTrail);
					used.Add(newTrail.Tail);
				}
			}

		}

		check.Compare(rv);
		return rv;
	}
	public class Trail18
	{
		public List<Point> Points { get; } = [];
		public Trail18(Point point)
		{
			Points.Add(point);
		}
		public Point Tail => Points.Last();
		public Trail18(Trail18 other, Point move)
		{
			Points = other.Points.ToList();
			Points.Add(move);
		}

		public override string ToString()
		{
			return $"t:{Tail} c:{Points.Count()}";
		}
	}
	public class Map18 : GridMapXY
	{
		public Point Size { get; }
		public Map18(int x, int y) : base(x, y)
		{
			Size = new Point(x, y);
		}

		internal void Add(IEnumerable<string> lines)
		{
			foreach(var line in lines)
			{
				var p = Point.Parse(line);
				Set(p, '#');
			}
		}

		internal IEnumerable<Trail18> Step(Trail18 trail)
		{
			var head = trail.Points.Last();
			var moves = head.AllMoves();
			var rv = new List<Trail18>();
			foreach(var move in moves)
			{
				var c = Get(move);
				if (c == '.' && !trail.Points.Any(t => t.Same(move)))
					rv.Add(new Trail18(trail, move));
			}
			return rv;
		}

		internal void Draw(Trail18 trail)
		{
			var text = this.ToString();
			var lines = text.Split(Environment.NewLine);
			foreach (var pt in trail.Points)
				GridMapXY.DrawOnText(lines, pt, '0');

			Console.WriteLine(trail);
			foreach (var line in lines)
				Console.WriteLine(line);
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, "6,1");
		else
			check = new StarCheck(key, "20,64");

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		string rv = string.Empty;
		// magic
		var start = 1024;
		var size = 71;
		if (!IsReal)
		{
			start = 12;
			size = 7;
		}
		var map = new Map18(size, size);
		var end = new Point(size - 1, size - 1);
		Trail18? last = null;
		for(int i = start; i < lines.Count(); i++)
		{
			var sublines = lines.Take(i);

			if (last != null)
			{
				var lastPt = Point.Parse(sublines.Last());
				if (!last.Points.Contains(lastPt))
					continue;
			}
			Console.WriteLine("Take " + i);
			map.Add(sublines);
			//Console.WriteLine(map.ToString());
			var trails = new List<Trail18>() { new Trail18(new Point(0, 0)) };
			var used = new List<Point>() { new Point(0, 0) };
			var steps = 0;
			var shortest = 0;
			while (trails.Any() && shortest == 0)
			{
				var newTrails = new List<Trail18>();
				foreach (var trail in trails)
					newTrails.AddRange(map.Step(trail));
				trails = [];
				//Console.WriteLine($"NewSet s:{steps++} t:{newTrails.Count}");
				foreach (var newTrail in newTrails)
				{
					//map.Draw(newTrail);
					if (newTrail.Tail.Same(end))
					{
						//map.Draw(newTrail);
						last = newTrail;
						shortest = newTrail.Points.Count() - 1;
						break;
					}
					if (!used.Contains(newTrail.Tail))
					{
						trails.Add(newTrail);
						used.Add(newTrail.Tail);
					}
				}
			}
			if (shortest == 0)
			{
				rv = sublines.Last();
				break;
			}
		}
		check.Compare(rv);
		return rv;
	}
}

