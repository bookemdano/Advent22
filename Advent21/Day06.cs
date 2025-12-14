using AoCLibrary;
namespace Advent21;

internal class Day06 : IRunner
{
	// Day https://adventofcode.com/2021/day/6
	// Input https://adventofcode.com/2021/day/6/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 5934L);
		else
			res.Check = new StarCheck(key, 363101L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var counts = lines[0].Split(',').Select(p => int.Parse(p)).ToList();
		for(int i = 0; i < 80; i++)
		{
			var adds = new List<int>();
            for (int iCount = 0; iCount < counts.Count(); iCount++)
            {
				if (counts[iCount] == 0)
				{
					counts[iCount] = 6;
					adds.Add(8);
				}
				else
                    counts[iCount]--;
            }
			counts.AddRange(adds);
        }
		rv = counts.Count();

        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 26984457539L);
		else
			res.Check = new StarCheck(key, 1644286074024L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var days = lines[0].Split(',').Select(p => int.Parse(p)).ToList();
        Dictionary<int, long> dict = [];
        foreach(var day in days)
        {
            TryAdd(dict, day, 1);
        }

        for (int i = 0; i < 256; i++)
        {
            var oldZeros = 0L;

            if(dict.ContainsKey(0))
            {
                oldZeros = dict[0];
                dict[0] = 0;
            }
            foreach (var day in dict.Keys.OrderBy(c => c))
            {
                if (dict[day] > 0)
                {
                    TryAdd(dict, day - 1, dict[day]);
                    dict[day] = 0;
                }
            }
            TryAdd(dict, 8, oldZeros);
            TryAdd(dict, 6, oldZeros);
        }
        rv = dict.Sum(d => d.Value);
        res.CheckGuess(rv);
        return res;
	}
    void TryAdd(Dictionary<int, long> dict, int days, long val)
    {
        if (!dict.ContainsKey(days))
            dict.Add(days, 0);

        dict[days] += val;

    }
}

