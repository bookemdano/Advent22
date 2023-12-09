using AoCLibrary;
namespace Advent23
{
	internal class Day09 : IDayRunner
	{
		public bool IsReal => true;

		// Day https://adventofcode.com/2023/day/9
		// Input https://adventofcode.com/2023/day/9/input
		public object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			foreach(var line in lines)
			{
				var nums = Utils.SplitNums(' ', line);
				var newLines = new List<long[]>();
				newLines.Add(nums);
				var last = nums.ToArray();
				while (true)
				{
					var deltas = new List<long>();
					for (int i = 0; i < last.Length - 1; i++)
					{
						deltas.Add(last[i + 1] - last[i]);
					}
					if (deltas.All(d => d == 0))
						break;
					last = deltas.ToArray();
					newLines.Add(last);
				}
				newLines.Reverse();
				var lastDelta = 0L;
				foreach (var newLine in newLines)
					lastDelta = newLine.Last() + lastDelta;
				rv += lastDelta;
			}
            if (!IsReal)
                Utils.Assert(rv, 114L);
			// 1953784198
			return rv;
		}
		public object? Star2()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
			foreach (var line in lines)
			{
				var nums = Utils.SplitNums(' ', line);
				var newLines = new List<long[]>();
				newLines.Add(nums);
				var last = nums.ToArray();
				while (true)
				{
					var deltas = new List<long>();
					for (int i = 0; i < last.Length - 1; i++)
					{
						deltas.Add(last[i + 1] - last[i]);
					}
					if (deltas.All(d => d == 0))
						break;
					last = deltas.ToArray();
					newLines.Add(last);
				}
				newLines.Reverse();
				var firstDelta = 0L;
				foreach (var newLine in newLines)
					firstDelta = newLine.First() - firstDelta;
				rv += firstDelta;
			}
			if (!IsReal)
                Utils.Assert(rv, 2L);
			//957
			return rv;
		}

	}
}
