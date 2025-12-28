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

        var rules = new List<Rule19>();
        var msgs = new List<string>();
        foreach (var line in lines)
        {
            if (line.Contains(':'))
                rules.Add(new(line));
            else if (string.IsNullOrWhiteSpace(line))
                continue;
            else 
                msgs.Add(line);
        }
        Rule19.AllRules = rules;
        /*var r1 = new Rule19("6: 4 5");
        var sz1 = r1.GetRuleStrings();
        var r2 = new Rule19("7: 4 5 | 5 4");
        var sz2 = r2.GetRuleStrings();
        var sz3 = rules[0].GetRuleStrings();
        */

        var rule0 = Rule19.GetRule(0);
        foreach (var msg in msgs)
            if (rule0.Matches(msg))
                rv++;

        // not 0
        res.CheckGuess(rv);
        return res;
    }
    class SubRulesSet
    {
        int[] _ruleIds;
        public SubRulesSet(string subPart)
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
            foreach(var ruleId in _ruleIds)
                rv.Add(Rule19.GetRule(ruleId));
            return rv;
        }
    }

    class Rule19
	{
        public int Id { get; }
        public char Target { get; } = '-';
        public List<SubRulesSet> SubRuleIds { get; } = [];


        public Rule19(string line)
        {
            var parts = line.Split(": ");            
            Id = int.Parse(parts[0]);
            var rule = parts[1];
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
                return $"{Id}: {Target}";
            else
                return $"{Id}: {string.Join("|| ", SubRuleIds)}";
        }

        internal bool HasTarget => Target != '-';

        internal bool Matches(string msg)
        {
            var targetStrings = GetRuleStrings();
            return (targetStrings.Any(t => msg == t));
        }
        List<string>? _ruleStrings;
        internal List<string> GetRuleStrings()
        {
            if (_ruleStrings != null)
                return _ruleStrings;

            if (HasTarget)
            {

                _ruleStrings = new List<string> { Target.ToString() };
                return _ruleStrings;
            }

            var rv = new List<string>();
            foreach (var orRule in SubRuleIds)
            {
                var strs = new List<string>() { string.Empty };
                foreach (var andRule in orRule.GetRules())
                {
                    var ruleStrings = andRule.GetRuleStrings();
                    strs = AddToAll(strs, ruleStrings);
                }
                rv.AddRange(strs);
            }
            _ruleStrings = rv;
            return rv;
        }

        List<string> AddToAll(List<string> strs, string added)
        {
            return strs.Select(s => s + added).ToList();
        }
        List<string> AddToAll(List<string> strs, List<string> addeds)
        {
            var rv = new List<string>();
            foreach (var added in addeds)
                rv.AddRange(AddToAll(strs, added));
            return rv;
        }
        public static List<Rule19> AllRules { get; internal set; }

        static public Rule19 GetRule(int ruleId)
        {
            return AllRules.Single(r => r.Id == ruleId);
        }

    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

