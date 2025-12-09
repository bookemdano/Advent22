using AoCLibrary;
namespace Advent25;

internal class Day01 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/1
	// Input https://adventofcode.com/2025/day/1/input
	public RunnerResult Star1(bool isReal)
	{
		var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 3L);
		else
            res.Check = new StarCheck(key, 1034L);

		var lines = Program.GetLines(key);
        //var text = Program.GetText(check.Key);
        var rv = 0L;
		// magic
		var d = 50;
		int max = 100;
		foreach (var line in lines)
		{
			var c = line[0];
			var v = int.Parse(line[1..]);
			if (c == 'R')
				d += v;
			else if (c == 'L')
				d -= v;
			while (d < 0)
			{
				d += max;
			}
			while (d >= max)
			{
				d -= max;
			}
			if (d == 0)
				rv++;
		}

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
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var d = 50;
		foreach (var line in lines)
		{
			var origD = d;
			var c = line[0];
			var v = int.Parse(line[1..]);
			if (c == 'L')
				v = 0 - v;
            d = Rotate(d, v, out int zeros);
			rv += zeros;

        }
        // wrong 7207
        // too low 6059
        // too high 6173
        // wrong 6151
        res.CheckGuess(rv);
        return res;
    }
    int Rotate(int from, int clicks, out int zeros)
	{
		var step = 1;
        int max = 100;
        if (clicks < 0)
			step = -1;	
		var to = from + step;
        if (to < 0)
            to += max;
        else if (to >= max)
            to -= max;
        
		zeros = 0;
		for(int i = 1; i < Math.Abs(clicks); i++)
		{
			if (to == 0)
				zeros++;
            to += step;
			if (to < 0)
				to += max;
            else if (to >= max)
                to -= max;
        }
		if (to == 0)
			zeros++;
        return to;
    }

    int Rotate2(int from, int clicks, out int zeros)
	{
        int max = 100;
		int rv = from;
		zeros = 0;
		var turns = Math.Abs(clicks) / max;
		if (turns > 0)
			zeros += turns;
		if (from == 0)
			zeros--;

		clicks = clicks % max;
		rv += clicks;

        if (rv < 0)
        {
            rv += max;
            zeros++;
        }
        if (rv >= max)
        {
            rv -= max;
            zeros++;
        }
		if (rv == 0)
            zeros++;
        return rv;
	}
}

