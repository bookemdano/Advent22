using AoCLibrary;

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
        public long Light(Point pt, Point from)
		{
			var beams = new List<Beam>();
            Beam.Reset();
            foreach (var v in Values)
                v.Energized = false;
			Beam.Rows = _rows;
			Beam.Cols = _cols;
			var paths = new List<List<Point>>();

			beams.Add(new Beam(pt, from));
			while(beams.Any())
			{
                var extraBeams = new List<Beam>();
				foreach(var beam in beams)
				{
					var node = Find(beam.Pt)!;
					node.Energized = true;
					bool endsOffGrid = false;
					bool foundPath = true;
					foreach (var path in paths.OrderByDescending(p => p.Count))
					{
						for(int i = 1; i < path.Count; i++)
						{
							var pathKey = $"{path[i-1]} to {path[i]}";
							if (pathKey == beam.Key)
							{
								foundPath = true;
								for (int j = i + 1; j < path.Count; j++)
								{
									var pathPt = path[j];
									if (Beam.Valid(pathPt))
									{
										Find(pathPt)!.Energized = true;
										beam.Move(pathPt);
									}
									else
										endsOffGrid = true;
								}
							}
						}
						if (foundPath)
							break;
					}
					if (endsOffGrid)
						continue;
					var dir = beam.GetDir();
					if (node.Char == '|')
					{
						if (dir == DirEnum.East || dir == DirEnum.West)
						{
                            extraBeams.Add(new Beam(beam.Pt, DirEnum.North));
                            beam.Move(DirEnum.South);
						}
						else if (dir == DirEnum.North || dir == DirEnum.South)
                        {
                            beam.Move(dir);
                        }
                    }
                    else if (node.Char == '-')
                    {
                        if (dir == DirEnum.North || dir == DirEnum.South)
                        {
							extraBeams.Add(new Beam(beam.Pt, DirEnum.West));
                            beam.Move(DirEnum.East);
                        }
                        else if (dir == DirEnum.East || dir == DirEnum.West)
                        {
                            beam.Move(dir);
                        }
                    }
                    else if (node.Char == '\\')
                    {
                        if (dir == DirEnum.North)
                            beam.Move(DirEnum.West);
                        else if (dir == DirEnum.East)
                            beam.Move(DirEnum.South);
                        else if (dir == DirEnum.South)
                            beam.Move(DirEnum.East);
                        else if (dir == DirEnum.West)
                            beam.Move(DirEnum.North);
                    }
                    else if (node.Char == '/')
                    {
                        if (dir == DirEnum.North)
                            beam.Move(DirEnum.East);
                        else if (dir == DirEnum.West)
                            beam.Move(DirEnum.South);
                        else if (dir == DirEnum.South)
                            beam.Move(DirEnum.West);
                        else if (dir == DirEnum.East)
                            beam.Move(DirEnum.North);
                    }
                    else if (node.Char == '.')
                    {
                        beam.Move(dir);
                    }
                }

				foreach (var beam in beams.Where(b => !b.IsValid()))
				{
					if (beam.Path.Count > 2)
						paths.Add(beam.Path);
				}
				beams.RemoveAll(b => !b.IsValid());
				beams.AddRange(extraBeams.Where(b => b.IsValid()));
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
            var best = 0L;
            for(int iRow = 0; iRow < _rows; iRow++)
            {
                var val = Light(new Point(iRow, 0), new Point(iRow, -1));
                if (val > best)
                    best = val;
                val = Light(new Point(iRow, _cols-1), new Point(iRow, _cols));
                if (val > best)
                    best = val;
            }
            for (int iCol = 0; iCol < _cols; iCol++)
            {
                var val = Light(new Point(0, iCol), new Point(-1, iCol));
                if (val > best)
                    best = val;
                val = Light(new Point(_rows-1, iCol), new Point(_rows, iCol));
                if (val > best)
                    best = val;
            }
            return best;
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
		public Beam(Point pt, Point lastPt)
		{
			Pt = pt;
			_lastPt = lastPt;
			Path.Add(_lastPt);
			Path.Add(pt);
		}

		public Beam(Point pt, DirEnum dir) : this(Translate(pt, dir), pt)
		{
		}

		public DirEnum GetDir()
		{
			if (_lastPt.Col < Pt.Col)
				return DirEnum.East;
			else if (_lastPt.Col > Pt.Col)
				return DirEnum.West;
			else if (_lastPt.Row > Pt.Row)
				return DirEnum.North;
			else if (_lastPt.Row < Pt.Row)
				return DirEnum.South;
			return DirEnum.NA;
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

		public void Move(DirEnum dir)
		{
			var newPt = Translate(Pt, dir);
			Move(newPt);
		}
		public bool IsValid()
		{
			if (_repeat)
				return false;
			return Valid(Pt);
		}

		static public bool Valid(Point pt)
		{
			if (pt.Col < 0 || pt.Col >= Cols || pt.Row < 0 || pt.Row >= Rows)
				return false;
			return true;
		}
		static List<string> _visited = [];
		bool _repeat = false;
		internal List<Point> Path { get; } =  [];
		static public void Reset()
		{
			_visited.Clear();
		}
		public string Key
		{
			get
			{
				return $"{_lastPt} to {Pt}";
			}
		}
		internal void Move(Point pt)
		{
			if (_visited.Contains(Key))
				_repeat = true;
			else
				_visited.Add(Key);
			Path.Add(pt);
			_lastPt = Pt;
			Pt = pt;
		}

		public Point Pt { get; private set; }
        public static int Rows { get; internal set; }
        public static int Cols { get; internal set; }

        Point _lastPt;
	}
}
