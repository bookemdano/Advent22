using AoCLibrary;
using System.Data;

namespace Advent20;
// #working
internal class Day19 : IRunner
{
    // Day https://adventofcode.com/2020/day/19
    // Input https://adventofcode.com/2020/day/19/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 2L);
        else
            res.Check = new StarCheck(key, 111L);

        var lines = RunHelper.GetLines(key);
        //var text = RunHelper.GetText(key);
        var rv = 0L;
        // magic
        RuleSet19.Clear();
        var msgs = new List<string>();
        foreach (var line in lines)
        {
            if (line.Contains(':'))
                RuleSet19.Add(new(line));
            else if (string.IsNullOrWhiteSpace(line))
                continue;
            else
                msgs.Add(line);
        }
        /*var r1 = new Rule19("6: 4 5");
        var sz1 = r1.GetRuleStrings();
        var r2 = new Rule19("7: 4 5 | 5 4");
        var sz2 = r2.GetRuleStrings();
        var sz3 = rules[0].GetRuleStrings();
        */

        var rule0 = RuleSet19.GetRule(0);
        foreach (var msg in msgs)
            if (rule0.Matches(msg))
                rv++;

        // not 0
        res.CheckGuess(rv);
        return res;
    }
    class SubRuleIdSet
    {
        int[] _ruleIds;
        public SubRuleIdSet(string subPart)
        {
            _ruleIds = subPart.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray();
        }

        public override string ToString()
        {
            return string.Join(' ', _ruleIds);
        }
        public bool Contains(int id)
        {
            return _ruleIds.Contains(id);
        }
        internal List<Rule19> GetRules()
        {
            var rv = new List<Rule19>();
            foreach (var ruleId in _ruleIds)
                rv.Add(RuleSet19.GetRule(ruleId));
            return rv;
        }
    }
    static class RuleSet19
    {
        static List<Rule19> _rules = [];

        public static int MaxLen { get; internal set; } = 1000;

        static public void Add(string line)
        {
            var parts = line.Split(": ");
            var key = int.Parse(parts[0]);
            Remove(key);
            var val = new Rule19(key, parts[1]);
            _rules.Add(val);
        }

        internal static void Clear()
        {
            _rules.Clear();
        }
        static public Rule19 GetRule(int ruleId)
        {
            return _rules.Single(r => r.Id == ruleId);
        }

        internal static void Remove(int ruleId)
        {
            _rules.RemoveAll(r => r.Id == ruleId);
        }
    }

    class Rule19
	{
        public int Id { get; }
        public char Target { get; } = '-';
        public List<SubRuleIdSet> SubRuleIds { get; } = [];


        public Rule19(int key, string rule)
        {
            Id = key;
            if (rule.Contains('\"'))
            {
                var subParts = rule.Split('\"', StringSplitOptions.RemoveEmptyEntries).ToList();
                Target = subParts.Single()[0];
            }
            else
            {
                var subParts = rule.Split("| ", StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach(var subPart in subParts)
                {
                    SubRuleIds.Add(new(subPart));
                }
            }
        }
        public override string ToString()
        {
            if (HasTarget)
                return $"{Target}";
            else
                return $"{string.Join("|| ", SubRuleIds)}";
        }

        internal bool HasTarget => Target != '-';
        internal bool Matches(string msg)
        {
            var targetStrings = GetRuleStrings(0);
            //Utils.Assert(targetStrings.Distinct().Count() == targetStrings.Count(), "Just enough");

            return targetStrings.Contains(msg);
        }
        internal bool MatchAlong(string target)
        {
            var rv = SubMatchAlong(target, []);
            return rv.Any();
        }
        HashSet<string> SubMatchAlong(string target, List<int> checkeds)
        {
            if (checkeds.Contains(Id))
                return new HashSet<string> { "X" };
            checkeds.Add(Id);
            if (HasTarget)
            {
                return new HashSet<string> { Target.ToString() };
            }
            else
            {
                var rv = new List<string>();
                foreach (var orRule in SubRuleIds)
                {
                    var strs = new HashSet<string>() { string.Empty };
                    foreach (var andRule in orRule.GetRules())
                    {
                        var ruleStrings = andRule.SubMatchAlong(target, checkeds.ToList());
                        strs = AddToAll(strs, ruleStrings);
                    }
                    var matches = strs.Where(s => target.StartsWith(s)).ToList();
                    rv.AddRange(strs.Where(s => target.StartsWith(s)));
                }
                return rv.Distinct().ToHashSet();
            }
        }

        HashSet<string>? _ruleStrings;
        internal HashSet<string> GetRuleStrings(int depth)
        {
            if (_ruleStrings == null)
            {
                if (depth++ > RuleSet19.MaxLen)
                {
                    _ruleStrings = new HashSet<string>();
                }
                else if (HasTarget)
                {
                    _ruleStrings = new HashSet<string> { Target.ToString() };
                }
                else
                {
                    var rv = new List<string>();
                    foreach (var orRule in SubRuleIds)
                    {
                        var strs = new HashSet<string>() { string.Empty };
                        foreach (var andRule in orRule.GetRules())
                        {
                            var ruleStrings = andRule.GetRuleStrings(depth);
                            strs = AddToAll(strs, ruleStrings);
                        }
                        rv.AddRange(strs);
                    }
                    _ruleStrings = rv.Distinct().ToHashSet();
                }
            }
            return _ruleStrings;
        }
        HashSet<string> AddToAll(HashSet<string> strs, string added)
        {
            var rv = new HashSet<string>();
            foreach(var str in strs)
            {
                var newStr = str + added;
                if (newStr.Length > RuleSet19.MaxLen)
                    continue;
                rv.Add(newStr);
            }
            return rv;
        }
        HashSet<string> AddToAll(HashSet<string> strs, HashSet<string> addeds)
        {
            if (!addeds.Any())
                return strs;
            var rv = new List<string>();
            foreach (var added in addeds)
                rv.AddRange(AddToAll(strs, added));
            return rv.ToHashSet();
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 12L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        RuleSet19.Clear();
        var msgs = new List<string>();
        foreach (var line in lines)
        {
            if (line.Contains(':'))
                RuleSet19.Add(new(line));
            else if (string.IsNullOrWhiteSpace(line))
                continue;
            else
                msgs.Add(line);
        }
        // overwrite these rules
        RuleSet19.Add("8: 42 | 42 8");
        RuleSet19.Add("11: 42 31 | 42 11 31");
        RuleSet19.MaxLen = msgs.Max(m => m.Length);
        var rule0 = RuleSet19.GetRule(0);
        foreach (var msg in msgs)
            if (rule0.MatchAlong(msg))
            {
                ElfHelper.DayLogPlus(msg);
                rv++;
            }
        res.CheckGuess(rv);
        return res;
	}
}

