using AoCLibrary;
using System.IO.Pipelines;
using System.Reflection;
namespace Advent25;

internal class Day05 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/5
	// Input https://adventofcode.com/2025/day/5/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 3L);
		else
            res.Check = new StarCheck(key, 635L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
        // magic
        bool readRange = true;
        var ranges = new List<Tuple<long, long>>();
        var avails = new List<long>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            { 
                readRange = false;
                continue;
            }
            if (readRange)
            {
                var splits = line.Split('-');
                ranges.Add(new Tuple<long, long>(long.Parse(splits[0]), long.Parse(splits[1])));
            }
            else
            {
                var id = long.Parse(line);
                foreach(var range in ranges)
                {
                    if (id >= range.Item1 && id <= range.Item2)
                    {
                        rv++;
                        break;
                    }
                }
                avails.Add(id);
            }
        }

        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 14L);
		else
            res.Check = new StarCheck(key, 369761800782619L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
        // magic
        var ranges = new List<Range>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                // combine ranges
                bool changed = true;
                while (changed)
                {
                    changed = false;
                    ElfHelper.DayLog("Combine Ranges Pass, count: " + ranges.Count);
                    for(int i = 0; i < ranges.Count; i++)
                    {
                        var r1 = ranges[i];
                        for(int j = 0; j < ranges.Count; j++)
                        {
                            if (i == j)
                                continue;
                            var r2 = ranges[j];
                            var ar = r1.Add(r2);
                            if (ar != AREnum.NoChange)
                            {
                                changed = true;
                                if (ar == AREnum.RemoveCurrent)
                                {
                                    ranges.RemoveAt(i);
                                    break;
                                }
                                else if (ar == AREnum.RemoveOther)
                                {
                                    ranges.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                    }
                }
                break;
            }
            var splits = line.Split('-');
            ranges.Add(new Range(long.Parse(splits[0]), long.Parse(splits[1])));
        }
        foreach(var range in ranges)
            rv += range.End - range.Start + 1;

        // Too low 338852627934377
        //         369761800782619
        res.CheckGuess(rv);
        return res;
	}
    public enum AREnum
    {
        NoChange,
        Changed,
        RemoveOther,
        RemoveCurrent
    }
    class Range
    {
        public Range(long start, long end)
        {
            Start = start;
            End = end;
        }
        public AREnum Add(Range other)
        {
            //var rv = false;
            //var orig = new Range(Start, End);
            if (InRange(other.Start) && InRange(other.End))
            {
                // other is inside current
                return AREnum.RemoveOther;
            }
            if (Start == other.Start)
            {
                if (other.End >= End)
                    return AREnum.RemoveCurrent;
            }
            else if (InRange(other.Start))
            {
                End = other.Start - 1;
                return AREnum.Changed;
            }
            else if (End == other.End)
            {
                if (other.Start <= Start)
                    return AREnum.RemoveCurrent;
            }
            else if (InRange(other.End))
            {
                Start = other.End + 1;
                return AREnum.Changed;
            }
            Utils.Assert(Start <= End, "Valid Range " + this);
            return AREnum.NoChange;
        }
        public override string ToString()
        {
            return $"{Start}-{End}";
        }
        public bool InRange(long val)
        {
            return val >= Start && val <= End;
        }

        internal bool Same(Range other)
        {
            return (other.Start == Start && End == other.End);
        }

        public long Start;
        public long End;
    }
}

