using AoCLibrary;

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
		var keypad = new GridMapXY(new List<string>() { "789", "456", "123", " 0A" });
		var dirpad = new GridMapXY(new List<string>() { " ^A", "<v>" });
		foreach (var line in lines)
		{
			/*var remotes = new List<Trail21>();
			remotes.Add(new Trail21(new Point(2, 0)));	// robot2
			remotes.Add(new Trail21(new Point(2, 0)));
			remotes.Add(new Trail21(new Point(2, 0)));  // human
			*/
			var remote = new Trail21(new Point(2, 0));
			var keys = new Trail21(new Point(2, 3));
			foreach (var c in line)
			{
				var targKey = keypad.Find(c)!;
				if (keys.Head == targKey)
					break;
				var dirDists = DirDist.FindDirs(keys.Head, targKey);
				foreach(var dirDist in dirDists)
				{
					var moves = MoveToThis(dirpad, remote.Head, LocDir.DirChar(dirDist.Dir));
					remote.Move(moves, dirpad);

					var aMoves = MoveToThis(dirpad, remote.Head, 'A');
					remote.Move(aMoves, dirpad);
					/*foreach(var dirDist1 in dirDist1s)
					{
						var dirTarget2Char = LocDir.DirChar(dirDist1.Dir);
						var targ2Key = dirpad.Find(dirTarget1Char);
						var remote2 = remotes[1];
						var dirDist2s = DirDist.FindDirs(remote1.Head, targ1Key);
						foreach (var dirDist2 in dirDist2s)
						{
							var dirTarget3Char = LocDir.DirChar(dirDist2.Dir);
							var targ3Key = dirpad.Find(dirTarget2Char);
							var remote3 = remotes[2];
							var dirDist3s = DirDist.FindDirs(remote2.Head, targ1Key);
							remote3.Head.Move(dirDist3s);
						}
						remote2.Head.Move(dirDist2s);
					}
					remote1.Head.Move(dirDist1s);
					*/
				}
				keys.Move(dirDists, keypad);
			}

		}

		check.Compare(rv);
		return rv;
	}

	private static List<DirDist> MoveToThis(GridMapXY map, Point from, char targetC)
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
		public string _chars = string.Empty;
		internal void Move(List<DirDist> moves, GridMapXY map)
		{
			Moves.AddRange(moves);
			foreach(var move in moves)
				Head = Head.Move(move);
			_chars += map.Get(Head)??'?';
		}
		public override string ToString()
		{
			return _chars;
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

