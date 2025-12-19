using AoCLibrary;

namespace Advent21;

// #working
internal class Day18 : IRunner
{
	// Day https://adventofcode.com/2021/day/18
	// Input https://adventofcode.com/2021/day/18/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
            var subLine = line.Substring(1, line.Length - 2);
            var bin = new Bin18(line, 0);
            bin.Explode();
		}

        res.CheckGuess(rv);
        return res;
    }
    class Set18
    {
        public Set18(string str, int from, int to)
        {
            Str = str;
            To = to;
            From = from;
        }

        public Set18(int from)
        {
            From = from;
        }

        public override string ToString()
        {
            return $"{From}-{To}";
        }
        public int From { get; }
        public int To { get; }
        public string Str { get; }
        public int Comma { get; set; }
        public Half18 LeftHand { get; internal set; }
        public Half18 RightHand { get; internal set; }
    }
    class Half18
    {
        private int _digit;
        List<Half18> _halves = [];
        private Set18 set18;

        public Half18(int digit)
        {
            _digit = digit;
        }

        public Half18(Set18 set18)
        {
            this.set18 = set18;
        }
    }
    class Bin18
    {
        public void Add(int v)
        {
            if (!LeftDone)
                _lh = v;
            else
                _rh = v;
        }
        public void Add(Bin18 bin)
        {
            if (!LeftDone)
                _lhBin = bin;
            else
                _rhBin = bin;
        }
        bool LeftDone => (_lhBin != null || _lh >= 0);
        bool RightDone => (_rhBin != null || _rh >= 0);
        public override string ToString()
        {
            var str = string.Empty;
            if (_lhBin!= null)
               str += _lhBin;
            else
                str += _lh;
            str += ",";
            if (_rhBin != null)
                str += _rhBin;
            else
                str += _rh;
            return $"[{str}]({_level})";
        }
        int _lh = -1;
        int _rh = -1;
        Bin18? _lhBin;
        Bin18? _rhBin;
        bool IsDone()
        {
            return (LeftDone && RightDone);

        }

        internal void Explode()
        {
            if (_level >= 4)
            {

            }    
            if (_lhBin != null)
                _lhBin.Explode();
            if (_rhBin != null)
                _rhBin.Explode();
        }

        int _used;
        private int _level;

        public Bin18(string line, int level)
        {
            _level = level;
            Utils.Assert(line[0] == '[', "Opens right " + this);
            for (_used = 1; _used < line.Length; _used++)
            {
                var c = line[_used];
                if (int.TryParse(c.ToString(), out int digit))
                {
                    Utils.Assert(!IsDone(), "Not Done " + this);
                    Add(digit);
                }
                else if (c == '[')
                {
                    var bin = new Bin18(line.Substring(_used), level + 1);
                    Add(bin);
                    _used += bin._used;
                }
                else if (c == ']')
                {
                    Utils.Assert(IsDone(), "IsDone " + this);
                    return;
                }
                else
                {
                    Utils.Assert(c == ',', "Only other is comma " + this);
                }
            }

        }

    }
    
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

