using AoCLibrary;
namespace Advent23
{
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
	public class Hand1
	{
		public Hand1(string cards, int bid)
		{
			//var ordered = cards.OrderByDescending(c => GetValue(c));
			Cards = cards;	// string.Join("", ordered);
			HandRank = Rank(cards);
			var hex = string.Empty;
			hex = HandRank.ToString("x");
			foreach(var c in Cards)
			{
				var v = GetValue(c);
				hex += v.ToString("x");     
			}
			Ordering = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            Bid = bid;
		}

        static RankEnum Rank(string cards)
        {
            var groups = cards.GroupBy(c => c).OrderByDescending(g => g.Count()).ToList();
            if (groups.Count() == 1)
                return RankEnum.Five;   // five of a kind
            else if (groups.Count() == 2)
            {
                if (groups[0].Count() > 3)
                    return RankEnum.Four;   // four of a kind
                else
                    return RankEnum.FullHouse;   // full house
            }
            else if (groups.Count() == 3)
            {
                if (groups[0].Count() == 3)
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
        int GetValue(char c)
		{
			if (char.IsDigit(c))
				return (int)c - '0';
            else if (c == 'T')
                return 10;
            else if (c == 'J')
                return 11;
            else if (c == 'Q')
                return 12;
            else if (c == 'K')
                return 13;
            else //if (c == 'A')
                return 14;
        }
        public override string ToString()
        {
            return $"{Cards} {HandRank} {Ordering.ToString("X")}";
        }
   
		public string Cards { get; set; }
		public int Ordering { get; set; }
		public int Bid { get; set; }
		public RankEnum HandRank
		{
			get; set;
		}
	}
    public class Hand2
    {
        public Hand2(string cards, int bid)
        {
            //var ordered = cards.OrderByDescending(c => GetValue(c));
            Cards = cards;  // string.Join("", ordered);
            HandRank = Rank(cards);
            var hex = string.Empty;
            hex = HandRank.ToString("x");
            foreach (var c in Cards)
            {
                var v = GetValue(c);
                hex += v.ToString("x");
            }
            Ordering = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            Bid = bid;
        }
        int GetValue(char c)
        {
            if (char.IsDigit(c))
                return (int)c - '0';
            else if (c == 'T')
                return 10;
            else if (c == 'J')
                return 1;
            else if (c == 'Q')
                return 12;
            else if (c == 'K')
                return 13;
            else //if (c == 'A')
                return 14;
        }
        public override string ToString()
        {
            return $"{Cards} {HandRank} {Ordering.ToString("X")}";
        }
        static RankEnum Rank(string cards)
        {
            var noJs = cards.Replace("J", "");
            var js = 5 - noJs.Count();
            var groups = noJs.GroupBy(c => c).OrderByDescending(g => g.Count()).ToList();

            if (groups.Count() == 1 || js == 5)
                return RankEnum.Five;   // five of a kind
            else if (groups.Count() == 2)
            {
                if (groups[0].Count() + js > 3)
                    return RankEnum.Four;   // four of a kind
                else
                    return RankEnum.FullHouse;   // full house
            }
            else if (groups.Count() == 3)
            {
                if (groups[0].Count() + js == 3)
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
        public string Cards { get; set; }
        public int Ordering { get; set; }
        public int Bid { get; set; }
        public RankEnum HandRank
        {
            get; set;
        }
    }
    internal class Day07 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/7
		// Input https://adventofcode.com/2023/day/7/input
		private object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var hands = new List<Hand1>();
			foreach(var line in lines)
			{
				var parts = Utils.Split(' ', line);
				hands.Add(new Hand1(parts[0], int.Parse(parts[1])));
        	}
			hands = hands.OrderBy(h => h.Ordering).ToList();
			int i = 1;
			foreach(var hand in hands)
			{
				rv += hand.Bid * i;
                //Utils.TestLog(i.ToString() + " " + hand);
                i++;
			}
            if (!IsReal)
                Utils.Assert(rv, 6440);
            //250404528 too low
            //250474325
            return rv;
		}
		private object? Star2()
		{
            var rv = 0L;
            var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var hands = new List<Hand2>();
            foreach (var line in lines)
            {
                var parts = Utils.Split(' ', line);
                hands.Add(new Hand2(parts[0], int.Parse(parts[1])));
            }
            hands = hands.OrderBy(h => h.Ordering).ToList();
            int i = 1;
            foreach (var hand in hands)
            {
                rv += hand.Bid * i;
                Utils.TestLog(i.ToString() + " " + hand);
                i++;
            }
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
}
