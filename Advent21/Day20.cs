using AoCLibrary;
using System.Text;

namespace Advent21;

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
            res.Check = new StarCheck(key, 5464L);

        var lines = Program.GetLines(key);
        //var text = Program.GetText(key);
        var rv = 0L;
        // magic
        var algo = lines[0];
        var len = algo.Length;
        var map = new GridMap(lines.Skip(2));
        int growBy = 1;
        for (int i = 0; i < 2; i++)
        {
            ElfHelper.DayLogPlus($"map{i}\n" + map);

            var newMap = new GridMap(map.Rows + (growBy * 2), map.Cols + (growBy * 2));
            for (int col = 0; col < newMap.Cols; col++)
            {
                for (int row = 0; row < newMap.Rows; row++)
                {
                    var newLoc = new Loc(row, col);
                    var oldLoc = new Loc(row - growBy, col - growBy);
                    var moves = oldLoc.All9Moves();
                    var sb = new StringBuilder();
                    foreach (var move in moves)
                    {
                        var c = map.Get(move);
                        if (c == null)
                        {
                            if (i > 0)
                                c = algo[0];
                            else
                                c = '.';
                        }
                        sb.Append(c == '#' ? "1" : "0");
                    }
                    var val = Convert.ToInt32(sb.ToString(), 2);
                    newMap.Set(newLoc, algo[val]);
                }
            }
            map = newMap;
        }
        ElfHelper.DayLogPlus($"mapFinal\n" + map);

        rv = map.FindAll('#').Count();
        // too high 5481
        // too low 5346
        // wrong 5679
        res.CheckGuess(rv);
        return res;
    }


    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 3351L);
        else
            res.Check = new StarCheck(key, 19228L);

        var lines = Program.GetLines(key);
        //var text = Program.GetText(key);

        var rv = 0L;
        // magic
        var algo = lines[0];
        var len = algo.Length;
        var map = new GridMap(lines.Skip(2));
        int growBy = 2;
        for (int i = 0; i < 50; i++)
        {
            ElfHelper.DayLogPlus($"map{i}\n" + map);

            var newMap = new GridMap(map.Rows + (growBy * 2), map.Cols + (growBy * 2));
            for (int col = 0; col < newMap.Cols; col++)
            {
                for (int row = 0; row < newMap.Rows; row++)
                {
                    var newLoc = new Loc(row, col);
                    var oldLoc = new Loc(row - growBy, col - growBy);
                    var moves = oldLoc.All9Moves();
                    var sb = new StringBuilder();
                    foreach (var move in moves)
                    {
                        var c = map.Get(move);
                        if (c == null)
                        {
                            if (i > 0)
                                c = algo[0];
                            else
                                c = '.';
                        }
                        sb.Append(c == '#' ? "1" : "0");
                    }
                    var val = Convert.ToInt32(sb.ToString(), 2);
                    newMap.Set(newLoc, algo[val]);
                }
            }
            map = newMap;
        }
        ElfHelper.DayLogPlus($"mapFinal\n" + map);

        rv = map.FindAll('#').Count();
        //rv = Day20x(lines, 50);
        //20617 too high
        res.CheckGuess(rv);
        return res;
    }
}
