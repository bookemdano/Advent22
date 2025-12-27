using AoCLibrary;

namespace Advent20;

internal class Day16 : IRunner
{
	// Day https://adventofcode.com/2020/day/16
	// Input https://adventofcode.com/2020/day/16/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 71L);
		else
			res.Check = new StarCheck(key, 20231L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
        // magic
        var ranges = new List<Range16>();
        var tickets = new List<Ticket16>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Contains("ticket")) 
                continue;

            if (line.Contains(':'))
                ranges.Add(new Range16(line));
            else
                tickets.Add(new Ticket16(line));
        }
        foreach(var ticket in tickets)
            rv += ticket.IsValid(ranges);

        res.CheckGuess(rv);
        return res;
    }
    class Range16
    {
        string _name;
        int _min1;
        int _max1;
        int _min2;
        int _max2;

        public Range16(string line)
        {
            var parts = line.Split(':');
            _name = parts[0];
            parts = parts[1].Split(" -".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            _min1 = int.Parse(parts[0]);
            _max1 = int.Parse(parts[1]);
            _min2 = int.Parse(parts[3]);
            _max2 = int.Parse(parts[4]);

        }
        public override string ToString()
        {
            return $"{_name}: {_min1}-{_max1} or {_min2}-{_max2}";
        }

        internal bool Contains(int val)
        {
            if (val >= _min1 && val <= _max1)
                return true;
            if (val >= _min2 && val <= _max2)
                return true;
            return false;
        }

        internal bool IsDeparture()
        {
            return _name.StartsWith("departure");
        }
    }

    class Ticket16
	{
		int[] _parts = [];
        public Ticket16(string line)
        {
            _parts = line.Split(',').Select(line => int.Parse(line)).ToArray();
        }
        public override string ToString()
        {
            return string.Join(',', _parts);
        }

        // returns invalid part
        internal int IsValid(List<Range16> ranges)
        {
            foreach(var part in _parts)
            {
                bool found = false;
                foreach (var range in ranges)
                {
                    if (range.Contains(part))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return part;
            }
            return 0;
        }

        internal void EliminateImpossibles(Dictionary<Range16, List<int>> possibles)
        {
            int iPart = 0;
            foreach(var part in _parts)
            {
                foreach (var possible in possibles)
                {
                    if (!possible.Value.Contains(iPart))
                        continue;
                    var range = possible.Key;
                    if (!range.Contains(part))
                        possible.Value.Remove(iPart);
                }
                iPart++;
            }
        }

        internal int GetField(int index)
        {
            return _parts[index];
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 13L);
		else
			res.Check = new StarCheck(key, 1940065747861L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var ranges = new List<Range16>();
        var tickets = new List<Ticket16>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.Contains("ticket"))
                continue;

            if (line.Contains(':'))
                ranges.Add(new Range16(line));
            else
            {
                var ticket = new Ticket16(line);
                if (ticket.IsValid(ranges) == 0)
                    tickets.Add(ticket);
            }
        }

        var possibles = new Dictionary<Range16, List<int>>(); // index, possible locations
        foreach(var range in ranges)
        {
            possibles.Add(range, new List<int>());
            for (int i = 0; i < ranges.Count; i++)
            {
                possibles[range].Add(i);
            }
        }
        foreach (var ticket in tickets)
            ticket.EliminateImpossibles(possibles);


        var keepGoing = true;
        while (keepGoing)
        {
            keepGoing = false;
            foreach(var possible in possibles)
            { 
                if (possible.Value.Count() > 1)
                    keepGoing = true;
                if (possible.Value.Count() == 1)
                {
                    var foundIndex = possible.Value.First();
                    ElfHelper.DayLogPlus($"Found{possible.Key} goes in {foundIndex}");
                    // get rid of possibles from other indexes
                    foreach (var p2 in possibles)
                    {
                        if (p2.Key != possible.Key && p2.Value.Contains(foundIndex))
                            p2.Value.Remove(foundIndex);
                    }
                }
            }
        }
        var yourTicket = tickets[0];
        var deps = possibles.Where(k => k.Key.IsDeparture());
        rv = 1;
        foreach(var dep in deps)
            rv *= yourTicket.GetField(dep.Value.Single());
        res.CheckGuess(rv);
        return res;
	}
}

