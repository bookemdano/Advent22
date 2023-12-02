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
                        stars[key].Add(new StarScore(member.Name, day.Star1.StarTime()));
                    }

                    if (day?.Star2 != null)
                    {
                        key = $"{iDay}.{2}";
                        if (!stars.ContainsKey(key))
                            stars.Add(key, []);
                        stars[key].Add(new StarScore(member.Name, day.Star2.StarTime()));
                        var delta = day.Star2.StarTime() - day.Star1.StarTime();
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

		public static ElfResult? OldExport(ILogger logger)
        {
            {
				var file = Path.Combine(Communicator.Dir, "CalcScore.csv");
				if (!File.Exists(file))
					return null;
				var inlines = File.ReadAllLines(Path.Combine(Communicator.Dir, "CalcScore.csv"));
                var scores = new Dictionary<DateTime, List<StarScore>>();
                foreach(var line in inlines)
                {
                    if (line.StartsWith("Hour"))
                        continue;
                    var c = new StarScore(line);
                    if (!scores.ContainsKey(c.Timestamp))
                        scores.Add(c.Timestamp, new List<StarScore>());
                    var score = scores[c.Timestamp].FirstOrDefault(s => s.Name == c.Name);
                    if (score == null)
                        scores[c.Timestamp].Add(c);
                    else
                        score.Score = c.Score;
                }
                var rows = new Dictionary<string, List<int>>();
                //foreach(var name in names)
                //{
                 //   row
                //}
            }


            var outs = new List<string>();
            var all = ReadAll();
			if (all == null)
				return null;
            var finalResult = all.Last().Value;
            var lines = new List<string>();
            foreach (var member in finalResult.AllMembers())
            {
                if (member.LocalScore == 0)
                    continue;
                var partScore = member.GuessScore();
                lines.Add(string.Join(",", partScore));
            }
            File.WriteAllLines(Path.Combine(Communicator.Dir, "times.csv"), lines);
            // header
            var parts = new List<string>();
            parts.Add("");
            parts.Add("url");
            foreach (var key in all.Keys)
                parts.Add(key.ToString("M/d H:mm"));
            outs.Add(string.Join(",", parts));

            // export for graph
            var firstMember = all.First().Value;
            foreach (var member in firstMember.AllMembers())
            {
                var name = member.Name;
                if (finalResult.FindbyName(name).LocalScore == 0)
                    continue;
                parts = new List<string>();
                parts.Add(member.GetName());
                parts.Add(member.GetUrl()??"");
                foreach (var kvp in all)
                    parts.Add(kvp.Value.FindbyName(name).LocalScore.ToString());
                outs.Add(string.Join(",", parts));
            }
            File.WriteAllLines(Path.Combine(Communicator.Dir, "UserByRow.csv"), outs);


            outs = [];
            ElfResult? prevResult = null;
            foreach (var kvp in all)
            {
                var date = kvp.Key;
                var aocResult = kvp.Value;
                if (!outs.Any())
                {
                    var headerParts = new List<string>();
                    headerParts.Add("Date");
                    foreach (var member in aocResult.AllMembers())
                    {
                        if (finalResult.FindbyName(member.Name).LocalScore > 0)
                            headerParts.Add(member.GetName());
                    }
                    outs.Add(string.Join(",", headerParts));
                }

                parts = new List<string>();
                parts.Add(date.ToString());
                foreach (var member in aocResult.AllMembers())
                {
                    if (finalResult.FindbyName(member.Name).LocalScore > 0)
                    {
                        //parts.Add(member.LocalScore.ToString());
                        var prev = prevResult?.FindbyName(member.Name);
                        if (prev == null)
                            parts.Add(member.LocalScore.ToString());
                        else
                            parts.Add((member.LocalScore - prev.LocalScore).ToString());
                    }
                }
                prevResult = aocResult;
                outs.Add(string.Join(",", parts));
            }
            try
            {
                File.WriteAllLines(Path.Combine(Communicator.Dir, "scores.csv"), outs);
            }
            catch (Exception ex)
            {
                logger.Log("Error " + ex.Message);
            }
            return finalResult;
        }
        static Dictionary<DateTime, ElfResult>? ReadAll()
        {
            if (!Directory.Exists(Communicator.Dir))
                return null;
            var files = Directory.GetFiles(Communicator.Dir, "url*.json").Order();
            if (!files.Any())
                return null;
            ElfResult? last = null;
            var rv = new Dictionary<DateTime, ElfResult>();
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                var res = Deserialize(json);
				if (res != null && res.HasChanges(last, null))
                {
                    rv.Add(Communicator.TimeFromFile(file), res);
                    last = res;
                }
            }
            return rv;
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
	}
}
