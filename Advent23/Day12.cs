using AoCLibrary;

namespace Advent23
{
	internal class Day12 : IDayRunner
	{
		public bool IsReal => false;
		// Day https://adventofcode.com/2023/day/12
		// Input https://adventofcode.com/2023/day/12/input
		public object? Star1()
		{
			var rv = 0L;
			StarCheck check;
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			if (IsReal)
				check = new StarCheck(key, 7025L);
			else
				check = new StarCheck(key, 21L);
			
			var lines = Program.GetLines(check.Key);
			foreach(var line in lines)
			{
				var rec = new Record11(line, 1);
				if (rec.Check(rec.Record))
					rv++;
				else
				{
					//ElfHelper.DayLog($"{rec} {rec.Record.Length}");
					var found = rec.Options();
					/*foreach(var option in options)
						if (rec.Check(option))
							found++;*/
					ElfHelper.DayLog($"{rec} f:{found}");
					rv += found;
				}
			}
			ElfHelper.DayLog($"{check} v:{rv}");
			check.Compare(rv);
			//rv = 7025
			return rv;
		}
		public object? Star2()
		{
			var rv = 0L;
			StarCheck check;
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			if (IsReal)
				check = new StarCheck(key, 11461095383315L);
			else
				check = new StarCheck(key, 525152L);

			var lines = Program.GetLines(check.Key);
			var iLine = 0;
			var multiplier = 5;
			ElfHelper.DayLog($"{check} m:{multiplier}");
			foreach (var line in lines)
			{
				var rec = new Record11(line, multiplier);
				if (rec.Check(rec.Record))
					rv++;
				else
				{
					ElfHelper.DayLog($"{++iLine} {rec} c:{rec.Record.Count(r => r == '?')}");
					var found = rec.Options();
					ElfHelper.DayLog($"{iLine} {rec} f:{found}");
					rv += found;
				}
			}
			ElfHelper.DayLog($"{check} v:{rv}");
			check.Compare(rv);
			return rv;
		}

	}
	public class Record11
	{
		public long[] OrigNums { get; }
		public long[] Nums { get; }
		public string OrigRecord { get; }
		public string Record { get; }
		//public long Sum { get; }

		public Record11(string line, int multiple)
		{
			var parts = Utils.Split(' ', line);
			var nums = new List<long>();
			OrigNums = Utils.SplitLongs(',', parts[1]);
			OrigRecord = parts[0];
			var copies = new List<string>();
			for (int i = 0; i < multiple; i++)
			{
				nums.AddRange(OrigNums);
				copies.Add(OrigRecord);
			}
			Record = string.Join("?", copies);
			Record = Record.Replace("..", ".").Replace("..", ".").Replace("..", ".").Replace("..", ".");
			Nums = nums.ToArray();
			//var sures = Record.Count(r => r == '#');
			//Sum = Nums.Sum(n => n) - sures;
		}
		public bool Check(char[] record)
		{
			return Check(string.Join("", record));
		}
		public bool Check(string record)
		{
			var parts = Utils.Split('.', record);
			if (parts.Length != Nums.Length)
				return false;

			for (int i = 0; i < Nums.Length; i++)
			{
				if (parts[i].Length != Nums[i])
					return false;
			}
			return true;
		}

