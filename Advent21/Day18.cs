using AoCLibrary;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

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
			res.Check = new StarCheck(key, 129L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;

        var binX = new Bin18("[[[[[9,8],1],2],3],4]", 0, 0);
        ExplodeAll(binX);
        Utils.Assert(binX.ToShortString(), "[[[[0,9],2],3],4]");
        binX = new Bin18("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", 0, 0);
        ExplodeAll(binX);
        Utils.Assert(binX.ToShortString(), "[[3,[2,[8,0]]],[9,[5,[7,0]]]]");

        {
            var lineA = "[[[[4,3],4],4],[7,[[8,4],9]]]";
            var lineB = "[1,1]";
            var newLine = $"[{lineA},{lineB}]";
            var bin1 = new Bin18(newLine, 0, 0);
            Utils.Assert(bin1.ToShortString(), "[[[[[4,3],4],4],[7,[[8,4],9]]],[1,1]]");
            var changes = ExplodeAll(bin1);
            Utils.Assert(bin1.ToShortString(), "[[[[0,7],4],[15,[0,13]]],[1,1]]");
            bin1.Split();
            Utils.Assert(bin1.ToShortString(), "[[[[0,7],4],[[7,8],[0,[6,7]]]],[1,1]]");
            changes = ExplodeAll(bin1);
            Utils.Assert(bin1.ToShortString(), "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]");

        }

        var last = lines[0];
        Bin18? bin = null;
        // magic
        for (var iLine = 1; iLine < lines.Count(); iLine++)
        {
            if (string.IsNullOrWhiteSpace(lines[iLine]))
            {
                last = lines[++iLine];
                continue;
            }
            var newLine = $"[{last},{lines[iLine]}]";
            bin = new Bin18(newLine, 0, 0);
            int changes = 1;
            while (ExplodeAll(bin) > 0) ;
            while (bin.Split() > 0) ;
            last = bin.ToShortString();
        }
        if (bin != null)
            rv = bin.Magnitude();

        res.CheckGuess(rv);
        return res;
    }

    private static int ExplodeAll(Bin18 bin)
    {
        bin.SetLimits(0); // resets levels
        var exs = bin.Find4s();

        foreach (var ex in exs)
        {
            var leftBin = bin.LeftOf(ex.LeftBin);
            if (leftBin != null)
                leftBin.Increment(ex.LeftBin.Val);
            var rightBin = bin.RightOf(ex.RightBin);
            if (rightBin != null)
                rightBin.Increment(ex.RightBin.Val);
            ex.Reset();
        }
        return exs.Count();
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
        public long Magnitude()
        {
            if (HasVal)
                return Val;
            else if (!HasVal)
                return (LeftBin.Magnitude()) * 3 + (2*RightBin.Magnitude());
            return -1;
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
            return ToShortString();
            var str = string.Empty;

            if (Val >= 0)
            {
                //return $"{Val}";
                return $"{Val}({_level})<{_index}>";
            }
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
            if (_level == 4)
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


        internal Bin18? LeftOf(Bin18? other)
        {
            var allBins = AllBins();
            var i = allBins.IndexOf(other);
            if (i == 0)
                return null;
            return allBins[i - 1];
        }
        internal Bin18? RightOf(Bin18? other)
        {
            var allBins = AllBins();
            var i = allBins.IndexOf(other);
            if (i == allBins.Count() - 1)
                return null;
            return allBins[i + 1];
        }
        internal void SetLimits(int level)
        {
            if (_level != level)
            {   if (HasVal)
                    _level = level - 1;
                else
                    _level = level;
            }
            
            if (LeftBin != null)
                LeftBin.SetLimits(level + 1);
            if (RightBin != null)
                RightBin.SetLimits(level + 1);
        }

        internal List<Bin18> AllBins()
        {
            var rv = new List<Bin18>();

            if (HasVal)
            {
                rv.Add(this);
            }
            else
            {
                if (LeftBin != null)
                    rv.AddRange(LeftBin.AllBins());
                if (RightBin != null)
                    rv.AddRange(RightBin.AllBins());
            }
            return rv;
        }

        internal void Increment(int val)
        {
            Utils.Assert(HasVal, "HasVal");
            Val += val;
        }

        internal void Reset()
        {
            LeftBin = null;
            RightBin = null;
            Val = 0;
            _level--;
        }

        internal int Split()
        {
            if (Val > 9)
            {
                var lh = (int) (Val / 2);
                _level++;
                LeftBin = new Bin18(lh, 0, _level);
                RightBin = new Bin18(Val - lh, 0, _level);
                Val = -1;
                return 1;
            }
            else
            {
                int rv = 0;
                if (LeftBin != null)
                    rv += LeftBin.Split();
                if (RightBin != null)
                    rv += RightBin.Split();
                return rv;
            }
        }

        internal string ToShortString()
        {
            var str = string.Empty;

            if (Val >= 0)
                return $"{Val}";

            return $"[{LeftBin?.ToShortString()},{RightBin?.ToShortString()}]";
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

