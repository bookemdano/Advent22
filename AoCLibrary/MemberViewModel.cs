namespace AoCLibrary
{
	public class MemberViewModel
	{
		Member _member;
		public int Place { get; }
		int _prevScore;
		private int _maxScore;

		public MemberViewModel(Member member, int place, int prevScore, int maxScore)
		{
			_member = member;
			Place = place;
			_prevScore = prevScore;
			_maxScore = maxScore;
		}
		public string Name
		{
			get
			{
				return _member.GetName();
			}
		}
		public string Score
		{
			get
			{
				var delta = _member.LocalScore - _prevScore;
				if (delta == 0)
					return _member.LocalScore.ToString();
				else
					return $"{_member.LocalScore} ({delta})";
			}
		}
		public string Stars
		{
			get
			{
				var days = (DateTime.Today - new DateTime(ElfHelper.Year, 11, 30)).TotalDays;
				if (days > 25)
					days = 25;

				if (_member.Stars == days * 2)
					return "🌟";
				else
				{
					var rv = (_member.Stars - (days * 2.0)).ToString();
					if (_member.GetDay(ElfHelper.DayIndex)?.StarCount() == 2)
						rv += "*";
					return rv;
				}
			}
		}

		public string Average
		{
			get
			{
				var rv = "-";
				if (_member.Stars > 0)
				{
					var avg = (_maxScore + 1) - (double)_member.LocalScore / _member.Stars;
					rv = avg.ToString("0.00");
				}
				var firsts = _member.AllStars().Values.Count(s => s.Rank == 1);
				if (firsts > 0)
					rv += $" ({firsts})";
				return rv;
			}
		}
		public string Time
		{
			get
			{
				return ElfHelper.TimeString(_member.LastStarTime);
			}
		}

		public string Gap
		{
			get
			{
				var times = AllTimes();
				if (times.Count() < 2)
					return "";
				return ElfHelper.DeltaString(times[0] - times[1]);
			}
		}
		public string Places
		{
			get
			{
				var stars = _member.AllStars();
				if (!stars.Any())
					return "";
				var parts = new List<string>();
				var end = ElfHelper.DayIndex - 5;
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

		List<DateTime> AllTimes()
		{
			return _member.AllStars().Values.Select(s => s.StarTime).OrderByDescending(t => t).ToList();
		}

	}

}
