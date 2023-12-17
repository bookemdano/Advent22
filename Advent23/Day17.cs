using AoCLibrary;
namespace Advent23
{
	internal class Day17 : IDayRunner
	{
		public bool IsReal => false;

		// Day https://adventofcode.com/2023/day/17
		// Input https://adventofcode.com/2023/day/17/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 102L);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
            // magic
            var grd = new Grid17(lines);
            grd.FindPaths();

			check.Compare(rv);
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 0L);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic

			check.Compare(rv);
			return rv;
		}
	}
	public class Grid17 : GridPlain
	{
        public Grid17(string[] lines)
        {
            var nodes = GetNodes(lines);
            Init(nodes);
        }


        internal void FindPaths()
        {
            var from = new Point(0, -1);
            var root = new Walk17(from, this);
            var start = new Point(0, 0);
            root.Add(start);
            while (!root.IsDone())
            {
                root.Step(from);
                WriteLocal("step", root.AllPts());
            }
            var completes = root.Completes();

        }
        internal Point EndPoint()
        {
            return new Point(_rows - 1, _cols - 1);
        }
        public void WriteLocal(string tag, List<Point> pts)
        {
            var lines = new List<string>();
            for (int row = 0; row < _rows; row++)
            {
                var parts = new List<string>();
                for (int col = 0; col < _cols; col++)
                {
                    var node = Find(new Point(row, col))!;
                    if (pts.Contains(node.Pt))
                        parts.Add("B");
                    else
                        parts.Add(node.Char.ToString());
                }
                lines.Add(string.Join(",", parts));
            }
            ElfUtils.WriteLines("Base", tag, lines);
        }
    }
    public class Walk17
    {
        public Walk17(Point pt, Grid17 grd)
        {
            Pt = pt;
            _parent = null;
            _grd = grd;
            _processed = true;
        }

        public Walk17(Point pt, Walk17 parent)
        {
            Pt = pt;
            _parent = parent;
            _grd = parent._grd;
        }


        public Point Pt { get; private set; }
        Grid17 _grd;
        List<Walk17> _children = [];
        Walk17? _parent = null;
        bool _processed;
        internal void Add(Point oldPt, DirEnum dir)
        {
            var newPt = oldPt.Translate(dir);
            Add(newPt);
        }
        internal void Add(Point pt)
        {
            if (!_grd.IsValid(pt))
                return;

            if (HasParentPoint(pt))
                return;

            //var pts = ParentPoints();
            //_grd.WriteLocal("TryAdd", pts);
            var p3 = _parent?._parent;
            if (p3 != null)
            {
                var dCol = Math.Abs(p3.Pt.Col - pt.Col);
                var dRow = Math.Abs(p3.Pt.Row - pt.Row);
                if ((dCol == 3 && dRow == 0) || (dRow == 3 && dCol == 0))
                    return; // too many in a row
            }
            _children.Add(new Walk17(pt, this));
            //Utils.Assert(_iChildren <= 2, "up to two children");
        }
        List<Point> ParentPoints()
        {
            var rv = new List<Point>();
            rv.Add(Pt);
            if (_parent != null)
              rv.AddRange(_parent.ParentPoints());
            return rv;
        }
        bool HasParentPoint(Point pt)
        {
            if (_parent == null)
                return false;
            if (pt.Equals(Pt))
                return true;
            return _parent.HasParentPoint(pt);
        }
        int ParentSize()
        {
            if (_parent == null)
                return 0;
            return 1 + _parent.ParentSize();
        }
        string Key
        {
            get
            {
                return $"{_parent?.Pt} to {Pt}";
            }
        }
        Walk17 GetRoot()
        {
            if (_parent == null)
                return this;
            else
                return _parent.GetRoot();
        }
        void AddSetNot(DirEnum dir)
        {
            if (Pt == _grd.EndPoint())
                return;
            if (dir != DirEnum.North)
                Add(Pt, DirEnum.North);
            if (dir != DirEnum.South)
                Add(Pt, DirEnum.South);
            if (dir != DirEnum.East)
                Add(Pt, DirEnum.East);
            if (dir != DirEnum.West)
                Add(Pt, DirEnum.West);

        }
        internal void Step(Point? from)
        {
            if (!_processed)
            {
                if (from != null)
                    AddSetNot(Pt.GetDir(from));// opposite direction
                _processed = true;
            }
            else
            {
                foreach (var child in _children)
                    child?.Step(Pt);
            }
        }
        public override string ToString()
        {
            return $"{Pt}({Size()})";   // [{string.Join(',', _children)}]";
        }
        internal int Size()
        {
            return 1 + _children.Sum(c => c.Size());
        }
        internal List<Point> AllPts()
        {
            var rv = new List<Point>();
            if (_grd.IsValid(Pt))
                rv.Add(Pt);
            foreach (var child in _children)
                if (child != null)
                    rv.AddRange(child.AllPts());
            return rv;
        }
        public bool IsDone()
        {
            if (!_processed)
                return false;
            if (Pt == _grd.EndPoint())
                return true;
            foreach (var child in _children)
                if (child.IsDone() == false)
                    return false;
            return true;
        }
        public List<Walk17> Completes()
        {
            var rv = new List<Walk17>();
            if (Pt.Equals(_grd.EndPoint()))
                rv.Add(this);
            foreach (var child in _children)
                rv.AddRange(child.Completes());
            return rv;
        }
        public bool AtEnd()
        {
            if (Pt.Equals(_grd.EndPoint()))
                return true;
            foreach (var child in _children)
                if (child.AtEnd())
                    return true;
            return false;
        }
        //readonly Point InvalidPt = new Point(-1, -1);
        bool HasPoint(Point pt)
        {
            if (Pt.Equals(pt))
                return true;
            foreach (var child in _children)
                if (child?.HasPoint(pt) == true)
                    return true;
            return false;
        }
    }
}
