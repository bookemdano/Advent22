namespace AoCLibrary
{
	public class ElfResult
    {
		public MemberGroup Members { get; set; } = new MemberGroup();

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
		// return null for no changes
		public List<string> HasChanges(ElfResult? last)
        {
			var rv = new List<string>();
			var allMembers = AllMembers();
            var lastAllMembers = last?.AllMembers();
			if (lastAllMembers == null)
				return rv;

			foreach (var member in allMembers)
			{
				var lastM = lastAllMembers.FirstOrDefault(m => m.Name == member.Name);
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
			int star1 = ElfHelper.MaxScore();
			int star2 = ElfHelper.MaxScore();
			foreach(var member in Members.AllMembers(true))
			{
				var day = member.GetDay(ElfHelper.DayIndex());
				if (day?.Star1 != null)
					star1--;
				if (day?.Star2 != null)
					star2--;
			}
			return $"Star1: {star1} Star2: {star2}";
		}
	}
}
