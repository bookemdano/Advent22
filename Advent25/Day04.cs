using AoCLibrary;
namespace Advent25;

internal class Day04 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2025/day/4
	// Input https://adventofcode.com/2025/day/4/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        StarCheck check;
        if (!isReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic

        var res = new RunnerResult();
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        StarCheck check;
        if (!isReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic

        var res = new RunnerResult();
        res.StarValue = rv;
        res.StarSuccess = check.Compare(rv);
        return res;
	}
}

