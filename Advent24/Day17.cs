using AoCLibrary;
namespace Advent24;

internal class Day17 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/17
	// Input https://adventofcode.com/2024/day/17/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, "4,6,3,5,6,3,5,2,1,0");
		else
			check = new StarCheck(key, "1,2,3,1,3,2,5,3,1");

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = string.Empty;
		// magic
		var prog = new Prog17(lines);
		if (IsReal == false)
		{
			Prog17 t;
			t = Prog17.Test(0, 2024, 43690, new List<long>() { 4, 0 });
			Utils.Assert(t.B == 44354, "t5");

			t = Prog17.Test(0, 29, 0, new List<long>() { 1, 7 });
			Utils.Assert(t.B == 26, "t4");

			t = Prog17.Test(2024, 0, 0, new List<long>() { 0, 1, 5, 4, 3, 0 });
			Utils.Assert(t.OutString == "4,2,5,6,7,7,7,7,3,1,0", "t3 out");
			Utils.Assert(t.A == 0, "t3");

			t = Prog17.Test(0, 0, 9, new List<long>() { 2, 6 });
			Utils.Assert(t.B == 1, "t1");
			t = Prog17.Test(10, 0, 0, new List<long>() { 5, 0, 5, 1, 5, 4 });
			Utils.Assert(t.OutString == "0,1,2", "t2");

		}

		while (prog.Step()) ;

		ElfHelper.DayLog(prog.OutString);
		rv = prog.OutString;
		// not 7,6,1,1,5,1,3,3,1
		// 1,2,3,1,3,2,5,3,1
		check.Compare(rv);
		return rv;
	}
	public class Prog17
	{
		public long[] Prog { get; set; }
		public long A { get; set; }
		public long B { get; set; }
		public long C { get; set; }
		int _ip = 0;
		public List<long> Outs { get; set; } = [];
		public string OutString => string.Join(',', Outs);
		int _iStep;

		private Prog17()
		{
			
		}
		public Prog17(string[] lines)
		{
			A = Utils.SplitLongs(":".ToCharArray(), lines[0])[0];
			B = Utils.SplitLongs(":".ToCharArray(), lines[1])[0];
			C = Utils.SplitLongs(":".ToCharArray(), lines[2])[0];
			Prog = Utils.SplitLongs(":,".ToCharArray(), lines[4]);
		}

		public Prog17(Prog17 prog, long a)
		{
			A = a;
			B = prog.B;
			C = prog.C;
			//Outs = [];
			Prog = prog.Prog.ToArray();
			_iStep = prog._iStep;
			_ip = prog._ip;
		}

		public override string ToString()
		{
			return $"{A},{B},{C} {OutString} {_iStep}";
		}

		public bool Step()
		{
			_iStep++;	
			if (_ip + 1 >= Prog.Count())
				return false;
			var ins = Prog[_ip];
			var lopd = Prog[_ip + 1];
			var copd = lopd;
			if (lopd == 4)
				copd = A;
			else if (lopd == 5)
				copd = B;
			else if (lopd == 6)
				copd = C;
			//else if (lopd == 7)
			//	Utils.Assert(lopd != 7, $"{lopd} != 7");

			var newIp = _ip;
			if (ins == 0)
				A = (long)(A / (Math.Pow(2, copd)));
			else if (ins == 1)
				B = (long)(B ^ lopd);
			else if (ins == 2)
				B = (long)(copd % 8);
			else if (ins == 3)
			{
				if (A != 0)
					newIp = (int)lopd;
			}
			else if (ins == 4)
				B = (long)(B ^ C);
			else if (ins == 5)
				Outs.Add(copd % 8);
			else if (ins == 6)
				B = (long)(A / (Math.Pow(2, copd)));
			else if (ins == 7)
			{
				var p = Math.Pow(2, copd);
				var c = (double) A / (Math.Pow(2, copd));
				C = (long)(A / (Math.Pow(2, copd)));
			}

			if (newIp != _ip)
				_ip = newIp;
			else
				_ip += 2;
			return true;
		}

		internal static Prog17 Test(long a, long b, long c, List<long> prog)
		{
			var rv = new Prog17();
			rv.A = a; rv.B = b; rv.C = c;
			rv.Prog = prog.ToArray();
			while (rv.Step()) ;
			return rv;
		}
		internal bool Solve(bool checkPartial)
		{
			while (Step())
			{
				if (checkPartial && !PartialCompare(Prog, Outs))
					return false;
			}
			return true;

		}
		bool PartialCompare(long[] target, List<long> current)
		{
			if (current.Count == 0)
				return true;
			if (target.Count() < current.Count)
				return false;
			for (int i = 0; i < current.Count; i++)
			{
				if (target[i] != current[i])
					return false;
			}
			return true;
		}
	}
	bool FullCompare(long[] target, List<long> current)
	{
		if (target.Count() != current.Count)
			return false;
		for (int i = 0; i < current.Count; i++)
		{
			if (target[i] != current[i])
				return false;
		}
		return true;
	}
	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 117440L);
		else
			check = new StarCheck(key, 105706277661082L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		if (IsReal == false)
		{
			Prog17 t;
			t = Prog17.Test(117440, 0, 0, new List<long>() { 0, 3, 5, 4, 3, 0 });
			Utils.Assert(t.OutString == "0,3,5,4,3,0", "t2.1");
		}
		var oProg = new Prog17(lines);

		var next = DateTime.Now;
		var targetLast = oProg.Prog.Last();
		var targetNextToLast = oProg.Prog[oProg.Prog.Length - 2];
		var count2 = 0;
		for (long p = 0; p < 100; p += 3)
		{
			var end = (long) Math.Pow(2, p + 2);
			var count = (long)Math.Pow(2, p);
			var start = end - count;
			start = 0;
			end = long.MaxValue;

			for (var a = start; a < end; a++)
			{

				var tProg = new Prog17(oProg, a);
				if (tProg.Solve(false))
				{
					if (FullCompare(tProg.Prog, tProg.Outs))
					{
						ElfHelper.DayLog(tProg);
						rv = a;
						break;
					}
				}
				//ElfHelper.DayLogPlus($"a:{a} l:{last} o:{tProg.OutString}");
				var last = tProg.Outs.Last();
				if (!tProg.OutString.EndsWith(",3,0"))
				{
					if (count2 != 0)
						ElfHelper.DayLogPlus($"a:{a} o:,3,0 c:{count2}");
					count2 = 0;
				}
				else
					count2++;

				if (DateTime.Now > next)
				{
					//ElfHelper.DayLogPlus($"a:{a} c:{count}");
					//Console.WriteLine($"{a}");
					next = DateTime.Now.AddSeconds(5);
				}

			}

		}

		/*
		 * 		var final = 0L;
		var count = 0L;
		var max = 0L;
for (var a = 0L; a < long.MaxValue; a++)
		{
			var tProg = new Prog17(oProg, a);//i
			var fail = false;
			tProg.Solve();
			//ElfHelper.DayLogPlus($"a:{a} mod:{a % 8} {tProg}");
			var last = tProg.Outs.Last();

			//ElfHelper.DayLogPlus($"a:{a} l:{last} {tProg}");

			if (last != targetLast)
			{
				if (count > 0)
				{
					if (max < count)
						max = count;
					ElfHelper.DayLogPlus($"from:{a - count} - {a-1} m:{max}");
					count = 0;
					a += max - 1;
				}
				continue;
			}
			count++;

			//ElfHelper.DayLogPlus($"{a}, $\"{{a}}, {tProg.Outs.Last()}");

			/*{
				if (!PartialCompare(tProg.Prog, tProg.Outs))
				{
					fail = true;
					break;
				}
			}
			if (!fail && FullCompare(tProg.Prog, tProg.Outs))
			{
				ElfHelper.DayLog(tProg);
				rv = a;
				break;
			}
			if (DateTime.Now > next)
			{
				//ElfHelper.DayLogPlus($"a:{a} m:{max}");
				//Console.WriteLine($"{a}");
				next = DateTime.Now.AddSeconds(5);
			}
		}*/

		check.Compare(rv);
		return rv;
	}
}

