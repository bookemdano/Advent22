using AoCLibrary;
namespace Advent19;

internal class Day01 : IRunner
{
	// Day https://adventofcode.com/2019/day/1
	// Input https://adventofcode.com/2019/day/1/input
	public RunnerResult Star1(bool isReal)
	{
		var key = new StarCheckKey(StarEnum.Star1, isReal, null);
		var res = new RunnerResult();
		if (!isReal)
			res.Check = new StarCheck(key, 34241L);
		else
			res.Check = new StarCheck(key, 3271994L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var lngs = lines.Select(l => long.Parse(l)).ToList();
		foreach (var lng in lngs)
		{
			var fuel = (lng / 3) - 2;
			rv += fuel;
        }

        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 51316L);
		else
			res.Check = new StarCheck(key, 4905116L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var lngs = lines.Select(l => long.Parse(l)).ToList();
        foreach (var lng in lngs)
        {
            var added = (lng / 3) - 2;
            var fuel = 0L;
            while (added > 0)
			{
                fuel += added;
                added = (added / 3) - 2;
            }
            rv += fuel;
        }

        res.CheckGuess(rv);
        return res;
	}
}

