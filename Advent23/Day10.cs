using System.Xml.Linq;
using AoCLibrary;
namespace Advent23
{
	internal class Day10 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/10
		// Input https://adventofcode.com/2023/day/10/input
		public object? Star1()
		{
			var rv = 0L;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			var grd = new Grid(lines);
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
            var grd = new Grid(lines);
            var starts = grd.FindStarts().ToArray();
            int step = 0;

            while (starts.Count() > 0)
            {
                step++;
                var newStarts = new List<Node>();
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

            rv = grd.Insides();
            grd.WriteCounts();
            if (!IsReal)
                Utils.Assert(rv, 8L);
			return rv;
            // 873 too high
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
            return Row * 1000 + Col;
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
        public Node(int row, int col, char c)
        {
            Pt = new Point(row, col);
            Char = c;
            ConnPts = Connections(Pt, Char);
        }

        public Point Pt { get; }
        public char Char { get; private set; }
        public int? Count { get; set; } = null;
        public List<Point> ConnPts { get; private set; }
        public bool? Escape { get; internal set; }

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
        static internal List<Point> Connections(Point pt, char c)
        {
            var rv = new List<Point>();
            if (South(c))
                rv.Add(new(pt.Row + 1, pt.Col));
            if (North(c))
                rv.Add(new(pt.Row - 1, pt.Col));
            if (East(c))
                rv.Add(new(pt.Row, pt.Col + 1));
            if (West(c))
                rv.Add(new(pt.Row, pt.Col - 1));
    
            return rv;
        }
        static bool North(char c)
        {
            return (c == '|' || c == 'L' || c == 'J');
        }
        static bool South(char c)
        {
            return (c == '|' || c == '7' || c == 'F');
        }
        static bool East(char c)
        {
            return (c == '-' || c == 'L' || c == 'F');
        }
        static bool West(char c)
        {
            return (c == '-' || c == '7' || c == 'J');
        }
        public bool ConnectsTo(Point pt)
        {
            return ConnPts.Contains(pt);

        }

        internal void SetStart(List<Node> vals)
        {
            Count = 0;
            ConnPts = vals.Select(v => v.Pt).ToList();
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
		List<Node>  _grd = new List<Node>();
        private int _rows;
        private int _cols;

        public Grid(string[] lines)
		{
			_rows = lines.Length;
			_cols = lines[0].Length;
			int iRow = 0;
			foreach (var line in lines)
			{
				int iCol = 0;
				foreach (var c in line)
					_grd.Add(new Node(iRow, iCol++, c));
                iRow++;
            }
		}
        internal List<Node> Connections(Node gv)
        {
            var pts = gv.ConnPts;
            if (!pts.Any())
                return new List<Node>();
            var gvs = pts.Select(p => Find(p)).ToList();
            return gvs.Where(v => v.Count == null).ToList();
        }
        Node? Find(Point pt)
        {
            return _grd.FirstOrDefault(v => v.Pt.Equals(pt));
        }
        bool CanEscape(Node node, List<Node> fromPath)
        {
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
            foreach(var from in fromPath)
                neighbors.Remove(from);

            if (neighbors.Any(n => n.Escape == true))
            {
                node.Escape = true;
                return true;
            }
            foreach (var neighbor in neighbors)
            {
                var newPath = new List<Node>(fromPath);
                newPath.Add(node);
                if (CanEscape(neighbor, newPath))
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
            foreach (var node in _grd)
            {
                if (!node.IsWall())
                    node.ResetChar();
            }
            foreach (var node in _grd)
            {
                CanEscape(node, new List<Node>());
            }
            return _grd.Count(n => n.Escape == false && n.IsWall() == false);
            int rv = 0;
            for (int row = 0; row < _grd.Max(v => v.Pt.Row); row++)
            {
                bool inside = false;
                bool inwall = false;
                Node? lastNode = null;
                for (int col = 0; col < _grd.Max(v => v.Pt.Col); col++)
                {
                    var node = Find(new Point(row, col));
                    if (node.IsWall())
                    {
                        var nextPt = new Point(row, col + 1);
                        var nextNode = Find(nextPt);

                        var connected = node.Interconnected(nextNode);
                        if (!inwall && !inside)
                        {
                            if (connected)
                                inwall = true;
                            else
                                inside = true;
                        }
                        else if (!inwall && inside)
                        {
                            if (connected)
                                inwall = true;
                            else
                                inside = false;
                        }
                        else if (inwall && !inside)
                        {
                            if (!connected)
                                inwall = false;
                        }
                        else if (inwall && inside)
                        {
                            if (!connected)
                                inwall = false;
                        }
                    }
                    else
                    {
                        Utils.Assert(inwall == false, "not in wall");
                        if (inside)
                            rv++;
                    }
                    lastNode = node;
                }
            }
            return rv;
        }
        public void WriteCounts()
        {
            var lines = new List<string>();
            for (int row = 0; row < _grd.Max(v => v.Pt.Row); row++)
            {
                var parts = new List<string>();
                for (int col = 0; col < _grd.Max(v => v.Pt.Col); col++)
                {
                    var v = Find(new Point(row, col))!;
                    if (v.IsWall())
                        parts.Add("*");
                    else if (v.Escape == false)
                        parts.Add("e");
                    //parts.Add(v.Count.Value.ToString());
                    else
                        parts.Add("-");
                }
                lines.Add(string.Join(",", parts));
            }
            File.WriteAllLines(Path.Combine(Utils.Dir, "counts.csv"), lines);
        }
        public List<Node> FindStarts()
        {
            var start = _grd.First(v => v.Char == 'S');
            var vals = _grd.Where(v => v.ConnPts.Any(c => c.Equals(start.Pt))).ToList();
            start.SetStart(vals);
            return vals;
        }
    }
}
