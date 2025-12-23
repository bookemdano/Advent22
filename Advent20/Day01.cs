using AoCLibrary;
namespace Advent20;

internal class Day01 : IRunner
{
	// Day https://adventofcode.com/2020/day/1
	// Input https://adventofcode.com/2020/day/1/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 514579L);
		else
			res.Check = new StarCheck(key, 926464L);

		var lines = RunHelper.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var expenses = lines.Select(l => int.Parse(l)).ToList();
		for (int i = 0; i < expenses.Count() - 1; i++)
		{
			for (int j = i + 1; j < expenses.Count(); j++)
			{
				if (expenses[i] + expenses[j] == 2020)
				{
					rv = expenses[i] * expenses[j];
					break;
				}
            }
			if (rv > 0)
				break;
		}

		res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 241861950L);
		else
			res.Check = new StarCheck(key, 65656536L);

		var lines = RunHelper.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var expenses = lines.Select(l => int.Parse(l)).ToList();
        for (int i = 0; i < expenses.Count() - 2; i++)
        {
            for (int j = i + 1; j < expenses.Count() - 1; j++)
            {
                for (int k = j + 1; k < expenses.Count(); k++)
                {
                    if (expenses[i] + expenses[j] + expenses[k] == 2020)
                    {
                        rv = expenses[i] * expenses[j] * expenses[k];
                        break;
                    }
                }
                if (rv > 0)
                    break;
            }
            if (rv > 0)
                break;
        }

        res.CheckGuess(rv);
        return res;
	}
}

