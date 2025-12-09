using AoCLibrary;
namespace Advent25;

internal class Day09 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/9
	// Input https://adventofcode.com/2025/day/9/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 0L);
		else
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        var locs = Loc.ReadAll(lines);

        res.CheckGuess(rv);
        return res;
    }
	class Corners
	{
        public Loc Point { get; set; }
        public DirEnum ClosestsCorner { get; set; }
        public double Distance { get; set; }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 0L);
		else
			res.Ccheck = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

