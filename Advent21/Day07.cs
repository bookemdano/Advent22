using AoCLibrary;
namespace Advent21;

internal class Day07 : IRunner
{
	// Day https://adventofcode.com/2021/day/7
	// Input https://adventofcode.com/2021/day/7/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 37L);
		else
			res.Check = new StarCheck(key, 356958L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        var crabs = lines[0].Split(',').Select(p => int.Parse(p)).ToList();
        var dict = new Dictionary<int, long>();
        foreach(var crab in crabs)
            TryAdd(dict, crab, 1);

        //var avg = (int) crabs.Average();
        rv = long.MaxValue;
        for(int i = 0; i < crabs.Max(); i++)
        {
            var sum = dict.Sum(kvp => Cost1(kvp.Key, kvp.Value, i));
            if (sum < rv)
                rv = sum;
        }

        res.CheckGuess(rv);
        return res;
    }
    void TryAdd(Dictionary<int, long> dict, int key, long val)
    {
        if (!dict.ContainsKey(key))
            dict.Add(key, 0);

        dict[key] += val;

    }
    long Cost1(int key, long val, int target)
    {
        return Math.Abs(key - target) * val;
    }
    long Cost2(int key, long val, int target)
    {
        var diff = Math.Abs(key - target);

        var rv = 0L;
        for (int i = 0; i <= diff; i++)
            rv += i;
        return rv * val;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 168L);
		else
			res.Check = new StarCheck(key, 105461913L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var crabs = lines[0].Split(',').Select(p => int.Parse(p)).ToList();
        var dict = new Dictionary<int, long>();
        foreach (var crab in crabs)
            TryAdd(dict, crab, 1);

        //var avg = (int) crabs.Average();
        rv = long.MaxValue;
        for (int i = 0; i < crabs.Max(); i++)
        {
            var sum = dict.Sum(kvp => Cost2(kvp.Key, kvp.Value, i));
            if (sum < rv)
                rv = sum;
        }
        res.CheckGuess(rv);
        return res;
	}
}

