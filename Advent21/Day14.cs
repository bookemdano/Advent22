using AoCLibrary;
using System;
using System.Text;

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
        rv = Recur14.Recurse(lines, 10);

        var template = lines[0];
        var rules = new Dictionary<string, char>();
        foreach (var line in lines.Skip(2))
        {
            var pair = line.Substring(0, 2);
            rules.Add(pair, line[6]);
        }
        for(int i = 0; i < 10; i++)
            template = Rule14.RunRulesPaired(template, rules);

        var dict = Histogram(template);
        rv = dict.Max(x => x.Value) - dict.Min(x => x.Value);

        rv = Recur14.Recurse(lines, 10);

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

    class RecurKey14
    {
        public RecurKey14(char a, char b, int depth)
        {
            A = a; B = b; Depth = depth;
            _hashcode = ToString().GetHashCode();
        }
        public char A { get; }

        public char B { get; }
        public int Depth { get; }

        private int _hashcode;

        public override bool Equals(object? obj)
        {
            if (obj is RecurKey14 other)
                return (other.A == A && other.B == B && other.Depth == Depth);
            return false;
        }
        public override int GetHashCode()
        {
            return _hashcode;
        }
        public override string ToString()
        {
            return $"{A} {B} {Depth}";
        }
    }
    class Recur14
    {
        static Dictionary<string, char> rules = [];
        static Dictionary<RecurKey14, Dictionary<char, long>> memo = [];

        static public long Recurse(string[] lines, int depth)
        {
            var template = lines[0];
            foreach (var line in lines.Skip(2))
            {
                var sp = line.Split(" -> ", StringSplitOptions.None);
                rules[sp[0]] = sp[1][0];
            }

            // Counter(tpl)
            var counts = new Dictionary<char, long>();
            foreach (var ch in template)
                counts[ch] = counts.GetValueOrDefault(ch) + 1;

            // sum(map(f, tpl, tpl[1:]), Counter(tpl))
            for (int i = 0; i < template.Length - 1; i++)
            {
                var add = F(template[i], template[i + 1], depth);
                AddCounts(counts, add);
            }

            long max = counts.Values.Max();
            long min = counts.Values.Min();

            return max - min;
        }
        // Equivalent of:
        // @cache
        // def f(a, b, depth):
        //   if depth == 0: return Counter('')
        //   x = rules[a+b]
        //   return Counter(x) + f(a,x,depth-1) + f(x,b,depth-1)
        static Dictionary<char, long> F(char a, char b, int depth)
        {
            var key = new RecurKey14(a, b, depth);
            if (memo.TryGetValue(key, out var cached))
                return cached; // safe because we never mutate cached dictionaries

            if (depth == 0)
            {
                var empty = new Dictionary<char, long>();
                memo[key] = empty;
                return empty;
            }

            var pair = new string(new[] { a, b });
            char x = rules[pair];

            var result = new Dictionary<char, long>();
            result[x] = 1; // Counter(x)

            AddCounts(result, F(a, x, depth - 1));
            AddCounts(result, F(x, b, depth - 1));

            memo[key] = result;
            return result;
        }

        static void AddCounts(Dictionary<char, long> target, Dictionary<char, long> add)
        {
            foreach (var (k, v) in add)
                target[k] = target.GetValueOrDefault(k) + v;
        }
    }


    class Rule14
	{
        private char _lh;
        private char _rh;
        public char Insert;
        public string Pair { get; }
        public string Replacement { get; }
        public Rule14(string line)
		{
            //NC -> O
            _lh = line[0];
            _rh = line[1];
            Insert = line[6];
            Pair = line.Substring(0, 2);
            Replacement = "" + _lh + Insert + _rh; 
        }

        internal bool Matches(char lh, char rh)
        {
            return (lh == _lh && rh == _rh);
        }

        internal static string RunRulesPaired(string template, Dictionary<string, char> rules)
        {
            var sb = new StringBuilder();
            for (var iChar = 0; iChar < template.Length - 1; iChar++)
            {
                sb.Append(template[iChar]);
                var pair = template.Substring(iChar, 2);
                if (rules.ContainsKey(pair))
                    sb.Append(rules[pair]);
                else
                    sb.Append(pair[0]); // never happens
            }
            sb.Append(template.Last());
            return sb.ToString();
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
        class LinkedList
        {

        }
        internal static Template14 RunRules2(Template14 template, Dictionary<string, string> rules)
        {
            var sb = new StringBuilder();
            var newTemplate = new Template14();
            foreach(var kvp in template)
            {
                var newList = kvp.Value.Select(v => v * 2).ToList();
                var newKey = rules[kvp.Key];
                newTemplate.Add(newKey, newList);
            }
            return newTemplate;
        }
    }
    class Template14 : Dictionary<string, List<int>>
    {
        public Template14()
        {
        }

        public Template14(string str)
        {
            for (var iChar = 0; iChar < str.Length - 1; iChar++)
            {
                var pair = str.Substring(iChar, 3);
                if (!ContainsKey(pair))
                    Add(pair, new List<int>());
                this[pair].Add(iChar);
            }
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 2188189693529L);
		else
			res.Check = new StarCheck(key, 3459822539451L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        rv = Recur14.Recurse(lines, 40);

        res.CheckGuess(rv);
        return res;
	}
}

