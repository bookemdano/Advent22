using System.Diagnostics;
using System.Text.Json;

namespace AoCLibrary
{
    public class ElfHelper
    {

        // returns last
        static public void Export(ElfResult res, ILogger logger)
        {

            var stars = new Dictionary<string, List<StarScore>>();
            string key;
            var bestDeltas = new Dictionary<string, TimeSpan>();
            foreach (var member in res.AllMembers(true))
            {
                int iDay = 1;
                foreach (var day in member.CompetitionDayLevel.AllDays())
                {

                    if (day?.Star1 != null)
                    {
                        key = $"{iDay}.{1}";
                        if (!stars.ContainsKey(key))
                            stars.Add(key, []);
                        stars[key].Add(new StarScore(member.Name, day.Star1.StarTime));
                    }

                    if (day?.Star2 != null)
                    {
                        key = $"{iDay}.{2}";
                        if (!stars.ContainsKey(key))
                            stars.Add(key, []);
                        stars[key].Add(new StarScore(member.Name, day.Star2.StarTime));
                        var delta = day.Star2.StarTime - day.Star1.StarTime;
                        bestDeltas.Add($"{Member.GetName(member.Name)} Day {iDay}", delta);
                    }
                    iDay++;
                }
            }

            var bests = new List<string>();
            foreach (var order in bestDeltas.OrderBy(kvp => kvp.Value).Take(10))
                bests.Add($"{order.Key}: {order.Value.TotalSeconds:0}s");
            File.WriteAllLines(Path.Combine(Communicator.Dir, "BestDelta.csv"), bests);

            var allStars = new List<StarScore>();
            foreach (var kvp in stars)
            {
                var starScores = kvp.Value.OrderBy(s => s.Timestamp);
                int score = 22;
                foreach (var starScore in starScores)
                {
                    starScore.Score = score--;
                }
                allStars.AddRange(starScores);
            }
            // cummalitive scores
            var byMembers = allStars.GroupBy(s => s.Name);
            foreach(var scoreSet in byMembers)
            {
                var val = 0;
                var starVal = 0;
                foreach (var score in scoreSet)
                {
                    score.Score += val;
                    val = score.Score;
                    if (val > 0)
                    {
                        starVal++;
                        score.Stars = starVal;
                    }
                }
            }

            var groups = allStars.GroupBy(s => s.GetHour()).OrderBy(k => k.Key);
            var outs = new Dictionary<string, List<string>>();
            var outs2 = new Dictionary<string, List<string>>();
            foreach (var member in res.AllMembers())
            {
                outs.Add(member.Name, new ());
                outs2.Add(member.Name, new ());
            }
            var hours = new List<string>();
            foreach (var kvp in groups)
            {
                hours.Add(kvp.Key.ToString("M/d H:mm"));
                foreach (var member in res.AllMembers())
                {
                    var founds = kvp.Where(m => m.Name ==  member.Name);
                    if (founds?.Any() == true)
                    {
                        outs[member.Name].Add(founds.Max(f => f.Score).ToString());
                        outs2[member.Name].Add(founds.Max(f => f.Stars).ToString());
                    }
                    else
                    {
                        if (!outs[member.Name].Any())
                        {
                            outs[member.Name].Add("0");
                            outs2[member.Name].Add("0");
                        }
                        else
                        {
                            outs[member.Name].Add(outs[member.Name].Last());
                            outs2[member.Name].Add(outs2[member.Name].Last());
                        }
                    }
                }
            }
            var outlines2 = new List<string>();
            outlines2.Add("Name,url," + string.Join(',', hours));
            foreach (var outPart in outs2)
            {
                outlines2.Add(Member.GetName(outPart.Key) + "," + Member.GetUrl(outPart.Key) + "," + string.Join(',', outPart.Value));
            }

            File.WriteAllLines(Path.Combine(Communicator.Dir, "CalcedStars.csv"), outlines2);

            var outlines = new List<string>();
            outlines.Add("Name,url," + string.Join(',', hours));
            foreach(var outPart in outs)
            {
                outlines.Add(Member.GetName(outPart.Key) + "," + Member.GetUrl(outPart.Key) + "," + string.Join(',', outPart.Value));
            }

            File.WriteAllLines(Path.Combine(Communicator.Dir, "CalcedScores.csv"), outlines);
        }
        class StarScore
        {
            public StarScore(string name, DateTime starTime)
            {
                Name = name;
                Timestamp = starTime; 
            }
            public StarScore(string line)
            {
                var parts = line.Split(",");
                Timestamp = DateTime.ParseExact(parts[0], "M/d H:mm", null);
                Name = parts[1];
                Score = int.Parse(parts[3]);
            }
            public DateTime GetHour()
            {
                return new DateTime(Timestamp.Year, Timestamp.Month, Timestamp.Day, Timestamp.Hour, 0, 0);
            }
            public DateTime Timestamp { get; set; }
            public string Name { get; set; }
            public int Score { get; set; }
            public int Stars { get; set; }
        }
		public static void Log(object o, Stopwatch? sw = null)
		{
			var str = o?.ToString() ?? "";
			if (sw != null)
				str += $" {sw.ElapsedMilliseconds:0}ms";
			str = $"{DateTime.Now} {str}";
			File.AppendAllText(Path.Combine(Communicator.Dir, $"endless{DateTime.Today.Year}.log"), str + Environment.NewLine);
			Console.WriteLine(str);
		}

		public static ElfResult? Deserialize(string json)
		{
			return JsonSerializer.Deserialize<ElfResult>(json, _jsonOptions);
		}
		static readonly JsonSerializerOptions _jsonOptions = new () { PropertyNameCaseInsensitive = true, WriteIndented = true };

		public static readonly int Year = DateTime.Today.Year;
		public static string DayString()
		{
			return $"{Day():00}";
		}
		static public string CodeDir()
		{
			return "C:\\repos\\Advent22\\Advent" + DateTime.Today.ToString("yy");
		}
		public static int Day()
		{
			return (int)(DateTime.Today - new DateTime(Year, 11, 30)).TotalDays;
		}
		public static string Serialize(ElfResult result)
		{
			return JsonSerializer.Serialize(result, _jsonOptions);
		}

		public static void WriteStubFiles(int day, bool updatePrj)
		{
			var strDay = $"{day:00}";
			var codeDir = ElfHelper.CodeDir();
			var assetDir = Path.Combine(codeDir, "assets");
			var cs = File.ReadAllText(Path.Combine(assetDir, "DayCS.txt"));
			cs = cs.Replace("Day : IDayRunner", $"Day{strDay} : IDayRunner");
			File.WriteAllText(Path.Combine(codeDir, $"Day{strDay}.cs"), cs);
			File.Copy(Path.Combine(assetDir, "Day01.txt"), Path.Combine(assetDir, $"Day{strDay}.txt"), true);
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

		public static async Task<ElfResult?> Read(bool force)
		{
			var jsonFile = Path.Combine(Communicator.Dir, $"aoc{DateTime.Today:yyyyMMdd}.json");
			ElfResult? rv = null;
			if (File.Exists(jsonFile) && !force)
			{
				var json = File.ReadAllText(jsonFile);
				rv = Deserialize(json);
			}
			if (rv != null && rv.Timestamp + TimeSpan.FromMinutes(15) > DateTime.Now)
				return rv;
			
			var str = await Communicator.Read($"https://adventofcode.com/{Year}/leaderboard/private/view/1403088.json");
			rv = Deserialize(str);
			if (rv == null)
				return rv;
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
	}
}
