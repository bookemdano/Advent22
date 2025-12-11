using AoCLibrary;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
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
			res.Check = new StarCheck(key, 539L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
        var connections = lines.Select(l => new Conn11(l)).ToList();
        var paths = new Stack<Path11>();
        paths.Push(new Path11("you"));
        while(paths.Any())
        {
            var path = paths.Pop();
            var con = connections.Single(c => c.Source == path.Head);
            foreach(var conOut in con.Outs)
            {
                if (conOut == "out")
                    rv++;
                else
                {
                    var newPath = new Path11(path);
                    if (newPath.SetHead(conOut))
                        paths.Push(newPath);
                }
            }
        }
        res.CheckGuess(rv);
        return res;
    }
	class Conn11
	{
        public string Source { get; }
        public List<String> Outs { get; }
        public Conn11(string line)
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

    class Path11
    {
        public Path11(string head)
        {
            SetHead(head);
        }

        public Path11(Path11 path)
        {
            _visitedDac = path._visitedDac;
            _visitedFft = path._visitedFft;
        }
        public string Head { get; private set; }
        public bool SetHead(string head)
        {
            if (head == "dac")
                _visitedDac = true;
            if (head == "fft")
                _visitedFft = true;
            Head = head;
            return true;
        }
        internal bool VistedEnough => _visitedDac && _visitedFft;
        internal bool VistedAtAll => _visitedDac || _visitedFft;
        bool _visitedDac;
        bool _visitedFft;
        public override string ToString()
        {
            return $"{Head} (d:{_visitedDac} f:{_visitedFft})";
        }

        public override bool Equals(object? obj)
        {
            if (obj is Path11 other)
                return (Head == other.Head && _visitedDac == other._visitedDac && _visitedFft == other._visitedFft);
            return false;
        }
        public override int GetHashCode()
        {
            return Head.GetHashCode() ^ (_visitedDac ? 0 : 2) ^ (_visitedFft ? 0 : 4);
        }
    }

    class Conn11s
    {
        List<Conn11> conns = [];
        Dictionary<Path11, long> _cache = [];

        public Conn11s(List<Conn11> conns)
        {
            this.conns = conns;
        }
        public long Process(Path11 path)
        {
            if (path.Head == "out")
            {
                if (path.VistedEnough)
                    return 1;
                else
                    return 0;
            }
            if (_cache.ContainsKey(path))
                return _cache[path];

            var conn = conns.Single(c => c.Source == path.Head);
            var sum = 0L;
            foreach(var connOut in conn.Outs)
            {
                var newPath = new Path11(path);
                newPath.SetHead(connOut);
                sum += Process(newPath);
            }
            _cache[path] = sum;

            return sum;
        }
 
    }
 
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 2L);
		else
			res.Check = new StarCheck(key, 413167078187872L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic

        var connections = lines.Select(l => new Conn11(l)).ToList();
        var graph = new Conn11s(connections);
        rv = graph.Process(new Path11("svr"));

        res.CheckGuess(rv);
        return res;
	}
}

