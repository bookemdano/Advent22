using AoCLibrary;
using System.Xml.Linq;

namespace Advent23
{
	internal class Day16 : IDayRunner
	{
		public bool IsReal => false;

		// Day https://adventofcode.com/2023/day/16
		// Input https://adventofcode.com/2023/day/16/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 46L);
			else
				check = new StarCheck(key, 7415);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var grd = Grid16.FromLines(lines);
			rv = grd.Light(new Point(0, 0), new Point(0, -1));

			check.Compare(rv);

			//7415
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 51L);
			else
				check = new StarCheck(key, 7943L);

			var lines = Program.GetLines(check.Key);
			// magic
			var grd = Grid16.FromLines(lines);
			var rv = grd.BestLight();


			check.Compare(rv);
			//	rv	7943	
			return rv;
		}
	}
	public class Node16 : Node
	{
		public Node16(Point pt, char c) : base(pt, c)
		{
		}
		public bool Energized { get; set; }
	}
	public class Grid16 : Grid<Node16>
	{
		public Grid16(List<Node16> nodes)
		{
			Init(nodes);
		}
		internal static Grid16 FromLines(string[] lines)
		{
			return new Grid16(GetNodes(lines));
		}
		//MOVE PATH to instance so part 2 can use it. Now it is looping
		static Dictionary<string, List<Point>> _allPaths = [];
		public long Light(Point pt, Point from)
		{
			var beams = new List<Beam>();
			foreach (var v in Values)
				v.Energized = false;
			var root = new Beam(from);
			root.Add(pt);
			while (!root.IsDone(this, true))
			{
				root.Step(this, from);
				root.Prune();
				//WriteLocal("beamed", beams);
			}
			return this.Values.Count(n => n.Energized);
		}


		public void WriteLocal(string tag, List<Beam> beams)
		{
			var lines = new List<string>();
			for (int row = 0; row < _rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < _cols; col++)
				{
					var node = Find(new Point(row, col))!;
					if (beams.Any(b => b.Pt.Equals(node.Pt)))
						parts.Add("B");
					else if (node.Energized)
						parts.Add("#");
					else
						parts.Add(node.Char.ToString());
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Base", tag, lines);
		}

		internal long BestLight()
		{
			_allPaths.Clear();

			var best = 0L;
			for (int iRow = 0; iRow < _rows; iRow++)
			{
				var val = Light(new Point(iRow, 0), new Point(iRow, -1));
				if (val > best)
					best = val;
				val = Light(new Point(iRow, _cols - 1), new Point(iRow, _cols));
				if (val > best)
					best = val;
			}
			for (int iCol = 0; iCol < _cols; iCol++)
			{
				var val = Light(new Point(0, iCol), new Point(-1, iCol));
				if (val > best)
					best = val;
				val = Light(new Point(_rows - 1, iCol), new Point(_rows, iCol));
				if (val > best)
					best = val;
			}
			return best;
		}
		internal Node16? Find16(Point pt)
		{
			return Find(pt);
		}


		internal bool Valid(Point pt)
		{
			if (pt.Col < 0 || pt.Col >= _cols || pt.Row < 0 || pt.Row >= _rows)
				return false;
			return true;
		}
	}
	public enum DirEnum
	{
		NA,
		North,
		East,
		South,
		West
	}

	public class Beam
	{
		public Beam(Point pt)
		{
			Pt = pt;
		}
		public Point Pt { get; private set; }

		List<Beam> _children = [];

		internal void Add(Point pt)
		{
			_children.Add(new Beam(pt));
			Utils.Assert(_children.Count() <= 2, "up to two children");
		}

		internal void Step(Grid16 grd, Point? from)
		{
			if (!_children.Any() && from != null && grd.Valid(Pt))
			{
				var dir = GetDir(from, Pt);
				var node = grd.Find16(Pt)!;

				if (node.Char == '.')
					Add(Translate(Pt, dir));
				else if (node.Char == '|')
				{
					if (dir == DirEnum.East || dir == DirEnum.West)
					{
						Add(Translate(Pt, DirEnum.North));
						Add(Translate(Pt, DirEnum.South));
					}
					else
					{
						Add(Translate(Pt, dir));
					}
				}
				else if (node.Char == '-')
				{
					if (dir == DirEnum.North || dir == DirEnum.South)
					{
						Add(Translate(Pt, DirEnum.East));
						Add(Translate(Pt, DirEnum.West));
					}
					else
					{
						Add(Translate(Pt, dir));
					}
				}
				else if (node.Char == '\\')
				{
					if (dir == DirEnum.North)
						Add(Translate(Pt, DirEnum.West));
					else if (dir == DirEnum.East)
						Add(Translate(Pt, DirEnum.South));
					else if (dir == DirEnum.South)
						Add(Translate(Pt, DirEnum.East));
					else if (dir == DirEnum.West)
						Add(Translate(Pt, DirEnum.North));
				}
				else if (node.Char == '/')
				{
					if (dir == DirEnum.North)
						Add(Translate(Pt, DirEnum.East));
					else if (dir == DirEnum.West)
						Add(Translate(Pt, DirEnum.South));
					else if (dir == DirEnum.South)
						Add(Translate(Pt, DirEnum.West));
					else if (dir == DirEnum.East)
						Add(Translate(Pt, DirEnum.North));
				}
			}
			else
			{
				foreach(var child in _children)
					child.Step(grd, Pt);
			}
		}
		public override string ToString()
		{
			return $"{Pt}({Size()}) [{string.Join(',', _children)}]";
		}
		internal int Size()
		{
			return 1 + _children.Sum(c => c.Size());
		}
		public bool IsDone(Grid16 grd, bool root)
		{
			if (!root && !grd.Valid(Pt))
				return true;
			if (!_children.Any())
				return false;

			foreach (var child in _children)
				if (!child.IsDone(grd, false))
					return false;
			return true;
		}


		static Point Translate(Point pt, DirEnum dir)
		{
			Point rv;
			if (dir == DirEnum.North)
				rv = new Point(pt.Row - 1, pt.Col);
			else if (dir == DirEnum.South)
				rv = new Point(pt.Row + 1, pt.Col);
			else if (dir == DirEnum.East)
				rv = new Point(pt.Row, pt.Col + 1);
			else //if (dir == DirEnum.West)
				rv = new Point(pt.Row, pt.Col - 1);
			return rv;
		}

		static DirEnum GetDir(Point from, Point to)
		{
			if (from.Col < to.Col)
				return DirEnum.East;
			else if (from.Col > to.Col)
				return DirEnum.West;
			else if (from.Row > to.Row)
				return DirEnum.North;
			else if (from.Row < to.Row)
				return DirEnum.South;
			return DirEnum.NA;
		}

		internal void Prune()
		{
			foreach(var child in _children)
			{
				var key = $"{Pt} + {child.Pt}";
				var b = child.FindBeam(key);
				if (b != null)
				{
					b.Pt = InvalidPt;
					b._children.Clear();
				}
				child.Prune();
			}
		}
		readonly Point InvalidPt = new Point(-1, -1);
		Beam? FindBeam(string key)
		{
			foreach (var child in _children)
			{
				if (child.Pt.Equals(InvalidPt))
					continue;

				var subKey = $"{Pt} + {child.Pt}";
				if (subKey == key)
					return child;
				var found = child.FindBeam(key);
				if (found != null)
					return found;
			}
			return null;
		}
	}

}
