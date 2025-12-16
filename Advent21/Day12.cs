using AoCLibrary;

namespace Advent21;

internal class Day12 : IRunner
{
	// Day https://adventofcode.com/2021/day/12
	// Input https://adventofcode.com/2021/day/12/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 10L);
		else
			res.Check = new StarCheck(key, 5212L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var allCons = lines.Select(l => new Conn12(l)).ToList();
        var head = "start";
        Path12.AllCons = allCons;
        var paths = new Stack<Path12>();
        paths.Push(new Path12(head));
        var goodPaths = new List<Path12>();
        while(paths.Any())
        {
            var path = paths.Pop();
            var cons = allCons.Where(c => c.Any(path.Head));
            foreach(var con in cons)
            {
                var otherEnd = con.Other(path.Head);
                if (otherEnd == "end")
                {
                    if (path.ValidEnd())
                        goodPaths.Add(new Path12(path, "end"));
                    continue;
                }
                var newPath = path.IsValid1(otherEnd);
                if (newPath == null)
                    continue;
                paths.Push(newPath);
            }
        }
        rv = goodPaths.Count();
        res.CheckGuess(rv);
        return res;
    }
    class Path12
    {
        List<string> _path = [];
        public string Head => _path.Last();

        public static List<Conn12> AllCons { get; internal set; }

        public override string ToString()
        {
            return $"{Head} d:{_doubleSmall} {string.Join("->",_path)}";
        }
        public Path12(string head)
        {
            _path = new List<string>() { head };
        }
        public Path12(Path12 other, string newHead)
        {
            _path = other._path.ToList();
            _doubleSmall = other._doubleSmall;
            if (IsSmall(newHead) && _path.Contains(newHead))
                _doubleSmall++;
            _path.Add(newHead);
        }

        static bool IsSmall(string s)
        {
            return char.IsLower(s[0]);
        }
        internal Path12? IsValid1(string to)
        {
            if (to == "start" || (IsSmall(to) && _path.Contains(to)))
                return null;

            return new Path12(this, to);
        }
        int _doubleSmall = 0;
        internal Path12? IsValid2(string to)
        {
            if (to == "start")
                return null;
            var doubleSmall = IsSmall(to) && _path.Contains(to);
            if (_doubleSmall > 0 && doubleSmall)
                return null;
            return new Path12(this, to);
        }

        internal bool ValidEnd()
        {
            return true;
        }
    }
    class Conn12
	{
        public Conn12(string line)
        {
            var parts = line.Split("-");
            From = parts[0];
            To = parts[1];
        }
        public override string ToString()
        {
            return $"{From}-{To}";
        }
        public string From { get;}
        public string To { get; }
        public bool Any(string node)
        {
            return (From == node || To == node);
        }
        public string Other(string node)
        {
            if (From == node)
                return To;
            else if (To == node)
                return From;
            return string.Empty;
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 36L);
		else
			res.Check = new StarCheck(key, 134862L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        var allCons = lines.Select(l => new Conn12(l)).ToList();
        var head = "start";
        Path12.AllCons = allCons;
        var paths = new Stack<Path12>();
        paths.Push(new Path12(head));
        var goodPaths = new List<Path12>();
        while (paths.Any())
        {
            var path = paths.Pop();
            var cons = allCons.Where(c => c.Any(path.Head));
            foreach (var con in cons)
            {
                var otherEnd = con.Other(path.Head);
                if (otherEnd == "end")
                {
                    if (path.ValidEnd())
                        goodPaths.Add(new Path12(path, "end"));
                    continue;
                }
                var newPath = path.IsValid2(otherEnd);
                if (newPath == null)
                    continue;
                paths.Push(newPath);
            }
        }
        rv = goodPaths.Count();

        res.CheckGuess(rv);
        return res;
	}
}

