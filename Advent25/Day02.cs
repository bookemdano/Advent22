using AoCLibrary;
using System.Collections.Generic;
using System.Globalization;
namespace Advent25;

internal class Day02 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2025/day/2
	// Input https://adventofcode.com/2025/day/2/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 1227775554L);
		else
			check = new StarCheck(key, 30608905813L);

		//var lines = Program.GetLines(check.Key);
		var text = Program.GetText(check.Key);
		var sets = text.Split(',');
		var rv = 0L;
		// magic
		foreach(var set in sets)
		{
			var parts = set.Split('-');
			var min = long.Parse(parts[0]);
			var max = long.Parse(parts[1]);
			for(var i = min; i <= max; i++)
			{
				var str = i.ToString();
				var len = str.Length;
				if (len % 2 != 0)
					continue;
                var first = str.Substring(0, len / 2);
                var second = str.Substring(len / 2, len / 2);
				if (first == second)
					rv += i;
            }
        }
		check.Compare(rv);
		return rv;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 4174379265L);
		else
			check = new StarCheck(key, 0L);

        var text = Program.GetText(check.Key);
        var sets = text.Split(',');
        var rv = 0L;
        // magic
        foreach (var set in sets)
        {
            var splits = set.Split('-');
            var min = long.Parse(splits[0]);
            var max = long.Parse(splits[1]);
            for (var i = min; i <= max; i++)
            {
                var str = i.ToString();
                var len = str.Length;
				for (var sections = 2; sections <= len; sections++)
				{
                    if (len % sections != 0)
                        continue;
					var first = str.Substring(0, len / sections);
					bool found = true;
					for(var k = 1; k < sections; k++)
					{
                        var t = str.Substring(k* (len / sections), len / sections);
						if (t != first)
						{
							found = false;
                            break;
						}
                    }
                    if (found)
                    {
                        rv += i;
                        break;
                    }

                }
            }
        }

        check.Compare(rv);
		return rv;
	}
}

