using AoCLibrary;
namespace Advent23
{
	internal class Day09 : IDayRunner
	{
		public bool IsReal => true;

		// Day https://adventofcode.com/2023/day/9
		// Input https://adventofcode.com/2023/day/9/input
		public long Star(StarEnum star)
		{
			long rv = 0;
			var lines = Program.GetLines(star, IsReal);
			foreach (var line in lines)
			{
				var nums = Utils.SplitNums(' ', line);
				var newLines = new List<long[]> { nums };
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
					last = [.. deltas];
					newLines.Add(last);
				}
				newLines.Reverse();
				var lastDelta = 0L;
				foreach (var newLine in newLines)
				{
					if (star == StarEnum.Star1)
						lastDelta = newLine.Last() + lastDelta;
					else
						lastDelta = newLine.First() - lastDelta;
				}
				rv += lastDelta;
			}
			return rv;
		}

		public object? Star1()
		{
			var rv = Star(StarEnum.Star1);
            if (!IsReal)
                Utils.Assert(rv, 114L);
			else
				Utils.Assert(rv, 1953784198L);
			// 1953784198
			return rv;
		}
		public object? Star2()
		{
			var rv = Star(StarEnum.Star2);
			if (!IsReal)
                Utils.Assert(rv, 2L);
			else
				Utils.Assert(rv, 957L);
			//957
			return rv;
		}

	}
}
