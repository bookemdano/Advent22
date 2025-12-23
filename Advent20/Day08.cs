using AoCLibrary;

namespace Advent20;

internal class Day08 : IRunner
{
	// Day https://adventofcode.com/2020/day/8
	// Input https://adventofcode.com/2020/day/8/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 5L);
		else
			res.Check = new StarCheck(key, 1727L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
        // magic
        var ops = lines.Select(l => new Instruction8(l)).ToList();
        var reg = new Reg8();
        RunOps(reg, ops);
        rv = reg.Accumulator;
        res.CheckGuess(rv);
        return res;
    }
    public enum OpEnum8
    {
        NoOp,
        Acc,
        Jmp,
    }
    class Reg8
    {
        public int Instruction { get; set; }
        public int Accumulator { get; set; }
    }
	class Instruction8
	{
        public OpEnum8 Op { get; set; }
        public int Amount { get; set; }

        public Instruction8(string line)
        {
            //nop +0
            var parts = line.Split(' ');
            if (parts[0] == "nop")
                Op = OpEnum8.NoOp;
            else if (parts[0] == "acc")
                Op = OpEnum8.Acc;
            else if (parts[0] == "jmp")
                Op = OpEnum8.Jmp;
            Amount = int.Parse(parts[1]);
        }

        public Instruction8(OpEnum8 op, int amt)
        {
            Op = op;
            Amount = amt;
        }

        public Reg8 Act(Reg8 reg)
        {
            if (Op == OpEnum8.Jmp)
                reg.Instruction += Amount;
            else
                reg.Instruction++;

            if (Op == OpEnum8.Acc)
                reg.Accumulator += Amount;
            return reg;
        }
        public override string ToString()
        {
            return $"{Op} {Amount}";
        }
    }
    bool RunOps(Reg8 reg, List<Instruction8> ops)
    {
        reg.Instruction = 0;
        var runs = new List<int>();
        while (true)
        {
            if (reg.Instruction == ops.Count())
                return true;
            if (runs.Contains(reg.Instruction))
                return false;
            runs.Add(reg.Instruction);
            ops[reg.Instruction].Act(reg);
        }
        return false;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 8L);
		else
			res.Check = new StarCheck(key, 552L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var ops = lines.Select(l => new Instruction8(l)).ToList();
        for(int i = 0; i < ops.Count; i++)
        {
            var op = ops[i];   
            if (op.Op == OpEnum8.Jmp)
            {
                var newOps = ops.ToList();
                newOps.RemoveAt(i);
                newOps.Insert(i, new Instruction8(OpEnum8.NoOp, op.Amount));
                var reg = new Reg8();
                if (true == RunOps(reg, newOps))
                {
                    rv = reg.Accumulator;
                    break;
                }
            }
        }

        res.CheckGuess(rv);
        return res;
	}
}

