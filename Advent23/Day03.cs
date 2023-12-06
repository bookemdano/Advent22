
using System.Diagnostics;
using AoCLibrary;

namespace Advent23
{
	internal class Symbol
	{
		public Symbol(int row, int col, char c)
		{
			Row = row;
			Col = col;
			C = c;
		}
		internal int Row { get; }
		internal int Col { get; }
		internal char C { get; }
		public override string ToString()
		{
			return $"{C} ({Row},{Col})";
		}
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		public override bool Equals(object? obj)
		{
			if (obj is Symbol other)
			{
				return C == other.C && Row == other.Row && Col == other.Col;
			}
			return false;
		}

		internal bool IsGear()
		{
			return C == '*';
		}
	}
	internal class Day03 : IDayRunner
	{
		private object? Star1()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star1, IsReal);
			for(int iRow = 0; iRow < lines.Length; iRow++)
			{
				var line = lines[iRow];
				int digitCol = -1;

				for (int iCol = 0; iCol < line.Length; iCol++)
				{
					var c = line[iCol];
					if (char.IsDigit(c))
					{
						if (digitCol == -1)
							digitCol = iCol;
					}
					else if (digitCol != -1)
					{
						var number = line.Substring(digitCol, iCol - digitCol);
						if (NearSymbol(lines, iRow, digitCol, number))
							rv += int.Parse(number);

						digitCol = -1;
					}
				}
				if (digitCol != -1)
				{
					var iCol = line.Length;
					var number = line.Substring(digitCol, iCol - digitCol);
					if (NearSymbol(lines, iRow, digitCol, number))
						rv += int.Parse(number);
				}
			}
			return rv;
			// 398242 too low
			// 535980 wrong
			// 531561 right!

		}

		private List<Symbol> NearSymbolCheck(string[] lines, int row, int firstCol, string number)
		{
			var rv = new List<Symbol>();
			var maxLen = (number.Length + 2) * 3;

			var startRow = row - 1;
			if (startRow < 0)
				startRow = 0;
			var endRow = row + 1;
			if (endRow >= lines.Length)
				endRow = lines.Length - 1;

			var startCol = firstCol - 1;
			if (startCol < 0)
				startCol = 0;
			var endCol = startCol + number.Length + 2;
			if (endCol >= lines[row].Length)
				endCol = lines[row].Length - 1;

			string check = string.Empty;
			for (int iRow = startRow; iRow <= endRow; iRow++)
			{
				for (int iCol = startCol; iCol < endCol; iCol++)
				{
					var c = lines[iRow][iCol];
					check += c;
					if (c != '.' && !char.IsDigit(c))
					{
						//ElfHelper.Log($"TRUE Checked {check} for {row} - {firstCol},{lastCol}");
						rv.Add(new Symbol(iRow, iCol, c));
					}
				}
				check += '|';
			}

			if (check.Length - 3 > maxLen)
				Utils.TestLog("off");

			Utils.TestLog($"{rv} Checked {check}({check.Length}) {number} for {row} - {firstCol}");
			return rv;
		}
		private bool NearSymbol(string[] lines, int row, int firstCol, string number)
		{
			var maxLen = (number.Length + 2) * 3;

			var startRow = row - 1;
			if (startRow < 0)
				startRow = 0;
			var endRow = row + 1;
			if (endRow >= lines.Length)
				endRow = lines.Length - 1;

			var startCol = firstCol - 1;
			if (startCol < 0)
				startCol = 0;
			var endCol = startCol + number.Length + 2;
			if (endCol >= lines[row].Length)
				endCol = lines[row].Length - 1;

			string check = string.Empty;
			var rv = false;
			for (int iRow = startRow; iRow <= endRow; iRow++)
			{
				for (int iCol = startCol; iCol < endCol; iCol++)
				{
					var c = lines[iRow][iCol];
					check += c;
					if (c != '.' && !char.IsDigit(c))
					{
						//ElfHelper.Log($"TRUE Checked {check} for {row} - {firstCol},{lastCol}");
						rv = true;
					}
				}
				check += '|';
			}

			if (check.Length - 3 > maxLen)
				Utils.TestLog("off");

			Utils.TestLog($"{rv} Checked {check}({check.Length}) {number} for {row} - {firstCol}");
			return rv;
		}

		private object? Star2()
		{
			var rv = 0;
			var lines = Program.GetLines(StarEnum.Star2, IsReal);
			Dictionary<Symbol, List<string>> dict = [];
			for (int iRow = 0; iRow < lines.Length; iRow++)
			{
				var line = lines[iRow];
				int digitCol = -1;

				for (int iCol = 0; iCol < line.Length; iCol++)
				{
					var c = line[iCol];
					if (char.IsDigit(c))
					{
						if (digitCol == -1)
							digitCol = iCol;
					}
					else if (digitCol != -1)
					{
						var number = line.Substring(digitCol, iCol - digitCol);
						var symbols = NearSymbolCheck(lines, iRow, digitCol, number);
						foreach (var symbol in symbols.Where(s => s.IsGear()))
						{
							if (!dict.ContainsKey(symbol))
								dict.Add(symbol, new List<string>());
							dict[symbol].Add(number);
						}
						digitCol = -1;
					}
				}
				if (digitCol != -1)
				{
					var iCol = line.Length;
					var number = line.Substring(digitCol, iCol - digitCol);
					var symbols = NearSymbolCheck(lines, iRow, digitCol, number);
					foreach(var symbol in symbols.Where(s => s.IsGear()))
					{
						if (!dict.ContainsKey(symbol))
							dict.Add(symbol, new List<string>());
						dict[symbol].Add(number);
					}
				}
			}
			long lrv = 0;
			foreach(var kvp in dict.Where(kvp => (kvp.Key.IsGear() && kvp.Value.Count() > 1)))
			{
				Debug.Assert(kvp.Value.Count() == 2);
				var gr = int.Parse(kvp.Value[0]) * int.Parse(kvp.Value[1]);
				lrv += gr;
			}
			return lrv;
			// 83279367
		}

		public bool IsReal
		{
			get
			{
				return true;
			}
		}

		public RunnerResult Run()
		{
			var rv = new RunnerResult();
			rv.Star1 = Star1();
			rv.Star2 = Star2();
			return rv;
		}
	}
}
