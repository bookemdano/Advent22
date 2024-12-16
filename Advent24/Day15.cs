using AoCLibrary;

namespace Advent24;

internal class Day15 : IDayRunner
{
	public bool IsReal => false;
	public bool ExtraLog => !IsReal;
	// Day https://adventofcode.com/2024/day/15
	// Input https://adventofcode.com/2024/day/15/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 10092L);
		else
			check = new StarCheck(key, 1492518L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var mapLines = new List<string>();
		var moveLine = string.Empty;
		bool broken = false;
		foreach (var line in lines)
		{
			if (line.Length == 0)
				broken = true;
			else if (!broken)
				mapLines.Add(line);
			else
				moveLine += line;
		}
		//var movesText = "<^^>>>vv<v>>v<<";
		var moves = moveLine.Select(c => LocDir.ParseDir(c)).ToList();
		var map = new Map15(mapLines);
		var bot = map.Find('@');
		if (ExtraLog)
			Console.Clear();
		foreach(var move in moves)
		{
			if (ExtraLog)
				WriteToConsole(map.ToString());

			//Console.WriteLine(map);
			bot = map.Move(bot, move);
			
		}
		//Console.WriteLine(map);

		var boxes = map.FindAll('O');
		foreach (var box in boxes)
			rv += box.Row * 100 + box.Col;

		// 1492518
		check.Compare(rv);
		return rv;
	}
	void WriteToConsole(string text)
	{
		var lines = text.Split(Environment.NewLine);
		if (lines.Count() >= Console.WindowHeight)
			return;
		var iLine = 0;
		foreach(var line in lines)
		{
			Console.SetCursorPosition(0, iLine++);
			Console.WriteLine(line);
		}
		Thread.Sleep(TimeSpan.FromMilliseconds(10));
	}
	public class Map15 : GridMap
	{
		public Map15(IEnumerable<string>? lines) : base(lines)
		{
		}

		internal Loc Move(Loc bot, DirEnum move)
		{
			var check = bot.Move(move);
			var Os = new List<Loc>();
			while (true)
			{
				var c = Get(check);
				if (c == '#')
					return bot;
				else if (c == 'O')
				{
					Os.Add(check);
					check = check.Move(move);
				}
				else
				{
					if (Os.Any())
						this.Swap(Os.First(), check);
					this.Swap(bot, bot.Move(move));
					return bot.Move(move);
				}
			}
		}
		internal Loc Move2(Loc bot, DirEnum move)
		{
			var checks = new List<Loc>() { bot.Move(move) };
			var Os = new List<Loc>();
			while (checks.Any())
			{
				var newChecks = new List<Loc>();
				foreach (var check in checks)
				{
					var c = Get(check);
					if (c == '#')	// we hit a wall, cancel everything
						return bot;
					else if (c == '[')
					{
						Os.Add(check);
						var next = check.Move(move);
						newChecks.Add(next);
						if (move == DirEnum.N || move == DirEnum.S)
						{
							Os.Add(check.Plus(new Loc(0, 1)));
							newChecks.Add(next.Plus(new Loc(0, 1)));
						}
					}
					else if (c == ']')
					{
						Os.Add(check);
						var next = check.Move(move);
						newChecks.Add(next);
						if (move == DirEnum.N || move == DirEnum.S)
						{
							Os.Add(check.Plus(new Loc(0, -1)));
							newChecks.Add(next.Plus(new Loc(0, -1)));
						}
					}
				}
				checks = newChecks;
			}
			Os.Reverse();
			var dones = new List<Loc>();
			foreach (var o in Os)
			{
				if (dones.Any(d => d.Same(o)))
					continue;
				this.Swap(o, o.Move(move));
				dones.Add(o);
			}

			//Console.WriteLine(this);
			this.Swap(bot, bot.Move(move));

			return bot.Move(move);
		}

		private void Swap(Loc from, Loc to)
		{
			var fromC = Get(from);
			var toC = Get(to);
			if (toC == null || fromC == null)
				return;
			Set(from, (char) toC);
			Set(to, (char) fromC);
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 9021L);
		else
			check = new StarCheck(key, 1512860L);

		var lines = Program.GetLines(check.Key);
		//var lines = File.ReadAllLines(@"assets\Day15FakeSmall.txt");
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var mapLines = new List<string>();
		var moveLine = string.Empty;
		bool broken = false;
		foreach (var line in lines)
		{
			if (line.Length == 0)
				broken = true;
			else if (!broken)
				mapLines.Add(line.Replace(".", "..").Replace("#","##").Replace("O", "[]").Replace("@","@."));
			else
				moveLine += line;
		}
		//var movesText = "<^^>>>vv<v>>v<<";
		var moves = moveLine.Select(c => LocDir.ParseDir(c)).ToList();
		var map = new Map15(mapLines);
		Console.WriteLine(map);
		var bot = map.Find('@');
		if (ExtraLog)
			Console.Clear();

		var iMove = 0;
		foreach (var move in moves)
		{
			if (ExtraLog)
				WriteToConsole(map.ToString());

			bot = map.Move2(bot, move);
			//Console.WriteLine($"{iMove++} {move}");
			//Console.WriteLine(map);
		}
		Console.WriteLine(map);

		var boxes = map.FindAll('[');
		foreach (var box in boxes)
			rv += box.Row * 100 + box.Col;


		check.Compare(rv);
		return rv;
	}
}

