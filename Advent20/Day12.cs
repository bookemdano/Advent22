using AoCLibrary;

namespace Advent20;

internal class Day12 : IRunner
{
	// Day https://adventofcode.com/2020/day/12
	// Input https://adventofcode.com/2020/day/12/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 25L);
		else
			res.Check = new StarCheck(key, 636L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var ship = new Ship12();
		foreach(var move in lines)
            ship.Move1(move);

        rv = ship.Distance();
        res.CheckGuess(rv);
        return res;
    }
	class Ship12
	{
		Loc _loc = new Loc(0, 0);
		DirEnum _facing = DirEnum.E;
        Loc _waypoint = new Loc(-1, 10);
        public void Move1(string move)
        {
            var c = move.First();
            var amt = int.Parse(new string(move.Skip(1).ToArray()));
            var dir = Loc.ParseDir(c);
            if (dir != null)
                _loc = _loc.Move((DirEnum)dir, amt);
            else if (c == 'L' || c == 'R')
                Spin1(c, amt);
            else if (c == 'F')
                _loc = _loc.Move(_facing, amt);
        }
        public void Move2(string move)
        {
            var c = move.First();
            var amt = int.Parse(new string(move.Skip(1).ToArray()));
            var dir = Loc.ParseDir(c);
            if (dir != null)
                _waypoint = _waypoint.Move((DirEnum)dir, amt);
            else if (c == 'L' || c == 'R')
                Spin2(c, amt);
            else if (c == 'F')
            {
                var delta = Delta();
                _loc = new Loc(_loc.Row + delta.Row * amt, _loc.Col + delta.Col * amt);
                _waypoint = new Loc(_waypoint.Row + delta.Row * amt, _waypoint.Col + delta.Col * amt);
            }
        }
        Loc Delta()
        {
            return new Loc(_waypoint.Row - _loc.Row, _waypoint.Col - _loc.Col);
        }
        public override string ToString()
        {
            return $"s:{LocString(_loc)} {_facing} w:{LocString(Delta())}";
        }
        static string LocString(Loc loc)
        {
            var str = "";
            if (loc.Col <= 0)
                str += "W" + Math.Abs(loc.Col);
            else
                str += "E" + loc.Col;
            if (loc.Row <= 0)
                str += "N" + Math.Abs(loc.Row);
            else
                str += "S" + loc.Row;
            return str;
        }

        internal long Distance()
        {
            return Math.Abs(_loc.Row) + Math.Abs(_loc.Col);
        }
        private void Spin1(char dir, int amt)
        {
            if (amt == 180)
            {
                if (_facing == DirEnum.E)
                    _facing = DirEnum.W;
                else if (_facing == DirEnum.W)
                    _facing = DirEnum.E;
                else if (_facing == DirEnum.N)
                    _facing = DirEnum.S;
                else if (_facing == DirEnum.S)
                    _facing = DirEnum.N;
            }
            else if (dir == 'L')
            {
                if (amt == 270)
                {
                    Spin1('R', 90);
                    return;
                }
                Utils.Assert(amt == 90, "Left turn");
                if (_facing == DirEnum.E)
                    _facing = DirEnum.N;
                else if (_facing == DirEnum.N)
                    _facing = DirEnum.W;
                else if (_facing == DirEnum.W)
                    _facing = DirEnum.S;
                else if (_facing == DirEnum.S)
                    _facing = DirEnum.E;
            }
            else if (dir == 'R')
            {
                if (amt == 270)
                {
                    Spin1('L', 90);
                    return;
                }
                Utils.Assert(amt == 90, "Right turn");
                if (_facing == DirEnum.E)
                    _facing = DirEnum.S;
                else if (_facing == DirEnum.S)
                    _facing = DirEnum.W;
                else if (_facing == DirEnum.W)
                    _facing = DirEnum.N;
                else if (_facing == DirEnum.N)
                    _facing = DirEnum.E;
            }
        }
        private void Spin2(char dir, int amt)
        {
            var delta = Delta();
            var spins = amt / 90;
            if (dir == 'L')
                spins = 4-spins;
            
            for (int i = 0; i < spins; i++)
                delta = new Loc(delta.Col, 0 - delta.Row);
            
            _waypoint = new Loc(_loc.Row + delta.Row, _loc.Col + delta.Col);
        }
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 286L);
		else
			res.Check = new StarCheck(key, 26841L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var ship = new Ship12();
        foreach (var move in lines)
            ship.Move2(move);

        rv = ship.Distance();
        //43209 too high
        res.CheckGuess(rv);
        return res;
	}
}