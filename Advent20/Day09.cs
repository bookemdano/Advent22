using AoCLibrary;

namespace Advent20;

internal class Day09 : IRunner
{
	// Day https://adventofcode.com/2020/day/9
	// Input https://adventofcode.com/2020/day/9/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 127L);
		else
			res.Check = new StarCheck(key, 177777905L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var ns = lines.Select(l => long.Parse(l)).ToList();
		var preamble = 5;
		if (isReal)
			preamble = 25;
		for(int i = preamble; i < ns.Count; i++)
		{
            var found = IsValid(ns[i], ns.Skip(i - preamble).Take(preamble).ToList());
			if (!found)
			{
				rv = ns[i];
				break;
			}
		}

        res.CheckGuess(rv);
        return res;
    }

    private bool IsValid(long target, List<long> ns)
    {
        for (int i = 0; i < ns.Count() - 1; i++)
        {
            for (int j = i + 1; j < ns.Count(); j++)
            {
                if (ns[i] + ns[j] == target)
                {
					return true;
                }
            }
        }
		return false;
    }
    class Set9 : List<long>
    {
        public long Total => this.Sum();
        public override string ToString()
        {
            return string.Join(',', this);
        }
    }


    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 62L);
		else
			res.Check = new StarCheck(key, 23463012L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var ns = lines.Select(l => long.Parse(l)).ToList();

        var sets = new List<Set9>();
        var preamble = 5;
        if (isReal)
            preamble = 25;
        var target = 127L;
        if (isReal)
            target = 177777905L;
        for (int i = 0; i < ns.Count; i++)
        {
            foreach (var set in sets)
                set.Add(ns[i]);
            sets.RemoveAll(s => s.Count() > preamble);
            sets.Add(new Set9() { ns[i] });
            if (i < preamble)
                continue;
            foreach (var set in sets)
            {
                var found = set.Total == target;
                if (found)
                {
                    rv = set.Max() + set.Min();
                    break;
                }
            }
            if (rv > 0L)
                break;
        }

        //not 50
        res.CheckGuess(rv);
        return res;
	}
}

