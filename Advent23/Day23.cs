using AoCLibrary;
using System.IO;
using System.Xml.Linq;

namespace Advent23
{
	internal class Day23 : IDayRunner
	{
        public bool IsReal => true;

		// Day https://adventofcode.com/2023/day/23
		// Input https://adventofcode.com/2023/day/23/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 94);
			else
				check = new StarCheck(key, 2282);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var grd = new Grid23(lines);
			var paths = new List<Path23>();
            paths.Add(new Path23(grd.GetStart()));
			var endNode = grd.GetEnd();

            while (paths.Any())
			{
                //grd.WriteLocal("all", paths);

                ElfHelper.DayLog($"{paths.Count()} {endNode.Steps}");
				var newPaths = new List<Path23>();
                foreach (var path in paths)
				{
                    //ElfHelper.DayLog($"{path.CurrentNode}");
                    var nearNodes = grd.Neighbors(path.CurrentNode).Where(n => n.Char != '#');
					foreach(var nearNode in nearNodes)
					{
						if (path.Contains(nearNode))
							continue;

                        if (path.CanBeNextStep(nearNode))
                            newPaths.Add(Path23.Copy(path, nearNode));
                    }
                }
				paths = newPaths.ToList()!;
			}
            grd.WriteLocal("all", paths);
            rv = endNode.Steps;
            check.Compare(rv);
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 154);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
            // magic
            var grd = new Grid23(lines);
            foreach(var node in grd.Values)
            {
                if (node.Char != '#')
                    node.SetChar('.');
            }
            var paths = new List<Path23>();
            paths.Add(new Path23(grd.GetStart()));
            var endNode = grd.GetEnd();
            grd.WriteBase("start");

            while (paths.Any())
            {
                grd.WriteLocal("all", paths);
                ElfHelper.DayLog($"{paths.Count()} {endNode.Steps}");
                var newPaths = new List<Path23>();
                foreach (var path in paths.OrderByDescending(p => p.Count()))
                {
                    //ElfHelper.DayLog($"{path.CurrentNode}");
                    var nearNodes = grd.Neighbors(path.CurrentNode).Where(n => n.Char != '#');
                    foreach (var nearNode in nearNodes)
                    {
                        if (path.Contains(nearNode))
                            continue;

                        if (path.CanBeNextStep(nearNode))
                            newPaths.Add(Path23.Copy(path, nearNode));
                    }
                }
                paths = newPaths.ToList()!;
            }
            grd.WriteLocal("all", paths);
            rv = endNode.Steps;
            //  5046 too low
            check.Compare(rv);
			return rv;
		}
	}
	public class Path23 : List<Node23>
	{
        public Path23(Node23 node)
        {
            Add(node);
        }
        public Path23(Path23 other)
        {
			AddRange(other);
        }
        internal static Path23 Copy(Path23 path, Node23 node)
        {
			var rv = new Path23(path);
            node.Steps = rv.Count();
            rv.Add(node);
			return rv;
        }

        public Node23 CurrentNode => this.Last();

        bool AreWeLonger(Node23 node)
        {
            return (node.Steps < this.Count());
        }

        internal bool CanBeNextStep(Node23 node)
        {
            if (node.Char == '.')
                return AreWeLonger(node);
            // must be slope
            var otherNodeDir = Point.OtherDir(node.GetDir());
            var ourDirection = CurrentNode.Pt.GetDir(node.Pt);
            //ElfHelper.DayLog($"o:{ourDirection} d:{node.GetDir()}");
            if (ourDirection == otherNodeDir)
                return false;
            return true;
        }

    }
    public class Node23 : Node
    {
        public Node23(Point pt, char c) : base(pt, c)
        {
        }
        public int Steps { get; set; }
		internal DirEnum GetDir()
		{
            if (Char == 'v')
                return DirEnum.South;
            else if (Char == '<')
                return DirEnum.West;
            else if (Char == '>')
                return DirEnum.East;
            else if (Char == '^')
                return DirEnum.North;
			else
                return DirEnum.NA;
        }
        public override string ToString()
        {
            return $"{base.ToString()} {Steps}";
        }
    }
	public class Grid23 : Grid<Node23>
	{
		public Grid23(IEnumerable<string> lines)
		{
			Init(GetNodes(lines));
		}
        internal Node23 GetStart()
        {
            //var row = NodesInRow(0);
            return Values.First(r => r.Char == '.')!;
        }
        internal Node23 GetEnd()
        {
            //var row = NodesInRow(0);
            return Values.Last(r => r.Char == '.')!;
        }
        public void WriteLocal(string tag, List<Path23> paths)
        {
            var lines = new List<string>();
            for (int row = 0; row < Rows; row++)
            {
                var parts = new List<string>();
                for (int col = 0; col < Cols; col++)
                {
                    var node = Find(new Point(row, col))!;
                    if (paths.Any(p => p.CurrentNode.Pt.Equals(node.Pt)))
                        parts.Add(" x ");
                    else if (node.Char == '.')
                        parts.Add($"{Utils.CompactNumber(node.Steps, 3)}");
                    else
                        parts.Add($" {node.Char} ");
                }
                lines.Add(string.Join("", parts));
            }
            ElfUtils.WriteLines("Base", tag, lines);
        }

    }
}
