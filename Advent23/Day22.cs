using AoCLibrary;
namespace Advent23
{
	internal class Day22 : IDayRunner
	{
		public bool IsReal => true;

		// Day https://adventofcode.com/2023/day/22
		// Input https://adventofcode.com/2023/day/22/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 5L);
			else
				check = new StarCheck(key, 492L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var bricks = new Bricks22();
			var i = 0;
			foreach(var line in lines)
			{
				bricks.Add(new Brick22(i, line));
				i++;
			}
			ElfHelper.DayLog("Fall");
			bricks.Write("x", true);
			bricks.Write("y", false);
			var orderedBricks = bricks.OrderBy(b => b.MinZ).ToList();
			foreach (var brick in orderedBricks)
				bricks.Falling(brick);
			bricks.Write("x", true);
			bricks.Write("y", false);

			foreach(var brick in bricks)
			{
				var newSupportedBy = bricks.Supporting(brick);
				Utils.Assert(newSupportedBy?.Count()??0, brick.SupportedBy?.Count()??0);
				brick.SupportedBy = bricks.Supporting(brick);
				if (brick.SupportedBy == null)
					continue;
				if (brick.SupportedBy.Count() == 1)
					brick.SupportedBy.First().SoleSupporter = true;
			}
			//var demos = bricks.Where(b => b.SoleSupporter == 0).ToList();
			rv = bricks.Count(b => !b.SoleSupporter);
			ElfHelper.DayLog($"Total:{bricks.Count()} Sole:{bricks.Count(b => b.SoleSupporter)}  Multi:{bricks.Count(b => !b.SoleSupporter)}");

			// 506	too high
			// 452 too low
			check.Compare(rv);
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 7L);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var bricks = new Bricks22();
			var i = 0;
			foreach (var line in lines)
			{
				bricks.Add(new Brick22(i, line));
				i++;
			}
			ElfHelper.DayLog("Fall");
			bricks.Write("x", true);
			bricks.Write("y", false);
			var orderedBricks = bricks.OrderBy(b => b.MinZ).ToList();
			foreach (var brick in orderedBricks)
			{
				bricks.Falling(brick);
			}
			bricks.Write("x", true);
			bricks.Write("y", false);

			foreach (var brick in bricks)
			{
				brick.SupportedBy = bricks.Supporting(brick);
				if (brick.SupportedBy == null)
					continue;
				foreach (var brick2 in brick.SupportedBy)
				{
					brick2.Supports.Add(brick);
					if (brick.SupportedBy.Count() == 1)
						brick2.SoleSupporterFor.Add(brick);
				}
			}
			var soleSupporters = bricks.Where(b => b.SoleSupporterFor.Any()).ToList();
			
			foreach (var brick in soleSupporters)
			{
				bricks.ResetCrumble();
				bricks.Crumble(brick);
				var count = bricks.Crumbled.Count() - 1;	// -1 because "other bricks left" besides brick
				rv += count;
			}
			check.Compare(rv);
			// 128505	too high!
			// 86556
			// 100 too low

