using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using AoCLibrary;
namespace Advent23
{
	internal class Day18 : IDayRunner
	{
		public bool IsReal => false;

		// Day https://adventofcode.com/2023/day/18
		// Input https://adventofcode.com/2023/day/18/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 62);
			else
				check = new StarCheck(key, 53844);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			//var grd = new GridPlain(lines);
			var start = new Point18(0, 0, DirEnum.East);
			var lastPt = start;
			var edgePts = new List<Point18>();
			edgePts.Add(lastPt);
			foreach (var line in lines)
			{
				var (dir, len) = SplitLine(line, key.Star);
				for (int i = 0; i < len; i++)
				{
					var movePt = lastPt.Translate(dir);

					//Utils.Assert(pt.Row >= 0, "valid");
					var newPt = new Point18(movePt.Row, movePt.Col, movePt.GetDir(lastPt));
					edgePts.Add(newPt);
					lastPt.InDir = lastPt.GetDir(newPt);
					lastPt = newPt;
				}
			}
			edgePts.First().InDir = start.GetDir(lastPt);
			edgePts = edgePts.OrderBy(p => p.GetHashCode()).ToList();
			//Normalize(edgePts);
			WritePts(edgePts);
			rv = GetInsides(edgePts);

			check.Compare(rv);
			//53844
			return rv;
		}

