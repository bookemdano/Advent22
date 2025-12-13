using AoCLibrary;
namespace Advent21;

internal class Day04 : IRunner
{
	// Day https://adventofcode.com/2021/day/4
	// Input https://adventofcode.com/2021/day/4/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 4512L);
		else
			res.Check = new StarCheck(key, 10680L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var numbers = lines[0].Split(',').Select(s => int.Parse(s));
		var boards = new List<Board4>();	
		var iRow = 2;
		while(iRow < lines.Length)
		{
			var sublines = lines.Skip(iRow).Take(5);
            boards.Add(new Board4(sublines.ToArray()));
			iRow += 6;
		}

		foreach(var n in numbers)
		{
			foreach(var board in boards)
			{
                if (board.CallIt(n))
				{
                    rv = board.Score() * n;
					break;
				}
			}
			if (rv > 0)
				break;
		}
        res.CheckGuess(rv);
        return res;
    }
	class Board4
	{
		int[,] _spaces;

        public Board4(string[] lines)
        {
			_spaces = new int[5,5];
			for(int iLine = 0; iLine < lines.Count(); iLine++) {
				var ns = lines[iLine].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(n => int.Parse(n)).ToArray();
				for(int iCol = 0; iCol < ns.Count(); iCol++)
					_spaces[iLine, iCol] = ns[iCol];
			}
        }
		
        public bool CallIt(int n)
        {
			for (int x = 0; x < 5; x++)
			{
				for (int y = 0; y < 5; y++)
				{
					if (_spaces[x, y] == n)
					{
						_spaces[x, y] = -1;
						return Check();
					}
				}
			}
			return false;
        }

        internal long Score()
        {
			var rv = 0;
			for (int x = 0; x < 5; x++)
			{
				for (int y = 0; y < 5; y++)
				{
					if (_spaces[x, y] != -1)
						rv += _spaces[x, y];
                }
			}
			return rv;
        }

        private bool Check()
        {
            // check cols
			for (int x = 0; x < 5; x++)
            {
				var good = true;
                for (int y = 0; y < 5; y++)
                {
                    if (_spaces[x, y] != -1)
					{
						good = false;
						break;
					}
                }
				if (good == true)
					return true;
            }
            // check rows
            for (int y = 0; y < 5; y++)
            {
                var good = true;
                for (int x = 0; x < 5; x++)
                {
                    if (_spaces[x, y] != -1)
                    {
                        good = false;
                        break;
                    }
                }
                if (good == true)
                    return true;
            }
			return false;
        }
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 1924L);
		else
			res.Check = new StarCheck(key, 31892L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var numbers = lines[0].Split(',').Select(s => int.Parse(s));
        var boards = new List<Board4>();
        var iRow = 2;
        while (iRow < lines.Length)
        {
            var sublines = lines.Skip(iRow).Take(5);
            boards.Add(new Board4(sublines.ToArray()));
            iRow += 6;
        }

        foreach (var n in numbers)
        {
            var removeBoards = new List<Board4>();
            foreach (var board in boards)
            {
                if (board.CallIt(n))
                {
                    if (boards.Count() == 1)
                    {
                        rv = board.Score() * n;
                        break;
                    }
                    removeBoards.Add(board);
                }
            }
            if (rv > 0)
                break;
            foreach (var board in removeBoards)
                boards.Remove(board);
        }
        res.CheckGuess(rv);
        return res;
	}
}

