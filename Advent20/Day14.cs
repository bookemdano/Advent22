using AoCLibrary;
namespace Advent20;

internal class Day14 : IRunner
{
	// Day https://adventofcode.com/2020/day/14
	// Input https://adventofcode.com/2020/day/14/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 165L);
		else
			res.Check = new StarCheck(key, 9615006043476L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
        MaskSet14? mask = new MaskSet14(lines[0]);
		
		var masks = new List<MaskSet14>();
		// magic
		foreach (var line in lines.Skip(1))
		{
			if (line.StartsWith("mask"))
			{
				masks.Add(mask);
				mask = new MaskSet14(line);
			}
			else
				mask.AddWrite(line);
		}
        masks.Add(mask);

        Dictionary<long, long> mem = [];
		int i = 0;
		foreach (var imask in masks)
		{
			imask.Act1(mem);
            //ElfHelper.DayLogPlus($"{++i} {mem.Values.Sum()}");
        }

        rv = mem.Values.Sum();

        // too high 9223372036854775807
        // 9615006043476
        // too low  9610066024360
        res.CheckGuess(rv);
        return res;
    }
	class Write14
	{
		public int Index { get; }
        public long Val { get; }
        public Write14(int loc, long val)
        {
            Utils.Assert(loc > 0, "loc > 0");
            Utils.Assert(val >= 0, "val > 0");
            Index = loc;
			Val = val;
        }
        public override string ToString()
        {
            return $"[{Index}] {Val}";
        }
        public string PaddedVal()
        {
            return PaddedLong(Val);
        }
        public string PaddedKey()
        {
            return PaddedLong(Index);
        }
        static string PaddedLong(long val)
		{
            var str = Convert.ToString(val, 2);
            var pad = new string('0', 36 - str.Length);
            var rv = pad + str;
            Utils.Assert(rv.Length == 36, "rv padded enough");
            return rv;
        }
    }
	class MaskSet14
	{
		Dictionary<int, char> _mask = [];
		List<Write14> _writes = [];
        
        public MaskSet14(string line)
        {
			var mask = line.Substring(7);
			Utils.Assert(mask.Length == 36, "mask ok");
			for (int i = 0; i < mask.Length; i++)
			{
				if (mask[i] != 'X')
				{
					Utils.Assert(mask[i] == '0' || mask[i] == '1', "mask expected");
                    _mask.Add(i, mask[i]);

                }
            }
        }
		public void Act1(Dictionary<long, long> mem)
		{
			foreach (var write in _writes)
			{
				var bin = write.PaddedVal().ToCharArray();
//                ElfHelper.DayLogPlus("acting");
                //ElfHelper.DayLogPlus(_mask);
                //ElfHelper.DayLogPlus(new string(bin));
				foreach(var mask in _mask)
					bin[mask.Key] = mask.Value;
				var val = Convert.ToInt64(new string(bin), 2);
                //ElfHelper.DayLogPlus($"{write.Index} : {val}");
                mem[write.Index] = val;
			}
		}
        List<string> AddAll(List<string> addresses, char c)
        {
            var rv = new List<string>();
            foreach (var address in addresses)
            {
                rv.Add(address + c);
            }
            return rv;
        }
        public void Act2(Dictionary<long, long> mem)
        {
            foreach (var write in _writes)
            {
                var bin = write.PaddedKey().ToCharArray();
                var addresses = new List<string>();
                addresses.Add("");
                for(int i = 0; i < bin.Length; i++)
                {
                    var newAddresses = new List<string>();
                    if (!_mask.ContainsKey(i))
                    {
                        newAddresses.AddRange(AddAll(addresses, '0'));
                        newAddresses.AddRange(AddAll(addresses, '1'));
                    }
                    else if (_mask[i] == '0')
                        newAddresses.AddRange(AddAll(addresses, bin[i]));
                    else if (_mask[i] == '1')
                        newAddresses.AddRange(AddAll(addresses, '1'));  
                    addresses = newAddresses;
                }
                foreach (var address in addresses)
                {
                    var iAddress = Convert.ToInt64(address, 2);
                    mem[iAddress] = write.Val;
                }
            }
        }

        public void AddWrite(string line)
		{
			Utils.Assert(line.StartsWith("mem["), "Not mem");
			var parts = line.Split("mem[] =".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			_writes.Add(new Write14(int.Parse(parts[0]), long.Parse(parts[1])));
		}
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 208L);
		else
			res.Check = new StarCheck(key, 4275496544925L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        MaskSet14? mask = new MaskSet14(lines[0]);

        var masks = new List<MaskSet14>();
        // magic
        foreach (var line in lines.Skip(1))
        {
            if (line.StartsWith("mask"))
            {
                masks.Add(mask);
                mask = new MaskSet14(line);
            }
            else
                mask.AddWrite(line);
        }
        masks.Add(mask);

        Dictionary<long, long> mem = [];
        int i = 0;
        foreach (var imask in masks)
        {
            imask.Act2(mem);
            //ElfHelper.DayLogPlus($"{++i} {mem.Values.Sum()}");
        }

        rv = mem.Values.Sum();

        res.CheckGuess(rv);
        return res;
	}
}

