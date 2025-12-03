using AoCLibrary;

namespace Advent25;

internal class Day03 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2025/day/3
	// Input https://adventofcode.com/2025/day/3/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
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

		check.Compare(rv);
		return rv;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
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

        check.Compare(rv);
		return rv;
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

