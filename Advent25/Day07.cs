using AoCLibrary;
namespace Advent25;

internal class Day07 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2025/day/7
	// Input https://adventofcode.com/2025/day/7/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 21L);
		else
            res.Check = new StarCheck(key, 1516L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        var grid = new GridMap(lines.Where(l => !l.All(c => c == '.')));
        var locs = new List<Loc>();
        locs.Add(grid.Find('S')!);
        while(locs.Any())
        { 
            var newLocs = new List<Loc>();
            foreach (var loc in locs)
            {
                var locBelow = loc.Move(DirEnum.S);
                var cBelow = grid.Get(locBelow);
                if (cBelow == '.')
                    newLocs.Add(locBelow);
                else if (cBelow == '^')
                {
                    bool found = false;
                    if (!newLocs.Contains(loc.Move(DirEnum.SE)))
                    {
                        found = true;
                        newLocs.Add(loc.Move(DirEnum.SE));
                    }
                    if (!newLocs.Contains(loc.Move(DirEnum.SW)))
                    {
                        found = true;
                        newLocs.Add(loc.Move(DirEnum.SW));
                    }
                    if (found)
                        rv++;
                }
            }
            locs = newLocs;
        }

        res.CheckGuess(rv);
        //1484 too low
        return res;
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 40L);
		else
            res.Check = new StarCheck(key, 1393669447690L);

		var lines = Program.GetLines(key);
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

        res.CheckGuess(rv);
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

