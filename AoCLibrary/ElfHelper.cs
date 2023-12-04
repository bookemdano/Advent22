using System.Diagnostics;
using System.Text.Json;

namespace AoCLibrary
{
    public class ElfHelper
    {
		static public string AppName { get; set; } = string.Empty;
		public static void Log(object o, Stopwatch? sw = null)
		{
			var str = $"{AppName} {o}";
			if (sw != null)
				str += $" {sw.ElapsedMilliseconds:0}ms";
			str = $"{DateTime.Now} {str}";
			File.AppendAllText(Path.Combine(Communicator.Dir, $"endless{DateTime.Today:yyyy}.log"), str + Environment.NewLine);
			Console.WriteLine(str);
		}
		static string _testLogFile = $"endless{DateTime.Today:yyyyMMdd}.log";
		public static void ResetTestLog()
		{
			File.Delete(Path.Combine(Communicator.Dir, _testLogFile));
		}
		public static void TestLog(object o, Stopwatch? sw = null)
		{
			var str = o?.ToString() ?? "";
			if (sw != null)
				str += $" {sw.ElapsedMilliseconds:0}ms";
			str = $"{DateTime.Now} {str}";
			File.AppendAllText(Path.Combine(Communicator.Dir, _testLogFile), str + Environment.NewLine);
			Console.WriteLine(str);
		}

		public static ElfResult? Deserialize(string json)
		{
			return JsonSerializer.Deserialize<ElfResult>(json, _jsonOptions);
		}
		static readonly JsonSerializerOptions _jsonOptions = new () { PropertyNameCaseInsensitive = true, WriteIndented = true };
		public static string Serialize(ElfResult result)
		{
			return JsonSerializer.Serialize(result, _jsonOptions);
		}

		public static readonly int Year = DateTime.Today.Year;

		public static int MaxScore() => 22;

		public static string DayString()
		{
			return $"{Day:00}";
		}
		public static int Day
		{
			get
			{
				return (int)(DateTime.Today - new DateTime(Year, 11, 30)).TotalDays;
			}
		}

		static public string CodeDir()
		{
			return "C:\\repos\\Advent22\\Advent" + DateTime.Today.ToString("yy");
		}

		public static async Task WriteStubFiles(int day, bool updatePrj)
		{
			var strDay = $"{day:00}";
			var codeDir = ElfHelper.CodeDir();
			var assetDir = Path.Combine(codeDir, "assets");
			var cs = File.ReadAllText(Path.Combine(assetDir, "DayCS.txt"));
			cs = cs.Replace("Day : IDayRunner", $"Day{strDay} : IDayRunner");
			File.WriteAllText(Path.Combine(codeDir, $"Day{strDay}.cs"), cs);
			var str = await Communicator.Read($"{DailyUrl}/input");
			if (string.IsNullOrEmpty(str))
				File.Copy(Path.Combine(assetDir, "Day01.txt"), Path.Combine(assetDir, $"Day{strDay}.txt"), true);
			else
				File.WriteAllText(Path.Combine(assetDir, $"Day{strDay}.txt"), str);

			File.Copy(Path.Combine(assetDir, "Day01FakeStar1.txt"), Path.Combine(assetDir, $"Day{strDay}FakeStar1.txt"), true);
			File.Copy(Path.Combine(assetDir, "Day01FakeStar2.txt"), Path.Combine(assetDir, $"Day{strDay}FakeStar2.txt"), true);

			if (updatePrj)
			{
				var block = "\r\n    <Content Include=\"Assets\\Day|DD|.txt\">\r\n      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\r\n    </Content>\r\n    <Content Include=\"Assets\\Day|DD|FakeStar1.txt\">\r\n      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\r\n    </Content>\r\n    <Content Include=\"Assets\\Day|DD|FakeStar2.txt\">\r\n      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\r\n    </Content>";
				block = block.Replace("|DD|", strDay);
				var prj = File.ReadAllText(Path.Combine(codeDir, "Advent23.csproj"));
				var i = prj.IndexOf("<ItemGroup>");
				if (i == -1)
					return;
				i += "<ItemGroup>".Length;
				File.WriteAllText(Path.Combine(codeDir, "Advent23.csproj"), prj.Insert(i, block));
			}
		}
		public static TimeSpan MinApiRefresh => TimeSpan.FromMinutes(15);
		public static async Task<ElfResult?> Read(bool force)
		{
			var jsonFile = Path.Combine(Communicator.Dir, $"aoc{DateTime.Today:yyyyMMdd}.json");
			ElfResult? rv = null;
			if (File.Exists(jsonFile) && !force)
			{
				var json = File.ReadAllText(jsonFile);
				rv = Deserialize(json);
			}
			Log($"Read({force}) Data Time: {rv?.Timestamp} Data Expires: {rv?.Timestamp + MinApiRefresh}");
			if (rv != null && rv.Timestamp + MinApiRefresh > DateTime.Now)
			{
				Log($"New enough");
				return rv;  // new enough
			}
			
			var str = await Communicator.Read($"{LeaderUrl}.json");
			rv = Deserialize(str);
			if (rv == null)
				return rv;
			rv.CalcRank();
			File.WriteAllText(jsonFile, Serialize(rv));
			return rv;
		}

