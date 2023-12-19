using AoCLibrary;
using System.Text.Unicode;
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
			var start = new Point(0, 0);
			var edgePts = new List<Point18>();
			var lastPt = new Point18(start, DirEnum.West, DirEnum.East, 0);
			foreach (var line in lines)
			{
				var (dir, len) = SplitLine(line, key.Star);
				var newPt = new Point18(lastPt.EndPt, Point.OtherDir(lastPt.Dir), dir, len);
				edgePts.Add(newPt);
				lastPt = newPt;
			}
			edgePts.First().LastDir = Point.OtherDir(lastPt.Dir);
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
					pt = pt.Translate(dir, 1);
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
			long rv = 0L;
			var rowRangeDict = new Dictionary<int, RangeList>();
			//insideList.Add(new Range(0, 1));
			var runningList = new RangeList();
			foreach (var col in cols)
			{
				var allPtsInCol = edgePts.Where(e => e.Col == col).ToList();
				var colList = RangeList.FromCol(edgePts, col);
				bool inside = false;
				foreach (var pt in allPtsInCol)
				{
					var allPtsInRow = edgePts.Where(e => e.Row == pt.Row).ToList();

					if (!rowRangeDict.ContainsKey(pt.Row))
						rowRangeDict[pt.Row] = RangeList.FromRow(edgePts, pt.Row);
					var rowRanges = rowRangeDict[pt.Row];
					var rowPt = allPtsInRow.FirstOrDefault(r => r.Contains(pt));
					if (rowPt != null)
					{
						if (rowPt.Equals(pt) || rowPt.EndPt.Equals(pt))
							inside = !inside;
					}
					else
					{
						if (!inside && runningList.Any(r => r.Overlaps(pt.Row)))
						{
							break;
						}
					}
				}

				//bool inside = false;
				/*				if (!lastColList.Any())
				{
					lastColList = colList;
					continue;
				}

				 * foreach (var pt in allPtsInCol)
				{
					if (!rowRangeDict.ContainsKey(pt.Row))
						rowRangeDict[pt.Row] = RangeList.FromRow(edgePts, pt.Row);
					var rowRanges = rowRangeDict[pt.Row];
					//colList.MergeRange(pt.GetRowRange());
				}*/
				foreach (var range in colList)
					runningList.MergeRange(range);
				rv += runningList.Sum(r => r.Len);
			}
			return rv;
		}
	}
	public class Point18 : Point
	{
		public DirEnum LastDir { get; set; }
		public DirEnum Dir { get; }
		public int Len { get; set; }
		public Point18(Point pt, DirEnum lastDir, DirEnum dir, int len) : base(pt)
		{
			LastDir = lastDir;
			Dir = dir;
			Len = len;
		}
		public bool Contains(Point pt)
		{
			if (pt.Col != Col && pt.Row != Row)
				return false;
			if (Dir == DirEnum.East &&
				pt.Row == Row && pt.Col >= Col && pt.Col <= EndPt.Col)
				return true;
			else if (Dir == DirEnum.West &&
				pt.Row == Row && pt.Col <= Col && pt.Col >= EndPt.Col)
				return true;
			else if (Dir == DirEnum.South &&
				pt.Col == Col && pt.Row >= Row && pt.Row <= EndPt.Row)
				return true;
			else if (Dir == DirEnum.North &&
				pt.Col == Col && pt.Row <= Row && pt.Row >= EndPt.Row)
				return true;
			return false;

			{

			}

		}
		public bool IsCorner => LastDir != Dir;

		public Point EndPt
		{
			get
			{
				return Translate(Dir, Len);
			}
		}

		internal char CornerChar()
		{
			var dir1 = (DirEnum)Math.Min((int)LastDir, (int)Dir);
			var dir2 = (DirEnum)Math.Max((int)LastDir, (int)Dir);
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

		internal Range18 GetRowRange()
		{
			if (Dir == DirEnum.South)
				return new Range18(Row, Len);
			else if(Dir == DirEnum.North)
				return new Range18(Row, 0 - Len);
			else
				return new Range18(Row, 1);
		}
		internal Range18 GetColRange()
		{
			if (Dir == DirEnum.East)
				return new Range18(Col, Len);
			else if (Dir == DirEnum.West)
				return new Range18(Col, 0 - Len);
			else 
				return new Range18(Col, 1);
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
	internal class Range18
	{
		internal void Merge(Range18 range)
		{
			Utils.Assert(Overlaps(range), "Merge Not Overlapped");
			if (Start > range.Start)
				Start = range.Start;
			if (End < range.End)
				End = range.End;
		}
		internal bool Overlaps(Range18 range)    // or touch, identical is not consider overlap
		{
			if (Equals(range))
				return false;
			if ((range.Start >= Start && range.Start <= End) ||
				(range.End >= Start && range.End <= End))
				return true;
			if (Start == range.End + 1 || End + 1 == range.Start)
				return true;
			return false;
		}
		internal bool Overlaps(int i)    // or touch, identical is not consider overlap
		{
			return (i >= Start && i <= End);
		}
		public Range18(int start, int len)
		{
			if (len < 0)
			{
				End = start;
				Start = start + len + 1;
			}
			else
			{
				Start = start;
				End = start + len - 1;
			}
			
		}
		public int Len => (End - Start + 1);
		public override string ToString()
		{
			return $"{Start}-{End}";
		}
		public int Start { get; set; }
		public int End { get; set; }
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		public override bool Equals(object? obj)
		{
			if (obj is Range18 other)
			{
				return other.Start == Start && other.End == End;
			}
			return false;
		}
	}
	internal class RangeList : List<Range18>
	{
		static public RangeList FromRow(IEnumerable<Point18> pts, int iRow)
		{
			var rv = new RangeList();
			foreach (var pt in pts.Where(e => e.Row == iRow))
				rv.MergeRange(pt.GetColRange());
			return rv;
		}
		static public RangeList FromCol(IEnumerable<Point18> pts, int iCol)
		{
			var rv = new RangeList();
			foreach (var pt in pts.Where(e => e.Col == iCol))
				rv.MergeRange(pt.GetRowRange());
			return rv;
		}

		internal void MergeRange(Range18 newRange)
		{
			if (this.Any(r => r.Equals(newRange)))
				return;

			var overlaps = this.FirstOrDefault(r => r.Overlaps(newRange));
			if (overlaps == null)
			{
				Add(newRange);
				return;
			}

			var tryToMerge = newRange;
			overlaps.Merge(tryToMerge);

			tryToMerge = overlaps;
			while (true)
			{
				overlaps = this.FirstOrDefault(r => r.Overlaps(tryToMerge));
				if (overlaps == null)
					break;
				overlaps.Merge(tryToMerge);
				Remove(tryToMerge);
				tryToMerge = overlaps;

			}
		}
	}

}
