using AoCLibrary;
namespace Advent23
{
	internal class Day10 : IDayRunner
	{
		public bool IsReal => true;
		// Day https://adventofcode.com/2023/day/10
		// Input https://adventofcode.com/2023/day/10/input
		public object? Star1()
		{
			var rv = 0L;
			var checks = new List<StarCheck>();
			if (IsReal)
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal), 7063));
			else
			{
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal, 0), 4));
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal, 1), 8));
			}
			foreach(var check in checks)
			{
				var lines = Program.GetLines(check.Key);
				var grd = Grid10.FromLines(lines);
				var starts = grd.FindStarts().ToArray();
				int step = 0;

				while (starts.Count() > 0)
				{
					step++;
					var newStarts = new List<Node10>();
					for (int i = 0; i < 2; i++)
					{
						var start = starts[i];
						start.Count = step;
						var cons = grd.Connections(start);
						Utils.Assert(cons.Count() < 2, "Less than 2");
						if (cons.Any())
							newStarts.Add(cons.First());
					}
					starts = newStarts.ToArray();
				}
				rv = step;
				check.Compare(rv);
			}
			return rv;
            // 7063
		}
		public object? Star2()
		{
			var rv = 0L;

			var checks = new List<StarCheck>();
			if (IsReal)
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star2, IsReal), 589));
			else
			{
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star2, IsReal, 0), 4));
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star2, IsReal, 1), 8));
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star2, IsReal, 2), 10));
			}

			foreach(var check in checks)
			{
				var lines = Program.GetLines(check.Key);
				var grdOrig = Grid10.FromLines(lines);
				int step = 0;
				var grd = grdOrig.AddSpaces();
				grd.WriteCounts("add");
				var starts = grd.FindStarts().ToArray();

				while (starts.Count() > 0)
				{
					step++;
					Utils.TestLog($"{step} Starts:{starts.Count()}");
					var newStarts = new List<Node10>();
					foreach (var start in starts)
					{
						start.Count = step;
						var cons = grd.Connections(start);
						Utils.Assert(cons.Count() < 2, "Less than 2");
						if (cons.Any())
							newStarts.Add(cons.First());
					}
					starts = newStarts.ToArray();
				}
				grd.WriteCounts(false, "pre");


				rv = grd.Insides();
				// now shrink
				grd.WriteCounts(false, "done");
				check.Compare(rv);
			}
			return rv;
			// 873 too high
			// 589

		}

	}

	public class Node10 : Node
	{
		public Node10(Point pt, char c) : base(pt, c)
		{
			ConnPts = Connections();
			StartTag = false;
			Og = false;
		}
		public Node10(Point pt, char c, bool startTag, bool og) : base(pt, c)
		{
			ConnPts = Connections();
			StartTag = startTag;
			Og = og;
		}
		public int? Count { get; set; } = null;
        public List<Point> ConnPts { get; private set; }
        public bool? Escape { get; internal set; }
		public bool StartTag { get; private set; }
		public bool Og { get; private set; }

		public override string ToString()
        {
            return $"{base.ToString()} {Count} e:{Escape} ({string.Join("-", ConnPts.ToList())})";
        }

        List<Point> Connections()
        {
            var rv = new List<Point>();
            if (South())
                rv.Add(new(Pt.Row + 1, Pt.Col));
            if (North())
                rv.Add(new(Pt.Row - 1, Pt.Col));
            if (East())
                rv.Add(new(Pt.Row, Pt.Col + 1));
            if (West())
                rv.Add(new(Pt.Row, Pt.Col - 1));
    
            return rv;
        }
		internal bool North()
        {
            return (Char == '|' || Char == 'L' || Char == 'J');
        }
		internal bool South()
        {
            return (Char == '|' || Char == '7' || Char == 'F');
        }
        internal bool East()
        {
            return (Char == '-' || Char == 'L' || Char == 'F');
        }
		internal bool West()
        {
            return (Char == '-' || Char == '7' || Char == 'J');
        }
        public bool ConnectsTo(Point pt)
        {
            return ConnPts.Contains(pt);

        }
        internal void SetStart(List<Node10> vals)
        {
			if (!StartTag)
			{
				StartTag = true;
				var pt1 = vals[0];
				var pt2 = vals[1];
				if ((Pt.Row == pt1.Pt.Row + 1 && Pt.Row == pt2.Pt.Row - 1) || (Pt.Row == pt2.Pt.Row + 1 && Pt.Row == pt1.Pt.Row - 1))
					Char = '-';
				else if ((Pt.Col == pt1.Pt.Col + 1 && Pt.Col == pt2.Pt.Col - 1) || (Pt.Col == pt2.Pt.Col + 1 && Pt.Col == pt1.Pt.Col - 1))
					Char = '|';
				else if ((Pt.Col == pt1.Pt.Col - 1 && Pt.Row == pt2.Pt.Row + 1) || (Pt.Col == pt2.Pt.Col - 1 && Pt.Row == pt1.Pt.Row + 1))
					Char = 'L';
				else if ((Pt.Col == pt1.Pt.Col - 1 && Pt.Row == pt2.Pt.Row - 1) || (Pt.Col == pt2.Pt.Col - 1 && Pt.Row == pt1.Pt.Row - 1))
					Char = 'F';
				else if ((Pt.Col == pt1.Pt.Col + 1 && Pt.Row == pt2.Pt.Row + 1) || (Pt.Col == pt2.Pt.Col + 1 && Pt.Row == pt1.Pt.Row + 1))
					Char = 'J';
				else if ((Pt.Col == pt1.Pt.Col + 1 && Pt.Row == pt2.Pt.Row - 1) || (Pt.Col == pt2.Pt.Col + 1 && Pt.Row == pt1.Pt.Row - 1))
					Char = '7';
				ConnPts = vals.Select(v => v.Pt).ToList();
			}
			Count = 0;
        }

        internal bool Interconnected(Node10 other)
        {
            return (ConnectsTo(other.Pt) && other.ConnectsTo(Pt));
        }
        internal bool IsWall()
        {
            return Count != null;
        }
        internal void ResetChar()
        {
            if (Char != '.')
                Char = '_';
            ConnPts.Clear();
        }
    }
	public class Grid10 : Grid<Node10>
	{
		public Grid10(List<Node10> nodes)
		{
			Init(nodes);
		}
		internal static Grid10 FromLines(string[] lines)
		{
			return new Grid10(GetNodes(lines));
		}

		public Grid10 AddSpaces()
		{
			FindStarts();	// renames start to a wall
			var newNodes = new List<Node10>();
			var newRow = 0;
			// add cols
			for (int row = 0; row < _rows; row++)
			{
				var newCol = 0;
				for (int col = 0; col < _cols; col++)
				{
					var node = Find(new Point(row, col))!;
					newNodes.Add(new Node10(new Point(newRow, newCol), node.Char, node.StartTag, og: true));
					// add col
					if (node.Char == '-' || node.Char == 'L' || node.Char == 'F')
						newNodes.Add(new Node10(new Point(newRow, newCol + 1), '-'));
					else //if (node.Char == '.' || node.Char == '|' || node.Char == 'J' || node.Char == '7')
						newNodes.Add(new Node10(new Point(newRow, newCol + 1), '.'));

					if (node.Char == '|' || node.Char == 'F' || node.Char == '7')
						newNodes.Add(new Node10(new Point(newRow + 1, newCol), '|'));
					else //if (node.Char == '.' || node.Char == '|' || node.Char == 'J' || node.Char == '7')
						newNodes.Add(new Node10(new Point(newRow + 1, newCol), '.'));

					newNodes.Add(new Node10(new Point(newRow + 1, newCol + 1), '.'));
					newCol += 2;
				}
				newRow += 2;
			}

			return new Grid10(newNodes.OrderBy(n => n.Pt.GetHashCode()).ToList());
		}
		internal List<Node10> Connections(Node10 gv)
        {
            var pts = gv.ConnPts;
            if (!pts.Any())
                return new List<Node10>();
			List<Node10> nodes = pts.Select(p => Find(p)).Where(p => p != null).ToList();
            return nodes.Where(n => n.Count == null).ToList();
        }
   
        bool? CanEscape(Node10 node, List<Point> path)
        {
			if (DateTime.Now > _nextLog)
			{
				WriteCounts(false, "can");
				_nextLog = DateTime.Now.AddSeconds(1);
			}
			if (node.Escape != null)
                return node.Escape.Value;
			if (node.IsWall())
			{
				node.Escape = false;
				return false;
			}
			else if (node.Pt.Row == 0 || node.Pt.Col == 0 || node.Pt.Row == _rows - 1 || node.Pt.Col == _cols - 1)
			{
				node.Escape = true;
				return true;
			}

			if (path.Count() > 5)
				return null;    // give up for now and try later

			List<Node10> neighbors = node.Neighbors().Select(n => Find(n)).Where(n => n != null).ToList();
            if (neighbors.Any(n => n.Escape == true))
            {
                node.Escape = true;
                return true;
            }
			bool all = true;
            foreach (var neighbor in neighbors)
            {
				if (path.Any(x => x == neighbor.Pt))
				{
					all = false;
					continue;
				}
				var newPath = new List<Point>(path);
				newPath.Add(node.Pt);
				var res = CanEscape(neighbor, newPath);
				if (res == null)
					all = false;
				else if (res == true)
                {
                    node.Escape = true;
                    break;
                }
            }
            if (all && node.Escape == null)
                node.Escape = false;
            return node.Escape;
        }
		DateTime _nextLog = DateTime.MinValue;
        public int Insides()
        {
            foreach (var node in Values)
            {
                if (!node.IsWall())
                    node.ResetChar();
            }
			//NearestEscape();
			CalcEscape();

			return Values.Count(n => n.Escape == false && n.IsWall() == false && n.Og == true);
        }

		private long NearestEscape()
		{
			// slow but works
			var count = 0;
			while (count != Values.Count(n => n.Escape == null))
			{
				count = Values.Count(n => n.Escape == null);
				foreach (var node in Values.Where(n => n.Escape == null))
					CanEscape(node, new List<Point>());
				WriteCounts(false, "can");

			}
			return Values.Count(n => n.Escape == null && n.Og == true);
		}

		private void CalcEscape()
		{
			foreach (var kvp in this)
				kvp.Value.Escape = null;

			var last = 0;
			while (Values.Count(n => n.Escape == null) != last)
			{
				Utils.TestLog("Remaining " + Values.Count(n => n.Escape == null));
				WriteCounts(false, "calc");
				last = Values.Count(n => n.Escape == null);
				foreach (var kvp in this.Where(kvp => kvp.Value.Escape == null))
				{
					var node = kvp.Value;
					if (node.IsWall())
						node.Escape = false;
					else if (node.Pt.Row == 0 || node.Pt.Col == 0 || node.Pt.Row == _rows - 1 || node.Pt.Col == _cols - 1)
						node.Escape = true;
					else
					{
						var ns = node.Neighbors().Select(n => Find(n)).Where(n => n != null).ToList();
						if (ns.Any(n => n?.Escape == true))
							node.Escape = true;
						else if (!ns.Any(n => n?.Escape == null))
							node.Escape = false;
					}
				}
			}
			Values.Where(n => n.Escape == null).ToList().ForEach(n => n.Escape = false);
			WriteCounts(false, "calc");
		}

		public void WriteCounts(bool raw, string tag)
        {
			if (raw)
				WriteCounts(tag);
            var lines = new List<string>();
            for (int row = 0; row < _rows; row++)
            {
                var parts = new List<string>();
                for (int col = 0; col < _cols; col++)
                {
                    var v = Find(new Point(row, col))!;
					if (raw)
						parts.Add(v.Char.ToString());
					else
					{
						if (v.IsWall())
							parts.Add(v.Char.ToString());
							//parts.Add(v.Count.Value.ToString());
						else if (v.Escape == false)
							parts.Add("I");
						else if (v.Escape == true)
							parts.Add("0");
						else 
							parts.Add(" ");
					}
				}
                lines.Add(string.Join(",", parts));
            }
            File.WriteAllLines(Path.Combine(Utils.Dir, $"counts{ElfHelper.DayString()}{raw}{tag}.csv"), lines);
        }
        public List<Node10> FindStarts()
        {
			var start = Values.FirstOrDefault(v => v.StartTag);
			if (start == null)
				start = Values.First(v => v.Char == 'S');

			var vals = Values.Where(v => v.ConnPts.Any(c => c.Equals(start.Pt))).ToList();
            start.SetStart(vals);
            return vals;
        }
    }
}
