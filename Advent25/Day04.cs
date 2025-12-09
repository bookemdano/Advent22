using AoCLibrary;
using System.Net.Security;
using System.Threading;
namespace Advent25;

internal class Day04 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2025/day/4
	// Input https://adventofcode.com/2025/day/4/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 13L);
		else
            res.Check = new StarCheck(key, 1433L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
        var map = new GridMap(lines);
        var locs = map.FindAll('@');
        ElfHelper.DayLog(key);
        foreach (var loc in locs)
        {
            var surrounding = map.FindSurroundingChars(loc);
            if (surrounding.Count(c => c == '@') < 4)
            {
                ElfHelper.DayLog("Remove " + loc);
                rv++;
            }
        }

        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 43L);
		else
            res.Check = new StarCheck(key, 8616);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        var map = new GridMap(lines);
        var locs = map.FindAll('@');
        var oldRv = -1L;
        ElfHelper.DayLog(key);
        while(oldRv != rv)
        {
            ElfHelper.DayLog("Loop");
            oldRv = rv;
            var removes = new List<Loc>();
            foreach (var loc in locs)
            {
                var surrounding = map.FindSurroundingChars(loc);
                if (surrounding.Count(c => c == '@') < 4)
                {
                    ElfHelper.DayLog("Remove " + loc);
                    map.Set(loc, '.'); // could do in removes but slows down algo
                    removes.Add(loc);
                    rv++;
                }
            }
            foreach (var remove in removes)
            {
                //map.Set(remove, '.');
                locs.Remove(remove);
            }
        }
        res.CheckGuess(rv);
        return res;
	}
}

