using AoCLibrary;

namespace Advent21;
// #working
internal class Day14 : IRunner
{
	// Day https://adventofcode.com/2021/day/14
	// Input https://adventofcode.com/2021/day/14/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 1588L);
		else
			res.Check = new StarCheck(key, 3009L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var template = lines[0];
        var rules = new List<Rule14>();
        foreach (var line in lines.Skip(2))
            rules.Add(new Rule14(line));
        for(int i = 0; i < 10; i++)
            template = Rule14.RunRules(template, rules);

        var dict = Histogram(template);
        rv = dict.Max(x => x.Value) - dict.Min(x => x.Value);

        res.CheckGuess(rv);
        return res;
    }
    Dictionary<char, int> Histogram(string str)
    {
        var dict = new Dictionary<char, int>();
        foreach (var c in str)
        {
            if (!dict.ContainsKey(c))
                dict.Add(c, 0);
            dict[c]++;
        }
        return dict;

    }
    class Rule14
	{
        private char _lh;
        private char _rh;
        public char Insert;

        public Rule14(string line)
		{
            //NC -> O
            _lh = line[0];
            _rh = line[1];
            Insert = line[6];
        }

        internal bool Matches(char lh, char rh)
        {
            return (lh == _lh && rh == _rh);
        }

        internal static string RunRules(string template, List<Rule14> rules)
        {
            var newString = string.Empty;
            for (var iChar = 0; iChar < template.Length - 1; iChar++)
            {
                var c = template[iChar];
                var nextC = template[iChar + 1];
                newString += template[iChar];
                foreach (var rule in rules)
                    if (rule.Matches(c, nextC))
                        newString += rule.Insert;
            }
            newString += template.Last();
            return newString;
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

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var template = lines[0];
        var rules = new List<Rule14>();
        foreach (var line in lines.Skip(2))
            rules.Add(new Rule14(line));
        for (int i = 0; i < 40; i++)
            template = Rule14.RunRules(template, rules);

        var dict = Histogram(template);
        rv = dict.Max(x => x.Value) - dict.Min(x => x.Value);

        res.CheckGuess(rv);
        return res;
	}
}

