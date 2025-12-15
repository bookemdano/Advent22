using AoCLibrary;
namespace Advent21;

internal class Day11 : IRunner
{
	// Day https://adventofcode.com/2021/day/11
	// Input https://adventofcode.com/2021/day/11/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 1656L);
		else
			res.Check = new StarCheck(key, 1640L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        var map = new GridMap(lines);
        bool log = false;
        for (int i = 0; i < 100; i++)
        {
            if (log)
                ElfHelper.DayLogPlus("Iter " + i + "\n" + map);

            IncMap(map);
            if (log)
                ElfHelper.DayLogPlus("After Inc \n" + map);
            var blinks = new List<Loc>();
            var colons = map.FindAll(':');
            var flashers = new Stack<Loc>(colons);

            while (flashers.Any())
            {
                var flasher = flashers.Pop();
                map.SetInt(flasher, 0);
                if (blinks.Contains(flasher))
                    continue;
                
                blinks.Add(flasher);
                var moves = flasher.All8Moves();
                foreach(var move in moves)
                {
                    if (blinks.Contains(move))
                        continue;
                    if (!map.IsValid(move))
                        continue;
                    var val = map.GetInt(move);
                    if (val <= 9)
                        map.IncInt(move);
                    if (val == 9 )
                        flashers.Push(move);
                }
                if (log)
                    ElfHelper.DayLogPlus($"Current {flasher} Flashers Left {flashers.Count()}\n" + map);
            }
            rv += blinks.Count();
        }
        res.CheckGuess(rv);
        return res;
    }
    void IncMap(GridMap map)
    {
        for (int iRow = 0; iRow < map.Rows; iRow++)
        {
            for (int iCol = 0; iCol < map.Cols; iCol++)
            {
                var loc = new Loc(iRow, iCol);
                map.IncInt(loc);
            }
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 195L);
		else
			res.Check = new StarCheck(key, 312L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var map = new GridMap(lines);
        bool log = false; 
        for (int i = 0; i < int.MaxValue; i++)
        {
            if (log)
                ElfHelper.DayLogPlus("Iter " + i + "\n" + map);

            IncMap(map);
            if (log)
                ElfHelper.DayLogPlus("After Inc \n" + map);
            var blinks = new List<Loc>();
            var colons = map.FindAll(':');
            var flashers = new Stack<Loc>(colons);

            while (flashers.Any())
            {
                var flasher = flashers.Pop();
                map.SetInt(flasher, 0);
                if (blinks.Contains(flasher))
                    continue;

                blinks.Add(flasher);
                var moves = flasher.All8Moves();
                foreach (var move in moves)
                {
                    if (blinks.Contains(move))
                        continue;
                    if (!map.IsValid(move))
                        continue;
                    var val = map.GetInt(move);
                    if (val <= 9)
                        map.IncInt(move);
                    if (val == 9)
                        flashers.Push(move);
                }
                if (log)
                    ElfHelper.DayLogPlus($"Current {flasher} Flashers Left {flashers.Count()}\n" + map );
            }
            if (blinks.Count() == map.Rows * map.Cols)
            {
                rv = i + 1;
                break;
            }
        }

        res.CheckGuess(rv);
        return res;
	}
}

