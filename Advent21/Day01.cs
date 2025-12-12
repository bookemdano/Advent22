using AoCLibrary;

namespace Advent21;

internal class Day01 : IRunner
{
	// Day https://adventofcode.com/2021/day/1
	// Input https://adventofcode.com/2021/day/1/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 7L);
		else
			res.Check = new StarCheck(key, 1288L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		
		// magic

		var ds = lines.Select(l => (int) int.Parse(l)).ToArray();
        int last = int.MaxValue;
        foreach(var d in ds)
		{
            if (d > last)
                rv++;
            last = d;
        }

        res.CheckGuess(rv);
        return res;
    }
    class Set01
    {
        public int Current { get; set; }
        public int? Last { get; set; }

        internal bool Check()
        {
            var rv = (Last != null && (Current > Last));
            Last = Current;
            Current = 0;
            return rv;
        }
        public override string ToString()
        {
            return $"c:{Current} l:{Last}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 5L);
		else
			res.Check = new StarCheck(key, 1311L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var ds = lines.Select(l => (int)int.Parse(l)).ToArray();
        Set01[] sets = new Set01[4];
        var lasts = new Queue<int>();
        for (var i = 0; i < ds.Length; i++)
        {
            var d = ds[i];
            var mod = i % 4;

            lasts.Enqueue(d);
            if (lasts.Count == 4)
            {
                var old = lasts.Take(3).Sum();
                var cur = lasts.Skip(1).Take(3).Sum();
                if (cur > old)
                    rv++;
                lasts.Dequeue();
            }
        }

        res.CheckGuess(rv);
        return res;
	}
}

