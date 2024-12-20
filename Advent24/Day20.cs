using AoCLibrary;
using System.Linq;
using System.Net.WebSockets;
using static Advent24.Day13;
using static Advent24.Day18;

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
			check = new StarCheck(key, 5L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var map = new Map20(lines);
		var trail = new Trail20(map.Find('S')!);
		var nonCheat = map.Solve(trail, maxScore: int.MaxValue).First();

		var cheats = map.Solve(trail, maxScore: nonCheat - 20);
		rv = cheats.Count();
		check.Compare(rv);
		return rv;
	}
	public class Trail20
	{
		public Point Pos { get; set; }
		public List<Point> History { get; } = [];
		public int Score => History.Count();
		public bool Cheated => Cheats.Any();
		public List<Point> Cheats { get; } = [];
		public Trail20(Point pos)
		{
			Pos = pos;
		}
		public Trail20 Add(Point pos)
		{
			var rv = new Trail20(pos);
			rv.History.AddRange(History);
			rv.History.Add(pos);
			rv.Cheats.AddRange(Cheats);
			return rv;
		}
		public override string ToString()
		{
			return $"{Pos} s:{Score} c:{Cheated}";
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

		public List<int> Solve(Trail20 head, int maxScore)
		{
			var trails = new List<Trail20>();
			var wins = new List<Trail20>();
			var allowCheats = maxScore != int.MaxValue;
			trails.Add(head);
			while(trails.Any())
			{
				var newTrails = new List<Trail20>();
				foreach(var trail in trails)
					newTrails.AddRange(Move(trail, allowCheats));

				trails = new List<Trail20>();
				foreach (var newTrail in newTrails)
				{
					if (newTrail.Score > maxScore)
						continue;

					if (Get(newTrail.Pos) == 'E')
					{
						Console.WriteLine(maxScore- newTrail.Score);
						Draw(newTrail);
						wins.Add(newTrail);
						//rv.Add(newTrail.Score);
					}
					else
						trails.Add(newTrail);
				}
			}
			var groups = wins.GroupBy(t => t.Cheats.FirstOrDefault()?.ToString());
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
			foreach (var pt in trail.Cheats)
				GridMapXY.DrawOnText(lines, pt, 'X');

			Console.WriteLine(trail);
			foreach (var line in lines)
				Console.WriteLine(line);
		}
		
		public List<Trail20> Move(Trail20 trail, bool allowCheat)
		{
			var rv = new List<Trail20>();
			var moves = trail.Pos.AllDirMoves();
			foreach(var move in moves)
			{
				if (trail.History.Contains(move))
					continue;
				if (!IsValid(move))
					continue;
				var blocked = Get(move) == '#';
				if (!blocked)
				{
					rv.Add(trail.Add(move));
					continue;
				}

				if (allowCheat && !trail.Cheated)
				{
					var cheat1 = trail.Add(move);
					cheat1.Cheats.Add(move);

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
					}
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

