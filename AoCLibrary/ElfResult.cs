namespace AoCLibrary
{
	public class ElfResult
    {
		public string Event { get; set; } = "-noevent-";
		public Dictionary<string, Member> Members { get; set; } = [];

		// just for our app to use
		public DateTime Timestamp { get; set; } = DateTime.Now;    

		public Member FindByName(string name)
        {
            return AllMembers(false).First(m => m.Name == name);
        }
        public Member[] AllMembers(bool hideZeros)
        {
			var rv = Members.Values.ToArray();
			if (rv == null)
				return [];
			if (hideZeros && rv != null)
				rv = rv.Where(m => m?.LocalScore > 0).ToArray();
			return rv;
        }
		// return null for no changes
		public List<string> HasChanges(ElfResult? last)
        {
			var rv = new List<string>();
			var allMembers = AllMembers(false);
            var lastAllMembers = last?.AllMembers(false);
			if (lastAllMembers == null)
				return rv;

			foreach (var member in allMembers)
			{
				var lastM = lastAllMembers.FirstOrDefault(m => m.Name == member.Name);
				if (lastM == null)
				{
					rv.Add($"New player! {member.GetName()}");
					continue;
				}
				if (member.LocalScore == 0)
					continue;
				var lastScore = 0;
				if (lastM != null)
					lastScore = lastM.LocalScore;
				if (member.LocalScore != lastScore)
					rv.Add($"{member.Name} Gained {member.LocalScore - lastScore}!");
            }
            return rv;
        }

		public string PointsLeftToday()
		{
			var max = Members.Count();
			int star1 = max;
			int star2 = max;
			foreach(var member in AllMembers(true))
			{
				var day = member.GetDay(ElfHelper.DayIndex);
				if (day == null)
					continue;
				if (day.ContainsKey("1"))
					star1--;
				if (day.ContainsKey("2"))
					star2--;
			}
			return $"{star1 + star2}({star1},{star2}) players:{max}";
		}

		public void CalcRank()
		{
			var allStars = new List<Dictionary<StarKey, StarLevel>>();
			foreach(var member in AllMembers(true))
				allStars.Add(member.AllStars());

			// starKeys done so far
			var starKeys = new List<StarKey>();
			foreach(var starDict in allStars)
			{
				foreach(var kvp in starDict)
				{
					if (!starKeys.Contains(kvp.Key))
						starKeys.Add(kvp.Key);
				}
			}

			// go through each and set rank base on everyone who completed
			foreach(var starKey in starKeys)
			{
				var starLevels = new List<StarLevel>();
				foreach(var allStar in allStars)
				{
					if (allStar.ContainsKey(starKey))
						starLevels.Add(allStar[starKey]);
				}
				int i = 1;
				foreach(var starLevel in starLevels.OrderBy(s => s.StarTime))
					starLevel.Rank = i++;
			}	
		}
	}
}