		internal long MaxOptions()
		{
			var unknowns = Record.Count(r => r == '?');
			if (unknowns <= 2)
				return unknowns;
			var rv = 2L;
			for (int i = 3; i <= unknowns; i++)
				rv *= i;
			return rv;
		}
		public override string ToString()
		{
			return $"{OrigRecord} {string.Join(",", OrigNums)}";
		}
		internal List<string> UkOptions(string stub, long uks, long hits)
		{
			var rv = new List<string>();
			if (uks < 0 || hits < 0)
				return rv;
			if (uks == 0)
			{
				if (hits == 0)
					rv.Add(stub);
				return rv;
			}
			if (uks >= hits)
				rv.AddRange(UkOptions(stub + "#", uks - 1, hits - 1));
			rv.AddRange(UkOptions(stub + ".", uks - 1, hits));
			return rv;
		}
		bool CheckSoFar(string record, CheckCurrent cur)
		{
			var iNum = cur.INum;
			var numSize = 0;
			bool inNum = false;
			bool badPattern = false;
			for(int i = cur.Index; i < record.Length; i++)
			//foreach(var c in record)
			{
				var c = record[i];
				if (c == '?')
				{
					if (inNum && Nums[iNum] < numSize)
						badPattern = true;	// we are already over

					inNum = false;	// don't check pattern any more
					break;
				}

				if (c == '.')
				{
					if (inNum == true)
					{
						inNum = false;
						if (Nums[iNum++] != numSize)
						{
							badPattern = true;
							break;
						}
					}
					cur.Index = i;
					numSize = 0;
				}
				else if (c == '#')
				{
					if (inNum == false)
					{
						inNum = true;
						numSize = 0;
					}
					numSize++;
				}
			}
			cur.INum = iNum;
			if (inNum == true)
			{
				if (Nums[iNum] != numSize)
					badPattern = true;
			}
			return !badPattern;
		}
		//Dictionary<string, int> _cons = [];
		internal long BuildOptions(string record, long unknowns, long hitsToPlace, CheckCurrent cur)
		{
			//_cons.Add(record, 0);

			if (unknowns < 0 || hitsToPlace < 0)
			{
				//_cons[record] = -1;
				//_cons.Remove(record);
				return 0;
			}
			if (!CheckSoFar(record, cur))
			{
				//_cons[record] = -2;
				//_cons.Remove(record);
				return 0;
			}
			if (unknowns == 0)
			{
				if (hitsToPlace == 0)
				{
					//_cons[record] = 3;
					//_cons.Remove(record);
					// if (Check(record)) already checked
					return 1;
				}
				else
				{
					//_cons[record] = -4;
					//_cons.Remove(record);
					return 0;
				}
			}
			//_cons[record] = 5;
			var next = record.IndexOf('?');
			if (hitsToPlace == 0)
				return BuildOptions(record.Replace('?', '.'), 0, hitsToPlace, new CheckCurrent(cur));

			long rv = 0;
			var key = CheckKey(record, cur);
			if (_cache.ContainsKey(key))
				return _cache[key];

			rv += BuildOptions(ReplaceChar(record, next, '.'), unknowns - 1, hitsToPlace, new CheckCurrent(cur));
			if (unknowns >= hitsToPlace && hitsToPlace > 0)
				rv += BuildOptions(ReplaceChar(record, next, '#'), unknowns - 1, hitsToPlace - 1, new CheckCurrent(cur));
			if (cur.Index > 10)
				_cache[key] = rv;
			return rv;
		}
		Dictionary<string, long> _cache = new Dictionary<string, long>();
		string CheckKey(string record, CheckCurrent cur)
		{
			return $"{record.Substring(cur.Index)} {cur}";
		}
		static string ReplaceChar(string srce, int index, char c)
		{
			var array = srce.ToCharArray();
			array[index] = c;
			return string.Join("", array);
		}
		internal long Options()
		{
			//_cons.Clear();
			var unknowns = Record.Count(r => r == '?');
			var toPlace = Nums.Sum(n => n) - Record.Count(r => r == '#');
			var rv = BuildOptions(Record, unknowns, toPlace, new CheckCurrent());
			return rv;
		}
		internal List<string> BulkOptions()
		{
			var len = Record.Length;
			var unknowns = Record.Count(r => r == '?');
			var toPlace = Nums.Sum(n => n) - Record.Count(r => r == '#');
			var ops = UkOptions("", unknowns, toPlace);
			var max = Math.Pow(2, unknowns);
			var rv = new List<string>();
			ElfHelper.DayLog($"{this} m:{max}");
			for (long i = 0; i < max; i++)
			{
				var bin = Convert.ToString(i, 2);

				if (bin.Count(c => c == '1') != toPlace)
					continue;
				while (bin.Length < unknowns)
					bin = "0" + bin;

				var record = Record.ToCharArray();
				var j = 0;
				for (int iChar = 0; iChar < len; iChar++)
				{
					var c = record[iChar];
					if (c == '#' || c == '.')
						continue;
					else if (c == '?')
						record[iChar] = (bin[j++] == '1' ? '#' : '.');
				}
				var str = string.Join("", record);
				if (!rv.Contains(str))
					rv.Add(str);
				else
					rv.Add(str);
			}
			return rv;
		}
		internal List<string> BruteOptions()
		{
			var unknowns = Record.Count(r => r == '?');
			var sures = Record.Count(r => r == '#');
			var toPlace = Nums.Sum(n => n) - sures;

			var rv = new List<string>();
			var max = MaxOptions();
			ElfHelper.DayLog($"{this} {max}");
			for (int i = 0; i < max; i++)
			{
				var bin = Convert.ToString(i, 2);
				if (bin.Count(c => c == '1') != toPlace)
					continue;
				while (bin.Length < unknowns)
					bin = "0" + bin;
				int j = 0;
				var record = string.Empty;
				for (int iChar = 0; iChar < Record.Length; iChar++)
				{
					if (Record[iChar] == '?')
						record += (bin[j++] == '1'?'#':'.');
					else
						record += Record[iChar];
				}
				if (!rv.Contains(record))
					rv.Add(record);
			}
			return rv;
		}


	}
	internal class CheckCurrent
	{
		public CheckCurrent()
		{
			
		}
		public CheckCurrent(CheckCurrent other)
		{
			Index = other.Index;
			INum = other.INum;
		}
		public override string ToString()
		{
			return $"o:{Index} n:{INum}";
		}

		public int Index { get; set; }
		public int INum { get; set; }
	}
}
