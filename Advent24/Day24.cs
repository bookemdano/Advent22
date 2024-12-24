using AoCLibrary;
using Microsoft.Win32;
namespace Advent24;

internal class Day24 : IDayRunner
{
	public bool IsReal => false;

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
		var ops = new List<Ops24>();
		foreach (var line in lines)
		{
			if (line.Contains(':'))
			{
				var parts = line.Split(':', StringSplitOptions.TrimEntries);
				values.Add(parts[0], int.Parse(parts[1]));
			}
			else if (!string.IsNullOrWhiteSpace(line))
				ops.Add(new Ops24(line));
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
	public class Ops24
	{
		public Ops24(string str)
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
		var values = new Dictionary<string, int>();
		var ops = new List<Ops24>();
		foreach (var line in lines)
		{
			if (line.Contains(':'))
			{
				var parts = line.Split(':', StringSplitOptions.TrimEntries);
				values.Add(parts[0], int.Parse(parts[1]));
			}
			else if (!string.IsNullOrWhiteSpace(line))
				ops.Add(new Ops24(line));
		}
		var x = GetBinary('x', values);
		var y = GetBinary('y', values);
		var planZ = x & y;


		check.Compare(rv);
		return rv;
	}
}

