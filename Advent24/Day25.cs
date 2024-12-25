using AoCLibrary;
namespace Advent24;

internal class Day25 : IDayRunner
{
	public bool IsReal => false;

	// Day https://adventofcode.com/2024/day/25
	// Input https://adventofcode.com/2024/day/25/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 3L);
		else
			check = new StarCheck(key, 3057L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		
		// magic
		var block = new List<string>();
		var keyLocks = new List<KeyLock25>();
		foreach(var line in lines)
		{
			if (string.IsNullOrEmpty(line))
			{
				keyLocks.Add(new KeyLock25(block));
				block = [];
			}
			else
				block.Add(line);
		}
        keyLocks.Add(new KeyLock25(block));

        foreach (var iLock in keyLocks.Where(k => k.KeyLock == KeyLock25Enum.Lock))
        {
			foreach (var iKey in keyLocks.Where(k => k.KeyLock == KeyLock25Enum.Key))
			{
				if (iLock.Fits(iKey))
				{
					Console.WriteLine($"{iLock} fits {iKey}");
					rv++;
				}
			}
        }

        check.Compare(rv);
		return rv;
	}
	public enum KeyLock25Enum
	{
		Key,
		Lock
	}
	public class KeyLock25
	{
        public KeyLock25Enum KeyLock { get; }
		public List<int> Heights { get; } = [];
		public int _depth;
        public KeyLock25(List<string> block)
        {
			_depth = block.Count - 1;
            KeyLock = block[0] == new string('#', 5) ? KeyLock25Enum.Lock : KeyLock25Enum.Key;
			var cols = 5;
            for (var iCol = 0; iCol < cols; iCol++)
            {
				var h = 0;
				foreach (var l in block)
				{
                    if (l[iCol] == '#')
                        h++;
                }
                Heights.Add(h - 1);
            }

        }
        public override string ToString()
        {
            return $"{KeyLock} {string.Join(',', Heights)}";
        }

        internal bool Fits(KeyLock25 other)
        {
			for (var i = 0; i < Heights.Count; i++)
				if (Heights[i] + other.Heights[i] >= _depth)
					return false;
			return true;
        }
    }
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 0L);
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic

		check.Compare(rv);
		return rv;
	}
}

