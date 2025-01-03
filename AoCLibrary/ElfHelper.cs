using System.Diagnostics;

namespace AoCLibrary;

public class ElfHelper
{
	public static TimeSpan MinApiRefresh
	{
		get
		{
			/*
			var hour = DateTime.Now.Hour;
			if (hour == 0 || hour == 10)
				return TimeSpan.FromMinutes(10);
			else if (hour > 5 && hour < 10)
				return TimeSpan.FromMinutes(5);
			*/
			return TimeSpan.FromMinutes(15);    //AoC requests 15 minutes
		}
	}

	public static readonly int Year = DateTime.Today.AddDays(-31).Year;
	public static readonly int TotalDays = 25;
        public static bool IsActive
        {
            get
            {
			return (RawDay >= 1 && RawDay < TotalDays);
            }
        }

        public static double RawDay
        {
            get
            {
                return (DateTime.Now - new DateTime(Year, 11, 30)).TotalDays;
            }
        }
        public static int Day
        {
            get
            {
                var rv = (int) RawDay;
                if (rv > TotalDays)
                    return TotalDays;
                return rv;
            }
        }

        internal static int DayIndex => Day - 1;

	public static string DayString => $"{Day:00}";

	static public string CodeDir()
	{
		var root = string.Empty;
		if (Utils.IsWindows)
			root = "C:\\repos";
		else
			root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Projects");
		return Path.Combine(root, "Advent22", "Advent" + Year2);
	}
	static public string Year2 => DateTime.Today.ToString("yy");
	public static string SmallString(TimeSpan elapsed)
	{
		var ms = elapsed.TotalMilliseconds;
		if (ms > 1000)
			return $"{elapsed.TotalSeconds:0.00}s";
		else if (ms > 100)
			return $"{elapsed.TotalMilliseconds:0}ms";
		else
			return $"{elapsed.TotalMilliseconds:0.0}ms";
	}
	public static async Task<string> WriteInputFileAsync(int day)
	{
		var str = await Communicator.ReadAsync($"{DayUrl(day)}/input", returnError: false) ?? string.Empty;
		File.WriteAllText(Path.Combine(CodeDir(), "assets", $"Day{day:00}.txt"), str);
		return str;
	}
	public static async Task WriteStubFilesAsync(int day, bool updatePrj)
	{
		var strDay = $"{day:00}";
		var codeDir = CodeDir();
		var assetDir = Path.Combine(codeDir, "assets");
		var cs = File.ReadAllText(Path.Combine(assetDir, "DayCS.txt"));
		var dayUrl = DayUrl(day);
		cs = cs.Replace("|URL|", dayUrl);
		cs = cs.Replace("|INPUTURL|", $"{dayUrl}/input");
		cs = cs.Replace("|DD|", strDay);
		File.WriteAllText(Path.Combine(codeDir, $"Day{strDay}.cs"), cs);
		await WriteInputFileAsync(day);
		File.WriteAllText(Path.Combine(assetDir, $"Day{strDay}Fake.txt"), "");

		if (updatePrj)
		{
			var block = "\r\n    <Content Include=\"Assets\\Day|DD|.txt\">\r\n      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\r\n    </Content>\r\n    <Content Include=\"Assets\\Day|DD|Fake.txt\">\r\n      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\r\n    </Content>";
			block = block.Replace("|DD|", strDay);
			var prj = File.ReadAllText(Path.Combine(codeDir, $"Advent{Year2}.csproj"));
			var i = prj.IndexOf("<ItemGroup>");
			if (i == -1)
				return;
			i += "<ItemGroup>".Length;
			File.WriteAllText(Path.Combine(codeDir, $"Advent{Year2}.csproj"), prj.Insert(i, block));
		}
	}
	public static ElfResult? ReadFromFile()
	{
		var files = Directory.GetFiles(Utils.Dir, "aoc*.json").OrderByDescending(f => f);
		if (!files.Any())
			return null;

		var jsonFile = files.First();
		if (File.Exists(jsonFile))
		{
			var json = File.ReadAllText(jsonFile);
			return Utils.Deserialize<ElfResult>(json);
		}
		return null;
	}
	public static void WriteToFile(ElfResult result)
	{
		var jsonFile = Path.Combine(Utils.Dir, $"aoc{DateTime.Today:yyyyMMdd}.json");
		File.WriteAllText(jsonFile, Utils.Serialize(result));
	}
	public static async Task<ElfResult?> ReadAsync(bool force)
	{
		ElfResult? rv = null;
		if (!force)
			rv = ReadFromFile();

		Utils.MonthLog($"Read({force}) Data Time: {rv?.Timestamp} Data Expires: {rv?.Timestamp + MinApiRefresh}");
		if (rv != null && rv.Timestamp + MinApiRefresh > DateTime.Now)
		{
			Utils.MonthLog($"New enough");
			return rv;  // new enough
		}
		
		var str = await Communicator.ReadAsync($"{LeaderUrl}.json", returnError: false);
		if (str == null)
			return null;
		rv = Utils.Deserialize<ElfResult>(str);
		if (rv == null)
			return rv;
		rv.CalcRank();
		WriteToFile(rv);
		return rv;
	}


	public static string DayUrl(int day)
	{
		return $"https://adventofcode.com/{Year}/day/{day}";
	}

