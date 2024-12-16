using AoCLibrary;

namespace Advent24;

internal class Day16 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/16
	// Input https://adventofcode.com/2024/day/16/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 11048L);
		else
			check = new StarCheck(key, 65436L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var map = new Grid16(lines);
		var start = new LocDir(map.Find('S')!, DirEnum.E);
		var paths = new List<Path16>() { new Path16(start) };
		var dones = new List<Path16>();
		var explored = new Dictionary<LocDir, long>();
		while(paths.Any())
		{
			var newPaths = new List<Path16>();
			foreach (var path in paths)
			{
				//map.Draw(path, '0');
				if (map.Get(path.Head) == 'E')
					dones.Add(path);
				else
					newPaths.AddRange(map.Step(path));
			}
			paths = new List<Path16>();
			foreach (var newPath in newPaths.OrderBy(p => p.Score))
			{
				if (explored.ContainsKey(newPath.Head))
				{
					if (explored[newPath.Head] < newPath.Score)
						continue;
				}
				explored[newPath.Head] = newPath.Score;
				if (!paths.Any(p => p.Head.SameDir(newPath.Head)))
					paths.Add(newPath);
			}
		}
		rv = dones.Min(d => d.Score);

		check.Compare(rv);
		return rv;
	}
	public class Path16
	{
		public List<Loc> Locs { get; set; } = [];
		public LocDir Head { get; set; }
		public long Score;
		public Path16(LocDir head)
		{
			Head = head;
			Locs.Add(head);
		}

		internal Path16 Copy(LocDir move)
		{
			var rv = new Path16(move);
			rv.Score = Score;
			rv.Score += 1;
			if (move.Dir != Head.Dir)
				rv.Score += 1000;
			rv.Locs = Locs.ToList();	// this overwrites locs
			rv.Locs.Add(move);
			return rv;
		}

		public bool Same(Path16 other)
		{
			if (!other.Head.Same(Head))
				return false;
			if (other.Locs.Count() != Locs.Count())
				return false;
			for(int i = 0; i < Locs.Count(); i++)
			{
				if (!other.Locs[i].Same(Locs[i]))
					return false;
			}

			return true;
		}
		public override string ToString()
		{
			return $"{Head} s:{Score} l:{Locs.Count()}";
		}

		internal void Update(LocDir next)
		{
			Score += 1;
			if (next.Dir != Head.Dir)
				Score += 1000;
			Locs.Add(next);
			Head = next;
		}
	}
	public class Grid16 : GridMap
	{
		public Grid16(IEnumerable<string>? lines) : base(lines)
		{
		}
		internal List<LocDir> OtherValid(LocDir head)
		{
			var rv = new List<LocDir>(2);
			foreach (var dir in Enum.GetValues<DirEnum>())
			{
				if (dir != head.Dir && dir != LocDir.Opposite(head.Dir))
				{
					var locDir = head.DirMove(dir)!;
					if (!IsWall(locDir))
						rv.Add(locDir);
				}
			}
			return rv;
		}
		internal bool IsWall(Loc loc)
		{
			return Get(loc) == '#';
		}
		internal IEnumerable<Path16> Step(Path16 path)
		{
			var rv = new List<Path16>();
			while(true)
			{
				var moves = OtherValid(path.Head);
				if (moves.Any())
				{
					foreach(var move in moves)
						rv.Add(path.Copy(move));
				}
				var next = path.Head.DirMove();
				if (IsWall(next))
					break;
				path.Update(next);
			}

			if (Get(path.Head) == 'E')
				rv.Add(path);
			return rv;
		}

		internal void Draw(List<Path16> paths)
		{
			var lines = ToString().Split(Environment.NewLine);
			var c = '0';
			foreach(var path in paths)
			{
				foreach (var loc in path.Locs)
				{
					var row = lines[loc.Row].ToCharArray();
					row[loc.Col] = c;
					lines[loc.Row] = new string(row);
				}
				c++;
			}
			foreach (var line in lines)
				Console.WriteLine(line);
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 64L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var map = new Grid16(lines);
		var end = map.Find('E')!;
		var start = new LocDir(map.Find('S')!, DirEnum.E);
		var paths = new List<Path16>() { new Path16(start) };
		var dones = new List<Path16>();
		var heads = new Dictionary<LocDir, long>();
		var next = DateTime.Now;
		while (paths.Any())
		{
			var min = paths.Min(p => p.Score);
			var newPaths = new List<Path16>();
			foreach (var path in paths)
			{
				if (end.Same(path.Head))
					dones.Add(path);
				else
				{
					if (!IsReal && path.Score > 11048L)
						continue;
					if (IsReal && path.Score > 65436L)
						continue;
					newPaths.AddRange(map.Step(path));
				}
			}
			paths = new List<Path16>();
			var groups = newPaths.GroupBy(p => p.Head);
			foreach (var group in groups)
			{
				var minScore = group.Min(g => g.Score);
				if (heads.ContainsKey(group.Key))
				{
					if (heads[group.Key] < minScore)
						continue;
				}
				heads[group.Key] = minScore;
				paths.AddRange(group.Where(p => p.Score == minScore));
			}
			if (DateTime.Now > next)
			{
				Console.WriteLine($"{paths.Count()} {paths.Min(p => p.Score)} {paths.Max(p => p.Score)}");
				ElfHelper.DayLog($"{paths.Count()} {paths.Min(p => p.Score)} {paths.Max(p => p.Score)}");
				next = DateTime.Now.AddSeconds(5);
			}
		}
		var c = '0';
		map.Draw(dones);
		var seats = new List<Loc>();
		foreach (var done in dones)
			foreach(var loc in done.Locs)
			{
				if (!seats.Any(s => s.Same(loc)))
					seats.Add(loc);
			}

		rv = seats.Count();

		check.Compare(rv);
		return rv;
	}
}

