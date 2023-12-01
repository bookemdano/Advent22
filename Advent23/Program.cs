namespace Advent23
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var lines = File.ReadAllLines("day01.txt");
			var total = 0;
			foreach(var line in lines)
			{
				var f = line.First(c => char.IsDigit(c)).ToString();
				var l = line.Last(c => char.IsDigit(c)).ToString();
				total += int.Parse(f + l);
			}
			Console.WriteLine(total);
		}
	}
}
