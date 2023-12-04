namespace AoCLibrary
{
	static public class ElfExport
	{
		// returns last
		static public void Export(ElfResult res)
		{

			var stars = new Dictionary<string, List<StarScore>>();
			string key;
			var bestDeltas = new Dictionary<string, TimeSpan>();
			int maxScore = res.Members.Count();
			foreach (var member in res.AllMembers(true))
			{
				int iDay = 1;
				foreach (var day in member.AllDays(false))
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
				int score = maxScore;
				foreach (var starScore in starScores)
				{
					starScore.Score = score--;
				}
				allStars.AddRange(starScores);
			}
			// cummalitive scores
			var byMembers = allStars.GroupBy(s => s.Name);
			foreach (var scoreSet in byMembers)
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
			foreach (var member in res.AllMembers(false))
			{
				outs.Add(member.Name, new());
				outs2.Add(member.Name, new());
			}
			var hours = new List<string>();
			foreach (var kvp in groups)
			{
				hours.Add(kvp.Key.ToString("M/d H:mm"));
				foreach (var member in res.AllMembers(false))
				{
					var founds = kvp.Where(m => m.Name == member.Name);
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
			foreach (var outPart in outs)
			{
				outlines.Add(Member.GetName(outPart.Key) + "," + Member.GetUrl(outPart.Key) + "," + string.Join(',', outPart.Value));
			}

			File.WriteAllLines(Path.Combine(Communicator.Dir, "CalcedScores.csv"), outlines);
		}
	
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
}
