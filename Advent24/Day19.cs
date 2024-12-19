using AoCLibrary;
using System.Net;
using static Advent24.Day19;

namespace Advent24;

internal class Day19 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/19
	// Input https://adventofcode.com/2024/day/19/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 6L);
		else
			check = new StarCheck(key, 363L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var towels = Utils.Split(',', lines[0]);
		var patterns = lines.Skip(2).ToList();

		foreach (var pattern in patterns)
		{

			if (CheckPattern(pattern, towels))
				rv++;
		}

		check.Compare(rv);
		return rv;
	}

	public bool CheckPattern(string pattern, string[] towels)
	{
		var posses = new List<string>() { string.Empty };
		while (posses.Any())
		{
			var newPosses = new List<string>();
			foreach (var poss in posses)
			{
				foreach (var towel in towels)
				{
					var newPoss = poss + towel;
					if (pattern == newPoss)
						return true;
					if (pattern.StartsWith(newPoss))
						newPosses.Add(newPoss);
				}
			}
			posses = new List<string>();
			foreach(var poss in newPosses)
			{
				if (!posses.Contains(poss))
					posses.Add(poss);
			}
		}
		return false;
	}
	public List<TowelSet> CheckPattern2b(string pattern, string[] towels)
	{
		var rv = new List<TowelSet>();

		var posses = new List<TowelSet>() { new TowelSet() };
		while (posses.Any())
		{
			var newPosses = new List<TowelSet>();
			foreach (var poss in posses)
			{
				foreach (var towel in towels)
				{
					var newPoss = poss.Add(towel);
					if (pattern == newPoss.Pattern)
						rv.Add(newPoss);
					else if (pattern.StartsWith(newPoss.Pattern))
						newPosses.Add(newPoss);
				}
			}
			posses = new List<TowelSet>();
			foreach (var poss in newPosses)
			{
				if (!posses.Any(p => p.PatternSep == poss.PatternSep))
					posses.Add(poss);
			}
		}
		return rv;
	}
	public class TowelSet
	{
		public TowelSet()
		{
			
		}
		public TowelSet(TowelSet other, string newTowel)
		{
			PatternSep = other.PatternSep + "," + newTowel;
			Pattern = other.Pattern + newTowel;
		}

		public string Pattern { get; set; }
		public string PatternSep { get; set; }
		public override string ToString()
		{
			return PatternSep;
		}

		internal TowelSet Add(string towel)
		{
			var rv = new TowelSet();
			rv.Pattern += towel;
			rv.Pattern += "," + towel;
			return rv;
		}

		internal bool Same(TowelSet other)
		{
			return (PatternSep == other.PatternSep);
		}
	}
	public class TowelRange
	{
		public int Start { get; }
		public int End { get; } 
		public string Pattern => PatternSep.Replace(",", "");
		public string PatternSep { get; }
		public long SubCount { get; private set; }
		public long PrevCounts { get; private set; }	// ways to get to here
		public List<TowelRange> PrevRanges { get; } = [];

		public TowelRange(int start, string patternSep, IEnumerable<TowelRange>? ranges)
		{
			Start = start;
			PatternSep = patternSep;
			End = Start + Pattern.Length;
			if (ranges?.Any() == true)
			{
				PrevCounts = ranges.Sum(r => r.PrevCounts);
				PrevRanges.AddRange(ranges.ToList());
			}
			else
				PrevCounts = 1;
		}

		public override string ToString()
		{
			return $"p:{PatternSep} s:{Start} e:{End} c:{PrevCounts} subs:{PrevRanges.Count()}";
		}

		internal bool Same(TowelRange other)
		{
			return (Start == other.Start && PatternSep == other.Pattern);
		}

		internal void AddSubs(IEnumerable<TowelRange> ranges)
		{
			PrevCounts += ranges.Sum(r => r.PrevCounts);
			foreach(var range in ranges)
			{
				if (!PrevRanges.Any(r => r.Same(range)))
					PrevRanges.Add(range);
			}

		}/*
		internal List<string> GetWholes()
		{
			var rv = new List<string>();
			if (PrevRanges.Any())
			{
				rv.Add(PatternSep);
				return rv;
			}
			foreach (var sub in PrevRanges)
			{
				var subWholes = sub.GetWholes();
				rv.AddRange(subWholes.Select(s => s + "," + PatternSep));
			}
			return rv;
		}*/
		static int _iters = 0;
		static Dictionary<int, long> _dict = [];
		internal long GetWholeCounts(List<TowelRange> allRanges)
		{
			var rv = 0L;
			_iters++;

			if (_dict.ContainsKey(Start))
			{
				_iters--;
				return _dict[Start];
			}

			var befores = allRanges.Where(r => r.End == Start);
			if (befores.Any())
				rv = befores.Count() - 1;
			foreach(var before in befores)
				rv += before.GetWholeCounts(allRanges);
			_dict[Start] = rv;
				
			_iters--;

			/*var rv = 1;
			if (Start == 0)
				return rv;
			var starts = new List<int>() {Start};
			while(starts.Any())
			{
				var newStarts = new List<int>();
				foreach(var start in starts)
				{
					var befores = allRanges.Where(r => r.End == start);
					foreach (var before in befores)
					{
						rv++;
						if (!newStarts.Contains(before.Start))
							newStarts.Add(before.Start);
					}
				}
				starts = newStarts;
			}
			*/
			return rv;
		}

		internal static void ResetCache()
		{
			_dict = [];
		}
	}
	public List<TowelRange> GetRanges(string pattern, string[] towels)
	{
		var allRanges = new List<TowelRange>();
		var ranges = new List<TowelRange>() { new TowelRange(0, "", null) };

		var rv = new List<TowelRange>();
		while (ranges.Any())
		{
			var newRanges = new List<TowelRange>();
			var rangeGroups = ranges.GroupBy(r => r.End);
			foreach (var rangeGroup in ranges.GroupBy(r => r.End))
			{
				var reals = rangeGroup.Where(r => r.End != 0);
				var sub = pattern[rangeGroup.Key..];
				foreach (var towel in towels)
				{
					if (sub.StartsWith(towel))
					{
						var newRange = new TowelRange(rangeGroup.Key, towel, reals);

						var allRange = allRanges.SingleOrDefault(r => r.Same(newRange));
						if (allRange != null)
							allRange.AddSubs(reals);
						else
						{
							allRanges.Add(newRange);
							if (newRange.End < pattern.Length)
								newRanges.Add(newRange);
						}
					}
				}

			}
			ranges = newRanges;
		}
		return allRanges;
	}
	public long CheckPattern2(string pattern, string[] towels)
	{
		var rv = 0L;
		TowelRange.ResetCache();
		var allRanges = GetRanges(pattern, towels);

		var successes = allRanges.Where(r => r.End == pattern.Length).ToList();
		foreach (var range in successes)
		{
			rv += range.GetWholeCounts(allRanges) + 1;
			//rv += range.PrevCounts;
		}

		return rv;
		//return results.Count();
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 16L);
		else
			check = new StarCheck(key, 642535800868438L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var towels = Utils.Split(',', lines[0]);
		var patterns = lines.Skip(2).ToList();

		foreach (var pattern in patterns)
		{
			//var pats = CheckPattern2b(pattern, towels);
			rv += CheckPattern2(pattern, towels);
			Console.WriteLine($"Up to {rv} with {pattern}");
		}
		// 2371874931913 too low
		// 642535800868438
		check.Compare(rv);
		return rv;
	}
}

