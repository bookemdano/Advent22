using AoCLibrary;
using System;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
namespace Advent25;

internal class Day07 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2025/day/7
	// Input https://adventofcode.com/2025/day/7/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        StarCheck check;
        if (!isReal)
			check = new StarCheck(key, 21L);
		else
			check = new StarCheck(key, 1516L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        var grid = new GridMap(lines);
        for (var iRow = 1; iRow < grid.Rows; iRow++)
        {
            for (var iCol = 0; iCol < grid.Cols; iCol++)
            {
                var loc = new Loc(iRow, iCol);
                var c = grid.Get(iRow, iCol);
                var cUp = grid.Get(iRow - 1, iCol);

                if (c == '^' && cUp == '|')
                {
                    var lLoc = new Loc(iRow, iCol - 1);
                    if (grid.Get(lLoc) == '.')
                        grid.Set(lLoc, '|');
                    var rLoc = new Loc(iRow, iCol + 1);
                    if (grid.Get(rLoc) == '.')
                        grid.Set(rLoc, '|');
                    rv++;
                }
                if (c != '.')
                    continue;
                if (cUp == 'S' || cUp == '|')
                    grid.Set(loc, '|');
            }
            //ElfHelper.DayLog(grid.ToString());
        }

        var res = new RunnerResult();
        res.StarValue = rv;
        //1484 too low
        res.StarSuccess = check.Compare(rv);
        return res;
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        StarCheck check;
        if (!isReal)
			check = new StarCheck(key, 40L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        var newLines = new List<string>();
        foreach(var line in lines)
        {
            if (!line.All(c => c == '.'))
                newLines.Add(line);
        }
        var dict = new Dictionary<Loc, long>();
        var mainGrid = new GridMap(newLines);
        for(int iRow = mainGrid.Rows - 1; iRow >= 0; iRow--)
        {
            for(int iCol = 0; iCol < mainGrid.Cols; iCol++)
            {
                var loc = new Loc(iRow, iCol);
                if (mainGrid.Get(loc) == '^')
                {
                    //search down
                    var val = FindVal(dict, mainGrid, loc.Move(DirEnum.W));
                    val += FindVal(dict, mainGrid, loc.Move(DirEnum.E));
                    dict.Add(loc, val);
                }
            }
        }
        var firstSplit = mainGrid.Find('S').Move(DirEnum.S);
        if (dict.ContainsKey(firstSplit))
            rv = dict[firstSplit];

        var res = new RunnerResult();
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
	}

    private static long FindVal(Dictionary<Loc, long> dict, GridMap mainGrid, Loc loc)
    {
        var locBelow = loc.Move(DirEnum.S);
        var rv = 0L;
        var found = false;
        while (mainGrid.IsValid(locBelow))
        {
            if (dict.ContainsKey(locBelow))
            {
                rv += dict[locBelow];
                found = true;
                break;
            }
            locBelow = locBelow.Move(DirEnum.S);
        }
        if (found == false)
            rv++;
        return rv;
    }
}
class GridMap7 : GridMap
{
    public Loc? ILoc;

    public GridMap7(IEnumerable<string>? lines) : base(lines)
    {
    }

    public GridMap7(GridMap other, Loc loc) : base()
    {
        ILoc = loc;
        _map = [];
        for(int iRow = 0; iRow < other.Rows; iRow++)
        {
            if (iRow <= loc.Row)
                _map.Add([]);
            else
                _map.Add(other.GetRow(iRow).ToArray());
        }
    }

    internal void Mark(Loc loc)
    {
        Set(loc, '|');
        ILoc = loc;
    }
}

