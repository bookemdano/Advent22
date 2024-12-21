using AoCLibrary;

namespace Advent24;

internal class Day20 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2024/day/20
	// Input https://adventofcode.com/2024/day/20/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 44L);
		else
			check = new StarCheck(key, 1323L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		int offset = 100;
		if (IsReal == false)
			//offset = 64;//1
			offset = 2;//44
			//offset = 20;//5

		var map = new Map20(lines);
		var trail = new Trail20(map.Find('S')!);
		var nonCheat = map.Solve(trail, maxScore: int.MaxValue, maxCheats: 0).First();

		var cheats = map.Solve(trail, maxScore: nonCheat - offset, maxCheats: 2);
		rv = cheats.Count();

		check.Compare(rv);
		return rv;
	}
	public class Trail20
	{
		public Point Pos { get; set; }
		public List<Point> History { get; } = [];
		public int Score => History.Count();
		List<Point> _cheats = [];
		public void AddCheat(Point cheat, Point last)
		{
			if (!_cheats.Any())
				_cheats.Add(last);
			_cheats.Add(cheat);
		}
		public int CheatCount => _cheats.Count;
		public List<Point> GetCheats => _cheats;
		public string CheatSet => $"{_cheats.FirstOrDefault()}{_cheats.LastOrDefault()}";
		public Trail20(Point pos)
		{
			Pos = pos;
		}
		public Trail20 Add(Point pos)
		{
			var rv = new Trail20(pos);
			rv.History.AddRange(History);
			rv.History.Add(pos);
			rv._cheats.AddRange(_cheats);
			return rv;
		}
		public override string ToString()
		{
			return $"{Pos} s:{Score} c:{CheatSet}({CheatCount})";
		}
	}
	public class Map20 : GridMapXY
	{
		public Map20(IEnumerable<string>? lines) : base(lines)
		{

		}

		public Map20(int x, int y) : base(x, y)
		{
		}

		Trail20? _best;

		public List<int> Solve(Trail20 head, int maxScore, int maxCheats)
		{
			var trails = new List<Trail20>();
			var wins = new List<Trail20>();
			var allowCheats = maxScore != int.MaxValue;
			trails.Add(head);
			var next = DateTime.Now;
			while(trails.Any())
			{
				var newTrails = new List<Trail20>();
				foreach(var trail in trails)
					newTrails.AddRange(Move(trail, allowCheats, maxCheats));

				trails = new List<Trail20>();
				foreach (var newTrail in newTrails)
				{
					if (allowCheats && newTrail.CheatCount > 0)
					{
						var index = _best.History.IndexOf(newTrail.Pos);
						if (index >= 0)
						{
							newTrail.History.AddRange(_best.History[(index + 1)..]);
							newTrail.Pos = _best.Pos;
						}
					}
					if (newTrail.Score > maxScore)
						continue;
					if (Get(newTrail.Pos) == 'E')
					{
						//Console.WriteLine($"{newTrail} {maxScore - newTrail.Score} left: {newTrails.Count()}");
						//Draw(newTrail);
						if (!wins.Any(t => t.CheatSet == newTrail.CheatSet))
							wins.Add(newTrail);
						//rv.Add(newTrail.Score);
					}
					else
						trails.Add(newTrail);
				}
			}
			if (allowCheats == false && wins.Count == 1)
				_best = wins.First();
			var groups = wins.GroupBy(t => t.CheatSet);
			var rv = new List<int>();
			foreach(var trailGroup in groups)
			{
				rv.Add(trailGroup.First().Score);	// we don't care what the score really was for cheat
			}
			return rv;
		}

		void Draw(Trail20 trail)
		{
			var text = this.ToString();
			var lines = text.Split(Environment.NewLine);
			foreach (var pt in trail.History)
				GridMapXY.DrawOnText(lines, pt, '0');
			foreach (var pt in trail.GetCheats)
				GridMapXY.DrawOnText(lines, pt, 'X');

			Console.WriteLine(trail);
			foreach (var line in lines)
				Console.WriteLine(line);
		}
		
		public List<Trail20> Move(Trail20 trail, bool allowCheat, int maxCheats)
		{
			var rv = new List<Trail20>();
			var moves = trail.Pos.AllMoves();
			foreach(var move in moves)
			{
				if (trail.History.Contains(move))
					continue;
				if (!IsValid(move))
					continue;
				var blocked = Get(move) == '#';
				if (!blocked)
				{
					var newTrail = trail.Add(move);
					if (newTrail.CheatCount > 0 && newTrail.CheatCount < maxCheats)
						newTrail.AddCheat(move, trail.Pos);
					rv.Add(newTrail);
					continue;
				}
				else if (allowCheat && trail.CheatCount < maxCheats )
				{
					var cheat1 = trail.Add(move);
					cheat1.AddCheat(move, trail.Pos);
					rv.Add(cheat1);
					/*
					//var move2 = move.Move(move.Dir);
					//var cheat2 = cheat1.Add(move2);
					//cheat2.Cheats.Add(move2);
					var cheatMoves2 = cheat1.Pos.AllMoves();
					foreach (var move2 in cheatMoves2)
					{
						if (cheat1.History.Contains(move2))
							continue;
						if (!IsValid(move2))
							continue;

						//var move3 = move2.Move(move.Dir);
						//if (IsValid(move3) && Get(move3) != '#')
						{
							var cheatTrail = cheat1.Add(move2);
							cheatTrail.Cheats.Add(move2);
							rv.Add(cheatTrail);
						}
						//Draw(cheatTrail);
					}*/
				}
			}
			return rv;
		}
	}

	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 285L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		int offset = 100;
		if (IsReal == false)
			offset = 50; //285
			//offset = 76;//3

		var map = new Map20(lines);
		var trail = new Trail20(map.Find('S')!);
		var nonCheat = map.Solve(trail, maxScore: int.MaxValue, maxCheats: 0).First();

		var cheats = map.Solve(trail, maxScore: nonCheat - offset, maxCheats: 20);
		rv = cheats.Count();

		check.Compare(rv);
		return rv;
	}
}

