using AoCLibrary;

namespace Advent24;

internal class Day13 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/13
	// Input https://adventofcode.com/2024/day/13/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 480L);
		else
			check = new StarCheck(key, 31761L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		for(var i = 0; i < lines.Length; i++)
		{
			var parts = lines[i++].Split(":,+= ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var a = new Point(int.Parse(parts[3]), int.Parse(parts[5]));
			parts = lines[i++].Split(":,+= ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var b = new Point(int.Parse(parts[3]), int.Parse(parts[5]));
			parts = lines[i++].Split(":,+= ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var prize = new Point(long.Parse(parts[2]), long.Parse(parts[4]));

			var det = a.X * b.Y - b.X * a.Y;
			if (det == 0)
				continue;
			var A = (prize.X * b.Y - b.X * prize.Y) / det;
			var B = (a.X * prize.Y - prize.X * a.Y) / det;

			var moves = new List<Move>();
			moves.Add(new Move(a, 3));
			moves.Add(new Move(b, 1));
			var winners = new List<int>();
			while (moves.Any())
			{
				var newMoves = new List<Move>();
				foreach (var move in moves)
				{
					var newMove = move.Add(a, 3);
					var res = newMove.Result(prize);
					if (res == true)
						winners.Add(newMove.Tokens);
					else if (res == null)
						newMoves.Add(newMove);

					newMove = move.Add(b, 1);
					res = newMove.Result(prize);
					if (res == true)
						winners.Add(newMove.Tokens);
					else if (res == null)
						newMoves.Add(newMove);
				}
				moves = new List<Move>();
				foreach(var newMove in newMoves)
				{
					var sameMove = moves.FirstOrDefault(m => m.Head.Same(newMove.Head));
					if (sameMove == null)
						moves.Add(newMove);
					else if (sameMove.Tokens > newMove.Tokens)
						sameMove.Tokens = newMove.Tokens;
					// else throw away new move
				}
			}
			if (winners.Any())
				rv += winners.Min();
		}

		check.Compare(rv);
		return rv;
	}
	public enum ResultEnum
	{
		Won,
		Lost,

	}

	public class Move
	{
		public Move(Point head, int tokens)
		{
			Head = head;
			Tokens = tokens;
		}

		public Point Head { get; set; }
		public int Tokens { get; set; }

		internal Move Add(Point a, int tokens)
		{
			return new Move(Head.Plus(a), Tokens + tokens);
		}
		public override string ToString()
		{
			return $"{Head} t:{Tokens}";
		}
		internal bool? Result(Point prize)
		{
			if (Head.X == prize.X && Head.Y == prize.Y)
				return true;
			if (Head.X > prize.X || Head.Y > prize.Y)
				return false;
			return null;
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 875318608908L); //?
		else
			check = new StarCheck(key, 90798500745591L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		for (var i = 0; i < lines.Length; i++)
		{
			var parts = lines[i++].Split(":,+= ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var a = new Point(long.Parse(parts[3]), long.Parse(parts[5]));
			parts = lines[i++].Split(":,+= ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var b = new Point(long.Parse(parts[3]), long.Parse(parts[5]));
			parts = lines[i++].Split(":,+= ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			var prize = new Point(int.Parse(parts[2]) + 10000000000000L, int.Parse(parts[4]) + 10000000000000L);


			//a.X * A + b.X * B = prize.X;
			//a.Y * A + b.Y * B = prize.Y;
			//a.X * A = prize.X - b.X * B
			//A = (prize.X - b.X * B) / a.X
			//a.Y * ((prize.X - b.X * B) / a.X) + b.Y * B = prize.Y

			var det = a.X * b.Y - b.X * a.Y;
			if (det == 0)
				continue;
			var A = (prize.X * b.Y - b.X * prize.Y) / det;
			var B = (a.X * prize.Y - prize.X * a.Y) / det;
			var guess = new Point(a.X * A + b.X * B, a.Y * A + b.Y * B);
			if (guess.Same(prize))
				rv += A*3 + B;
		}
		// 157687857400714 Too high
		// 90798500745591
		check.Compare(rv);
		return rv;
	}
}

