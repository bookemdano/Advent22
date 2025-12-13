using AoCLibrary;
namespace Advent21;

internal class Day03 : IRunner
{
	// Day https://adventofcode.com/2021/day/3
	// Input https://adventofcode.com/2021/day/3/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 198L);
		else
			res.Check = new StarCheck(key, 3549854L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var grid = new GridMapXY(lines);
		var gamma = string.Empty;
		for (int iCol = 0; iCol < grid.Cols; iCol++)
		{
            var col = grid.GetCol(iCol);
			var sum = col.ToCharArray().Sum(c => c - '0');
            if (sum > grid.Rows / 2)
                gamma += '1';
            else
                gamma += '0';
        }
		var epsilon = string.Empty;
		foreach (var c in gamma)
		{
			if (c == '1')
				epsilon += '0';
			else
				epsilon += "1";
		}
		//parse binary
		rv = Convert.ToInt32(gamma, 2) * Convert.ToInt32(epsilon, 2);

		res.CheckGuess(rv);
        return res;
    }
    bool MoreOnes(IEnumerable<string> lines, int iCol)
    {
        var ones = 0;
        foreach(var line in lines)
        {
            var c = line[iCol];
            if (c == '1')
                ones++;
        }

        var zeros = lines.Count() - ones;
        if (ones >= zeros)
            return true;
        return false;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 230L);
		else
			res.Check = new StarCheck(key, 3765399L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        // o2 rating
        var sublines = lines;
        var iCol = 0;
        while (sublines.Count() > 1)
        {
            var madeIt = new List<string>();
            var target = MoreOnes(sublines, iCol)?'1':'0';
            foreach (var row in sublines)
            {
                if (row[iCol] == target)
                    madeIt.Add(row);
            }
            iCol++;
            sublines = madeIt.ToArray();
        }
        var o2 = Convert.ToInt32(sublines.First(), 2);
        // CO2 rating
        sublines = lines;
        iCol = 0;
        while (sublines.Count() > 1)
        {
            var madeIt = new List<string>();
            var target = MoreOnes(sublines, iCol) ? '0' : '1';
            foreach (var row in sublines)
            {
                if (row[iCol] == target)
                    madeIt.Add(row);
            }
            iCol++;
            sublines = madeIt.ToArray();
        }
        var co2 = Convert.ToInt32(sublines.First(), 2);
        rv = co2 * o2;

        res.CheckGuess(rv);
        return res;
	}
}

