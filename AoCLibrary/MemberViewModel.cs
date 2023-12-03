namespace AoCLibrary
{
	public class MemberViewModel
	{
		Member _member;
		public int Place { get; }
		int _prevScore;
		public MemberViewModel(Member member, int place, int prevScore)
		{
			_member = member;
			Place = place;
			_prevScore = prevScore;
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
					var rv = ElfHelper.Fraction(_member.Stars / (days * 2.0), 8);
					if (_member.GetDay(ElfHelper.DayIndex())?.Stars() == 2)
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
					var avg = 23 - (double)_member.LocalScore / _member.Stars;
					rv = ElfHelper.Fraction(avg, 8);
				}
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

		List<DateTime> AllTimes()
		{
			List<DateTime> times = [];
			foreach (var day in _member.AllDays(hideNulls: true))
			{
				if (day?.Star1 != null)
					times.Add(day.Star1.StarTime);
				if (day?.Star2 != null)
					times.Add(day.Star2.StarTime);
			}
			return times.OrderByDescending(t => t).ToList();
		}

	}

}
