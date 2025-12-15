using AoCLibrary;
namespace Advent21;

internal class Day08 : IRunner
{
	// Day https://adventofcode.com/2021/day/8
	// Input https://adventofcode.com/2021/day/8/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 26L);
		else
			res.Check = new StarCheck(key, 543L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
			var parts = line.Split('|');
			var pat = new Pattern8(parts[0]);
            var sols = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var sol in sols)
            {
                var n = pat.GetNumber(sol);
                if (n == 1 || n == 4 || n == 7 || n == 8)
                    rv++;
            }
        }

        res.CheckGuess(rv);
        return res;
    }
	class Pattern8
	{
		Dictionary<int, string> _dict = [];
		public Pattern8(string patterns)
        {
            var parts = patterns.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(s => RightString(s)).ToList();
            RemovePart(7, parts, p => p.Length == 3);
            RemovePart(4, parts, p => p.Length == 4);
            RemovePart(1, parts, p => p.Length == 2);
            RemovePart(8, parts, p => p.Length == 7);
            var top = Delta(_dict[7], _dict[1]);
            var right = _dict[1];
            RemovePart(3, parts, p => p.Length == 5 && ContainsAll(p, right));
            RemovePart(6, parts, p => p.Length == 6 && !ContainsAll(p, right));

            var upperRight = Delta(_dict[8], _dict[6]);
            RemovePart(5, parts, p => p.Length == 5 && !ContainsAll(p, upperRight));
            RemovePart(2, parts, p => p.Length == 5);
            var lowerLeft = Delta(_dict[6], _dict[5]);
            RemovePart(0, parts, p => p.Length == 6 && ContainsAll(p, lowerLeft));
            RemovePart(9, parts, p => p.Length == 6);
        }

        private string RightString(string str)
        {
            return new string(str.OrderBy(x => x).ToArray());
        }

        static bool ContainsAll(string str, string chars)
        {
            foreach (var c in chars)
                if (!str.Contains(c))
                    return false;
            return true;
        }
        private void RemovePart(int key, List<string> parts, Func<string, bool> func)
        {
            _dict[key] = parts.Single(func);
            parts.Remove(_dict[key]);
        }

        string Delta(string strLong, string strShort)
        {
            string rv = "";
            foreach (var c in strLong)
            {
                if (!strShort.Contains(c))
                    rv += c;
            }
            return rv;
        }
        internal int GetNumber(string val)
        {
            val = RightString(val);
            foreach (var kvp in _dict)
            {
                if (kvp.Value == val)
                    return kvp.Key;
            }
            return -1;
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 61229L);
		else
			res.Check = new StarCheck(key, 994266L);

		var lines = Program.GetLines(key);
        //var text = Program.GetText(key);

        var rv = 0L;
        // magic
        foreach (var line in lines)
        {
            var parts = line.Split('|');
            var pat = new Pattern8(parts[0]);
            var sols = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var digits = string.Empty;
            foreach (var sol in sols)
            {
                digits += pat.GetNumber(sol).ToString();
            }
            rv += int.Parse(digits);
        }


        res.CheckGuess(rv);
        return res;
	}
}

