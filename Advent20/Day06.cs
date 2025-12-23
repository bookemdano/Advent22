using AoCLibrary;
namespace Advent20;

internal class Day06 : IRunner
{
	// Day https://adventofcode.com/2020/day/6
	// Input https://adventofcode.com/2020/day/6/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 11L);
		else
			res.Check = new StarCheck(key, 6885L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
        // magic
        var chars = new List<char>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                rv += chars.Count();
                chars = new List<char>();
            }
            else
            {
                foreach(var c in line)
                    if (!chars.Contains(c))
                        chars.Add(c);
            }
        }
        rv += chars.Count();

        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 6L);
		else
			res.Check = new StarCheck(key, 3550L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var chars = new Dictionary<char, int>();
        int people = 0;
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                rv += chars.Count(k => k.Value == people);
                chars = new Dictionary<char, int>();
                people = 0;
            }
            else
            {
                people++;
                foreach (var c in line)
                {
                    if (!chars.ContainsKey(c))
                        chars.Add(c, 0);
                    chars[c]++;
                }
            }
        }
        rv += chars.Count(k => k.Value == people);

        res.CheckGuess(rv);
        return res;
	}
}

