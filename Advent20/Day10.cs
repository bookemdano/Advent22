using AoCLibrary;

namespace Advent20;
// #working
internal class Day10 : IRunner
{
	// Day https://adventofcode.com/2020/day/10
	// Input https://adventofcode.com/2020/day/10/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 220L);
		else
			res.Check = new StarCheck(key, 1820L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var lngs = lines.Select(l => long.Parse(l)).OrderBy(l => l).ToList();
		var last = 0L;
		var diff1s = 0L;
		var diff3s = 0L;
		for(int i = 0; i < lngs.Count; i++)
		{
			var delta = lngs[i] - last;
			if (delta == 1)
				diff1s++;
			else if (delta == 3)
				diff3s++;
			else
				diff1s++;
			last = lngs[i];
        }
        diff3s++;
		rv = diff1s * diff3s;
        res.CheckGuess(rv);
        return res;
    }
	Dictionary<int, long> _cache = [];
	long Connect(long[] lngs, int i, long target)
	{
        if (_cache.ContainsKey(i))
            return _cache[i];

        long rv = 0L;
		int max = int.Max(3, lngs.Length);

		var last = 0L;
		if (i >= 0)
			last = lngs[i];
		for(int j = i + 1; j < max; j++)
		{
			if (lngs[j] > last + 3)
				continue;
			if (lngs[j] == target)
				rv += 1;
			else
				rv += Connect(lngs, j, target);
        }
		if(!_cache.ContainsKey(i))
			_cache[i] = rv;
		return rv;
	}

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 19208L);
		else
			res.Check = new StarCheck(key, 3454189699072L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var lngs = lines.Select(l => long.Parse(l)).OrderBy(l => l).ToList();
		_cache = [];
        rv = Connect(lngs.ToArray(), -1, lngs.Last());
        res.CheckGuess(rv);
        return res;
	}
}

