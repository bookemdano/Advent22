using AoCLibrary;
namespace Advent24;

internal class Day02 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/2
	// Input https://adventofcode.com/2024/day/2/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 2L);
		else
			check = new StarCheck(key, 490L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		foreach(var line in lines)
		{
			var parts = line.Split(' ');
			var ints = parts.Select(x => int.Parse(x)).ToArray();
			if (CheckInts(ints))
				rv++;
		}

		check.Compare(rv);
		return rv;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 4L);
		else
			check = new StarCheck(key, 536L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic
		foreach (var line in lines)
		{
			var parts = line.Split(' ');
			var ints = parts.Select(x => int.Parse(x)).ToArray();
			if (CheckInts(ints))
				rv++;
			else
			{
				for (var i = 0; i < ints.Length; i++)
				{
					var i2 = ints.ToList();
					i2.RemoveAt(i);
					if (CheckInts(i2))
					{
						rv++;
						break;
					}
				}

			}
		}

		check.Compare(rv);
		return rv;
	}
	bool CheckInts(IEnumerable<int> ints)
	{
		bool? inc = null;
		var last = 0;
		foreach (var i in ints)
		{
			var diff = i - last;
			if (last == 0)
			{
				last = i;
			}
			else if (diff == 0 || Math.Abs(diff) > 3)
			{
				return false;
			}
			else if (inc == null)
			{
				if (diff > 0)
					inc = true;
				else
					inc = false;
			}
			else if ((diff > 0 && inc == false) || (diff < 0 && inc == true))
			{
				return false;
			}

			last = i;
		}
		return true;
	}
}
