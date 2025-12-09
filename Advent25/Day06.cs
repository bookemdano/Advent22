using AoCLibrary;
using System.IO;
namespace Advent25;

internal class Day06 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/6
	// Input https://adventofcode.com/2025/day/6/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 4277556L);
		else
			res.Check = new StarCheck(key, 6100348226985L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
        Group1[]? groups = null;
        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (groups == null)
            {
                groups = new Group1[parts.Count()];
                for (int i = 0; i < groups.Length; i++)
                    groups[i] = new Group1();
            }
            var iGroup = 0;
            foreach (var part in parts)
            {
                if (long.TryParse(part, out long val))
                    groups[iGroup].Values.Add(val);
                else
                    groups[iGroup].Op = part;
                iGroup++;
            }
        }
        foreach(var group in groups)
        {
            rv += group.Operate();
        }

        res.CheckGuess(rv);
        return res;
    }
    class Group1
    {
        public List<long> Values { get; set; } = [];
        public string Op { get; set; } = "";

        internal long Operate()
        {
            var rv = 0L;
            if (Op == "*")
            {
                rv = 1L;
                foreach (var val in Values)
                    rv *= val;
            }
            else if (Op == "+")
            {
                foreach (var val in Values)
                    rv += val;
            }
            else
                Utils.Assert(false, "Known operator " + Op);
            return rv;
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 3263827L);
		else
			res.Check = new StarCheck(key, 12377473011151L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        var lastLine = lines.Last();
        var groups = new List<Group2>();
        Group2? lastGroup = null;
        for (int i = 0; i < lastLine.Length; i++)
        {
            if (lastLine[i] != ' ')
            {
                var group = new Group2(i, lastLine[i]);
                groups.Add(group);
                lastGroup?.ColEnd = i - 1;
                lastGroup = group;
            }
        }
        lastGroup?.ColEnd = lines.Max(l => l.Length);

        for(int i = 0; i < lines.Length - 1; i++)
        {
            var line = lines[i];
            var parts = new List<string>();
            for (int iCol = 0; iCol < groups.Count(); iCol++)
            {
                groups[iCol].AddColumn(line);
            }

        }
        foreach (var group in groups)
        {
            rv += group.Operate();
        }
        res.CheckGuess(rv);
        return res;
	}
    class Group2
    {
        public override string ToString()
        {
            return $"{string.Join(',', _strings)} {_op}";
        }
        readonly List<string> _strings = [];
        readonly int _colStart;
        public int ColEnd { get; set; } = -1;
        public int Length()
        {
            return ColEnd - _colStart;
        }

        readonly char _op;
        public Group2(int colStart, char op)
        {
            _colStart = colStart;
            _op = op;
        }
        internal void AddColumn(string line)
        {
            var part = GetPart(line);
            _strings.Add(part);
        }

        public string GetPart(string line)
        {
            return line.Substring(_colStart, Length());
        }
        internal long Operate()
        {
            var rv = 0L;
            var vStrings = new string[Length()];
            var iV = 0;
            for(var iChar = _strings[0].Length - 1; iChar >= 0; iChar-- )
            {
                foreach(var str in _strings)
                {
                    if (str[iChar] != ' ')
                        vStrings[iV] += str[iChar];
                }
                iV++;
            }
            if (_op == '*')
            {
                rv = 1L;
                foreach (var val in vStrings)
                    rv *= long.Parse(val);

            }
            else if (_op == '+')
            {
                foreach (var val in vStrings)
                    rv += long.Parse(val);
            }
            else
                Utils.Assert(false, "Known operator " + _op);
            return rv;
        }

    }

}

