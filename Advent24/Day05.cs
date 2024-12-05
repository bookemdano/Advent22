using AoCLibrary;
using System.Runtime.CompilerServices;

namespace Advent24;

internal class Day05 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/5
	// Input https://adventofcode.com/2024/day/5/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 143L);
		else
			check = new StarCheck(key, 5713L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var rules = new List<Rule>();
		var pages = new List<List<int>>();
		var broken = false;
		foreach (var line in lines)
		{
			if (line.Length == 0)
				broken = true;
			else if (!broken)
				rules.Add(new Rule(line));
			else
			{
				var parts = line.Split(',');
				pages.Add(parts.Select(p => int.Parse(p)).ToList());
			}
		}

		foreach(var pageSet in pages)
		{
			if (Rule.CheckRules(rules, pageSet, false))
				rv += pageSet[pageSet.Count / 2];
		}

		check.Compare(rv);
		return rv;
	}
	public class Rule
	{
		public int P1 { get; set; }
		public int P2 { get; set; }
		public Rule(string line)
		{
			var parts = line.Split('|');
			P1 = int.Parse(parts[0]);
			P2 = int.Parse(parts[1]);
		}

		internal bool IsValid(List<int> pageSet, bool fix)
		{
			var f1 = pageSet.FindIndex(p => p == P1);
			var f2 = pageSet.FindIndex(p => p == P2);
			if (f1 == -1 || f2 == -1)
				return true;
			var rv = f1 < f2;
			if (fix && f2 < f1)
			{
				pageSet[f2] = P1;
				pageSet[f1] = P2;
			}
			return rv;
		}
		internal void Fix(List<int> pageSet)
		{
			var f1 = pageSet.FindIndex(p => p == P1);
			var f2 = pageSet.FindIndex(p => p == P2);
		}

		static public bool CheckRules(IEnumerable<Rule> rules, List<int> pageSet, bool fix)
		{
			var isValid = true;
			foreach (var rule in rules)
			{
				if (!rule.IsValid(pageSet, fix))
				{
					isValid = false;
					if (fix == false)
						break;
				}
			}
			return isValid;
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 123L);
		else
			check = new StarCheck(key, 5180L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		var rules = new List<Rule>();
		var pages = new List<List<int>>();
		var broken = false;
		foreach (var line in lines)
		{
			if (line.Length == 0)
				broken = true;
			else if (!broken)
				rules.Add(new Rule(line));
			else
			{
				var parts = line.Split(',');
				pages.Add(parts.Select(p => int.Parse(p)).ToList());
			}
		}
		rules = rules.OrderBy(r => r.P1).ToList();

		int max = 0;
		foreach (var pageSet in pages)
		{
			if (Rule.CheckRules(rules, pageSet, false))
				continue;   // already in order
			int i = 0;
			while(true)
			{
				i++;
				if (Rule.CheckRules(rules, pageSet, true))
				{
					rv += pageSet[pageSet.Count / 2];
					break;
				}
			}
			if (i > max)
				max = i;
		}

		check.Compare(rv);
		return rv;
	}
}

