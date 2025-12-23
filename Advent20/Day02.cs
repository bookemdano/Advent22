using AoCLibrary;
namespace Advent20;

internal class Day02 : IRunner
{
	// Day https://adventofcode.com/2020/day/2
	// Input https://adventofcode.com/2020/day/2/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 2L);
		else
			res.Check = new StarCheck(key, 439L);

		var lines = RunHelper.GetLines(key);
        //var text = RunHelper.GetText(key);
        var rv = 0L;
        // magic
        // 1-3 b: cdefg
        foreach(var line in lines)
        {
            var parts = line.Split(':');
            var pwd = parts[1].Trim();
            var rule = new Rule01(parts[0]);
            if (rule.IsValid1(pwd))
                rv++;
        }

        res.CheckGuess(rv);
        return res;
    }
    class Rule01
    {
        char _c;
        int _min;
        int _max;
        public Rule01(string str)
        {
            //1-3 b
            var parts = str.Split(' ');
            var range = Utils.SplitInts('-', parts[0]);
            _min = range[0];
            _max = range[1];
            _c = str.Last();
        }
        public bool IsValid1(string password)
        {
            var count = password.Count(c => c == _c);
            return (count >= _min && count <= _max);
        }
        public bool IsValid2(string password)
        {
            var foundOne = (password.Length > _min - 1 && password[_min - 1] == _c);
            var foundTwo = (password.Length > _max - 1 && password[_max - 1] == _c);
            if (foundOne && foundTwo)
                return false;
            return (foundOne || foundTwo);
        }
        public override string ToString()
        {
            return $"{_min}-{_max} {_c}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 1L);
		else
			res.Check = new StarCheck(key, 584L);

		var lines = RunHelper.GetLines(key);
        //var text = RunHelper.GetText(key);

        var rv = 0L;
        // magic
        foreach (var line in lines)
        {
            var parts = line.Split(':');
            var pwd = parts[1].Trim();
            var rule = new Rule01(parts[0]);
            if (rule.IsValid2(pwd))
                rv++;
        }

        // too high 684
        res.CheckGuess(rv);
        return res;
	}
}

