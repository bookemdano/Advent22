using AoCLibrary;
namespace Advent21;
// #working
internal class Day02 : IRunner
{
	// Day https://adventofcode.com/2021/day/2
	// Input https://adventofcode.com/2021/day/2/input
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
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

