using AoCLibrary;

namespace Advent24;

internal class Day14 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/14
	// Input https://adventofcode.com/2024/day/14/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 12L);
		else
			check = new StarCheck(key, 221655456L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var robots = new List<Robot>();
		foreach (var line in lines)
			robots.Add(new Robot(line));

		var size = new Point(robots.Max(r => r.Positon.X) + 1, robots.Max(r => r.Positon.Y) + 1);
		foreach(var robot in robots)
			robot.Move(100, size);

		var region = new Region14(new Point(0,0), size);
		var quads = region.Quadrants();
		rv = 1;
		foreach(var quad in quads)
		{
			rv *= robots.Count(r => quad.Contains(r.Positon));
		}


		check.Compare(rv);
		return rv;
	}
	public class Region14 : Region
	{
		public Region14(Point ul, Point br) : base(ul, br)
		{
		}
		public List<Region> Quadrants()
		{
			if (Utils.IsEven(Br.X) || Utils.IsEven(Br.Y))
				ElfHelper.DayLog("Even!");

			var mid = new Point(Br.X / 2 - 1, Br.Y / 2 - 1);
			var next = new Point(Br.X / 2 + 1, Br.Y / 2 + 1);

			var quads = new List<Region>();
			quads.Add(new Region(new Point(0, 0), mid.Copy()));
			quads.Add(new Region(new Point(next.X, 0), new Point(Br.X, mid.Y)));
			quads.Add(new Region(new Point(0, next.Y), new Point(mid.X, Br.Y)));
			quads.Add(new Region(new Point(next.X, next.Y), new Point(Br.X, Br.Y)));
			return quads;

		}
	}
	public class Robot
	{
		public Robot(string line)
		{
			var parts = Utils.SplitLongs("=, ".ToCharArray(), line);
			Positon = new Point(parts[0], parts[1]);
			Velocity = new Point(parts[2], parts[3]);
		}

		public Point Positon { get; set; }
		public Point Velocity { get; set; }

		internal void Move(int steps, Point size)
		{
			// changed after check
			var x = Positon.X;
			var y = Positon.Y;
			x += steps * Velocity.X;
			x = x % size.X;
			if (x < 0)
				x += size.X;
			y += steps * Velocity.Y;
			y = y % size.Y;
			if (y < 0)
				y += size.Y;
			Positon = new Point(x, y);
		}
		public override string ToString()
		{
			return Positon.ToString();
		}
		static public bool Print(List<Robot> robots, Point size)
		{
			var array = new List<char[]>();
			for (int y = 0; y < size.Y; y++)
				array.Add(new string(' ', (int) size.X).ToCharArray());
			foreach(var robot in robots)
				array[(int)robot.Positon.Y][robot.Positon.X] = 'x';

			var rv = false;
			var outs = new List<string>();
			foreach (var o in array)
			{
				var line = new string(o);
				if (line.Contains("xxxxxxx"))
					rv = true;
				outs.Add(line);
			}
			if (rv == true)
			{
				foreach (var line in outs)
					ElfHelper.DayLog(line);
			}
			//Console.WriteLine(o);
			return rv;
		}
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 0L);	//na
		else
			check = new StarCheck(key, 7858L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var robots = new List<Robot>();
		foreach (var line in lines)
			robots.Add(new Robot(line));

		var size = new Point(robots.Max(r => r.Positon.X) + 1, robots.Max(r => r.Positon.Y) + 1);
		var i = 1000;
		while(true)
		{
			robots = new List<Robot>();
			foreach (var line in lines)
				robots.Add(new Robot(line));

			foreach (var robot in robots)
				robot.Move(i, size);

			//Console.WriteLine(i);
			if (Robot.Print(robots, size))
			{
				ElfHelper.DayLog(i);
				rv = i;
				break;
			}
			//Thread.Sleep(500);

			if (i++ > 10000)
				break;
		}


		check.Compare(rv);
		return rv;
	}
}

