using AoCLibrary;
using System.Data.Common;
namespace Advent20;
// #working
internal class Day11 : IRunner
{
	// Day https://adventofcode.com/2020/day/11
	// Input https://adventofcode.com/2020/day/11/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 37L);
		else
			res.Check = new StarCheck(key, 2243L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);
        var change = true;
        int i = 0;
        while(change)
		{
            i++;
            change = false;
            var empties = map.FindAll('L');
            var fills = new List<Loc>();
            foreach (var empty in empties)
            {
                var dict = Nearbys(map, empty);
                if (!dict.ContainsKey('#'))
                {
                    change = true;
                    fills.Add(empty);
                }
            }
            foreach (var fill in fills)
                map.Set(fill, '#');
            //ElfHelper.DayLogPlus($"After r:{i} A\n" + map);
            fills = map.FindAll('#');
            empties = new List<Loc>();
            foreach (var fill in fills)
            {
                var dict = Nearbys(map, fill);
                if (dict.ContainsKey('#') && dict['#'] >= 4)
                {
                    change = true;
                    empties.Add(fill);
                }
            }
            foreach (var empty in empties)
                map.Set(empty, 'L');
            //ElfHelper.DayLogPlus($"After r:{i} B\n" + map);
        }
        rv = map.FindAll('#').Count();

        res.CheckGuess(rv);
        return res;
    }
    Dictionary<char, int> Nearbys(GridMap map, Loc loc)
    {
        var adjacents = loc.All8Moves();
        var rv = new Dictionary<char, int>();
        foreach (var adj in adjacents)
        {
            var c = map.Get(adj);
            if (c == null)
                continue;
            TryInc(rv, (char)c);
        }
        return rv;
    }
    void TryInc(Dictionary<char, int> dict, char c)
    {
        if (!dict.ContainsKey(c))
            dict.Add(c, 0);
        dict[c]++;

    }
    Dictionary<char, int> Inlines(GridMap map, Loc loc)
    {
        var rv = new Dictionary<char, int>();
        foreach (DirEnum dir in Enum.GetValues(typeof(DirEnum)))
        {
            var tryIt = loc.Move(dir);
            var c = map.Get(tryIt);
            while (c != null)
            {
                if (c == '#' || c == 'L')
                {
                    TryInc(rv, (char)c);
                    break;
                }
                tryIt = tryIt.Move(dir);
                c = map.Get(tryIt);
            }
        }
        return rv;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 26L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var map = new GridMap(lines);
        var change = true;
        int i = 0;
        while (change)
        {
            i++;
            change = false;
            var empties = map.FindAll('L');
            var fills = new List<Loc>();
            foreach (var empty in empties)
            {
                var dict = Inlines(map, empty);
                if (!dict.ContainsKey('#'))
                {
                    change = true;
                    fills.Add(empty);
                }
            }
            foreach (var fill in fills)
                map.Set(fill, '#');
            ElfHelper.DayLogPlus($"After r:{i} A\n" + map);
            fills = map.FindAll('#');
            empties = new List<Loc>();
            foreach (var fill in fills)
            {
                var dict = Inlines(map, fill);
                if (dict.ContainsKey('#') && dict['#'] >= 5)
                {
                    change = true;
                    empties.Add(fill);
                }
            }
            foreach (var empty in empties)
                map.Set(empty, 'L');
            ElfHelper.DayLogPlus($"After r:{i} B\n" + map);
        }
        rv = map.FindAll('#').Count();

        res.CheckGuess(rv);
        return res;
	}
}