		static public string Fraction(double origD, int denom)
		{
			var rv = "";
			var d = origD;
			if (d >= 1)
			{
				rv = ((int)d).ToString();
				d -= (int)d;
			}

			if (d == 0)
				return rv;
			else if (d <= 1 / 8.0 && denom > 8)
				return rv + "⅛";
			else if (d <= 1 / 4.0 && denom >= 4)
				return rv + "¼";
			else if (d <= 1 / 3.0 && denom >= 6)
				return rv + "⅓";
			else if (d <= 3 / 8.0 && denom >= 8)
				return rv + "⅜";
			else if (d <= 1 / 2.0 && denom >= 2)
				return rv + "½";
			else if (d <= 5 / 8.0 && denom >= 8)
				return rv + "⅝";
			else if (d <= 2 / 3.0 && denom >= 6)
				return rv + "⅔";
			else if (d <= 3 / 4.0 && denom >= 4)
				return rv + "¾";
			else if (d <= 7 / 8.0 && denom >= 8)
				return rv + "⅞";
			else
				return ((int)origD + 1).ToString(); ;
		}
		static internal string DeltaString(TimeSpan delta)
		{
			int n;
			string unit;
			var years = delta.TotalDays / 365.24;
			if (years > 5)
			{
				n = (int)years;
				unit = "year";
			}
			else if (years > 2)
			{
				return $"{Fraction(years, 2)} years";
			}
			else if (delta.TotalDays > 1)
			{
				unit = "day";
				n = (int)delta.TotalDays;
			}
			else if (delta.TotalHours > 1.5)
			{
				unit = "hour";
				n = (int)delta.TotalHours;
			}
			else if (delta.TotalMinutes > 1.5)
			{
				unit = "min";
				n = (int)delta.TotalMinutes;
			}
			else
			{
				unit = "sec";
				n = (int)delta.TotalSeconds;
			}
			if (n != 1)
				unit += "s";
			return $"{n} {unit}";
		}
		static public DateTime GetTime(int ts)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(ts)).AddHours(-5);
		}

		internal static int DayIndex => Day - 1;

		//https://adventofcode.com/2023/day/4/input/
		public static string DailyUrl => $"https://adventofcode.com/{Year}/day/{Day}";
		static public string LeaderUrl => $"https://adventofcode.com/{ElfHelper.Year}/leaderboard/private/view/1403088";
		static public string TimeString(DateTime dt)
		{
			if (DateTime.Today == dt.Date)
				return dt.ToString("HH:mm");
			else
				return dt.ToString("M/d HH:mm");
		}
		static public void Open(string filename)
		{
			try
			{
				var psi = new ProcessStartInfo
				{
					UseShellExecute = true,
					FileName = filename
				};
				Process.Start(psi);

			}
			catch (Exception ex)
			{
				Log($"Open({filename}) " + ex);
			}
		}

		public static int NextEmptyDay()
		{
			for (int day = Day; day <= 25; day++)
			{
				var dayFile = $"Day{day:00}.cs";
				if (!File.Exists(Path.Combine(CodeDir(), dayFile)))
				{
					return day;
				}
			}
			return -1;
		}
	}
}
