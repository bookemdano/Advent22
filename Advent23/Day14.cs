using AoCLibrary;

namespace Advent23
{
	internal class Day14 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/14
		// Input https://adventofcode.com/2023/day/14/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			var checks = new List<StarCheck>();
			if (IsReal)
				checks.Add(new StarCheck(key, 108935L));
			else
			{
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal, 0), 136L));
				//checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal, 1), 64L));
			}

			var rv = 0L;
			foreach (var check in checks)
			{
				rv = 0L;
				var lines = Program.GetLines(check.Key);
				// magic
				var grd = Grid14.FromLines(lines);
				rv = grd.Support();

				check.Compare(rv);
				// 		rv	108935	long

			}
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (IsReal)
				check = new StarCheck(key, 0L);
			else
				check = new StarCheck(key, 64L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
			//var cycles = 1000000000L;
			var cycles = 1L;
			var grd = Grid14.FromLines(lines);
			grd.TiltEast();
			grd.WriteLocal("tilted");


			for(int i = 0; i < cycles; i++)
			{
				grd.TiltNorth();
				ElfHelper.DayLog(grd);
				grd.TiltWest();
				ElfHelper.DayLog(grd);
				grd.TiltSouth();
				ElfHelper.DayLog(grd);
				grd.TiltEast();
				ElfHelper.DayLog(grd);
			}
			grd.WriteLocal("tilted");

			check.Compare(rv);
			return rv;
		}
	}
	public class Grid14 : Grid<Node>
	{
		Dictionary<int, int> _rowSizes = [];
		Dictionary<int, int> _colSizes = [];
		List<Point> _sprites = [];
		public Grid14(List<Node> nodes)
		{
			var os = nodes.Where(n => n.Char == 'O').ToList();
			foreach (var node in os)
			{
				_sprites.Add(node.Pt);
				node.SetChar('.');
			}

			Init(nodes);
		}
		public override string ToString()
		{
			return $"({_rows}, {_cols}) s:{_sprites.Count()}";
		}
		internal static Grid14 FromLines(string[] lines)
		{
			return new Grid14(GetNodes(lines));
		}
		internal void TiltNorth()
		{
			var spritePts = _sprites.ToList();
			foreach (var spritePt in spritePts)
			{
				int lastRow = spritePt.Row;
				var moved = false;
				for (int iRow = spritePt.Row - 1; iRow >= 0; iRow--)
				{
					var c = FindAny(new Point(iRow, spritePt.Col));
					if (c == 'O' || c == '#')
					{
						MoveSprite(spritePt, new Point(lastRow, spritePt.Col));
						moved = true;
						break;
					}
					else if (c == '.')
						lastRow = iRow;
				}
				if (!moved)
					MoveSprite(spritePt, new Point(lastRow, spritePt.Col));

				//WriteLocal("tilting");
			}
		}
		internal void TiltWest()
		{
			var spritePts = _sprites.ToList();
			foreach (var spritePt in spritePts)
			{
				int lastCol = spritePt.Col;
				var moved = false;
				for (int iCol = spritePt.Col - 1; iCol >= 0; iCol--)
				{
					var c = FindAny(new Point(spritePt.Row, iCol));
					if (c == 'O' || c == '#')
					{
						MoveSprite(spritePt, new Point(spritePt.Row, lastCol));
						moved = true;
						break;
					}
					else if (c == '.')
						lastCol = iCol;
				}
				if (!moved)
					MoveSprite(spritePt, new Point(spritePt.Row, lastCol));

				//WriteLocal("tilting");
			}
		}
		internal void TiltSouth()
		{
			var spritePts = _sprites.ToList();
			spritePts.Reverse();
			foreach (var spritePt in spritePts)
			{
				int lastPos = spritePt.Row;
				var moved = false;
				for (int iRow = spritePt.Row + 1; iRow < _rows; iRow++)
				{
					var c = FindAny(new Point(iRow, spritePt.Col));
					if (c == 'O' || c == '#')
					{
						MoveSprite(spritePt, new Point(lastPos, spritePt.Col));
						moved = true;
						break;
					}
					else if (c == '.')
						lastPos = iRow;
				}
				if (!moved)
					MoveSprite(spritePt, new Point(lastPos, spritePt.Col));

				//WriteLocal("tilting");
			}
		}
		internal void TiltEast()
		{
			var spritePts = _sprites.ToList();
			spritePts.Reverse();
			for(int iSprite = 0; iSprite < _sprites.Count();  iSprite++)
			{
				var spritePt = spritePts[iSprite];
				while (true)
				{
					if (FindAny(new Point(spritePt.Row, spritePt.Col + 1)) == '.')
						spritePt = new Point(spritePt.Row, spritePt.Col + 1);
					else
						break;
				}
				MoveSprite(spritePts[iSprite], spritePt);
			}
		}
		void MoveSprite(Point oldPt, Point newPt)
		{
			if (newPt.Equals(oldPt))
				return;
			Utils.Assert(_sprites.Remove(oldPt), "no old one?");
			_sprites.Add(newPt);
		}
		protected char? FindAny(Point pt)
		{
			if (_sprites.Contains(pt))
				return 'O';
			
			return Find(pt)?.Char;
		}


		internal long Support()
		{
			var rv = 0L;
			var allCols = AllCols();
			foreach (var colNodes in allCols)
			{
				var loadRow = _rows;
				var iRow = _rows;
				foreach (var node in colNodes)
				{
					if (_sprites.Contains(node.Pt))
					{
						rv += loadRow;
						loadRow--;
					}
					else if (node.Char == '#')
					{
						loadRow = iRow - 1;
					}
					//else if (node.Char == '.' && subSupport)
					//	break;
					iRow--;
				}
			}
			return rv;
		}
		public void WriteLocal(string tag)
		{
			var lines = new List<string>();
			for (int row = 0; row < _rows; row++)
			{
				var parts = new List<string>();
				for (int col = 0; col < _cols; col++)
				{
					if (_sprites.Contains(new Point(row, col)))
						parts.Add("O");
					else
						parts.Add(Find(new Point(row, col))!.Char.ToString());
				}
				lines.Add(string.Join(",", parts));
			}
			ElfUtils.WriteLines("Base", tag, lines);
		}

	}
}
