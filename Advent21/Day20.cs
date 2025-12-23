using AoCLibrary;
using System.Runtime.InteropServices;
using System.Text;
namespace Advent21;
// #working
internal class Day20 : IRunner
{
	// Day https://adventofcode.com/2021/day/20
	// Input https://adventofcode.com/2021/day/20/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 35L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var algo = lines[0];
		var map = new GridMap(lines.Skip(2));
        int growBy = 1;
        for (int i = 0; i < 2; i++)
		{
            var newMap = new GridMap(map.Rows + (growBy * 2), map.Cols + (growBy * 2));
            for (int col = -growBy; col < map.Cols + growBy; col++)
            {
                for (int row = -growBy; row < map.Rows + growBy; row++)
                {
                    var loc = new Loc(row, col);
                    var moves = loc.All9Moves();
                    var sb = new StringBuilder();
                    foreach (var move in moves)
                    {
                        var c = map.Get(move);
                        if (c == null)
                            c = '.';
                        sb.Append(c == '#'?"1": "0");
                    }
                    var val = Convert.ToInt32(sb.ToString(), 2);
                    var newLoc = new Loc(loc.Row + growBy, loc.Col + growBy);
                    newMap.Set(newLoc, algo[val]);
                }
            }
            map = newMap;
            ElfHelper.DayLogPlus(map);
        }
        rv = map.FindAll('#').Count();
        // too high 5481
        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

