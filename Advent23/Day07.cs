using AoCLibrary;
namespace Advent23
{

	internal class Day07 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/7
		// Input https://adventofcode.com/2023/day/7/input

		// after much testing, Hand is about twice as fast as HandHex, I think because of the "new"
		private long Star(StarEnum star)
		{
			var rv = 0L;
			ElfHelper.DayLog("Star() " + star);
			var lines = Program.GetLines(star, IsReal);
			var hands = new List<Hand>();
			foreach(var line in lines)
			{
				var parts = Utils.Split(' ', line);
				hands.Add(new Hand(parts[0], int.Parse(parts[1]), star));
        	}
			int i = 1;
			foreach(var hand in hands.OrderBy(h => h.Ordering))
			{
				rv += hand.Bid * i;
				if (!IsReal)
					ElfHelper.DayLog(i.ToString() + " " + hand);
                i++;
			}
			ElfHelper.DayLog("rv= " + rv);
			return rv;
		}
		public object? Star1()
		{
			var rv = Star(StarEnum.Star1);
			if (!IsReal)
				Utils.Assert(rv, 6440);
			//250404528 too low
			//250474325
			return rv;
		}

		public object? Star2()
		{
			var rv = Star(StarEnum.Star2);
			if (!IsReal)
                Utils.Assert(5905, rv);
            //248687135 too low
            //248909434
            return rv;
        }
		public RunnerResult Run()
		{
			var rv = new RunnerResult();
			rv.Star1 = Star1();
			rv.Star2 = Star2();
			return rv;
		}
	}
	public enum RankEnum
	{
		NA,
		HighCard,
		OnePair,
		TwoPair,
		Three,
		FullHouse,
		Four,
		Five
	}
	public class HandHex
	{
		public HandHex(string cards, int bid, StarEnum star)
		{
			//_originalCards = cards;
			_cards = new int[5];
			for(int i = 0; i < 5; i++)
			{
				var c = cards[i];
				if (c == 'T')
					_cards[i] = 10;
				else if (c == 'J')
				{

					if (star == StarEnum.Star1)
						_cards[i] = 11;
					else
					{
						_jokers++;
						_cards[i] = 0;
					}
				}
				else if (c == 'Q')
					_cards[i] = 12;
				else if (c == 'K')
					_cards[i] = 13;
				else if (c == 'A')
					_cards[i] = 14;
				else
					_cards[i] = c - '0';
			}
			_handRank = Rank(star);
			Ordering = ToHex(_cards);
			Bid = bid;
		}
		static long ToHex(int[] ints)
		{
			var rv = 0L;
			for (int i = 0; i < ints.Length; i++)
			{
				var p = ints.Length - i - 1;
				rv += ints[p] * (long)Math.Pow(16, i);
			}
			return rv;
		}
		public override string ToString()
		{
			return $"{_handRank} {Ordering.ToString("X")}";
		}
		RankEnum Rank(StarEnum star)
		{
			IEnumerable<IGrouping<int, int>> groups;
			if (star == StarEnum.Star1)
				groups = _cards.GroupBy(c => c);
			else
				groups = _cards.Where(j => j != 0).GroupBy(c => c);

			if (groups.Count() == 1 || _jokers == 5)
				return RankEnum.Five;   // five of a kind
			else if (groups.Count() == 2)
			{
				if (groups.OrderByDescending(g => g.Count()).First().Count() + _jokers > 3)
					return RankEnum.Four;   // four of a kind
				else
					return RankEnum.FullHouse;   // full house
			}
			else if (groups.Count() == 3)
			{
				if (groups.OrderByDescending(g => g.Count()).First().Count() + _jokers == 3)
					return RankEnum.Three;   // three of a kind
				else
					return RankEnum.TwoPair;   // two pair
			}
			else if (groups.Count() == 4)
			{
				return RankEnum.OnePair;   // one pair
			}
			else
				return RankEnum.HighCard;   // high-card
		}

		//string _originalCards;
		int _jokers = 0;
		int[] _cards;
		RankEnum _handRank;
		public long Ordering { get; }
		public int Bid { get; }
	}
	public class Hand
	{
		public Hand(string cards, int bid, StarEnum star)
		{
			Cards = cards;
			_handRank = Rank(cards, star);
			var hex = string.Empty;
			hex = _handRank.ToString("x");
			foreach (var c in Cards)
			{
				var v = GetValue(c, star);
				hex += v.ToString("x");
			}
			Ordering = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			Bid = bid;
		}
		int GetValue(char c, StarEnum star)
		{
			if (char.IsDigit(c))
				return (int)c - '0';
			else if (c == 'T')
				return 10;
			else if (c == 'J')
			{
				if (star == StarEnum.Star1)
					return 11;
				else
					return 1;
			}
			else if (c == 'Q')
				return 12;
			else if (c == 'K')
				return 13;
			else //if (c == 'A')
				return 14;
		}
		public override string ToString()
		{
			return $"{Cards} {_handRank} {Ordering.ToString("X")}";
		}
		static RankEnum Rank(string cards, StarEnum star)
		{
			IList<IGrouping<char, char>> groups;
			int jokers = 0;
			if (star == StarEnum.Star1)
				groups = cards.GroupBy(c => c).OrderByDescending(g => g.Count()).ToList();
			else //if (star == StarEnum.Star2)
			{
				var noJs = cards.Replace("J", "");
				groups = noJs.GroupBy(c => c).OrderByDescending(g => g.Count()).ToList();
				jokers = 5 - noJs.Count();
			}

			if (groups.Count() == 1 || jokers == 5)
				return RankEnum.Five;   // five of a kind
			else if (groups.Count() == 2)
			{
				if (groups[0].Count() + jokers > 3)
					return RankEnum.Four;   // four of a kind
				else
					return RankEnum.FullHouse;   // full house
			}
			else if (groups.Count() == 3)
			{
				if (groups[0].Count() + jokers == 3)
					return RankEnum.Three;   // three of a kind
				else
					return RankEnum.TwoPair;   // two pair
			}
			else if (groups.Count() == 4)
			{
				return RankEnum.OnePair;   // one pair
			}
			else
				return RankEnum.HighCard;   // high-card
		}
		public string Cards { get; }
		public int Ordering { get; }
		public int Bid { get; }
		public RankEnum _handRank;
	}

}
