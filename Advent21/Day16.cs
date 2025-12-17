using AoCLibrary;
using System.Text;
namespace Advent21;
// #working
internal class Day16 : IRunner
{
	// Day https://adventofcode.com/2021/day/16
	// Input https://adventofcode.com/2021/day/16/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 31L);
		else
			res.Check = new StarCheck(key, 927L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        foreach (var line in lines)
        {

            var bins = GetBinary(line);
            var bin16 = new Bin16(bins);
            rv = bin16.TotalVersion;
        }

        res.CheckGuess(rv);
        return res;
    }
	class Bin16
	{
        public int Version { get; }
        public int TotalVersion { get; }
        private long _type;

        public int BitsUsed { get; private set; }

        public long Val { get; private set; }

        public Bin16(IEnumerable<bool> bins)
        {
            Version = (int) GetNumber(bins.Take(3));
            TotalVersion += Version;
            _type = GetNumber(bins.Skip(3).Take(3));
            if (_type == 4)
                SetId4s(bins);
            else
            {
                var subBins = new List<Bin16>();
                var lengthType = bins.Skip(6).First();
                if (lengthType == false)    // length type
                {
                    var length = GetNumber(bins.Skip(7).Take(15));
                    var start = 7 + 15;
                    while(length > 0)
                    {
                        var subBin = new Bin16(bins.Skip(start));
                        TotalVersion += subBin.TotalVersion;
                        start += subBin.BitsUsed;
                        length -= subBin.BitsUsed;
                        subBins.Add(subBin);
                    }
                    BitsUsed += start;
                }
                else if (lengthType == true)
                {
                    var subpackets = GetNumber(bins.Skip(7).Take(11));
                    BitsUsed = 7 + 11;
                    var packets = new List<long>();
                    for(int i = 0; i < subpackets; i++)
                    {
                        var subBin = new Bin16(bins.Skip(BitsUsed));
                        BitsUsed += subBin.BitsUsed;
                        TotalVersion += subBin.TotalVersion;
                        subBins.Add(subBin);
                        //packets.Add(GetNumber(bins.Skip(start).Take(11)));
                    }
                }
                if (_type == 0)
                    Val = subBins.Sum(b => b.Val);
                else if (_type == 1)
                    Val = Product(subBins);
                else if (_type == 2)
                    Val = subBins.Min(b => b.Val);
                else if (_type == 3)
                    Val = subBins.Max(b => b.Val);
                else if (_type == 5)
                    Val = (subBins[0].Val > subBins[1].Val) ? 1L : 0L;
                else if (_type == 6)
                    Val = (subBins[0].Val < subBins[1].Val) ? 1L : 0L;
                else if (_type == 7)
                    Val = (subBins[0].Val == subBins[1].Val) ? 1L : 0L;
                else
                    Val = (subBins[0].Val == subBins[1].Val) ? 1L : 0L;

            }
        }

        static long Product(IEnumerable<Bin16> ls)
        {
            var rv = 1L;
            foreach (var l in ls)
                rv *= l.Val;
            return rv;
        }
        private void SetId4s(IEnumerable<bool> bins)
        {
            int i = 6;
            var ns = new List<long>();
            while (i < bins.Count())
            {
                var b = bins.Skip(i++).First();
                ns.Add(GetNumber(bins.Skip(i).Take(4)));
                i += 4;
                if (b == false)
                    break;
            }
            BitsUsed = i;
            Val = 0L;
            for (var iN = 0; iN < ns.Count(); iN++)
                Val += ns[iN] << ((ns.Count() - iN - 1) * 4);
        }

        public override string ToString()
        {
            return $"{_type} {Version} {Val}";
        }

        static long GetNumber(IEnumerable<bool> bins)
        {
            var rv = 0L;
            var i = 0;
            foreach (var b in bins)
            {
                if (b)
                    rv += (long)Math.Pow(2, bins.Count() - i - 1);
                i++;
            }
            return rv;
        }
    }
    bool[] GetBinary(string str)
    {
        var rv = new List<bool>();
        foreach (var c in str)
        {
            int n = c - '0';
            if (n > 9)
                n = c - 'A' + 10;
            for(int i = 3; i >= 0; i--)
            {
                var pow = (int)Math.Pow(2, i);
                rv.Add(((n & pow) == pow));
            }
        }
        return rv.ToArray();
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 1L);
		else
			res.Check = new StarCheck(key, 1725277876501L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        foreach (var line in lines)
        {

            var bins = GetBinary(line);
            var bin16 = new Bin16(bins);
            rv = bin16.Val;
        }
        // 747741473835 too low
        //1725277876501
        //13359975061613 too high
        res.CheckGuess(rv); 

        return res;
	}
}

