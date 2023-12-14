using AoCLibrary;
using System.Xml.Linq;

namespace Advent23
{
	internal class Day14 : IDayRunner
	{
		public bool IsReal => true;
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
				var grd = new Grid14(lines);
				var sl = grd.SupportLong();
				grd.TiltNorth();
				rv = grd.Support();
				Utils.Assert(rv, sl);
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
			var cycles = 1000000000L;

			int patternStartIndex = 1;
			var patternSize = 7;

			var grd = new Grid14(lines);
			var scores = new List<int>();

			if (IsReal)
			{
				// save from previous run- took  2-5 minutes.
				scores = new List<int>() { { 100593 }, { 100318 }, { 100211 }, { 100017 }, { 99841 }, { 99759 }, { 99648 }, { 99611 }, { 99569 }, { 99528 }, { 99432 }, { 99402 }, { 99421 }, { 99471 }, { 99504 }, { 99497 }, { 99506 }, { 99559 }, { 99602 }, { 99651 }, { 99678 }, { 99720 }, { 99728 }, { 99766 }, { 99781 }, { 99809 }, { 99829 }, { 99858 }, { 99853 }, { 99875 }, { 99880 }, { 99864 }, { 99845 }, { 99829 }, { 99812 }, { 99776 }, { 99726 }, { 99677 }, { 99651 }, { 99642 }, { 99661 }, { 99688 }, { 99719 }, { 99766 }, { 99811 }, { 99851 }, { 99891 }, { 99957 }, { 100028 }, { 100074 }, { 100081 }, { 100101 }, { 100121 }, { 100125 }, { 100098 }, { 100087 }, { 100066 }, { 100072 }, { 100087 }, { 100105 }, { 100135 }, { 100147 }, { 100166 }, { 100206 }, { 100246 }, { 100284 }, { 100316 }, { 100338 }, { 100373 }, { 100402 }, { 100428 }, { 100471 }, { 100521 }, { 100572 }, { 100620 }, { 100665 }, { 100693 }, { 100732 }, { 100767 }, { 100792 }, { 100821 }, { 100859 }, { 100900 }, { 100940 }, { 100959 }, { 100961 }, { 100956 }, { 100946 }, { 100915 }, { 100876 }, { 100839 }, { 100778 }, { 100707 }, { 100635 }, { 100581 }, { 100551 }, { 100518 }, { 100487 }, { 100481 }, { 100475 }, { 100459 }, { 100463 }, { 100467 }, { 100476 }, { 100481 }, { 100485 }, { 100501 }, { 100521 }, { 100542 }, { 100574 }, { 100613 }, { 100654 }, { 100701 }, { 100740 }, { 100783 }, { 100821 }, { 100859 }, { 100900 }, { 100940 }, { 100959 }, { 100961 }, { 100956 }, { 100946 }, { 100915 }, { 100876 }, { 100839 }, { 100778 }, { 100707 }, { 100635 }, { 100581 }, { 100551 }, { 100518 }, { 100487 }, { 100481 }, { 100475 }, { 100459 }, { 100463 }, { 100467 }, { 100476 }, { 100481 }, { 100485 }, { 100501 }, { 100521 }, { 100542 }, { 100574 }, { 100613 }, { 100654 }, { 100701 }, { 100740 }, { 100783 }, { 100821 }, { 100859 }, { 100900 }, { 100940 }, { 100959 }, { 100961 } };
				/*
				for (int i = 0; i < minPattern; i++)
				{
					grd.TiltNorth();
					grd.TiltWest();
					grd.TiltSouth();
					grd.TiltEast();
					int score = grd.Support();
					ElfHelper.DayLog($"{i} g:{grd} s:{score} l:{scores.FindIndex(s => s == score)}");
					scores.Add(score);
				}
				*/
				patternStartIndex = 80;
				patternSize = 35;
				/*for (int c = 115; c < 150; c++)
				{
					var goal = (int)((c - patternStartIndex) % patternSize) + patternStartIndex;
					ElfHelper.DayLog($"c:{c} g:{goal} s:{scores[goal]} aC:{c} aS:{scores[c]}");
				}
				*/
				//ElfHelper.DayLog($"c:{cycles} g:{goalTotal} s:{scores[goalTotal]}");
			}
			else
			{
				int minPattern = 100;
				for (int i = 0; i < minPattern; i++)
				{
					grd.TiltNorth();
					grd.TiltWest();
					grd.TiltSouth();
					grd.TiltEast();
					int score = grd.Support();
					ElfHelper.DayLog($"{i} g:{grd} s:{score} l:{scores.FindIndex(s => s == score)}");
					scores.Add(score);
				}

				patternStartIndex = 1;
				patternSize = 7;
				var goal = (int)((cycles) % patternSize) - patternStartIndex + patternSize;
				//ElfHelper.DayLog($"c:{cycles} l:{lastScore} g:{goal} s:{scores[goal]} b:{scores.IndexOf(lastScore)}");
				//Utils.Assert(lastScore, scores[goal]);
			}
			var iGoal = ((int)(cycles - patternStartIndex) % patternSize) + patternStartIndex - 1;
			rv = scores[iGoal];
			//100839 too low
			//100876

			check.Compare(rv);
			return rv;
		}
	}
	public class Grid14 : GridPlain
	{
		List<Point> _sprites = [];
		private int _spriteCount;

		public Grid14(string[] lines)
		{
			var nodes = GetNodes(lines);
			var os = nodes.Where(n => n.Char == 'O').ToList();
			foreach (var node in os)
			{
				_sprites.Add(node.Pt);
				node.SetChar('.');
			}
			_spriteCount = _sprites.Count();

			Init(nodes);
		}

		public override string ToString()
		{
			return $"{base.ToString()} s:{_spriteCount}";
		}
		internal void TiltNorth()
		{
			TiltNorthSouth(-1);
		}
		internal void TiltSouth()
		{
			TiltNorthSouth(1);
		}
		void TiltNorthSouth(int offset)
		{
			var spritePts = _sprites.OrderBy(s => s.Row).ToList();
			if (offset > 0)
				spritePts.Reverse();
			for (int iSprite = 0; iSprite < _spriteCount; iSprite++)
			{
				var spritePt = spritePts[iSprite];
				while (true)
				{
					if (FindAny(new Point(spritePt.Row + offset, spritePt.Col)) == '.')
						spritePt = new Point(spritePt.Row + offset, spritePt.Col);
					else
						break;
				}
				MoveSprite(spritePts[iSprite], spritePt);
			}
		}
		void TiltEastWest(int offset)
		{
			var spritePts = _sprites.OrderBy(s => s.Col).ToList();
			if (offset > 0)
				spritePts.Reverse();
			for (int iSprite = 0; iSprite < _spriteCount; iSprite++)
			{
				var spritePt = spritePts[iSprite];
				
				while (true)
				{
					var newPt = new Point(spritePt.Row, spritePt.Col + offset);
					if (FindAny(newPt) == '.')
						spritePt = newPt;
					else
						break;
				}
				MoveSprite(spritePts[iSprite], spritePt);
			}
		}
		internal void TiltEast()
		{
			TiltEastWest(1);
		}
		internal void TiltWest()
		{
			TiltEastWest(-1);
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

		internal int Support()
		{
			var rv = 0;
			foreach(var sprite in _sprites)
				rv += _rows - sprite.Row;
			return rv;
		}
		// don't need to tiltnorth first
		internal int SupportLong()
		{
			var rv = 0;
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