		private void WritePts(List<Point18> pts)
		{
			var minRow = pts.Min(p => p.Row);
			var maxRow = pts.Max(p => p.Row);
			var minCol = pts.Min(p => p.Col);
			var maxCol = pts.Max(p => p.Col);
			var lines = new List<string>();
			for (int row = minRow; row <= maxRow; row++)
			{
				var parts = new List<string>();
				for (int col = minCol; col <= maxCol; col++)
				{
					var pt = new Point(row, col);
					var foundPt = pts.FirstOrDefault(p => p.Equals(pt));
					if (foundPt == null)
						parts.Add(" ");
					else
						parts.Add($"{foundPt.CornerChar()}");
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Base", "pts", lines);
		}

		private static void Normalize(List<Point18> edgePts)
		{
			var minRow = edgePts.Min(n => n.Row);
			var minCol = edgePts.Min(n => n.Col);
			var edgeNodes = new List<Node18>();
			foreach (var edgePt in edgePts)
				edgeNodes.Add(new Node18(new Point(edgePt.Row + Math.Abs(minRow), edgePt.Col + Math.Abs(minCol)), '#'));
		}

		DirEnum ParseDir(string dir)
		{
			if (dir == "R" || dir == "0")
				return DirEnum.East;
            else if (dir == "L" || dir == "2")
                return DirEnum.West;
            else if (dir == "U" || dir == "3")
                return DirEnum.North;
            else if (dir == "D" || dir == "1")
                return DirEnum.South;
			else
				return DirEnum.NA;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 952408144115L);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var pt = new Point(0, 0);
			var edgePts = new List<Point18>();
			//edgePts.Add(pt);
			foreach (var line in lines)
			{
				var parts = Utils.Split(' ', line);
				var dist = parts[2].Substring(2, 5);
				var dir = ParseDir(parts[2].Substring(7, 1));
				var len = int.Parse(dist, System.Globalization.NumberStyles.HexNumber);
				for (int i = 0; i < len; i++)
				{
					pt = pt.Translate(dir);
					//Utils.Assert(pt.Row >= 0, "valid");
					//edgePts.Add(pt);
				}
			}
			GetInsides(edgePts);
			var minRow = edgePts.Min(n => n.Row);
			var minCol = edgePts.Min(n => n.Col);
			var edgeNodes = new List<Node18>();
			foreach (var edgePt in edgePts)
				edgeNodes.Add(new Node18(new Point(edgePt.Row + Math.Abs(minRow), edgePt.Col + Math.Abs(minCol)), '#'));

			var grd = Grid18.FromSparse(edgeNodes);
			grd.WriteBase("edge");
			rv = grd.CountInside();

			check.Compare(rv);
			return rv;
		}
		Tuple<DirEnum, int> SplitLine(string line, StarEnum star)
		{
			var parts = Utils.Split(' ', line);
			if (star == StarEnum.Star1)
			{
				var dir = ParseDir(parts[0]);
				var len = int.Parse(parts[1]);
				return new Tuple<DirEnum, int>(dir, len);
			}
			else
			{
				var dist = parts[2].Substring(2, 5);
				var dir = ParseDir(parts[2].Substring(7, 1));
				var len = int.Parse(dist, System.Globalization.NumberStyles.HexNumber);
				return new Tuple<DirEnum, int>(dir, len);
			}
		}
		long GetInsides(IEnumerable<Point18> edgePts)
		{
			var insides = edgePts.Count();
			var cols = edgePts.Select(p => p.Col).Distinct().Order();
			foreach(var col in cols)
			{
				var allPtsInCol = edgePts.Where(e => e.Col == col).ToList();
				Point18? ptStartInside = null;
				foreach (var pt in allPtsInCol)
				{
					if (ptStartInside == null)
					{
						if (pt.OutDir != DirEnum.South)
							ptStartInside = pt;
						continue;
					}
					else
					{
						if (pt.OutDir != DirEnum.South)
						{
							var insideCount = Math.Abs(pt.Row - ptStartInside.Row) - 1;
							insides += insideCount;
							ptStartInside = null;
						}
					}
				}
			}
			return insides;
		}
	}
	public class Point18 : Point
	{
		public DirEnum InDir { get; set; }
		public DirEnum OutDir { get; }
		public Point18(int row, int col, DirEnum fromDir) : base(row, col)
		{
			OutDir = fromDir;
		}
		public bool IsCorner => InDir != OutDir;
		internal char CornerChar()
		{
			var dir1 = (DirEnum)Math.Min((int)InDir, (int)OutDir);
			var dir2 = (DirEnum)Math.Max((int)InDir, (int)OutDir);
			char rv;
			if (dir1 == DirEnum.North && dir2 == DirEnum.South)
				rv = '|';
			else if (dir1 == DirEnum.North && dir2 == DirEnum.East)
				rv = '└';
			else if (dir1 == DirEnum.North && dir2 == DirEnum.West)
				rv = '┘';
			else if (dir1 == DirEnum.East && dir2 == DirEnum.South)
				rv = '┌';
			else if (dir1 == DirEnum.East && dir2 == DirEnum.West)
				rv = '─';
			else if (dir1 == DirEnum.South && dir2 == DirEnum.West)
				rv = '┐';
			else
				rv ='x';
			return rv;
		}
		public override string ToString()
		{
			// NESW
			return $"{base.ToString()} {CornerChar()} c:{IsCorner}";
		}
	}
    public class Node18 : Node
    {
        public Node18(Point pt, char c) : base(pt, c)
        {
        }
        public bool? CanEscape { get; set; }
    }
    public class Grid18 : Grid<Node18>
	{
		public long CountInside()
        {
            var rv = 0;
			var lastCount = 0;
			while (lastCount != Values.Count(n => n.CanEscape == null))
			{
				lastCount = Values.Count(n => n.CanEscape == null);
				ElfHelper.DayLog("CountInside() " + lastCount);
				foreach (var node in Values.Where(n => n.CanEscape == null))
					CanEscapeQuick(node);
				//WriteLocal("stepQuick");
			}
			foreach (var node in Values)
				if (node.CanEscape != true)
					rv++;
			return rv;
		}
		void CanEscapeQuick(Node18 node)
		{
			if (node.CanEscape != null)
				return;
			if (node.Char != '.')
			{
				node.CanEscape = false;
				return;
			}
			var nears = node.Neighbors();
			if (nears.Any(n => !IsValid(n)))
			{
				node.CanEscape = true;  // on edge
				return;
			}
			foreach (var near in nears)
			{
				var nearNode = Find(near);
				if (nearNode == null)
					continue;
				if (nearNode.CanEscape == true)
				{
					node.CanEscape = true;
					return;
				}
			}
		}
		bool? CanEscape(Node18 node, List<Point> path)
		{
			if (node.CanEscape != null)
				return node.CanEscape.Value;
			if (node.Char != '.')
			{
				node.CanEscape = false;
				return false;
			}
			var nears = node.Neighbors();
			if (nears.Any(n => !IsValid(n)))
			{
				node.CanEscape = true;  // on edge
				return true;
			}
            path.Add(node.Pt);
			if (path.Count() > 50)
				return null;
			bool nulls = false;
            foreach (var near in nears)
			{
                var nearNode = Find(near);
                if (nearNode == null)
                    continue;
				if (nearNode.CanEscape == true)
                {
                    node.CanEscape = true;
                    return true;
                }
                if (path.Contains(near))
				{
					nulls = true;
					continue;
				}

				if (CanEscape(nearNode, path) == true)
				{
					node.CanEscape = true; 
					return true;
				}
			}
			if (nulls)
				return null;
            node.CanEscape = false;
            return false;
		}

        public Grid18(List<Node18> nodes)
        {
            Init(nodes);
        }

        internal static Grid18 FromSparse(List<Node18> nodes)
        {
            var rows = nodes.Max(n => n.Pt.Row) + 1;
            var cols = nodes.Max(n => n.Pt.Col) + 1;
            var allNodes = new List<Node18>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var pt = new Point(row, col);
                    var node = nodes.FirstOrDefault(n => n.Pt.Equals(pt));
                    if (node == null)
                        allNodes.Add(new Node18(pt, '.'));
                    else
                        allNodes.Add(node);
                }
            }
            return new Grid18(allNodes);
        }
        public void WriteLocal(string tag)
        {
            var lines = new List<string>();
            for (int row = 0; row < _rows; row++)
            {
                var parts = new List<string>();
                for (int col = 0; col < _cols; col++)
                {
                    var node = Find(new Point(row, col))!;
                    if (node.CanEscape == true)
                        parts.Add(" ");
                    else
                        parts.Add($"{node.Char}");
                }
                lines.Add(string.Join(",", parts));
            }
            ElfUtils.WriteLines("Base", tag, lines);
        }
    }
}
