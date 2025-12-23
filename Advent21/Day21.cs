using AoCLibrary;
using System.Collections;
namespace Advent21;
// #working
internal class Day21 : IRunner
{
	// Day https://adventofcode.com/2021/day/21
	// Input https://adventofcode.com/2021/day/21/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 739785L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var players = new List<Player21>();
		players.Add(new Player21(lines[0].Last() - '0'));
        players.Add(new Player21(lines[1].Last() - '0'));
		var scores = new List<int>();
		var dice = new Dice21();
		while (!players.Any(p=>p.IsWinner()))
		{
			for (int i = 0; i < 2; i++)
			{
                var roll = dice.Roll3();
				players[i].Advance(roll);
				if (players[i].IsWinner())
					break;
			}
		}
		rv = players.Single(p => !p.IsWinner()).Score * dice.Rolls;		
        
		res.CheckGuess(rv);
        return res;
    }
	class Player21
	{
		int _pos;	// 1-based
		public long Score { get; set; }
        public Player21(int pos)
        {
			_pos = pos;
			Score = 0;
        }
		internal bool IsWinner() => Score >= 1000;
        internal long Advance(int roll)
        {
			_pos += roll;
			_pos = _pos % 10;
			if (_pos == 0)
				_pos = 10;
			Score += _pos;
			return Score;
        }
        public override string ToString()
        {
            return $"p:{_pos} s:{Score}";
        }
    }
	class Dice21
	{
		int _val = 1;
		public int Rolls { get; private set; }
		public int Roll3()
		{
			var rv = 0;
			for(int i = 0; i < 3; i++)
			{
				Rolls++;

                if (_val > 1000)
					_val = 1;
				rv += _val;
				_val++;
            }
			return rv;
        }
        public override string ToString()
        {
            return $"v:{_val}";
        }
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, -1L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var players = new List<Player21>();
        players.Add(new Player21(lines[0].Last() - '0'));
        players.Add(new Player21(lines[1].Last() - '0'));
        var scores = new List<int>();
        var dice = new Dice21();
        while (!players.Any(p => p.IsWinner()))
        {
            for (int i = 0; i < 2; i++)
            {
                var roll = dice.Roll3();
                players[i].Advance(roll);
                if (players[i].IsWinner())
                    break;
            }
        }
        rv = players.Single(p => !p.IsWinner()).Score * dice.Rolls;

        res.CheckGuess(rv);
        return res;
	}
}

