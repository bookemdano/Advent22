using AoCLibrary;
using Microsoft.VisualBasic;

namespace Advent23
{
	internal class Day21 : IDayRunner
	{
		public bool IsReal => false;

		// Day https://adventofcode.com/2023/day/21
		// Input https://adventofcode.com/2023/day/21/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 16);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			var grd = new Grid21(lines);

			var start = grd.Find('S')!;
			var nodes = new List<Node>();
			int steps = 64;
			if (IsReal == false)
				steps = 6;
			nodes.Add(start);
			for(int i = 0; i < steps; i++)
			{
				var newNodes = new List<Node>();
				foreach(var node in nodes)
				{
					var nearbys = node.Neighbors();
					foreach (var nearbyPt in nearbys)
					{
						if (newNodes.Any(n => n.Pt.Equals(nearbyPt)))
							continue;
						var nearbyNode = grd.Find(nearbyPt);
						if (nearbyNode == null)
							continue;
						if (nearbyNode.Char == '#')
							continue;
						newNodes.Add(nearbyNode);
					}
				}
				nodes = newNodes;
				grd.WriteLocal("step", nodes);
			}
			rv = nodes.Count();
			check.Compare(rv);
			// 3776
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
			var grd = new Grid21(lines);

			var start = grd.Find('S')!;
			var pts = new List<Point>();
			var startCycleIndex = 58;
			var delta2 = 162;
			//int steps = startCycle - 1;
			pts.Add(start.Pt);
			var cycleSize = grd.Rows;
			var scores = new List<long>();
			var deltas = new List<long>();
			for (int i = 0; i < startCycleIndex; i++)
			{
				var newPts = new List<Point>();
				foreach (var node in pts)
				{
					var nearbys = node.Neighbors();
					foreach (var nearbyPt in nearbys)
					{
						if (newPts.Any(n => n.Equals(nearbyPt)))
							continue;
						var nearbyNode = grd.FindInfinite(nearbyPt);
						Utils.Assert(nearbyNode != null, "should always find node");
						if (nearbyNode.Char == '#')
							continue;
						newPts.Add(nearbyPt);
					}
				}
				pts = newPts;
				var currentScore = pts.Count();
				scores.Add(currentScore);
				if (scores.Count() > cycleSize)
				{
					var delta = currentScore - scores[i - cycleSize];
					deltas.Add(delta);
					ElfHelper.DayLog($"{i + 1} d:{delta}");
				}
				else
					deltas.Add(0);
				ElfHelper.DayLog($"{i + 1},{pts.Count()} ({pts.Min(p => p.Row)},{pts.Min(p => p.Col)})-({pts.Max(p => p.Row)},{pts.Max(p => p.Col)})");
				//grd.WriteLocal("step", nodes);

			}
			var preCycleCount = pts.Count();

			var target = 500;
			rv = preCycleCount;
			for (int i = startCycleIndex; i < target; i++)
			{
				//
				//var backI = (i - startCycle) % cycle + (startCycle - cycle);
				var backI = i - cycleSize;
				var delta = deltas[backI] + delta2;
				var score = delta + scores[backI];
				deltas.Add(delta);
				scores.Add(score);
				//ElfHelper.DayLog($"{i + 1} s:{scores} d:{delta}");
				rv = scores[i];
			}
			var oldrv = rv;
			var inCycleLength = target - startCycleIndex;
			var cycles = inCycleLength/cycleSize;
			rv = preCycleCount + (cycles * (delta2* cycleSize));

			//rv = pts.Count();
			check.Compare(rv);
			return rv;
		}
	}
	public class Node21 : Node
	{
		public Node21(Point pt, char c) : base(pt, c)
		{
		}
	}
	public class Grid21 : Grid<Node21>
	{
		public Grid21(IEnumerable<string> lines)
		{
			Init(GetNodes(lines));
		}
		internal Node21 FindInfinite(Point pt)
		{
			if (IsValid(pt))
				return Find(pt)!;

			var modCol = Math.Abs(pt.Col) % Cols;
			var modRow = Math.Abs(pt.Row) % Rows;

			if (pt.Row < 0 && modRow != 0)
				modRow = Rows - modRow;
			if (pt.Col < 0 && modCol != 0)
				modCol = Cols - modCol;
			return Find(new Point(modRow, modCol))!;
		}
		public void WriteLocal(string tag, List<Node> nodes)
		{
			var lines = new List<string>();
			for (int row = 0; row < Rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < Cols; col++)
				{
					var node = Find(new Point(row, col))!;
					if (nodes.Any(n => n.Pt.Equals(node.Pt)))
						parts.Add("O");
					else
						parts.Add($"{node.Char}");
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Base", tag, lines);
		}
	}
}
