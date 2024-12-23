using AoCLibrary;

namespace Advent24;

internal class Day22 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/22
	// Input https://adventofcode.com/2024/day/22/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 37327623L);
		else
			check = new StarCheck(key, 17577894908L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		Utils.Assert(Prune(100000000), 16113920);
		Utils.Assert(Mix(42, 15), 37);
		var n = 2000;
		foreach(var line in lines)
		{
			var s = long.Parse(line);
			//s = 123;
			for (int i = 0; i < n; i++)
			{
				s = Mix(s, s * 64);
				s = Prune(s);
				s = Mix(s, s / 32);
				s = Prune(s);
				s = Mix(s, s * 2048);
				s = Prune(s);
			}
			rv += s;
		}
		//17577894908
		check.Compare(rv);
		return rv;
	}
	long Prune(long s)
	{
		return s % 16777216;
	}
	long Mix(long s, long l)
	{
		return s ^ l;
	}

	private bool CheckStack(List<int> last4, Seq22 pattern)
	{
		if (last4.Count < 4)
			return false;
		var i = last4.Count - 4;
		var seq = new Seq22(last4[i..]);
		return seq.Same(pattern);
	}

	public class Seq22
	{
		long _hash;
		public string Pattern { get; set; }
		public Seq22(List<int> seq)
		{
			var n = Math.Min(seq.Count, 4);
			for (int i = 0; i < n; i++)
				_hash += (seq[i] + 9) * (long) Math.Pow(10, i * 2);
			Pattern = string.Join(",", seq[..n]);
		}

		internal static List<Seq22> Patterns()
		{
			var min = -3;
			var max = 3;
			new Seq22(new List<int>() { -9, 9, 0, -9 });
			var rv = new List<Seq22>();
			for (int i = min; i <= max; i++)
				for (int j = min; j <= max; j++)
					for (int k = min; k <= max; k++)
						for (int l = min; l <= max; l++)
							rv.Add(new Seq22(new List<int>() { i, j, k, l }));
			return rv;
		}
		internal bool Same(Seq22 pattern)
		{
			return (pattern._hash == _hash);
		}

	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 23L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var n = 2000;
		/*var bestSequences = new Dictionary<long, Seq22>();
		foreach (var line in lines)
		{
			var s = long.Parse(line);
			var orig = s;
			var seq = new List<int>();
			var lastP = -10;

			for (int i = 0; i < n; i++)
			{
				s = DoItUp(s);
				var p = (int)(s % 10);
				if (lastP != -10)
				{
					seq.Add(p - lastP);
					if (p == 9)
						bestSequences[orig] = new Seq22(seq.TakeLast(4).ToList());
				}
				lastP = 0;
			}
		}*/
		var sList = lines.Select(l => long.Parse(l));
		var allPrices = GetAllPrices(sList);

		var max = 0;
		var patterns = Seq22.Patterns();
		Seq22? bestPattern;
		foreach (var pattern in patterns)
		{
			var totalPrice = 0;
			foreach(var s in sList)
			{
				totalPrice += Find(allPrices[s], pattern);
			}
			if (totalPrice > max)
			{
				max = totalPrice;
				bestPattern = pattern;
			}
		}
		rv = max;

		// 1469 too low
		// 1728 too low
		check.Compare(rv);
		return rv;
	}

	private int Find(Orders22 orders22, Seq22 pattern)
	{
		if (!orders22.OrderString.Contains(pattern.Pattern))
			return -1;
		var orders = orders22.GetOrders();
		var changes = orders22.GetOrders().Select(o => o.Change).ToList();
		for (var i = 0; i < orders.Count() - 4; i++)
		{
			if (pattern.Same(new Seq22(changes[i..(i+3)])))
				return orders22.GetOrders()[i+3].Price;
		}
		return 0;
	}

	public class Order22
	{
		public Order22(int price, int change)
		{
			Price = price; Change = change;
		}
		public int Price { get; set; }

		public int Change { get; set; }

	}
	Dictionary<long, List<int>> _dict2 = [];
	private Dictionary<long, Orders22> GetAllPrices(IEnumerable<long> sList)
	{
		var n = 2000;
		var rv = new Dictionary<long, Orders22>();

		foreach (var orig in sList)
		{
			var orders = new List<Order22>();

			//s = 123;
			var lastP = -10;
			var s = orig;
			for (int i = 0; i < n; i++)
			{
				var p = (int)(s % 10);
				if (lastP != -10)
				{
					orders.Add(new Order22(p, p - lastP));
				}
				lastP = p;
				s = DoItUp(s);
			}
			rv[orig] = new Orders22(orders);
		}

		return rv;
	}
	public class Orders22
	{
		List<Order22> _orders;
		public string OrderString { get; set; }
		public Orders22(List<Order22> orders)
		{
			_orders = orders;
			OrderString = string.Join(',', orders);
		}

		internal List<Order22> GetOrders()
		{
			return _orders;
		}
	}
	private int GetPrice(IEnumerable<long> sList, Seq22 pattern)
	{
		var n = 2000;
		var rv = 0;

		foreach (var orig in sList)
		{
			var seq = new List<int>();

			//s = 123;
			var lastP = -10;
			var s = orig;
			for (int i = 0; i < n; i++)
			{
				var p = (int)(s % 10);
				if (lastP != -10)
				{
					seq.Add(p - lastP);
					if (CheckStack(seq, pattern))
					{
						rv += p;
						break;
					}
				}
				lastP = p;
				s = DoItUp(s);
			}
		}
		return rv;
	}

	Dictionary<long, long> _dict = [];
	private long DoItUp(long s)
	{
		if (_dict.ContainsKey(s))
			return _dict[s];
		var orig = s;
		s = Mix(s, s * 64);
		s = Prune(s);
		s = Mix(s, s / 32);
		s = Prune(s);
		s = Mix(s, s * 2048);
		s = Prune(s);
		_dict[orig] = s;
		return s;
	}
}

