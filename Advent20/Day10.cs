using AoCLibrary;
using Microsoft.Win32;
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
	long Connect(List<long> lngs, long last)
	{
		long rv = 0L;
        if (lngs.Count() == 1)
            return 1;
        if (lngs.Count() == 0)
            return 0;
		var max = int.Min(3, lngs.Count());
        for (int i = 0; i < max; i++)
		{
			if (lngs[i] > last + 3)
				continue;
			rv += Connect(lngs.Skip(i+1).ToList(), lngs[i]);
        }
		return rv;
	}

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 8L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var lngs = lines.Select(l => long.Parse(l)).OrderBy(l => l).ToList();
		rv = Connect(lngs, 0);
        res.CheckGuess(rv);
        return res;
	}
}

