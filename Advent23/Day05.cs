using System.Diagnostics;
using AoCLibrary;
namespace Advent23
{
	internal class Day05 : IDayRunner
	{
		private object? Star1()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var seedParts = lines[0].Split(":");
			int i = 0;
            var seeds = seedParts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();
			//seeds = new List<long>() { 82 };
			i++;
            var map = "";
			while(i < lines.Length)
			{
				var line = lines[i];
				if (line.Contains("map"))
				{
                    map = line;
					var ranges = new RangeList();
					while (true)
					{
						i++;
						if (i >= lines.Length)
							break;
                        line = lines[i];
						if (line.Contains("map"))
							break;
						ranges.Add(new CRange(line));
					}
					var newSeeds = new List<long>();
					foreach(var seed in seeds)
				        newSeeds.Add(ranges.Convert(seed));
                	seeds = newSeeds;
				}
				else
					i++;
			}
			
			return seeds.Min();
		}
        
		public class RangeList : List<CRange>
		{
            public List<SeedRange> Convert(SeedRange seedRange)
            {
				var rv = new List<SeedRange>();
                foreach (var range in this)
                {
					// seeds are all to the left
					if (range.Srce > seedRange.SrceEnd)
                        continue;
					// seeds are all to the right
                    if (range.SrceEnd < seedRange.Srce)
                        continue;
                    // seeds start to the left, ends in the middle
                    if (seedRange.Srce < range.Srce && seedRange.SrceEnd < range.SrceEnd)
                    {
                        var leftoverLeft = SeedRange.FromEnd(seedRange.Srce, range.Srce - 1);
                        var inRange = SeedRange.FromEnd(range.Srce, seedRange.SrceEnd);
                        Utils.Assert(leftoverLeft.Range + inRange.Range, seedRange.Range);
                        rv.AddRange(Convert(leftoverLeft));
                        rv.Add(range.Convert(inRange));
                        continue;
                    }
                    // seeds start to the left, ends in the right
                    if (seedRange.Srce < range.Srce && seedRange.SrceEnd > range.SrceEnd)
                    {
                        var leftoverLeft = SeedRange.FromEnd(seedRange.Srce, end: range.Srce - 1);
                        var inRange = SeedRange.FromEnd(range.Srce, end: range.SrceEnd);
                        var leftoverRight = SeedRange.FromEnd(range.SrceEnd + 1, end: seedRange.SrceEnd);
                        Utils.Assert(leftoverLeft.Range + inRange.Range + leftoverRight.Range, seedRange.Range);
                        rv.AddRange(Convert(leftoverLeft));
                        rv.AddRange(Convert(leftoverRight));
                        rv.Add(range.Convert(inRange));
                        continue;
                    }
                    // seeds start in the middle, ends in the middle
                    if (seedRange.Srce >= range.Srce && seedRange.SrceEnd <= range.SrceEnd)
                    {
                        var inRange = SeedRange.FromEnd(seedRange.Srce, end: seedRange.SrceEnd);
                        Utils.Assert(inRange.Range, seedRange.Range);
                        rv.Add(range.Convert(inRange));
                        continue;
                    }
                    // seeds start in the middle, ends to the right
                    if (seedRange.Srce < range.SrceEnd && seedRange.SrceEnd > range.SrceEnd)
                    {
                        var leftover = SeedRange.FromEnd(range.SrceEnd + 1, end: seedRange.SrceEnd);
                        var inRange = SeedRange.FromEnd(seedRange.Srce, end: range.SrceEnd);
                        Utils.Assert(leftover.Range + inRange.Range, seedRange.Range);
                        rv.AddRange(Convert(leftover));
                        rv.Add(range.Convert(inRange));
                        continue;
                    }
                }
                if (!rv.Any())
                    rv.Add(seedRange);
                return rv;
            }
            public long Convert(long val)
            {
                foreach (var range in this)
                {
                    var delta = val - range.Srce;
                    if (delta >= 0 && delta < range.Range)
                        return range.Dest + delta;
                }
                return val;
            }
        }
        public class SeedRange
        {
            static public SeedRange FromEnd(long srce, long end)
            {
                return new SeedRange(srce, end - srce + 1);
            }
            static public SeedRange FromRange(long srce, long range)
            {
                return new SeedRange(srce, range);
            }
            SeedRange(long srce, long range)
            {
                Srce = srce;
                Range = range;
            }

            public long Srce { get; }
            public long Range { get; set; }
            public long SrceEnd
            {
                get
                {
                    return Srce + Range - 1;
                }
                set
                {
                    Range = value - Srce + 1;
                }
            }
            public override string ToString()
            {
                return $"{Srce}-{Srce + Range - 1} r:{Range}";
            }
            public void Combine(SeedRange other)
            {
                if (SrceEnd >= other.Srce)
                {
                    SrceEnd = other.SrceEnd;
                    other.Range = 0;
                }
            }
        }
        public class CRange
		{
			public CRange(long srce, long range)
			{
				Srce = srce;
				Range = range;
			}
			public CRange(string line)
			{
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Dest = long.Parse(parts[0]);
                Srce = long.Parse(parts[1]);
                Range = long.Parse(parts[2]);
            }
            public long Dest { get; }
			public long Srce { get; }
			public long Range { get; }
			public long SrceEnd
			{
				get
				{
					return Srce + Range - 1;
				}
			}
            public override string ToString()
            {
                return $"{Srce}-{Srce + Range - 1} r:{Range} to {Dest}";
            }

            internal SeedRange Convert(SeedRange range)
            {
                var delta = Dest - Srce;
                return SeedRange.FromRange(range.Srce + delta, range.Range);
            }
        }
		
		private object? Star2()
		{
            var lines = Program.GetLines(StarEnum.Star1, IsReal);
            var seedParts = lines[0].Split(":");
            int i = 0;
			var seeds = Utils.Split(' ', seedParts[1]).Select(s => long.Parse(s)).ToList();
			var seedRanges = new List<SeedRange>();
			for(int j = 0; j < seeds.Count(); j+=2)
				seedRanges.Add(SeedRange.FromRange(seeds[j], seeds[j + 1]));
            //seedRanges.Add(SeedRange.FromRange(79, 14));
			//seeds = new List<int>() { 14 };
            i++;
            var map = "";
            while (i < lines.Length)
            {
                var line = lines[i];
                if (line.Contains("map"))
                {
                    map = line;
                    var ranges = new RangeList();
                    while (true)
                    {
                        i++;
                        if (i >= lines.Length)
                            break;
                        line = lines[i];
                        if (line.Contains("map"))
                            break;
                        ranges.Add(new CRange(line));
                    }

                    var newSeeds = new List<SeedRange>();
                    foreach (var seed in seedRanges)
                        newSeeds.AddRange(ranges.Convert(seed));
                    SeedRange? lastNew = null;
                    newSeeds = newSeeds.OrderBy(s => s.Srce).ToList();
                    foreach (var newSeed in newSeeds)
                    {
                        if (lastNew != null)
                            lastNew.Combine(newSeed);
                        lastNew = newSeed;  
                    }
                    seedRanges = newSeeds.Where(s => s.Range > 0).ToList();
                }
                else
                    i++;
            }

            return seedRanges.Min(r => r.Srce);
        }

		public bool IsReal
		{
			get
			{
				return true;
			}
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
