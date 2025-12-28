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
        static Dictionary<int, Rule19> _dict = [];
        static public void Add(string line)
        {
            var parts = line.Split(": ");
            var key = int.Parse(parts[0]);
            var val = new Rule19(parts[1]);
            _dict.Add(key, val);
        }

        internal static void Clear()
        {
            _dict.Clear();
        }
        static public Rule19 GetRule(int ruleId)
        {
            return _dict[ruleId];
        }
    }

    class Rule19
	{
        public char Target { get; } = '-';
        public List<SubRuleIdSet> SubRuleIds { get; } = [];


        public Rule19(string rule)
        {
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
        static int _maxLen;
        internal bool Matches(string msg)
        {
            var targetStrings = GetRuleStrings();
            var maxLen = targetStrings.Max(s => s.Length);
            if (maxLen > _maxLen)
                _maxLen = maxLen;
            //Utils.Assert(targetStrings.Distinct().Count() == targetStrings.Count(), "Just enough");

            return (targetStrings.Any(t => msg == t));
        }
        HashSet<string>? _ruleStrings;
        internal HashSet<string> GetRuleStrings()
        {
            if (_ruleStrings == null)
            {
                if (HasTarget)
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
                            var ruleStrings = andRule.GetRuleStrings();
                            strs = AddToAll(strs, ruleStrings);
                        }
                        rv.AddRange(strs);
                    }
                    Utils.Assert(rv.Distinct().Count() == rv.Count(), "Just enough");

                    _ruleStrings = rv.ToHashSet();
                }
            }
            return _ruleStrings;
        }
        HashSet<string> AddToAll(HashSet<string> strs, string added)
        {
            return strs.Select(s => s + added).ToHashSet();
        }
        HashSet<string> AddToAll(HashSet<string> strs, HashSet<string> addeds)
        {
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
        var rule8 = RuleSet19.GetRule(8);
        //rules.Remove(rule8);


        var rule0 = RuleSet19.GetRule(0);
        foreach (var msg in msgs)
            if (rule0.Matches(msg))
                rv++;
        res.CheckGuess(rv);
        return res;
	}
}

