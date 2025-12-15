using AoCLibrary;
namespace Advent21;

internal class Day10 : IRunner
{
	// Day https://adventofcode.com/2021/day/10
	// Input https://adventofcode.com/2021/day/10/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 26397L);
		else
			res.Check = new StarCheck(key, 323613L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var scores = new List<Score10>();
        foreach(var line in lines)
			scores.Add(Validate(line));
        rv = scores.Where(s => s.Chunk == ChunkEnum.Corrupt).Sum(s => s.Score);
        res.CheckGuess(rv);
        return res;
    }

 
    char CloserFor(char c)
    {
        if (c == '{')
            return '}';
        else if (c == '[')
            return ']';
        else if (c == '<')
            return '>';
        else if (c == '(')
            return ')';
        return 'x';
    }
    enum ChunkEnum
    {
        Good,
        Incomplete,
        Corrupt
    }
    int Score1(char c)
    {
        if (c == ')')
            return 3;
        else if (c == ']')
            return 57;
        else if (c == '}')
            return 1197;
        else if (c == '>')
            return 25137;
        return 0;
    }
    int Score2(char c)
    {
        if (c == ')')
            return 1;
        else if (c == ']')
            return 2;
        else if (c == '}')
            return 3;
        else if (c == '>')
            return 4;
        return 0;
    }
    class Score10
    {
        public ChunkEnum Chunk;
        public long Score;

        public Score10(ChunkEnum corrupt, long score)
        {
            Chunk = corrupt;
            Score = score;
        }
        public override string ToString()
        {
            return $"{Chunk} {Score}";
        }
    }
    Score10 Validate(string line)
    {
        Dictionary<char, int> _chunks = new Dictionary<char, int>();
        var chars = new Stack<char>();
        foreach (var c in line)
        {
            if (c == '{' || c == '<' || c == '(' || c == '[')
            {
                chars.Push(c);
            }
            else
            {
                var last = chars.Pop();
                if (CloserFor(last) != c)
                    return new Score10(ChunkEnum.Corrupt, Score1(c));
            }
        }
        if (chars.Any())
        {
            var rv = 0L;
            while (chars.Any())
                rv = (rv * 5) + Score2(CloserFor(chars.Pop()));
            return new Score10(ChunkEnum.Incomplete, rv);
        }
        return new Score10(ChunkEnum.Good, 0);
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 288957L);
		else
			res.Check = new StarCheck(key, 3103006161L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var scores = new List<Score10>();
        foreach (var line in lines)
            scores.Add(Validate(line));
        scores = scores.Where(s => s.Chunk == ChunkEnum.Incomplete).OrderBy(s => s.Score).ToList();
        // middle score
        rv = scores[(scores.Count() / 2)].Score;

        // too low 915390357
        // too low 971344890
        res.CheckGuess(rv);
        return res;
	}
}

