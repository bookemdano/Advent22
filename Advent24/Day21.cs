using AoCLibrary;
using static Advent24.Day13;
using static Advent24.Day21;

namespace Advent24;

internal class Day21 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2024/day/21
	// Input https://adventofcode.com/2024/day/21/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 126384L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var keymap = new Map21(new List<string>() { "789", "456", "123", " 0A" });
		var dirmap = new Map21(new List<string>() { " ^A", "<v>" });

		var test = "<vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A";
		var r1 = new Remote21("human", dirmap, null).Convert(test);
		var r2 = new Remote21("r1",dirmap, null).Convert(r1);
		var kb = new Remote21("keys", keymap, null).Convert(r2);

		foreach (var line in lines)
		{
			var human = new Remote21("human", dirmap, null);
			var robot1 = new Remote21("r1", dirmap, human);
			var keypad = new Trail21(new Point(2, 3));
			foreach (var c in line)
			{
				var targKey = keymap.Find(c)!;
				if (keypad.Head == targKey)
					break;
				var dirDists = DirDist.FindDirs(keypad.Head, targKey);
				robot1.MoveTo(dirDists);
				/*foreach (var dirDist in dirDists)
				{
					human.MoveTo(LocDir.DirChar(dirDist.Dir), dirDist.Same(dirDists.Last()));
				}*/
				keypad.Move(dirDists, keymap);
				keypad.Push(keymap);
			}
		}

		check.Compare(rv);
		return rv;
	}
	public class Map21 : GridMapXY
	{
		public Map21(IEnumerable<string>? lines) : base(lines)
		{
		}
	}
	public class Remote21
	{
		private Trail21 _trail;
		private string _name;
		private Map21 _map;

		public Remote21? Outer { get; set; }
		public Remote21(string name, Map21 map, Remote21? outer)
		{
			_name = name;
			_map = map;
			Outer = outer;
			_trail = new Trail21(map.Find('A')!);
		}

		internal void MoveTo(List<DirDist> dists)
		{
			foreach(var dist in  dists)
			{
				var c = LocDir.DirChar(dist.Dir);
				var moves = MovesToThis(_map, _trail.Head, c);
				if (Outer != null)
				{
					var target = _trail.Head;
					Outer.MoveTo(moves);
				}
				_trail.Move(moves, _map);
				for(int i = 0; i < dist.Dist; i++)
					_trail.Push(_map);
			}
			var aMoves = MovesToThis(_map, _trail.Head, 'A');
			if (Outer != null)
			{
				var target = _trail.Head;
				Outer.MoveTo(aMoves);
			}
			_trail.Move(aMoves, _map);
			_trail.Push(_map);
		}
		public override string ToString()
		{
			return _name + " " + _trail.ToString();
		}

		internal string Convert(string test)
		{
			var rv = string.Empty;
			var pt = _trail.Head;
			foreach(var c in test)
			{
				if (c == 'A')
					rv += _map.Get(pt);
				else
					pt = pt.Move(LocDir.ParseDir(c));
			}
			return rv;
		}
	}
	private static void MoveLast(GridMapXY dirpad, DirDist aMove1, Trail21 remote2)
	{
		var moves2 = MovesToThis(dirpad, remote2.Head, LocDir.DirChar(aMove1.Dir));
		remote2.Move(moves2, dirpad);

		var aMoves2 = MovesToThis(dirpad, remote2.Head, 'A');
		remote2.Move(aMoves2, dirpad);
	}

	private static List<DirDist> MovesToThis(GridMapXY map, Point from, char targetC)
	{
		var target = map.Find(targetC)!;
		return DirDist21.FindDirs21(from, target, map.Find(' ')!);
	}

	public class Trail21
	{
		public Trail21(Point head)
		{
			Head = head;
		}

		public Point Head { get; set; }
		public long Length => Moves.Count;
		public List<DirDist> Moves { get; set; } = [];
		string _chars = string.Empty;
		string _pushes = string.Empty;
		internal void Move(List<DirDist> moves, GridMapXY? map)
		{
			Moves.AddRange(moves);
			foreach (var move in moves)
			{
				Head = Head.Move(move);
				_chars += move.Chars();
			}
			/*var c = map.Get(Head);
			var dir = LocDir.TryParseDir(c);
			if (dir == null)
				_chars += c ?? '?';
			else
				_chars += dir.ToString();
			*/
			//_pushes += map.Get(Head) ?? '?';
		}
		public override string ToString()
		{
			return $"{_chars}";
		}

		internal void Push(Map21 map)
		{
			var c = map.Get(Head);
			var dir = LocDir.TryParseDir(c);
			if (dir == null)
				_chars += c ?? '?';
			else
				_chars += dir.ToString();
			_pushes += map.Get(Head) ?? '?';
		}
	}
	public class DirDist21
	{
		static internal List<DirDist> FindDirs21(Point from, Point target, Point danger)
		{
			var rv = new List<DirDist>();
			var current = from;
			while (!current.Same(target))
			{
				var dirDist = FindDir21(current, target, danger);
				current = current.Move(dirDist);
				Utils.Assert(!current.Same(danger), "Safe from danger.");
				rv.Add(dirDist);
			}
			if (rv.Count() == 0)
				ElfHelper.DayLog("Empty");
			return rv;
		}

		static internal DirDist FindDir21(Point from, Point target, Point danger)
		{
			if (from.Y == danger.Y)
			{
				if (target.Y > from.Y)
					return new DirDist(DirEnum.S, target.Y - from.Y);
				else if (from.Y > target.Y)
					return new DirDist(DirEnum.N, from.Y - target.Y);
				else if (target.X > from.X)
					return new DirDist(DirEnum.E, target.X - from.X);
				else if (from.X > target.X)
					return new DirDist(DirEnum.W, from.X - target.X);
			}
			else // if (from.X == danger.X) or doesn't matter
			{
				if (target.X > from.X)
					return new DirDist(DirEnum.E, target.X - from.X);
				else if (from.X > target.X)
					return new DirDist(DirEnum.W, from.X - target.X);
				else if (target.Y > from.Y)
					return new DirDist(DirEnum.S, target.Y - from.Y);
				else if (from.Y > target.Y)
					return new DirDist(DirEnum.N, from.Y - target.Y);
			}
			return new DirDist(DirEnum.E, 0);
		}

	}

	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic

		check.Compare(rv);
		return rv;
	}
}

