using AoCLibrary;
namespace Advent20;
// #working
internal class Day14 : IRunner
{
	// Day https://adventofcode.com/2020/day/14
	// Input https://adventofcode.com/2020/day/14/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic

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

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

