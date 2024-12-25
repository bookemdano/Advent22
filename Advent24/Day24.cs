using AoCLibrary;
namespace Advent24;

internal class Day24 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/24
	// Input https://adventofcode.com/2024/day/24/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 2024L);
		else
			check = new StarCheck(key, 53258032898766L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var values = new Dictionary<string, int>();
		var ops = new List<Op24>();
		foreach (var line in lines)
		{
			if (line.Contains(':'))
			{
				var parts = line.Split(':', StringSplitOptions.TrimEntries);
				values.Add(parts[0], int.Parse(parts[1]));
			}
			else if (!string.IsNullOrWhiteSpace(line))
				ops.Add(new Op24(line));
		}
		while (ops.Any(o => o.Val == null && o.Res.StartsWith('z')))
		{
			foreach (var op in ops.Where(o => o.Val == null))
			{
				if (values.ContainsKey(op.Lh) && values.ContainsKey(op.Rh))
				{
					op.Connect(values[op.Lh], values[op.Rh]);
					values[op.Res] = (int) op.Val;
				}
			}
		}
		rv = GetBinary('z', values);

		check.Compare(rv);
		return rv;
	}
	public enum Op24Enum
	{
		AND,
		OR,
		XOR
	}
	public class Op24
	{
		public Op24(string str)
		{
			var parts = str.Split(" ->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			Lh = parts[0];
			if (parts[1] == "XOR")
				Op = Op24Enum.XOR;
			else if (parts[1] == "AND")
				Op = Op24Enum.AND;
			else if (parts[1] == "OR")
				Op = Op24Enum.OR;
			Rh = parts[2];
			Res = parts[3];
		}
		public string Lh { get; }
		public string Rh { get; }
		public Op24Enum Op { get; }
		public string Res { get; }
		public int? Val { get; set; }
		public override string ToString()
		{
			return $"{Lh} {Op} {Rh} -> {Res} ({Val})";
		}

		internal void Connect(int lh, int rh)
		{
			if (Op == Op24Enum.AND)
				Val = lh & rh;
			else if (Op == Op24Enum.OR)
				Val = lh | rh;
			else if (Op == Op24Enum.XOR)
				Val = lh ^ rh;
		}
	}
    List<ValueStream24> GetValues(char c, Dictionary<string, ValueStream24> values)
    {
        var od = values.Where(kvp => kvp.Key.StartsWith(c)).OrderByDescending(kvp => kvp.Key).ToDictionary();
        var list = od.Values.ToList();
        return list;
    }
    long GetBinary(char c, Dictionary<string, ValueStream24> values)
    {
        var od = values.Where(kvp => kvp.Key.StartsWith(c)).OrderByDescending(kvp => kvp.Key).ToDictionary();
		var list = GetValues(c, values);
		long rv = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var v = list[list.Count - i - 1];
            if (v.Val == 1)
                rv += (long)Math.Pow(2, i);
        }
        return rv;
    }
    long GetBinary(char c, Dictionary<string, int> values)
	{
		var od = values.Where(kvp => kvp.Key.StartsWith(c)).OrderByDescending(kvp => kvp.Key).ToDictionary();
		var list = od.Values.ToList();
		long rv = 0;
		for (int i = 0; i < list.Count; i++)
		{
			var v = list[list.Count - i - 1];
			if (v == 1)
				rv += (long)Math.Pow(2, i);
		}
		return rv;
	}
	public class ValueStream24
	{
        public int Val { get; }
        public Op24? Op { get; }
        public ValueStream24(int val)
        {
            Val = val;
        }
        public ValueStream24(Op24 op)
        {
            Val = op.Val??-1;
            Op = op;
        }
        public override string ToString()
        {
            return $"{Val} by {Op}";
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
		var values = new Dictionary<string, ValueStream24>();
		var ops = new List<Op24>();
		foreach (var line in lines)
		{
			if (line.Contains(':'))
			{
				var parts = line.Split(':', StringSplitOptions.TrimEntries);
				values.Add(parts[0], new ValueStream24(int.Parse(parts[1])));
			}
			else if (!string.IsNullOrWhiteSpace(line))
				ops.Add(new Op24(line));
		}
        while (ops.Any(o => o.Val == null && o.Res.StartsWith('z')))
        {
            foreach (var op in ops.Where(o => o.Val == null))
            {
                if (values.ContainsKey(op.Lh) && values.ContainsKey(op.Rh))
                {
                    op.Connect(values[op.Lh].Val, values[op.Rh].Val);
					values[op.Res] = new ValueStream24(op);
                }
            }
        }

        var x = GetBinary('x', values);
		var y = GetBinary('y', values);
		var planZ = x & y;
        var zs = GetValues('z', values);
        var levels = new Dictionary<int, List<ValueStream24>>();
        foreach (var z in zs)
		{
			 Console.WriteLine(z);

			var vs = new List<ValueStream24>();
			vs.Add(z);
			var level = 0;
			while (vs.Any())
			{
				if (levels.ContainsKey(level))
					levels[level].AddRange(vs.ToList());
				else
					levels[level] = vs.ToList();
				level++;
                var newVs = new List<ValueStream24>();
				foreach (var v in vs)
				{
					if (v.Op != null)
					{
                        Console.WriteLine(v.Op);
                        newVs.Add(values[v.Op.Lh]);
						newVs.Add(values[v.Op.Rh]);
					}
				}
				vs = newVs;
			}
		}
		check.Compare(rv);
		return rv;
	}
}

