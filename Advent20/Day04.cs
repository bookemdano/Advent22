using AoCLibrary;

namespace Advent20;

internal class Day04 : IRunner
{
	// Day https://adventofcode.com/2020/day/4
	// Input https://adventofcode.com/2020/day/4/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 2L);
		else
			res.Check = new StarCheck(key, 206L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		
		var reqs = new List<string>() {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
		var creds = new List<string>();
		foreach(var line in lines)
		{
			if (string.IsNullOrWhiteSpace(line))
			{
				// check
				bool valid = true;
				foreach (var req in reqs)
					if (!creds.Contains(req))
						valid = false;
				if (creds.Count() >= 7)
					rv++;
				creds = new List<string>();
			}
			else
			{
				var parts = line.Split(' ');
				foreach(var part in parts)
				{
					var cred = part.Split(':')[0];
					if (!reqs.Contains(cred))
						ElfHelper.DayLog("other " + cred);
					if (cred != "cid")
	                    creds.Add(cred);
				}
			}
		}
        if (creds.Count() >= 7)
            rv++;
        // too low 205
        res.CheckGuess(rv);
        return res;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 2L);
		else
			res.Check = new StarCheck(key, 123L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var reqs = new List<string>() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
        var creds = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                // check
                if (creds.Count() >= 7)
                    rv++;
                creds = new List<string>();
            }
            else
            {
                var parts = line.Split(' ');
                foreach (var part in parts)
                {
                    var subs = part.Split(':');
                    var cred = subs[0];
                    var str = subs[1];
                    var valid = false;
                    if (cred == "byr")
                    {
                        if (int.TryParse(str, out int year))
                            if (year >= 1920 && year <= 2002)
                                valid = true;
                    }
                    else if (cred == "iyr")
                    {
                        if (int.TryParse(str, out int year))
                            if (year >= 2010 && year <= 2020)
                                valid = true;
                    }
                    else if (cred == "eyr")
                    {
                        if (int.TryParse(str, out int year))
                            if (year >= 2020 && year <= 2030)
                                valid = true;
                    }
                    else if (cred == "hgt")
                    {
                        var numberPart = str.Substring(0, str.Length - 2);
                        if (int.TryParse(numberPart, out int hgt))
                        {
                            if (str.Contains("cm") && hgt >= 150 && hgt <= 193)
                                valid = true;
                            else if (str.Contains("in") && hgt >= 59 && hgt <= 76)
                                valid = true;
                        }
                    }
                    else if (cred == "hcl")
                    {
                        if (str[0] == '#' && str.Length == 7)
                        {
                            valid = true;
                            for (int i = 1; i < str.Length; i++)
                            {
                                if (!IsHex(str[i]))
                                    valid = false;
                            }
                        }
                    }
                    else if (cred == "ecl")
                    {
                        if (str == "amb" || str == "blu" || str == "brn" || str == "gry" || str == "grn" || str == "hzl" || str == "oth")
                            valid = true;
                    }
                    else if (cred == "pid")
                    {
                        if (str.Length == 9 && str.All(c => char.IsNumber(c)))
                            valid = true;
                    }
                    if (valid)
                        creds.Add(cred);
                    else if (cred != "cid")
                        ElfHelper.DayLog("Something wrong " + part);
                }
            }
        }
        if (creds.Count() >= 7)
            rv++;

        res.CheckGuess(rv);
        return res;
	}
    static bool IsHex(char c)
    {
        if (char.IsUpper(c))
            return false;
        return char.IsAsciiHexDigit(c);
        /*var nFrom0 = c - '0';
        if (nFrom0 >= 0 && nFrom0 <= 9)
            return true;
        var nFroma = c - 'a';
        if (nFroma >= 0 && nFroma <= 5)
            return true;
        return false;
        */
    }
}

