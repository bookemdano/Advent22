using AoCLibrary;

namespace Advent25;

internal class Day03 : IDayRunner
{
    // Day https://adventofcode.com/2025/day/3
    // Input https://adventofcode.com/2025/day/3/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        StarCheck check;
        if (!isReal)
            check = new StarCheck(key, 357L);
		else
			check = new StarCheck(key, 17332L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		foreach(var line in lines)
		{
			var biggest = 0;
			var bChar = 0;
            for (int i = 0; i < line.Length-1; i++)
			{
				var v = line[i] - '0';
                if (v > biggest)
				{
					biggest = v;
					bChar = i;
					if (biggest == 9)
						break;
				}
            }
			var b2 = 0;
			for(int i = bChar + 1; i < line.Length; i++)
			{
                var v = line[i] - '0';
				if (v > b2)
				{
					b2 = v;
					if (b2 == 9)
						break;
                }
            }
			rv += biggest*10 + b2;
        }

        var res = new RunnerResult();
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        StarCheck check;
        if (!isReal)
            check = new StarCheck(key, 3121910778619L);
		else
			check = new StarCheck(key, 172516781546707L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
        foreach (var line in lines)
        {
            var digits = 12;
			var iChar = 0;
			var val = 0L;
			while (digits > 0)
			{
				var next = FindNext(line, iChar, digits--);
				var v = line[next] - '0';

                val += v * (long) Math.Pow(10, digits);
				iChar = next + 1;
            }
			rv += val;
        }

        var res = new RunnerResult();
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
    }

    private int FindNext(string line, int iChar, int digitsLeft)
    {
		var len = line.Length - digitsLeft + 1;
		var biggest = 0;
		var rv = -1;
		for (int i = iChar; i < len; i++)
		{
			var v = line[i] - '0';
            if (v > biggest)
			{
				biggest = v;
				rv = i;
				if (biggest == 9)
					break;
			}
        }

		return rv;
    }
}

