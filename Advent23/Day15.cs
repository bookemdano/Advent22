using AoCLibrary;
namespace Advent23
{
	internal class Day15 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/15
		// Input https://adventofcode.com/2023/day/15/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (IsReal)
				check = new StarCheck(key, 497373L);
			else
				check = new StarCheck(key, 1320L);

			var lines = Program.GetText(check.Key).Replace(Environment.NewLine, "").Replace("\n", "");
			var rv = 0L;
			// magic
			var parts = lines.Split(',');
			foreach(var part in parts)
			{
				//var part = "HASH";
				var cv = Hash(part);
				rv += cv;
			}
			check.Compare(rv);
			// 497373
			// 497495 too high
			// 2449242 too high
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (IsReal)
				check = new StarCheck(key, 259356L);
			else
				check = new StarCheck(key, 145L);

			var lines = Program.GetText(check.Key).Replace(Environment.NewLine, "").Replace("\n", "");
			var rv = 0L;
			// magic
			var parts = lines.Split(',');
			var boxes = new Dictionary<int, List<Lens>>();
			foreach (var part in parts)
			{
				var splits = part.Split("=-".ToCharArray());

				var label = splits[0];
				var hash = Hash(label);

				if (!boxes.ContainsKey(hash))
					boxes.Add(hash, new List<Lens>());

				var oldLens = boxes[hash].FirstOrDefault(l => l.Label == label);

				if (part.Contains('-') && oldLens != null)
					boxes[hash].Remove(oldLens);
				else if (part.Contains('='))
				{
					var focus = int.Parse(splits[1]);
					if (oldLens == null)
						boxes[hash].Add(new Lens(label, focus));
					else if (oldLens != null)
						oldLens.Focus = focus;
				}
			}

			foreach(var box in boxes)
			{
				int slot = 1;
				foreach(var lens in box.Value)
				{
					rv += (box.Key + 1) * slot * lens.Focus;
					slot++;
				}
			}
			check.Compare(rv);
			//		rv	259356	long

			return rv;
		}
		Dictionary<string, int> _dictHash = [];

		int Hash(string str)
		{
			if (!_dictHash.ContainsKey(str))
			{
				int hash = 0;
				foreach (var c in str)
					hash = (hash + c) * 17 % 256;
				_dictHash[str] = hash;
			}
			return _dictHash[str];
		}
	}
	public class Lens
	{
		public Lens(string label, int focus)
		{
			Label = label;
			Focus = focus;
		}
		public override string ToString()
		{
			return $"{Label} {Focus}";
		}
		public string Label { get; }
		public int Focus { get; set; }
	}
}
