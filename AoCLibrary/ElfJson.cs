using System.Text.Json.Serialization;

namespace AoCLibrary
{
	public class Member
	{
		public string Name { get; set; } = "-noname-";
		public int Id { get; set; }
		public int Stars { get; set; }

		[JsonPropertyName("Local_Score")]
		public int LocalScore { get; set; }
		[JsonPropertyName("Global_Score")]
		public int GlobalScore { get; set; }
		[JsonPropertyName("last_star_ts")]
		public int LastStarTs { get; set; }
		[JsonPropertyName("completion_day_level")]
		public DaysLevel CompetitionDayLevel { get; set; } = [];

		public DateTime LastStarTime
		{
			get
			{
				return Utils.GetTime(LastStarTs);
			}
		}


		public Member()
		{
		}

		public string? GetUrl() => ElfHelper.GetUrl(Name);

		public string GetName()
		{
			if (string.IsNullOrWhiteSpace(Name))
				Name = "JD" + Id.ToString().Substring(4,3);

			return ElfHelper.GetName(Name);
		}


		public override string ToString()
		{
			return $"{GetName()} {Stars} {LocalScore} {LastStarTime}";
		}

		internal Dictionary<StarKey, StarLevel> AllStars()
		{
			var rv = new Dictionary<StarKey, StarLevel>();
			if (CompetitionDayLevel == null)
				return rv;
			foreach(var kvpDay in CompetitionDayLevel)
			{
				var day = kvpDay.Key;
				foreach(var kvpStar in kvpDay.Value)
				{
					var star = kvpStar.Key;
					var starKey = new StarKey(day, star);
					rv[starKey] = kvpStar.Value;
				}
			}
			return rv;
		}
		internal DayLevel? GetDay(int dayIndex)
		{
			return CompetitionDayLevel.GetDay(dayIndex);
		}

		public string Places()	
		{
			var stars = AllStars();
			if (!stars.Any())
				return "";
			var parts = new List<string>();
			var end = ElfHelper.DayIndex - 7;
			if (end < 0)
				end = 0;
			for (int i = ElfHelper.DayIndex; i >= end; i--)
			{
				var star1 = stars.FirstOrDefault(s => s.Key.DayIndex == i && s.Key.Star == StarEnum.Star1).Value?.Rank.ToString() ?? "-";
				var star2 = stars.FirstOrDefault(s => s.Key.DayIndex == i && s.Key.Star == StarEnum.Star2).Value?.Rank.ToString() ?? "-";
				parts.Add($"({star1},{star2})");
			}
			return string.Join(",", parts.ToArray());
		}
	}

	public class DaysLevel : Dictionary<string, DayLevel>
	{
		internal DayLevel? GetDay(int dayIndex)
		{
			var key = (dayIndex + 1).ToString();
			if (ContainsKey(key))
				return this[key];
			return null;
		}
	}

	public class DayLevel : Dictionary<string, StarLevel>
	{
		internal int StarCount()
		{
			return this.Count();
		}
	}

	public class StarLevel
	{
		[JsonPropertyName("get_star_ts")]
		public int StarTs { get; set; }
		public DateTime StarTime
		{
			get
			{
				return Utils.GetTime(StarTs);
			}
		}

		public int Rank { get; set; }
	}
}