			return rv;
		}
	}
	class Bricks22 : List<Brick22>
	{

		public void Write(string tag, bool byX)
		{
			var minRow = 0;
			var maxRow = this.Max(b => b.MaxZ);
			var minCol = this.Min(b => b.MinX);
			var maxCol = this.Max(b => b.MaxX);
			if (byX == false)
			{
				minCol = this.Min(b => b.MinY);
				maxCol = this.Max(b => b.MaxY);
			}

			var lines = new List<string>();
			for (int row = maxRow; row >= minRow; row--)
			{
				var parts = new List<string>();
				for (int col = minCol; col <= maxCol; col++)
				{
					var foundBricks = this.Where(b => b.HasZ(row) && b.HasX(col));
					if (byX == false)
						foundBricks = this.Where(b => b.HasZ(row) && b.HasY(col));

					if (row == 0)
						parts.Add(" - ");
					else if (foundBricks.Count() > 1)
						parts.Add(" ? ");
					else if (foundBricks.Any())
						parts.Add(foundBricks.First().GetName());
					else
						parts.Add(" . ");
				}
				lines.Add(string.Join("", parts));
			}
			ElfUtils.WriteLines("Bricks", tag, lines);
		}

		internal List<Brick22>? Supporting(Brick22 brick)
		{
			if (brick.MinZ <= 1)
				return null;
			var newBrick = brick.NewFall();
			var rv= this.Where(b => b.Overlaps(newBrick) == true).ToList();
			if (!rv.Any())
				return null;
			return rv;
		}
		internal bool Falling(Brick22 brick)
		{
			if (brick.Id == 1097)
				ElfHelper.DayLog(brick);
			if (brick.MinZ <= 1)
				return false;
			var minX = brick.MinX;
			var maxX = brick.MaxX;
			var minY = brick.MinY;
			var maxY = brick.MaxY;
			var minZ = brick.MinZ;
			var nextZ = 0;
			List<Brick22>? supportedBy = null;
			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					var competitors = this.Where(b => (b.Id != brick.Id) && b.HasX(x) && b.HasY(y) && (b.MaxZ <= minZ));
					if (competitors.Any())
					{
						var spotZ = competitors.Max(b => b.MaxZ);
						if (spotZ > nextZ)
						{
							supportedBy = competitors.Where(b => b.MaxZ == spotZ).ToList();
							nextZ = spotZ;
						}
						else if (spotZ == nextZ)
						{
							supportedBy.AddRange(competitors.Where(b => b.MaxZ == spotZ));
						}
					}
				}
			}
			var delta = minZ - nextZ - 1;
			if (supportedBy != null)
				brick.SupportedBy = supportedBy.DistinctBy(b => b.Id).ToList(); // inaccurate for some reason
			if (delta <= 0)
				return false;
			brick.MoveFall(delta);
			return true;

			/* old
					while (true)
			{
				var newBrick = brick.NewFall();
				if (newBrick.MinZ <= 1)
					break;
				
				var wouldOverlap = this.Where(b => b.Overlaps(newBrick) == true).ToList();
				if (!this.Any(b => b.Overlaps(newBrick) == true))
				{
					brick.MoveFall();
					fell = true;
				}
				else
				{
					brick.SupportedBy = wouldOverlap;
					foreach (var overlap in wouldOverlap)
						overlap.Supports.Add(brick);
					break;

				}
			}
			return fell;
			*/
		}

		internal int CrumbleTrain(Brick22 brick)
		{
			var rv = 1;
			var crumbleTrain = new List<Brick22>();
			foreach(var supports in brick.SoleSupporterFor)
				rv += CrumbleTrain(supports);
			return rv;
		}
		internal void ResetCrumble()
		{
			Crumbled.Clear();
		}

		public List<Brick22> Crumbled { get; set; } = [];

		internal void Crumble(Brick22 brick)
		{
			if (!Crumbled.Any(b => b.Id == brick.Id))
				Crumbled.Add(brick);
			foreach (var supports in brick.Supports)
			{
				if (!AreSupportsLeft(supports))
					Crumble(supports);
			}
		}
		internal bool AreSupportsLeft(Brick22 brick)
		{
			if (brick.SupportedBy == null)
				return false;
			var supportCount = brick.SupportedBy.Count();
			foreach (var supportedBy in brick.SupportedBy)
			{
				if (Crumbled.Any(b => b.Id == supportedBy.Id))
					supportCount--;
			}
			Utils.Assert(supportCount >= 0, "SupportCount");
			return supportCount > 0;
		}

	}
	class Brick22
	{
		public int Id { get; }
		public Point3D End1 { get; set; }
		public Point3D End2 { get; set; }

		public Brick22(int i, Point3D end1, Point3D end2)
		{
			Id = i;
			End1 = end1;
			End2 = end2;
		}
		public Brick22(int id, string line)
		{
			var parts = Utils.Split('~', line);
			Id = id;
			End1 = Point3D.FromXYZ(parts[0]);
			End2 = Point3D.FromXYZ(parts[1]);
		}
		public override string ToString()
		{
			return $"{GetName()}({Id}) {End1}-{End2}";
		}

		internal int MinX => Math.Min(End1.X, End2.X);
		internal int MaxX => Math.Max(End1.X, End2.X);
		internal int MinY => Math.Min(End1.Y, End2.Y);
		internal int MaxY => Math.Max(End1.Y, End2.Y);
		internal int MinZ => Math.Min(End1.Z, End2.Z);
		internal int MaxZ => Math.Max(End1.Z, End2.Z);

		//public IEnumerable<Brick22> SupportedBy { get; internal set; }
		//public List<Brick22> Supports { get; internal set; } = [];
		public bool SoleSupporter { get; internal set; }
		public List<Brick22>? SupportedBy { get; internal set; }
		public List<Brick22> Supports { get; internal set; } = [];
		public List<Brick22> SoleSupporterFor { get; internal set; } = [];

		internal bool HasX(int x)
		{
			return (x >= MinX && x <= MaxX);
		}
		internal bool HasY(int y)
		{
			return (y >= MinY && y <= MaxY);
		}
		internal bool HasZ(int z)
		{
			return (z >= MinZ && z <= MaxZ);
		}

		internal Brick22 NewFall()
		{
			var end1 = new Point3D(End1.X, End1.Y, End1.Z - 1);
			var end2 = new Point3D(End2.X, End2.Y, End2.Z - 1);
			return new Brick22(Id, end1, end2);
		}

		internal bool? Overlaps(Brick22 testBrick)
		{
			//if (Id == 847)
			//	ElfHelper.DayLog("Overlaps " + this);

			if (testBrick.Id == Id)
				return null;
			var xOverlap = testBrick.HasX(MinX) || testBrick.HasX(MaxX) || HasX(testBrick.MinX) || HasX(testBrick.MaxX);
			var yOverlap = testBrick.HasY(MinY) || testBrick.HasY(MaxY) || HasY(testBrick.MinY) || HasY(testBrick.MaxY);
			var zOverlap = testBrick.HasZ(MinZ) || testBrick.HasZ(MaxZ) || HasZ(testBrick.MinZ) || HasZ(testBrick.MaxZ);
			return xOverlap && yOverlap && zOverlap;

		}
		bool _moved;
		internal void MoveFall(int delta)
		{
			_moved = true;
			End1.Z -= delta;
			End2.Z -= delta;
		}

		internal string GetName()
		{
			if (Id < 26)
				return " " + (char) ('A' + Id) + " ";

			var rv = Id.ToString("x");
			if (rv.Length == 1)
				return " " + rv + " ";
			else if (rv.Length == 2)
				return " " + rv;
			else
				return rv;
		}

	}
}
