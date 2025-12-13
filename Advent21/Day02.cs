using AoCLibrary;
namespace Advent21;

internal class Day02 : IRunner
{
	// Day https://adventofcode.com/2021/day/2
	// Input https://adventofcode.com/2021/day/2/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 150L);
		else
			res.Check = new StarCheck(key, 1427868L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
        var moves = lines.Select(l => new Move2(l));
        var loc = new Loc(0, 0);
        foreach (var move in moves)
        {
            loc = move.Move(loc);
        }
        rv = loc.Col * loc.Row;
        
        res.CheckGuess(rv);
        return res;
    }
    class Loc2
    {
        public Loc2()
        {
            
        }
        public Loc2(Loc2 other)
        {
            Aim = other.Aim;
            Pos = new Loc(other.Pos);
        }
        public int Aim { get; set; }
        public Loc Pos { get; set; } = new Loc(0,0);
        public override string ToString()
        {
            return $"{Aim} ({Pos})";
        }
    }
    class Move2
	{
		DirEnum _dir;
		int _amount;
        
        public Loc Move(Loc loc)
        {
            return loc.Move(_dir, _amount);
        }
        public Loc2 Aim(Loc2 loc)
        {
            /*
             * down X increases your aim by X units.
up X decreases your aim by X units.
forward X does two things:
It increases your horizontal position by X units.
It increases your depth by your aim multiplied by X.*/
            Loc2 rv = new Loc2(loc);

            if (_dir == DirEnum.S)
                rv.Aim = loc.Aim + _amount;
            else if (_dir == DirEnum.N)
                rv.Aim = loc.Aim - _amount;
            else // forward
            {
                rv.Pos = loc.Pos.Move(DirEnum.E, _amount);
                rv.Pos = rv.Pos.Move(DirEnum.S, rv.Aim * _amount);
            }
            return rv;

        }
        public Move2(string line)
        {
            var parts = line.Split(' ');
			if (parts[0] == "forward")
				_dir = DirEnum.E;
            else if (parts[0] == "up")
                _dir = DirEnum.N;
            else if (parts[0] == "down")
                _dir = DirEnum.S;
            _amount = int.Parse(parts[1]);
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 900L);
		else
			res.Check = new StarCheck(key, 1568138742L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var moves = lines.Select(l => new Move2(l));
        var loc = new Loc2();
        foreach (var move in moves)
        {
            loc = move.Aim(loc);
        }
        rv = loc.Pos.Col * loc.Pos.Row;

        res.CheckGuess(rv);
        return res;
	}
}