	//https://adventofcode.com/2023/day/4/input/
	public static string DailyUrl => DayUrl(Day);
	static public string LeaderUrl => $"https://adventofcode.com/{ElfHelper.Year}/leaderboard/private/view/1403088";
	static public string DayDir
	{
		get
		{
			var dir = Path.Combine(Utils.Dir, "Day" + DayString);
			Directory.CreateDirectory(dir);
			return dir;
		}
	}
	static readonly string _dayLogFile = $"day{DayString}-{DateTime.Today:yyyyMMdd}.log";
	public static void ResetDayLog()
	{
		File.Delete(Path.Combine(DayDir, _dayLogFile));
	}
	public static void MonthLogPlus(object o, Stopwatch? sw = null)
	{
		Utils.MonthLog(o, sw);
		DayLog(o, sw);
	}
	public static void DayLogPlus(object o)
	{
		DayLog(o, null);
		var str = o?.ToString() ?? "";
		Console.WriteLine(str);
	}

	public static void DayLog(object o, Stopwatch? sw = null)
	{
		var str = o?.ToString() ?? "";
		if (sw != null)
			str += $" {sw.ElapsedMilliseconds:0}ms";
		str = $"{DateTime.Now} {str}";
		File.AppendAllText(Path.Combine(DayDir, _dayLogFile), str + Environment.NewLine);
		//Console.WriteLine(str);
	}
	public static int NextEmptyDay()
	{
		for (int day = Day; day <= TotalDays; day++)
		{
			var dayFile = $"Day{day:00}.cs";
			if (!File.Exists(Path.Combine(CodeDir(), dayFile)))
			{
				return day;
			}
		}
		return -1;
	}
	static readonly Dictionary<string, string> _shortnames =
		new() {
			{ "Dan Francis", "Dano" },
			{ "alihacks", "Ali" },
			{ "FafaPaku", "Fafa" },
			{ "Derrick Fyfield", "Derrick" },
			{ "Greg Herpel", "Greg" },
			{ "Nelson Denn", "Nelson" },
			{ "jfitzsimmons2", "Joe" },
			{ "Jesse Rakowski", "Jesse" },
			{ "HamboneWilson", "Hambone" },
			{ "iangohjhu", "Ian" },
			{ "Jim Green", "Green" },
		};
	static public string GetName(string name)
	{
		if (!_shortnames.TryGetValue(name, out string? value))
			return name;

		return value;
	}
	static readonly Dictionary<string, string> _urls =
		new() {
			{ "Dan Francis", "https://ca.slack-edge.com/TB4KLF92L-U0389RE97GR-e3c92abca80a-512" },
			{ "alihacks", "https://ca.slack-edge.com/TB4KLF92L-UB57REZTM-97b89a97447f-512" },
			{ "FafaPaku", "https://ca.slack-edge.com/TB4KLF92L-UB6C5KWMV-f993b687636b-512" },
			{ "Derrick Fyfield", "https://ca.slack-edge.com/TB4KLF92L-UB7563K9B-4f73317ae1ac-512" },
			{ "Greg Herpel", "https://ca.slack-edge.com/TB4KLF92L-UG97TR1CG-0f7f046e40b1-512" },
			{ "Nelson Denn", "https://ca.slack-edge.com/TB4KLF92L-UB67BCMHR-391e25f1eb8d-512" },
			{ "jfitzsimmons2", "https://ca.slack-edge.com/TB4KLF92L-U02FRLVN8JF-e488e637c410-512" },
			{ "jtruit", "https://ca.slack-edge.com/TB4KLF92L-UB58QHN6Q-19f86b8e5625-512" },
			{ "Jesse Rakowski", "https://ca.slack-edge.com/TB4KLF92L-U020KKDNX5K-d8b8f0cfa119-512" },
			{ "iangohjhu", "https://ca.slack-edge.com/TB4KLF92L-UB6DY5ELV-b2bcc72bd21e-512" },
			{ "Jim Green", "https://jhuis.slack.com/archives/D068QSW145P/p1701696243394289" }
	};

	static public string? GetUrl(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			name = "-";

		if (!_urls.TryGetValue(name, out string? value))
			return null;

		return value;
	}

}
public class ElfUtils
{
	public static void WriteLines(string stub, string tag, List<string> lines)
	{
		File.WriteAllLines(Path.Combine(ElfHelper.DayDir, $"{stub}Day{ElfHelper.DayString}-{tag}.csv"), lines);
	}
}

public class StarCheckKey
{
	public StarCheckKey(StarEnum star, bool isReal, int? part = null)
	{
		Star = star;
		IsReal = isReal;
		Part = part;
	}
	public override string ToString()
	{
		var rv = $"{Star}";
		if (IsReal)
			rv += $" REAL";
		else
			rv += $" FAKE";
		if (Part != null)
			rv += $" part:{Part}";
		return rv;
	}
	public StarEnum Star { get; }
	public bool IsReal { get; }
	public int? Part { get; }
}

public class StarCheck
{
	public StarCheck(StarCheckKey key, long expected)
	{
		Key = key;
		Expected = expected;
	}
	public StarCheck(StarCheckKey key, string expected)
	{
		Key = key;
		ExpectedString = expected;
	}

	public StarCheckKey Key { get; }
	public string ExpectedString { get; }
	public long Expected { get; }
	bool UseString => !string.IsNullOrWhiteSpace(ExpectedString);
	public override string ToString()
	{
		if (!UseString)
			return $"{Key} e:{Expected}";
		else
			return $"{Key} e:{ExpectedString}";
	}
	public void Compare(long answer)
	{
		BaseCompare(answer, answer == Expected);
	}
	public void BaseCompare(object answer, bool success)
	{
		var str = success?"SUCCESS":"FAIL";
		ElfHelper.MonthLogPlus($"Compare {this} ?= a:{answer} {str}");
		Utils.CaptainsLog($"Compare {this} ?= a:{answer} {str}");
		Utils.Assert(success, $"{this} == a:{answer}");
	}
	public void Compare(string answer)
	{
		BaseCompare(answer, answer == ExpectedString);
	}
}
