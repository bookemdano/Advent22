using AoCLibrary;
namespace Advent19;

internal class Day02 : IRunner
{
	// Day https://adventofcode.com/2019/day/2
	// Input https://adventofcode.com/2019/day/2/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 3500L);
		else
			res.Check = new StarCheck(key, 5434663L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var parts = lines[0].Split(',').Select(x => long.Parse(x)).ToArray();
        if (isReal)
        {
            parts[1] = 12;
            parts[2] = 2;
        }

        for (int i = 0; i < parts.Length; i += 4)
		{
            var opCode = parts[i];
            var lh = parts[i + 1];
            var rh = parts[i + 2];
            var eq = parts[i + 3];
            if (opCode == 1)
            {
                parts[eq] = parts[lh] + parts[rh];
            }
            else if (opCode == 2)
            {
                parts[eq] = parts[lh] * parts[rh];
            }
            else if (opCode == 99)
            {
                break;
            }
            else
            {
                Utils.Assert(false, "Bad opCode " + opCode);
            }


            /*if (part == 19690720)
			{
				rv = 100 * parts[1] + parts[2];
				break;
			}*/

		}
        rv = parts[0];

        res.CheckGuess(rv);
        return res;
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 0L);
		else
			res.Check = new StarCheck(key, 4559L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        for(int n = 0; n <= 99; n++)
        {
            for(int v = 0; v <= 99; v++)
            {
                var parts = lines[0].Split(',').Select(x => long.Parse(x)).ToArray();
                if (isReal)
                {
                    parts[1] = n;
                    parts[2] = v;
                }

                for (int i = 0; i < parts.Length; i += 4)
                {
                    var opCode = parts[i];
                    var lh = parts[i + 1];
                    var rh = parts[i + 2];
                    var eq = parts[i + 3];
                    if (opCode == 1)
                    {
                        parts[eq] = parts[lh] + parts[rh];
                    }
                    else if (opCode == 2)
                    {
                        parts[eq] = parts[lh] * parts[rh];
                    }
                    else if (opCode == 99)
                    {
                        break;
                    }
                    else
                    {
                        Utils.Assert(false, "Bad opCode " + opCode);
                    }


                }
                if (parts[0] == 19690720)
                {
                    rv = 100 * n + v;
                    break;
                }

            }
        }

        res.CheckGuess(rv);
        return res;
	}
}

