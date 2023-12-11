using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using AoCLibrary;
using Microsoft.Win32;
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
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var grd = Grid.FromLines(lines);
			var starts = grd.FindStarts().ToArray();
            int step = 0;

            while (starts.Count() > 0)
			{
                step++;
                var newStarts = new List<Node>();
                for(int i = 0; i < 2; i++)
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
            if (!IsReal)
                Utils.Assert(rv, 8L);
			return rv;
            // 7063
		}
		public object? Star2()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
			var grdOrig = Grid.FromLines(lines);
			int step = 0;
			var grd = grdOrig.AddSpaces();
			grd.WriteCounts(true);
			var starts = grd.FindStarts().ToArray();

			while (starts.Count() > 0)
            {
                step++;
				Utils.TestLog($"{step} Starts:{starts.Count()}");
                var newStarts = new List<Node>();
                foreach(var start in starts)
                {
                    start.Count = step;
                    var cons = grd.Connections(start);
                    Utils.Assert(cons.Count() < 2, "Less than 2");
                    if (cons.Any())
                        newStarts.Add(cons.First());
                }
                starts = newStarts.ToArray();
            }
			grd.WriteCounts(false);


			rv = grd.Insides();
			// now shrink
            grd.WriteCounts(false);
            if (!IsReal)
                Utils.Assert(rv, 10L);
			return rv;
			// 873 too high
			// 589

		}

	}
	public class Point
	{
		public Point(int row, int col)
		{
			Row = row;
			Col = col;
		}
        public override int GetHashCode()
        {
            return Row * 100000 + Col;
        }
        public override bool Equals(object? obj)
        {
			if (obj is not Point other)
				return false;
			return (other.Row == Row && other.Col == Col);
        }
        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
        public int Row { get; }
		public int Col { get; }
	}
	public class Node
	{
		public Node(int row, int col, char c, bool startTag = false, bool og = false)
        {
            Pt = new Point(row, col);
            Char = c;
            ConnPts = Connections();
			StartTag = startTag;
			Og = og;
        }

        public Point Pt { get; }
        public char Char { get; private set; }
        public int? Count { get; set; } = null;
        public List<Point> ConnPts { get; private set; }
        public bool? Escape { get; internal set; }
		public bool StartTag { get; private set; }
		public bool Og { get; private set; }

		public override string ToString()
        {
            return $"{Pt} '{Char}' {Count} e:{Escape} ({string.Join("-", ConnPts.ToList())})";
        }
        internal List<Point> Neighbors()
        {
            var rv = new List<Point>();
            rv.Add(new(Pt.Row + 1, Pt.Col));
            rv.Add(new(Pt.Row - 1, Pt.Col));
            rv.Add(new(Pt.Row, Pt.Col + 1));
            rv.Add(new(Pt.Row, Pt.Col - 1));

            return rv;
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
        internal void SetStart(List<Node> vals)
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

        internal bool Interconnected(Node other)
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
	public class Grid
	{
		Dictionary<Point, Node>  _dict = [];
        private int _rows;
        private int _cols;

		static public Grid FromLines(string[] lines)
		{
			int iRow = 0;
			var nodes = new List<Node>();
			foreach (var line in lines)
			{
				int iCol = 0;
				foreach (var c in line)
					nodes.Add(new Node(iRow, iCol++, c));
                iRow++;
            }
			return new Grid(nodes);
		}

		public Grid(List<Node> nodes)
		{
			foreach(var node in nodes)
				_dict.Add(node.Pt, node);
			_rows = nodes.Max(n => n.Pt.Row) + 1;
			_cols = nodes.Max(n => n.Pt.Col) + 1;
		}

		public Grid AddSpaces()
		{
			FindStarts();	// renames start to a wall
			var newNodes = new List<Node>();
			var newRow = 0;
			// add cols
			for (int row = 0; row < _rows; row++)
			{
				var newCol = 0;
				for (int col = 0; col < _cols; col++)
				{
					var node = Find(new Point(row, col))!;
					newNodes.Add(new Node(newRow, newCol, node.Char, node.StartTag, og: true));
					// add col
					if (node.Char == '-' || node.Char == 'L' || node.Char == 'F')
						newNodes.Add(new Node(newRow, newCol + 1, '-'));
					else //if (node.Char == '.' || node.Char == '|' || node.Char == 'J' || node.Char == '7')
						newNodes.Add(new Node(newRow, newCol + 1, '.'));

					if (node.Char == '|' || node.Char == 'F' || node.Char == '7')
						newNodes.Add(new Node(newRow + 1, newCol, '|'));
					else //if (node.Char == '.' || node.Char == '|' || node.Char == 'J' || node.Char == '7')
						newNodes.Add(new Node(newRow + 1, newCol, '.'));

					newNodes.Add(new Node(newRow + 1, newCol + 1, '.'));
					newCol += 2;
				}
				newRow += 2;
			}

			return new Grid(newNodes.OrderBy(n => n.Pt.GetHashCode()).ToList());
		}
		internal List<Node> Connections(Node gv)
        {
            var pts = gv.ConnPts;
            if (!pts.Any())
                return new List<Node>();
			List<Node> nodes = pts.Select(p => Find(p)).Where(p => p != null).ToList();
            return nodes.Where(n => n.Count == null).ToList();
        }
        Node? Find(Point pt)
        {
			if (!_dict.TryGetValue(pt, out Node? value))
				return null;
            return value;
        }
        bool CanEscape(Node node)
        {
			if (node.Pt.Row == 1 && node.Pt.Col == 3)
				node.Escape = node.Escape;

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

            List<Node> neighbors = node.Neighbors().Select(n => Find(n)).Where(n => n != null).ToList();
            if (neighbors.Any(n => n.Escape == true))
            {
                node.Escape = true;
                return true;
            }
            foreach (var neighbor in neighbors)
            {
                if (CanEscape(neighbor))
                {
                    node.Escape = true;
                    break;
                }
            }
            if (node.Escape == null)
                node.Escape = false;
            return node.Escape.Value;
        }
        public int Insides()
        {
            foreach (var node in _dict.Values)
            {
                if (!node.IsWall())
                    node.ResetChar();
            }
			
			CalcEscape();
			return _dict.Values.Count(n => n.Escape == false && n.IsWall() == false && n.Og == true);
        }

		private void CalcEscape()
		{
			foreach (var kvp in _dict)
				kvp.Value.Escape = null;

			var last = 0;
			while (_dict.Values.Count(n => n.Escape == null) != last)
			{
				Utils.TestLog("Remaining " + _dict.Values.Count(n => n.Escape == null));
				WriteCounts(false);
				last = _dict.Values.Count(n => n.Escape == null);
				foreach (var kvp in _dict.Where(kvp => kvp.Value.Escape == null))
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
			_dict.Values.Where(n => n.Escape == null).ToList().ForEach(n => n.Escape = false);
			WriteCounts(false);
		}

		public void WriteCounts(bool raw)
        {
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
            File.WriteAllLines(Path.Combine(Utils.Dir, $"counts{raw}.csv"), lines);
        }
        public List<Node> FindStarts()
        {
			var start = _dict.Values.FirstOrDefault(v => v.StartTag);
			if (start == null)
				start = _dict.Values.First(v => v.Char == 'S');

			var vals = _dict.Values.Where(v => v.ConnPts.Any(c => c.Equals(start.Pt))).ToList();
            start.SetStart(vals);
            return vals;
        }
    }
}
