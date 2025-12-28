using AoCLibrary;
using System.Transactions;
namespace Advent20;
// #working
internal class Day18 : IRunner
{
	// Day https://adventofcode.com/2020/day/18
	// Input https://adventofcode.com/2020/day/18/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 26457L);
		else
			res.Check = new StarCheck(key, 86311597203806L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		foreach (var line in lines.Select(l => l.Replace(" ", "")))
		{
			var running = 0;
			var dict = new Dictionary<int, List<Node18>>();
			int depth = 0;
            long lastVal = 0;
            dict.Add(0, []);
            for (int i = 0; i < line.Length; i++)
			{
                var c = line[i];
                if (char.IsDigit(c))
                    lastVal = c - '0';
                else if (c == '*' || c == '+')
                {
                    dict[depth].Add(new Node18(lastVal, c));
                }
                else if (line[i] == '(')
                {
                    depth++;
                    if (!dict.ContainsKey(depth))
                        dict.Add(depth, []);
                }
                else if (line[i] == ')')
                {
                    dict[depth].Add(new Node18(lastVal, null));
                    var nodes = dict[depth];
                    var total = Node18.Act1(nodes);
                    lastVal = total;
                    dict[depth] = [];
                    depth--;
                }

            }
            dict[depth].Add(new Node18(lastVal, null));
            Utils.Assert(depth == 0, "back to the surface");
            rv += Node18.Act1(dict[0]);
        }

        res.CheckGuess(rv);
        return res;
    }
	class Node18
	{
		long _val;
		char? _op;
        public Node18(long val, char? op)
        {
			_val = val;
			_op = op;
        }

        internal static long Act1(List<Node18> nodes)
        {
            var rv = nodes[0]._val;
            for (int i = 0; i < nodes.Count() - 1; i++)
            {
                if (nodes[i]._op == '*')
                    rv *= nodes[i + 1]._val;
                if (nodes[i]._op == '+')
                    rv += nodes[i + 1]._val;
            }
            return rv;
        }
        internal static long Act2(List<Node18> nodes)
        {
            var newNodes = new List<Node18>();
            var last = nodes[0]._val;
            for (int i = 0; i < nodes.Count() - 1; i++)
            {
                if (nodes[i]._op == '+')
                    last += nodes[i + 1]._val;
                else if (nodes[i]._op == '*')
                {
                    newNodes.Add(new Node18(last, '*'));
                    last = nodes[i + 1]._val;
                }
            }
            newNodes.Add(new Node18(last, null));

            var rv = newNodes[0]._val;
            for (int i = 0; i < newNodes.Count() - 1; i++)
                rv *= newNodes[i + 1]._val;

            return rv;
        }

        public override string ToString()
        {
			return $"{_val}{_op}";
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 694173L);
		else
			res.Check = new StarCheck(key, 276894767062189L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        foreach (var line in lines.Select(l => l.Replace(" ", "")))
        {
            var running = 0;
            var dict = new Dictionary<int, List<Node18>>();
            int depth = 0;
            long lastVal = 0;
            dict.Add(0, []);
            bool tempDepth = false;
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (char.IsDigit(c))
                    lastVal = c - '0';
                else if (c == '*')
                {
                    dict[depth].Add(new Node18(lastVal, c));
                }
                else if (c == '+')
                {
                    dict[depth].Add(new Node18(lastVal, c));
                }
                else if (line[i] == '(')
                {
                    depth++;
                    if (!dict.ContainsKey(depth))
                        dict.Add(depth, []);
                }
                else if (line[i] == ')')
                {
                    dict[depth].Add(new Node18(lastVal, null));
                    var nodes = dict[depth];
                    lastVal = Node18.Act2(nodes);
                    dict[depth] = [];
                    depth--;
                }

            }
            dict[depth].Add(new Node18(lastVal, null));
            Utils.Assert(depth == 0, "back to the surface");
            var subTotal = Node18.Act2(dict[0]);
            rv += subTotal;
        }

        res.CheckGuess(rv);
        return res;
	}
}

