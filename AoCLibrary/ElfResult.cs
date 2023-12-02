// See https://aka.ms/new-console-template for more information
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace AoCLibrary
{
    public class ElfResult
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public MemberGroup Members { get; set; }    // read through json
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		// just for our app to use
		public DateTime Timestamp { get; set; } = DateTime.Now;    

		public Member FindbyName(string name)
        {
            return AllMembers().First(m => m.Name == name);
        }
        public Member[] AllMembers(bool hideZeros = true)
        {
            return Members.AllMembers(hideZeros);
        }
        public bool HasChanges(ElfResult? last, ILogger? logger)
        {
            if (last == null)
                return true;
            bool rv = false;
            var allMembers = AllMembers();
            var lastAllMembers = last.AllMembers();
			if (lastAllMembers == null)
				return true;
			for (int i = 0; i < allMembers.Length; i++)
            {
                var m = allMembers[i];
                var lastM = lastAllMembers[i];
                if (m.LocalScore != lastM.LocalScore)
                {
                    logger?.Log($"{m.Name} Gained {m.LocalScore - lastM.LocalScore}!");
                    rv = true;
                }
            }
            return rv;
        }
    }
    public class MemberGroup
    {
        public Member[] AllMembers(bool hideZeros = true)
        {
            if (Member08.Name == null)
                Member08.Name = "-";
            var rv = new Member[] {
                Member00, Member01, Member02, Member03, Member04, Member05, Member06, Member07, Member08, Member09,
                Member10, Member11, Member12, Member13, Member14, Member15, Member16, Member17, Member18, Member19,
                Member20, Member21};
            if (hideZeros)
                rv = rv.Where(m => m.LocalScore  != 0).ToArray();
            return rv;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

		[JsonPropertyName("2428221")]
        public Member Member00 { get; set; }    // Greg
        [JsonPropertyName("2497986")]
        public Member Member01 { get; set; }    // Jesse
        [JsonPropertyName("2439451")]
        public Member Member02 { get; set; }    // Dan
        [JsonPropertyName("2439454")]
        public Member Member03 { get; set; }    // Lan Vu
        [JsonPropertyName("1403088")]
        public Member Member04 { get; set; }    // Hambone
        [JsonPropertyName("2481606")]
        public Member Member05 { get; set; }    // Parker
        [JsonPropertyName("1555842")]
        public Member Member06 { get; set; }    // Colin
        [JsonPropertyName("2298841")]
        public Member Member07 { get; set; }    // Nelson
        [JsonPropertyName("1839258")]
        public Member Member08 { get; set; }    // "" 
        [JsonPropertyName("1539979")]
        public Member Member09 { get; set; }    // Ian
        [JsonPropertyName("678584")]
        public Member Member10 { get; set; }    // Ali
        [JsonPropertyName("1659760")]
        public Member Member11 { get; set; }    // Jesse0
        [JsonPropertyName("1861750")]
        public Member Member12 { get; set; }    // Fafa
        [JsonPropertyName("1650120")]
        public Member Member13 { get; set; }    //Brian Parker
        [JsonPropertyName("1538218")]
        public Member Member14 { get; set; }    // Derrick
        [JsonPropertyName("2454059")]
        public Member Member15 { get; set; }    // Joe
        [JsonPropertyName("1538420")]
        public Member Member16 { get; set; }    // Fomur
        [JsonPropertyName("1646913")]
        public Member Member17 { get; set; }    // Buddha
        [JsonPropertyName("2452095")]
        public Member Member18 { get; set; }    // JTruit
        [JsonPropertyName("1562200")]
        public Member Member19 { get; set; }    // Ryan G
        [JsonPropertyName("850621")]
        public Member Member20 { get; set; }    // Francis
        [JsonPropertyName("2500712")]
        public Member Member21 { get; set; }    // Michael Roy
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	}
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
                { "iangohjhu", "https://ca.slack-edge.com/TB4KLF92L-UB6DY5ELV-b2bcc72bd21e-512" }
            };
        static readonly Dictionary<string, string> _shortnames =
            new () {
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
            };
        public string GetName()
        {
            return GetName(Name);
        }
        static public string GetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "-";

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
        public DayLevels CompetitionDayLevel { get; set; } = new();

		public override string ToString()
		{
			return $"{GetStarString()} {GetName()} {LocalScore} {GetAverageString()} {TimeString(LastStarTs)} {GapString()}";
		}

		private string GapString()
		{
			var times = AllTimes();
			if (times.Count() < 2)
				return "";
			return ElfHelper.DeltaString(times[0] - times[1]);
		}
		
		private List<DateTime> AllTimes()
		{
			List<DateTime> times = [];
			foreach (var day in CompetitionDayLevel.AllDays())
			{
				if (day == null)
					continue;
				if (day.Star1 != null)
					times.Add(day.Star1.StarTime);
				if (day.Star2 != null)
					times.Add(day.Star2.StarTime);
			}
			return times.OrderByDescending(t => t).ToList();
		}

		string GetAverageString()
		{
			var avg = "-";
			if (Stars > 0)
				avg = ElfHelper.Fraction((double)LocalScore / Stars, 8);
			return avg;
		}
		private string GetStarString()
		{
			var days = (DateTime.Today - new DateTime(ElfHelper.Year, 11, 30)).TotalDays;
			if (days > 25)
				days = 25;
			var starString = "½";
			if (Stars == days * 2)
				starString = "*";   //⭐";
			return starString;
		}

		static private string TimeString(int ts)
		{
			var time = ElfHelper.GetTime(ts);
			if (DateTime.Today == time.Date)
				return time.ToString("HH:mm");
			else
				return time.ToString("M/d HH:mm");
		}
	}
    public class DayLevels
    {
		public DayLevel[] AllDays() => new DayLevel[] {
					   Day01, Day02, Day03, Day04, Day05, Day06, Day07, Day08, Day09,
				Day10, Day11, Day12, Day13, Day14, Day15, Day16, Day17, Day18, Day19,
				Day20, Day21, Day22, Day23, Day24, Day25};

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonPropertyName("1")]
		public DayLevel Day01 { get; set; }
		[JsonPropertyName("2")]
        public DayLevel Day02 { get; set; }
        [JsonPropertyName("3")]
        public DayLevel Day03 { get; set; }
        [JsonPropertyName("4")]
        public DayLevel Day04 { get; set; }
        [JsonPropertyName("5")]
        public DayLevel Day05 { get; set; }
        [JsonPropertyName("6")]
        public DayLevel Day06 { get; set; }
        [JsonPropertyName("7")]
        public DayLevel Day07 { get; set; }
        [JsonPropertyName("8")]
        public DayLevel Day08 { get; set; }
        [JsonPropertyName("9")]
        public DayLevel Day09 { get; set; }
        [JsonPropertyName("10")]
        public DayLevel Day10 { get; set; }
        [JsonPropertyName("11")]
        public DayLevel Day11 { get; set; }
        [JsonPropertyName("12")]
        public DayLevel Day12 { get; set; }
        [JsonPropertyName("13")]
        public DayLevel Day13 { get; set; }
        [JsonPropertyName("14")]
        public DayLevel Day14 { get; set; }
        [JsonPropertyName("15")]
        public DayLevel Day15 { get; set; }
        [JsonPropertyName("16")]
        public DayLevel Day16 { get; set; }
        [JsonPropertyName("17")]
        public DayLevel Day17 { get; set; }
        [JsonPropertyName("18")]
        public DayLevel Day18 { get; set; }
        [JsonPropertyName("19")]
        public DayLevel Day19 { get; set; }
        [JsonPropertyName("20")]
        public DayLevel Day20 { get; set; }
        [JsonPropertyName("21")]
        public DayLevel Day21 { get; set; }
        [JsonPropertyName("22")]
        public DayLevel Day22 { get; set; }
        [JsonPropertyName("23")]
        public DayLevel Day23 { get; set; }
        [JsonPropertyName("24")]
        public DayLevel Day24 { get; set; }
        [JsonPropertyName("25")]
        public DayLevel Day25 { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	}
	public class DayLevel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[JsonPropertyName("1")]
        public StarLevel Star1 { get; set; }
        [JsonPropertyName("2")]
        public StarLevel Star2 { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
    }
}
