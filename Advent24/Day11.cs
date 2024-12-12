using AoCLibrary;
namespace Advent24;

internal class Day11 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2024/day/11
	// Input https://adventofcode.com/2024/day/11/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 55312L);
		else
			check = new StarCheck(key, 217443L);

		var text = Program.GetText(check.Key);
        var rv = 0L;
		// magic
		var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var stones = parts.Select(p => long.Parse(p)).ToList();
		for(int i = 0; i < 25; i++)
		{
			var newStones = new List<long>();
			foreach(var stone in stones)
			{
				var n = stone.ToString().Length;
				if (stone == 0)
					newStones.Add(1);
				else if (IsEven(n))
				{
					newStones.Add(long.Parse(stone.ToString()[0..(n / 2)]));
					newStones.Add(long.Parse(stone.ToString()[(n / 2)..]));
				}
				else
					newStones.Add(stone * 2024);
			}
            stones = newStones;
		}
		rv = stones.Count();

		check.Compare(rv);
		return rv;
	}
	public bool IsEven(long l)
	{
		return (l / 2.0 == l / 2);
    }
	public void AddOrUpdate(Dictionary<long, long> dict, long key, long value)
	{
        if (!dict.ContainsKey(key))
            dict.Add(key, 0);
        dict[key] += value;
    }
    public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 65601038650482L);
		else
			check = new StarCheck(key, 257246536026785L);

        var text = Program.GetText(check.Key);
        var rv = 0L;
        // magic
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		var dict = new Dictionary<long, long>();
		foreach (var part in parts)
			dict.Add(long.Parse(part), 1);
        for (int i = 0; i < 75; i++)
        {
            var newDict = new Dictionary<long, long>();
            foreach (var kvp in dict)
            {
				var stone = kvp.Key;
                var n = stone.ToString().Length;
				if (stone == 0)
					AddOrUpdate(newDict, 1, kvp.Value);
				else if (IsEven(n))
				{
					var lh = long.Parse(stone.ToString()[0..(n / 2)]);
                    AddOrUpdate(newDict, lh, kvp.Value);
					var rh = long.Parse(stone.ToString()[(n / 2)..]);
                    AddOrUpdate(newDict, rh, kvp.Value);
				}
				else
				{
                    AddOrUpdate(newDict, stone * 2024, kvp.Value);
                }

            }
			dict = newDict;
			ElfHelper.DayLog("Stone " + i);
        }
        rv = dict.Sum(kvp => kvp.Value);

        check.Compare(rv);
		return rv;
	}
}

