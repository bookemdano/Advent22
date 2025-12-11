using AoCLibrary;
using System.Reflection.Metadata;
namespace Advent25;

internal class Day11 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/11
	// Input https://adventofcode.com/2025/day/11/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 5L);
		else
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
        var connections = lines.Select(l => new Conn12(l)).ToList();
        var paths = new Stack<string>();
        paths.Push("you");
        while(paths.Any())
        {
            var source = paths.Pop();
            var con = connections.Single(c => c.Source == source);
            foreach(var conOut in con.Outs)
            {
                if (conOut == "out")
                    rv++;
                else
                    paths.Push(conOut);
            }
        }
        res.CheckGuess(rv);
        return res;
    }
	class Conn12
	{
        public string Source { get; }
        public List<String> Outs { get; }
        public Conn12(string line)
        {
            var parts = line.Split(": ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Source = parts[0];
            Outs = parts.Skip(1).ToList();
        }
        public override string ToString()
        {
            return $"{Source} -> {string.Join(",", Outs)}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 0L);
		else
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

