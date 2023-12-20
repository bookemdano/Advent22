using AoCLibrary;
namespace Advent23
{
	internal class Day08 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/8
		// Input https://adventofcode.com/2023/day/8/input
		public object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var dir = lines[0];
			var rows = new List<Row>();

			foreach (var line in lines.Skip(1))
				rows.Add(new Row(line));
			var found = false;
			var row = rows.First(r => r.Key == "AAA");
           
            int step = 0;
			while (!found)
			{ 
				foreach (var c in dir)
				{
					step++;
                    if (c == 'R')
                        row = rows.First(r => r.Key == row.Right);
                    else
                        row = rows.First(r => r.Key == row.Left);
					if (row.Key == "ZZZ")
						found = true;
                }
            }
			rv = step;
			if (!IsReal)
                Utils.Assert(rv, 6L);
			return rv;
            // too low 921
            // 18727

        }
		public object? Star2()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
            var dir = lines[0];
            var rows = new Dictionary<string, Row>();

            foreach (var line in lines.Skip(1))
            {
                var row = new Row(line);
                rows.Add(row.Key, row);
            }

            var paths = rows.Values.Where(r => r.GhostKey == 'A').ToArray();
            var pathLens = new List<long>();
            long step = 0;
            long loop = 0;
            while (paths.Length > 0)
            {
                loop++;
                foreach (var c in dir)
                {
                    step++;
					for (int i = 0; i < paths.Length; i++)
                    {
                        if (c == 'R')
                            paths[i] = rows[paths[i].Right];
                        else
                            paths[i] = rows[paths[i].Left];
                    }
                    var dones = paths.Where(p => p.GhostKey == 'Z');
                    foreach (var done in dones)
                    {
                        pathLens.Add(step);
                        var list = paths.ToList();
                        list.Remove(done);
                        paths = [.. list];
                    }
                }
            }
            rv = pathLens[0];
            foreach(var len in pathLens)
                rv = Misc.FindLCM(rv, len);
            if (!IsReal)
                Utils.Assert(rv, 6L);
            return rv;
        }

    }
    public class Row
	{
		public Row(string line)
		{
			var parts = Utils.Split('=', line);
			Key = parts[0];
			var lr = parts[1].Split("(), ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			Left = lr[0];
			Right = lr[1];
		}
        public override string ToString()
        {
            return $"k:{Key} = ({Left}, {Right})";
        }
        public char GhostKey => Key[2];
        public string Key { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
    }
}
