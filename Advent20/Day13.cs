using AoCLibrary;

namespace Advent20;

internal class Day13 : IRunner
{
	// Day https://adventofcode.com/2020/day/13
	// Input https://adventofcode.com/2020/day/13/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 295L);
		else
			res.Check = new StarCheck(key, 6568L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var leave = long.Parse(lines[0]);
		var parts = lines[1].Split(',');
		var buses = new List<long>();
		foreach (var part in parts)
			if (long.TryParse(part, out long interval))
				buses.Add(interval);

		var i = leave;
		while(true)
		{
			foreach (var bus in buses)
			{
				if (i % bus == 0)
				{
					rv = (i - leave) * bus;
					break;
				}
			}
			if (rv > 0)
				break;
			i++;
		}

        res.CheckGuess(rv);
        return res;
    }
	public RunnerResult Star2(bool isReal)
	{
		var key = new StarCheckKey(StarEnum.Star2, isReal, null);
		var res = new RunnerResult();
		if (!isReal)
			res.Check = new StarCheck(key, 1068781L);
		else
			res.Check = new StarCheck(key, 0L); // skipping

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
		// magic
		var parts = lines[1].Split(',');
		var buses = new Dictionary<int, long>();
		for(int i = 0; i < parts.Length; i++)
		{
            if (long.TryParse(parts[i], out long interval))
				buses.Add(i, interval);
        }

        var timestamp = buses.First().Value;
        var period = timestamp;

        foreach (var bus in buses)
        {
			if (bus.Value == period)
				continue;
            var i = bus.Key;
            var id = bus.Value;
			while((timestamp + i)%id != 0)
			{
				timestamp += period;
			}
			period = period * id;
		}
		rv = timestamp;

        res.CheckGuess(rv);
        return res;
	}
}

