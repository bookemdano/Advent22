using AoCLibrary;
namespace Advent20;

internal class Day05 : IRunner
{
	// Day https://adventofcode.com/2020/day/5
	// Input https://adventofcode.com/2020/day/5/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 820L);
		else
			res.Check = new StarCheck(key, 933L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
            var row = new Range5(line.Take(7));
            var col = new Range5(line.Skip(7).Take(3));
            var seatId = row.Val * 8 + col.Val;
            if (seatId > rv)
                rv = seatId;
        }
        res.CheckGuess(rv);
        return res;
    }
	class Range5
	{
		int _min = 0;

		int _max = 127;
        public int Val => _max;
        public Range5(IEnumerable<char> str)
        {
            _max = (int) Math.Pow(2, str.Count()) - 1;
            foreach (var c in str)
                Act(c);

        }
        public void Act(char c)
        {
            if (c == 'F' || c == 'L')
                LowerHalf();
            else if (c == 'B' || c == 'R')
                UpperHalf();

        }
        public void LowerHalf()
		{
            var range = _max - _min + 1;
            _max -= range / 2;
		}
        public void UpperHalf()
        {
			var range = _max - _min + 1;
			_min += range / 2;
        }
        public override string ToString()
		{
			return _min + ".." + _max;
		}
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 0L); // no fake answer
		else
			res.Check = new StarCheck(key, 711L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var seatIds = new List<int>();
        foreach (var line in lines)
        {
            var row = new Range5(line.Take(7));
            var col = new Range5(line.Skip(7).Take(3));
            var seatId = row.Val * 8 + col.Val;
            seatIds.Add(seatId);
        }
        seatIds = seatIds.OrderBy(s => s).ToList();
        for(var i = 0; i < seatIds.Count - 1; i++)
        {
            if (seatIds[i + 1] - seatIds[i] == 2)
                rv = seatIds[i + 1] - 1;
        }

        res.CheckGuess(rv);
        return res;
	}
}

