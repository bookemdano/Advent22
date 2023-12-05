namespace Advent23
{
	internal class Day06 : IDayRunner
	{
		public bool IsReal => return false;

		private object? Star1()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			return rv;
		}
		private object? Star2()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
			return rv;
		}
		public RunnerResult Run()
		{
			var rv = new RunnerResult();
			rv.Star1 = Star1();
			rv.Star2 = Star2();
			return rv;
		}
	}
}
