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
				check = new StarCheck(key, 859L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var grd = Grid17.FromLines(lines);
            grd.FindPaths2();

			check.Compare(rv);
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 71L);
			else
				check = new StarCheck(key, 1027L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var grd = Grid17.FromLines(lines);
			grd.FindPaths2();

			check.Compare(rv);
			return rv;
		}
	}
	public class Node17 : Node
	{
		public Node17(Point pt, char c) : base(pt, c)
		{
		}
		public long MinSum { get; set; } = -1;
		public override string ToString()
		{
			return $"{base.ToString()} m:{MinSum}";
		}
		public long Cooling => Char - '0';
	}
	public class Grid17 : Grid<Node17>
	{
		public Grid17(List<Node17> nodes)
		{
			Init(nodes);
		}
		internal static Grid17 FromLines(string[] lines)
		{
			return new Grid17(GetNodes(lines));
		}


		internal void FindPaths2()
        {
			foreach (var node in Values)
				node.MinSum = -1;

			var start = new Point(0, 0);

			var paths = new List<Path17>();
			paths.Add(new Path17(this, start));


			while (!paths.All(p => p.IsDone()))
            {
				var newPaths = new List<Path17>();
				foreach (var path in paths)	//.OrderByDescending(p => p.Path.Count()).Take(1000))
					newPaths.AddRange(path.AddAll());
				paths.AddRange(newPaths);
				paths.RemoveAll(p => p.IsDone() && !p.Won());
				ElfHelper.DayLog($"{paths.Count()} a:{paths.Count(p => !p.IsDone())} w:{paths.Count(p => p.Won())}");
				WriteLocal("step");
			}

        }

		static List<Point> _pointCache = [];
        internal Point EndPoint()
        {
            return new Point(_rows - 1, _cols - 1);
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
					var min = Path17.GetMin(node.Pt);
					//if (pts.Contains(node.Pt))
					//    parts.Add("B");
					if (min > 0)
					    parts.Add(min.ToString("000"));
					else
						parts.Add($" {node.Char} ");
                }
                lines.Add(string.Join(",", parts));
            }
            ElfUtils.WriteLines("Base", tag, lines);
        }
    }
	public class Vector17 : Vector
	{
		public Vector17(Point pt, DirEnum dir, int streak) : base(pt, dir)
		{
			Streak = streak;
		}
		public int Streak { get; }

		public override bool Equals(object? obj)
		{
			if (obj is Vector17 other)
			{
				return base.Equals(other) && Streak == Streak;
			}
			return false;
		}
		public override int GetHashCode()
		{
			return base.GetHashCode() * Streak;
		}
		public override string ToString()
		{
			return $"{base.ToString()} s:{Streak}";
		}
		internal Vector17 Continue()
		{
			return new Vector17(Pt.Translate(Dir), Dir, Streak + 1);
		}
		//public long Score { get; set; }
	}
	public class Path17
	{
		private Grid17 _grd;

		public Vector17 Current { get; private set; }
		public long Score { get; set; }
		private bool _done = false;
		
		private Path17(Path17 other, Vector17 vector)
		{
			_grd = other._grd;
			Score = other.Score;
			SetCurrent(vector);
			//grd.Find(to)!.MinSum = Score;
		}
		static public long GetMin(Point pt)
		{
			var ptVecs = _vecs.Where(v => v.Key.Pt.Equals(pt));
			if (ptVecs.Any())
				return ptVecs.Min(v => v.Value);
			return 0;
		}
		static Dictionary<Vector17, long> _vecs = [];
		void SetCurrent(Vector17 vector)
		{
			Current = vector;
			Score += _grd.Find(Current.Pt)!.Cooling;
			if (_vecs.ContainsKey(vector) && _vecs[vector] < Score)
				_done = true;
			else
				_vecs[vector] = Score;
		}
		// only to initialize
		public Path17(Grid17 grd, Point to)
		{
			_grd = grd;
			Current = new Vector17(to, DirEnum.East, 1);
			// don't score first block
			//grd.Find(to)!.MinSum = Score;
		}
		
		internal List<Path17> AddAll()
		{
			var rv = new List<Path17?>();	
			if (Current.Dir != DirEnum.South && Current.Dir != DirEnum.North)
			{
				rv.Add(CopyThis(DirEnum.North));
				rv.Add(CopyThis(DirEnum.South));
			}
			if (Current.Dir != DirEnum.West && Current.Dir != DirEnum.East)
			{
				rv.Add(CopyThis(DirEnum.East));
				rv.Add(CopyThis(DirEnum.West));
			}
			var newV = Current.Continue();
			if (newV.Streak <= 3 && _grd.IsValid(newV.Pt))
				SetCurrent(newV);
			else
				_done = true;

			return rv.Where(p => p != null).ToList()!;
		}

		private Path17? CopyThis(DirEnum dir)
		{
			var newPt = Current.Pt.Translate(dir);
			if (!_grd.IsValid(newPt))	// on board
				return null;

			return new Path17(this, new Vector17(newPt, dir, 1));
		}

		static List<Vector17> _used = [];
		internal bool IsDone()
		{
			return (_done || Won());
		}
		internal bool Won()
		{
			return Current.Pt.Equals(_grd.EndPoint());
		}

		public override string ToString()
		{
			return $"{Current} w:{Won()} d:{_done}";
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
