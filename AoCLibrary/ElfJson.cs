using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace AoCLibrary
{
	public class Member
	{
		public Member()
		{
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
		public string GetName()
		{
			if (string.IsNullOrWhiteSpace(Name))
				Name = "JD" + Id.ToString().Substring(4,3);

			return GetName(Name);
		}
		static public string GetName(string name)
		{
			if (!_shortnames.TryGetValue(name, out string? value))
				return name;

			return value;
		}
		public string? GetUrl()
		{
			return GetUrl(Name);
		}
		static public string? GetUrl(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				name = "-";

			if (!_urls.TryGetValue(name, out string? value))
				return null;

			return value;
		}
		public string Name { get; set; } = "-noname-";
		public int Id { get; set; }
		public int Stars { get; set; }

		[JsonPropertyName("Local_Score")]
		public int LocalScore { get; set; }
		[JsonPropertyName("Global_Score")]
		public int GlobalScore { get; set; }
		public DateTime LastStarTime
		{
			get
			{
				return ElfHelper.GetTime(LastStarTs);
			}
		}

		[JsonPropertyName("last_star_ts")]
		public int LastStarTs { get; set; }
		[JsonPropertyName("completion_day_level")]
		public DaysLevel CompetitionDayLevel { get; set; }

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
	public enum StarEnum
	{
		NA,
		Star1,
		Star2
	}
	public class StarKey
	{
		public StarKey(string day, string star)
		{
			DayIndex = int.Parse(day) - 1;
			if (star == "1")
				Star = StarEnum.Star1;
			else if (star == "2")
				Star = StarEnum.Star2;
		}
		public StarKey(int dayIndex, StarEnum star)
		{
			DayIndex = dayIndex;
			Star = star;
		}
		public override int GetHashCode()
		{
			return DayIndex * 100 + (int) Star;
		}
		public override bool Equals(object? obj)
		{
			if (obj is StarKey other)
				return DayIndex == other.DayIndex && Star == other.Star;
			return false;
		}
		public int DayIndex { get; set; }
		public StarEnum Star { get; set; }
		public override string ToString()
		{
			return $"{(DayIndex + 1):00}-{Star}";
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
				return ElfHelper.GetTime(StarTs);
			}
		}

		public int Rank { get; set; }
	}
}
