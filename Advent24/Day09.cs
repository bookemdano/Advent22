using AoCLibrary;

namespace Advent24;

internal class Day09 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/9
	// Input https://adventofcode.com/2024/day/9/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 1928L);
		else
			check = new StarCheck(key, 6301895872542L);

		var text = Program.GetText(check.Key).Replace(Environment.NewLine, "");
		var rv = 0L;
		// magic
		var disk = new Disk(text);
		while(true)
		{
			if (!disk.Move())
				break;
		}
		rv = disk.Checksum();

		check.Compare(rv);
		return rv;
	}

	public class Free
	{
		public Free(int start, int len)
		{
			Start = start;
			Len = len;
		}
		public override string ToString()
		{
			return $"{Start},{Len}";
		}
		public int Start { get; set; }
		public int Len { get; set; }
	}
	public class FInfo : Free
	{
		public FInfo(int id, int start, int len) : base(start, len)
		{
			Id = id;
		}
		public int Id { get; set; }
		public override string ToString()
		{
			return $"{Id}-{base.ToString()}";
		}

		internal long CheckSum()
		{
			var rv = 0L;
			for (int i = 0; i < Len; i++)
				rv += (Start + i) * Id;
			return rv;
		}
	}
	public class Disk
	{
		public Disk(string text)
		{
			int iFile = 0;
			int iChar = 0;
			for (int i = 0; i < text.Length; i++)
			{
				var n1 = int.Parse(text[i].ToString());
				if (n1 > 0)
					Files.Add(new FInfo(iFile++, iChar, n1));
				iChar += n1;
				if (i == text.Length - 1)
					break;
				i++;
				if (int.TryParse(text[i].ToString(), out var n2))
				{
					if (n2 > 0)
						Frees.Add(new Free(iChar, n2));
					iChar += n2;
				}
			}
		}
		public List<Free> Frees { get; set; } = [];
		public List<FInfo> Files { get; set; } = [];

		internal long Checksum()
		{
			var rv = 0L;
			foreach(var file in Files)
			{
				rv += file.CheckSum();
			}
			return rv;
		}

		internal bool Move()
		{
			var lastFile = Files.Last();
			var firstFree = Frees.First();
			if (lastFile.Start < firstFree.Start)
				return false;

			var newLoc = firstFree.Start;
			var prevFile = Files.Where(f => f.Start < firstFree.Start).Last();
			if (prevFile.Id == lastFile.Id)
			{
				prevFile.Len++;
			}	
			else
			{
				Files.Add(new FInfo(lastFile.Id, newLoc, 1));
			}
			firstFree.Start++;
			firstFree.Len--;

			lastFile.Len--;


			// clean up empties
			if (firstFree.Len == 0)
				Frees.Remove(firstFree);
			if (lastFile.Len == 0)
				Files.Remove(lastFile);

			Files = Files.OrderBy(f => f.Start).ToList();
			Frees = Frees.OrderBy(f => f.Start).ToList();
			return true;
		}
	}

	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		var rv = 0L;
		// magic

		check.Compare(rv);
		return rv;
	}
}

