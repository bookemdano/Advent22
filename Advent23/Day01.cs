namespace Advent23
{
	internal class Day01 : IDayRunner
	{
		public object Run(string[] lines)
		{
			var total = 0;
			foreach (var line in lines)
			{
				var f = line.First(c => char.IsDigit(c)).ToString();
				var l = line.Last(c => char.IsDigit(c)).ToString();
				total += int.Parse(f + l);
			}
			return total;
		}
	}
}
