using AoCLibrary;
using System.Data;
using System.Text.RegularExpressions;
namespace Advent24;

internal class Day03 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/3
	// Input https://adventofcode.com/2024/day/3/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 161L);
		else
			check = new StarCheck(key, 174336360L);

		var line = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var regex = new Regex(@"mul\(\d+,\d+\)");
		var matches = regex.Matches(line);
		foreach(var match in matches)
		{
			var parts = match.ToString().Split("(,)".ToCharArray());
			if (parts.Length == 4)
			{
				rv += int.Parse(parts[1]) * int.Parse(parts[2]);
			}
		}
		/*
		var parts = line.Split("mul(");
		foreach (var part in parts)
		{
			int commas = 0;
			bool paren = false;
			var ns = new List<int>();
			for(int i = 0; i < part.Length; i++)
			{
				var c = part[i];
				if (c == ',')
				{
					commas++;
				}
				else if (c == ')')
				{
					paren = true;
					break;
				}
				else if (char.IsNumber(c))
				{
					var str = c.ToString();
					if (i < part.Length - 1)
						for (int j = i + 1; j < part.Length; j++)
						{
							if (char.IsNumber(part[j]))
							{
								str += part[j];
								i++;
							}
							else
								break;
						}
					ns.Add(int.Parse(str));
				}
				else
					break;
			}
			if (ns.Count == 2 && commas == 1 && paren == true)
			{
				rv += ns[0] * ns[1];
			}
			
		}
		*/
		// 28476021 too low
		check.Compare(rv);
		return rv;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 48L);
		else
			check = new StarCheck(key, 0L);

		var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var regex = new Regex(@"mul\(\d+,\d+\)|don't\(\)|do\(\)");
		var matches = regex.Matches(text);
		bool enabled = true;
		foreach (var match in matches)
		{
			if (match.ToString() == "do()")
				enabled = true;
			else if (match.ToString() == "don't()")
				enabled = false;
			if (enabled == false)
				continue;
			var parts = match.ToString().Split("(,)".ToCharArray());
			if (parts.Length == 4)
			{
				rv += int.Parse(parts[1]) * int.Parse(parts[2]);
			}
		}

		check.Compare(rv);
		return rv;
	}
}

