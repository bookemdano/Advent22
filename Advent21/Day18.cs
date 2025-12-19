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
            var bin = new Bin18(line, 0, 0);
            var exs = bin.Find4s();
            
            foreach(var ex in exs)
            {
                bin.Left(ex.Left)
            }
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
        public void Add(int val, int i)
        {
            if (!LeftDone)
                LeftBin = new Bin18(val, i, _level);
            else
                RightBin = new Bin18(val, i, _level);
        }
        public Bin18(int value, int i, int level)
        {
            Val = value;
            _level = level;
            _index = i;
        }
        public void Add(Bin18 bin)
        {
            if (!LeftDone)
                LeftBin = bin;
            else
                RightBin = bin;
        }
        bool LeftDone => (LeftBin != null);
        bool RightDone => (RightBin != null);
        public override string ToString()
        {
            var str = string.Empty;
            if (Val >= 0)
                return $"{Val}({_level})<{_index}>";
            return $"[{LeftBin},{RightBin}]";
        }
        public int Val { get; set; } = -1;
        public Bin18? LeftBin { get; set; }
        public Bin18? RightBin { get; set; }
        bool IsDone()
        {
            return (LeftDone && RightDone);

        }
        bool HasVal => Val >= 0;
        public List<Bin18> Find4s()
        {

            var rv = new List<Bin18>();
            if (_level >= 4)
            {
                rv.Add(this);
            }
            else if (!HasVal)
            {
                rv.AddRange(LeftBin.Find4s());
                rv.AddRange(RightBin.Find4s());
            }
            return rv;
        }

        internal void Explode()
        {

            if (_level >= 4)
            {

            }    
            if (LeftBin != null)
                LeftBin.Explode();
            if (RightBin != null)
                RightBin.Explode();
        }

        int _ichar;
        private int _level;
        private int _index;

        public Bin18(string line, int start, int level)
        {
            _level = level;
            Utils.Assert(line[start] == '[', "Opens right " + this);
            for (_ichar = start + 1; _ichar < line.Length; _ichar++)
            {
                var c = line[_ichar];
                if (int.TryParse(c.ToString(), out int digit))
                {
                    Utils.Assert(!IsDone(), "Not Done " + this);
                    Add(digit, _ichar);
                }
                else if (c == '[')
                {
                    var bin = new Bin18(line, _ichar, level + 1);
                    Add(bin);
                    _ichar = bin._ichar;
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

