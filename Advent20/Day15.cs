using AoCLibrary;
using System.Runtime;
namespace Advent20;
// #working
internal class Day15 : IRunner
{
	// Day https://adventofcode.com/2020/day/15
	// Input https://adventofcode.com/2020/day/15/input
	public RunnerResult Star1(bool isReal)
	{
		var key = new StarCheckKey(StarEnum.Star1, isReal, null);
		var res = new RunnerResult();
		if (!isReal)
			res.Check = new StarCheck(key, 1836L);
		else
			res.Check = new StarCheck(key, 206L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
            var lngs = line.Split(',').Select(l => long.Parse(l)).ToList();
            for (int i = 0; i < 2020; i++)
            {
                if (lngs.Count() > i)
                    continue;
                var last = lngs[i - 1];
                var lastI = LastBefore(lngs, i - 1, last);
                if (lastI == -1)
                    lngs.Add(0L);
                else
                    lngs.Add((i - 1) - lastI);
            }
            rv = lngs.Last();
        }
        res.CheckGuess(rv);
        return res;
    }
	int LastBefore(List<long> lngs, int start, long target)
	{
		for (int i = start - 1; i >= 0; i--)
			if (lngs[i] == target)
				return i;
		return -1;
	}
    public class LastOnes
    {
        public long Last { get; set; } = -1;

        public long Previous { get; set; } = -1;
        public void SetLast(long l)
        {
            Previous = Last;
            Last = l;
        }
        public override string ToString()
        {
            return $"l:{Last} p:{Previous}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 362L);
		else
			res.Check = new StarCheck(key, 955L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        foreach (var line in lines)
        {
            var lngs = line.Split(',').Select(l => long.Parse(l)).ToList();
            var founds = new Dictionary<long, LastOnes>();//number, last indexes
            var last = 0L;
            for (int i = 0; i < 3E7; i++)
            {
                long v = 0;
                if (lngs.Count() > i)
                    v = lngs[i];
                else if (founds[last].Previous == -1)
                    v = 0;
                else
                    v = (i - 1) - founds[last].Previous;

                if (!founds.ContainsKey(v))
                    founds.Add(v, new LastOnes());
                founds[v].SetLast(i);
                last = v;
                if (i == 2019)
                {
                    ElfHelper.DayLogPlus(line + " " + last);
                }
            }
            rv = last;
            ElfHelper.DayLogPlus(line + " " + rv);
        }
        res.CheckGuess(rv);
        return res;
	}
}

