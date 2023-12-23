using AoCLibrary;
using System.Xml.Linq;

namespace Advent23
{
	internal class Day16 : IDayRunner
	{
		public bool IsReal => true;

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
			var root = new Beam(from, this);
			root.Add(pt);
			while (!root.IsDone(true))
				root.Step(from);
            /*
			var allPts = root.AllPts().Distinct().OrderBy(p => p.GetHashCode()).ToList();
            foreach (var pt1 in allPts)
            {
                var node = Find(pt1);
                if (node != null)
                    node.Energized = true;
            }
            WriteLocal("beamed", beams);
			*/
            return root.AllPts().Distinct().Count();
		}


		public void WriteLocal(string tag, List<Beam> beams)
		{
			var lines = new List<string>();
			for (int row = 0; row < Rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < Cols; col++)
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
			for (int iRow = 0; iRow < Rows; iRow++)
			{
				var val = Light(new Point(iRow, 0), new Point(iRow, -1));
				if (val > best)
					best = val;
				val = Light(new Point(iRow, Cols - 1), new Point(iRow, Cols));
				if (val > best)
					best = val;
			}
			for (int iCol = 0; iCol < Cols; iCol++)
			{
				var val = Light(new Point(0, iCol), new Point(-1, iCol));
				if (val > best)
					best = val;
				val = Light(new Point(Rows - 1, iCol), new Point(Rows, iCol));
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
			if (pt.Col < 0 || pt.Col >= Cols || pt.Row < 0 || pt.Row >= Rows)
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
        public Beam(Point pt, Grid16 grd)
        {
            Pt = pt;
            _parent = null;
            _grd = grd;
            _processed = true;
        }

        public Beam(Point pt, Beam parent)
        {
            Pt = pt;
            _parent = parent;
			_grd = parent._grd;
        }


        public Point Pt { get; private set; }
		Grid16 _grd;
		Beam?[] _children = new Beam?[2];
		int _iChildren = 0;
		Beam? _parent = null;
		bool _processed;
		internal void Add(Point pt)
		{
			if (!_grd.Valid(pt))
				return;
            var key = $"{Pt} to {pt}";
            var root = GetRoot();
			if (root.HasBeam(key))
				return;
            _children[_iChildren++] = new Beam(pt, this);
			Utils.Assert(_iChildren <= 2, "up to two children");
		}
		string Key
		{
			get
			{
                return $"{_parent?.Pt} to {Pt}";
            }
		}
		Beam GetRoot()
		{
			if (_parent == null)
				return this;
			else
				return _parent.GetRoot();
		}
		internal void Step(Point? from)
		{
	        if (!_processed)
			{
				var dir = GetDir(from, Pt);
				var node = _grd.Find16(Pt)!;

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
				_processed = true;
			}
			else
			{
				foreach(var child in _children)
					child?.Step(Pt);
			}
		}
		public override string ToString()
		{
			return $"{Pt}({Size()}) [{string.Join(',', _children?.ToString())}]";
		}
		internal int Size()
		{
			return 1 + _children.Sum(c => c.Size());
		}
		internal List<Point> AllPts()
		{
			var rv = new List<Point>();
            if (_grd.Valid(Pt))
                rv.Add(Pt);
            foreach (var child in _children)
				if (child != null)
					rv.AddRange(child.AllPts());
            return rv;
        }
        public bool IsDone(bool root)
		{
			if (!_processed)
				return false;

			foreach (var child in _children)
				if (child?.IsDone(false) == false)
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

		static DirEnum GetDir(Point? from, Point to)
		{
			if (from == null)
				return DirEnum.NA;

			if (from.Col < to.Col)
				return DirEnum.East;
			else if (from.Col > to.Col)
				return DirEnum.West;
			else if (from.Row > to.Row)
				return DirEnum.North;
			else// if (from.Row < to.Row)
				return DirEnum.South;
		}

		//readonly Point InvalidPt = new Point(-1, -1);
		bool HasBeam(string key)
		{
			if (Key == key)
				return true;
			foreach (var child in _children)
				if (child?.HasBeam(key) == true)
					return true;
			return false;
        }
	}
}
